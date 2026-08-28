using System;
using System.Data;
using System.Data.SqlClient;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack &&
                AuthHelper.IsLoggedIn)
            {
                RedirectToHome();
            }
        }

        // =========================================================
        // LOGIN
        // =========================================================

        protected void btnLogin_Click(
            object sender,
            EventArgs e)
        {
            HideMessage();

            string username =
                txtUsername.Text.Trim();

            string password =
                txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrEmpty(password))
            {
                ShowError(
                    "Please enter both username and password.");

                return;
            }

            try
            {
                UserLoginData user =
                    LoadUser(username);

                if (user == null)
                {
                    ShowError(
                        "Invalid username or password.");

                    return;
                }

                if (!user.IsActive)
                {
                    ShowError(
                        "Your account is inactive. " +
                        "Please contact the administrator.");

                    return;
                }

                bool validPassword =
                    PasswordHelper.Verify(
                        password,
                        user.PasswordSalt,
                        user.PasswordHash);

                if (!validPassword)
                {
                    ShowError(
                        "Invalid username or password.");

                    return;
                }

                if (!IsSupportedRole(user.Role))
                {
                    ShowError(
                        "Your account does not have a valid role.");

                    return;
                }

                // -------------------------------------------------
                // AUTOMATIC PASSWORD HASH MIGRATION
                // -------------------------------------------------

                if (PasswordHelper.IsLegacyHash(
                    user.PasswordHash))
                {
                    UpgradePasswordHash(
                        user.UserId,
                        password);
                }

                // -------------------------------------------------
                // UPDATE LAST LOGIN
                // -------------------------------------------------

                UpdateLastLogin(
                    user.UserId);

                // -------------------------------------------------
                // CREATE AUTHENTICATION SESSION
                // -------------------------------------------------

                AuthHelper.SignIn(
                    user.UserId,
                    user.Username,
                    user.Role,
                    user.MustChangePassword,
                    true);

                // -------------------------------------------------
                // FORCE PASSWORD CHANGE
                // -------------------------------------------------

                if (user.MustChangePassword)
                {
                    Response.Redirect(
                        "~/ChangePassword.aspx",
                        false);

                    Context.ApplicationInstance
                        .CompleteRequest();

                    return;
                }

                RedirectToHome(user.Role);
            }
            catch
            {
                ShowError(
                    "Unable to sign in right now. " +
                    "Please try again later.");
            }
        }

        // =========================================================
        // LOAD USER
        // =========================================================

        private UserLoginData LoadUser(
            string username)
        {
            const string sql = @"
                SELECT
                    UserId,
                    Username,
                    PasswordHash,
                    PasswordSalt,
                    IsActive,
                    RoleName,
                    MustChangePassword
                FROM dbo.Users
                WHERE Username = @Username;";

            using (SqlConnection connection =
                   Db.OpenConnection())
            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection))
            {
                command.Parameters.Add(
                    "@Username",
                    SqlDbType.NVarChar,
                    100).Value = username;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    UserLoginData user =
                        new UserLoginData();

                    user.UserId =
                        Convert.ToInt32(
                            reader["UserId"]);

                    user.Username =
                        Convert.ToString(
                            reader["Username"]);

                    user.PasswordHash =
                        Convert.ToString(
                            reader["PasswordHash"]);

                    user.PasswordSalt =
                        Convert.ToString(
                            reader["PasswordSalt"]);

                    user.IsActive =
                        reader["IsActive"] != DBNull.Value &&
                        Convert.ToBoolean(
                            reader["IsActive"]);

                    user.Role =
                        Convert.ToString(
                            reader["RoleName"]);

                    user.MustChangePassword =
                        reader["MustChangePassword"] !=
                        DBNull.Value &&
                        Convert.ToBoolean(
                            reader["MustChangePassword"]);

                    return user;
                }
            }
        }

        // =========================================================
        // UPGRADE LEGACY PASSWORD
        // =========================================================

        private void UpgradePasswordHash(
            int userId,
            string password)
        {
            string newSalt =
                PasswordHelper.GenerateSalt();

            string newHash =
                PasswordHelper.Hash(
                    password,
                    newSalt);

            const string sql = @"
                UPDATE dbo.Users
                SET
                    PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt,
                    UpdatedDate = SYSDATETIME()
                WHERE UserId = @UserId
                  AND IsActive = 1;";

            using (SqlConnection connection =
                   Db.OpenConnection())
            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection))
            {
                command.Parameters.Add(
                    "@PasswordHash",
                    SqlDbType.NVarChar,
                    500).Value = newHash;

                command.Parameters.Add(
                    "@PasswordSalt",
                    SqlDbType.NVarChar,
                    500).Value = newSalt;

                command.Parameters.Add(
                    "@UserId",
                    SqlDbType.Int).Value = userId;

                command.ExecuteNonQuery();
            }
        }

        // =========================================================
        // UPDATE LAST LOGIN
        // =========================================================

        private void UpdateLastLogin(
            int userId)
        {
            const string sql = @"
                UPDATE dbo.Users
                SET
                    LastLoginDate = SYSDATETIME(),
                    UpdatedDate = SYSDATETIME()
                WHERE UserId = @UserId
                  AND IsActive = 1;";

            using (SqlConnection connection =
                   Db.OpenConnection())
            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection))
            {
                command.Parameters.Add(
                    "@UserId",
                    SqlDbType.Int).Value = userId;

                command.ExecuteNonQuery();
            }
        }

        // =========================================================
        // ROLE VALIDATION
        // =========================================================

        private bool IsSupportedRole(
            string role)
        {
            return
                string.Equals(
                    role,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    role,
                    "Donor",
                    StringComparison.OrdinalIgnoreCase);
        }

        // =========================================================
        // REDIRECT
        // =========================================================

        private void RedirectToHome()
        {
            if (AuthHelper.IsDonor)
            {
                Response.Redirect(
                    "~/DonorDashboard.aspx",
                    false);
            }
            else
            {
                Response.Redirect(
                    "~/Dashboard.aspx",
                    false);
            }

            Context.ApplicationInstance
                .CompleteRequest();
        }

        private void RedirectToHome(
            string role)
        {
            string destination;

            if (string.Equals(
                role,
                "Donor",
                StringComparison.OrdinalIgnoreCase))
            {
                destination =
                    "~/DonorDashboard.aspx";
            }
            else
            {
                destination =
                    "~/Dashboard.aspx";
            }

            Response.Redirect(
                destination,
                false);

            Context.ApplicationInstance
                .CompleteRequest();
        }

        // =========================================================
        // MESSAGE
        // =========================================================

        private void HideMessage()
        {
            lblMessage.Visible = false;
            lblMessage.Text = string.Empty;
        }

        private void ShowError(
            string message)
        {
            lblMessage.Text =
                Server.HtmlEncode(message);

            lblMessage.CssClass =
                "message message-error";

            lblMessage.Visible = true;
        }

        // =========================================================
        // LOGIN MODEL
        // =========================================================

        private class UserLoginData
        {
            public int UserId { get; set; }

            public string Username { get; set; }

            public string PasswordHash { get; set; }

            public string PasswordSalt { get; set; }

            public bool IsActive { get; set; }

            public string Role { get; set; }

            public bool MustChangePassword { get; set; }
        }
    }
}