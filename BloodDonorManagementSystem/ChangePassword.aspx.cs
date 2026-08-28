using System;
using System.Data;
using System.Data.SqlClient;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            AuthHelper.RequireLogin();

            if (!IsPostBack)
            {
                ConfigurePage();
            }
        }

        // =========================================================
        // PAGE CONFIGURATION
        // =========================================================

        private void ConfigurePage()
        {
            if (AuthHelper.MustChangePassword)
            {
                pnlNotice.Visible = true;
                pnlNotice.CssClass =
                    "message message-warning";

                litNotice.Text =
                    "Your account was created with a temporary " +
                    "password. You must create a new password " +
                    "before continuing.";
            }
            else
            {
                pnlNotice.Visible = false;
            }
        }

        // =========================================================
        // CHANGE PASSWORD
        // =========================================================

        protected void btnChange_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string currentPassword =
                txtCurrentPassword.Text;

            string newPassword =
                txtNewPassword.Text;

            string confirmPassword =
                txtConfirmPassword.Text;

            // -----------------------------------------------------
            // VALIDATE INPUT
            // -----------------------------------------------------

            string validationError =
                ValidatePasswordInput(
                    currentPassword,
                    newPassword,
                    confirmPassword);

            if (!string.IsNullOrEmpty(validationError))
            {
                ShowError(validationError);
                return;
            }

            try
            {
                // -------------------------------------------------
                // LOAD CURRENT USER
                // -------------------------------------------------

                UserPasswordData user =
                    LoadUserPassword();

                if (user == null)
                {
                    ShowError(
                        "Your account could not be found or is inactive.");

                    return;
                }

                // -------------------------------------------------
                // VERIFY CURRENT PASSWORD
                // -------------------------------------------------

                bool validCurrentPassword =
                    PasswordHelper.Verify(
                        currentPassword,
                        user.PasswordSalt,
                        user.PasswordHash);

                if (!validCurrentPassword)
                {
                    ShowError(
                        "The current password is incorrect.");

                    return;
                }

                // -------------------------------------------------
                // CREATE NEW PASSWORD
                // -------------------------------------------------

                string newSalt =
                    PasswordHelper.GenerateSalt();

                string newHash =
                    PasswordHelper.Hash(
                        newPassword,
                        newSalt);

                // -------------------------------------------------
                // UPDATE DATABASE
                // -------------------------------------------------

                UpdatePassword(
                    AuthHelper.UserId,
                    newHash,
                    newSalt);

                // -------------------------------------------------
                // REFRESH SESSION
                // -------------------------------------------------

                AuthHelper.SignIn(
                    AuthHelper.UserId,
                    AuthHelper.Username,
                    AuthHelper.Role,
                    false,
                    true);

                // -------------------------------------------------
                // REDIRECT
                // -------------------------------------------------

                RedirectAfterPasswordChange();
            }
            catch (Exception)
            {
                ShowError(
                    "Unable to change your password right now. " +
                    "Please try again later.");
            }
        }

        // =========================================================
        // PASSWORD VALIDATION
        // =========================================================

        private string ValidatePasswordInput(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                return "Please enter your current password.";
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return "Please enter a new password.";
            }

            if (!string.Equals(
                newPassword,
                confirmPassword,
                StringComparison.Ordinal))
            {
                return
                    "New password and confirmation password do not match.";
            }

            if (newPassword.Length < 8)
            {
                return
                    "New password must be at least 8 characters long.";
            }

            if (newPassword.Length > 100)
            {
                return
                    "New password cannot exceed 100 characters.";
            }

            bool hasLetter = false;
            bool hasNumber = false;
            bool hasSymbol = false;

            foreach (char character in newPassword)
            {
                if (char.IsLetter(character))
                {
                    hasLetter = true;
                }
                else if (char.IsDigit(character))
                {
                    hasNumber = true;
                }
                else
                {
                    hasSymbol = true;
                }
            }

            if (!hasLetter ||
                !hasNumber ||
                !hasSymbol)
            {
                return
                    "New password must contain letters, " +
                    "at least one number and at least one special character.";
            }

            if (string.Equals(
                currentPassword,
                newPassword,
                StringComparison.Ordinal))
            {
                return
                    "New password must be different from your current password.";
            }

            return string.Empty;
        }

        // =========================================================
        // LOAD USER PASSWORD
        // =========================================================

        private UserPasswordData LoadUserPassword()
        {
            const string sql = @"
                SELECT
                    PasswordHash,
                    PasswordSalt
                FROM dbo.Users
                WHERE UserId = @UserId
                  AND IsActive = 1;";

            using (SqlConnection connection =
                   Db.OpenConnection())
            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@UserId",
                    SqlDbType.Int).Value =
                    AuthHelper.UserId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new UserPasswordData
                    {
                        PasswordHash =
                            Convert.ToString(
                                reader["PasswordHash"]),

                        PasswordSalt =
                            Convert.ToString(
                                reader["PasswordSalt"])
                    };
                }
            }
        }

        // =========================================================
        // UPDATE PASSWORD
        // =========================================================

        private void UpdatePassword(
            int userId,
            string passwordHash,
            string passwordSalt)
        {
            const string sql = @"
                UPDATE dbo.Users
                SET
                    PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt,
                    MustChangePassword = 0,
                    UpdatedDate = SYSDATETIME()
                WHERE UserId = @UserId
                  AND IsActive = 1;";

            using (SqlConnection connection =
                   Db.OpenConnection())
            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@PasswordHash",
                    SqlDbType.NVarChar,
                    500).Value =
                    passwordHash;

                command.Parameters.Add(
                    "@PasswordSalt",
                    SqlDbType.NVarChar,
                    500).Value =
                    passwordSalt;

                command.Parameters.Add(
                    "@UserId",
                    SqlDbType.Int).Value =
                    userId;

                int affected =
                    command.ExecuteNonQuery();

                if (affected != 1)
                {
                    throw new InvalidOperationException(
                        "Password update failed.");
                }
            }
        }

        // =========================================================
        // REDIRECT
        // =========================================================

        private void RedirectAfterPasswordChange()
        {
            string destination;

            if (AuthHelper.IsDonor)
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
        // ERROR MESSAGE
        // =========================================================

        private void ShowError(
            string message)
        {
            pnlNotice.Visible = true;

            pnlNotice.CssClass =
                "message message-error";

            litNotice.Text =
                Server.HtmlEncode(message);
        }

        // =========================================================
        // PASSWORD MODEL
        // =========================================================

        private class UserPasswordData
        {
            public string PasswordHash { get; set; }

            public string PasswordSalt { get; set; }
        }
    }
}



























/*using System;
using System.Data;
using System.Data.SqlClient;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class ChangePassword :
        System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            AuthHelper.RequireLogin();

            if (!IsPostBack)
            {
                ConfigurePage();
            }
        }

        // =========================================================
        // PAGE CONFIGURATION
        // =========================================================

        private void ConfigurePage()
        {
            if (AuthHelper.MustChangePassword)
            {
                pnlNotice.Visible = true;

                pnlNotice.CssClass =
                    "message message-warning";

                litNotice.Text =
                    "Your account was created with a temporary " +
                    "password. You must create a new password " +
                    "before continuing.";
            }
            else
            {
                pnlNotice.Visible = false;
            }
        }

        // =========================================================
        // CHANGE PASSWORD
        // =========================================================

        protected void btnChange_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string currentPassword =
                txtCurrentPassword.Text;

            string newPassword =
                txtNewPassword.Text;

            string confirmPassword =
                txtConfirmPassword.Text;

            // -----------------------------------------------------
            // VALIDATE INPUT
            // -----------------------------------------------------

            string validationError =
                ValidatePasswordInput(
                    currentPassword,
                    newPassword,
                    confirmPassword);

            if (!string.IsNullOrEmpty(
                validationError))
            {
                ShowError(validationError);
                return;
            }

            try
            {
                UserPasswordData user =
                    LoadUserPassword();

                if (user == null)
                {
                    ShowError(
                        "Your account could not be found or is inactive.");

                    return;
                }

                // -------------------------------------------------
                // VERIFY CURRENT PASSWORD
                // -------------------------------------------------

                bool currentPasswordValid =
                    PasswordHelper.Verify(
                        currentPassword,
                        user.PasswordSalt,
                        user.PasswordHash);

                if (!currentPasswordValid)
                {
                    ShowError(
                        "The current password is incorrect.");

                    return;
                }

                // -------------------------------------------------
                // GENERATE NEW PASSWORD HASH
                // -------------------------------------------------

                string newSalt =
                    PasswordHelper.GenerateSalt();

                string newHash =
                    PasswordHelper.Hash(
                        newPassword,
                        newSalt);

                // -------------------------------------------------
                // UPDATE PASSWORD
                // -------------------------------------------------

                UpdatePassword(
                    AuthHelper.UserId,
                    newHash,
                    newSalt);

                // -------------------------------------------------
                // REFRESH AUTHENTICATION TICKET
                // -------------------------------------------------

                AuthHelper.SignIn(
                    AuthHelper.UserId,
                    AuthHelper.Username,
                    AuthHelper.Role,
                    false,
                    true);

                // -------------------------------------------------
                // REDIRECT
                // -------------------------------------------------

                RedirectAfterPasswordChange();
            }
            catch
            {
                ShowError(
                    "Unable to change your password right now. " +
                    "Please try again later.");
            }
        }

        // =========================================================
        // VALIDATE PASSWORD INPUT
        // =========================================================

        private string ValidatePasswordInput(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(
                currentPassword))
            {
                return "Please enter your current password.";
            }

            if (string.IsNullOrWhiteSpace(
                newPassword))
            {
                return "Please enter a new password.";
            }

            if (!string.Equals(
                newPassword,
                confirmPassword,
                StringComparison.Ordinal))
            {
                return
                    "New password and confirmation password do not match.";
            }

            if (newPassword.Length < 8)
            {
                return
                    "New password must be at least 8 characters long.";
            }

            if (newPassword.Length > 100)
            {
                return
                    "New password cannot exceed 100 characters.";
            }

            bool hasLetter = false;
            bool hasNumber = false;
            bool hasSymbol = false;

            foreach (char character in newPassword)
            {
                if (char.IsLetter(character))
                {
                    hasLetter = true;
                }
                else if (char.IsDigit(character))
                {
                    hasNumber = true;
                }
                else
                {
                    hasSymbol = true;
                }
            }

            if (!hasLetter ||
                !hasNumber ||
                !hasSymbol)
            {
                return
                    "New password must contain letters, " +
                    "at least one number and at least one special character.";
            }

            if (string.Equals(
                currentPassword,
                newPassword,
                StringComparison.Ordinal))
            {
                return
                    "New password must be different from your current password.";
            }

            return string.Empty;
        }

        // =========================================================
        // LOAD USER PASSWORD
        // =========================================================

        private UserPasswordData LoadUserPassword()
        {
            const string sql = @"
                SELECT
                    PasswordHash,
                    PasswordSalt
                FROM dbo.Users
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
                    SqlDbType.Int).Value =
                    AuthHelper.UserId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new UserPasswordData
                    {
                        PasswordHash =
                            Convert.ToString(
                                reader["PasswordHash"]),

                        PasswordSalt =
                            Convert.ToString(
                                reader["PasswordSalt"])
                    };
                }
            }
        }

        // =========================================================
        // UPDATE PASSWORD
        // =========================================================

        private void UpdatePassword(
            int userId,
            string passwordHash,
            string passwordSalt)
        {
            const string sql = @"
                UPDATE dbo.Users
                SET
                    PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt,
                    MustChangePassword = 0,
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
                    500).Value =
                    passwordHash;

                command.Parameters.Add(
                    "@PasswordSalt",
                    SqlDbType.NVarChar,
                    500).Value =
                    passwordSalt;

                command.Parameters.Add(
                    "@UserId",
                    SqlDbType.Int).Value =
                    userId;

                int affected =
                    command.ExecuteNonQuery();

                if (affected != 1)
                {
                    throw new InvalidOperationException(
                        "Password update failed.");
                }
            }
        }

        // =========================================================
        // REDIRECT
        // =========================================================

        private void RedirectAfterPasswordChange()
        {
            string destination;

            if (AuthHelper.IsDonor)
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

        private void ShowError(
            string message)
        {
            pnlNotice.Visible = true;

            pnlNotice.CssClass =
                "message message-error";

            litNotice.Text =
                Server.HtmlEncode(message);
        }

        // =========================================================
        // PASSWORD MODEL
        // =========================================================

        private class UserPasswordData
        {
            public string PasswordHash { get; set; }

            public string PasswordSalt { get; set; }
        }
    }
}*/