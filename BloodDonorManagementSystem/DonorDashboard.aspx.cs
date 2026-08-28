using System;
using System.Data.SqlClient;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class DonorDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireDonor();

            if (!IsPostBack)
            {
                LoadDonorInformation();
            }
        }


        private void LoadDonorInformation()
        {
            int userId = AuthHelper.UserId;

            // ---------------------------------------------------------
            // Always keep the account information available.
            // ---------------------------------------------------------

            litUsername.Text =
                E(string.IsNullOrWhiteSpace(AuthHelper.Username)
                    ? "Guest"
                    : AuthHelper.Username);

            litEmail.Text = "Not Set";


            try
            {
                const string sql = @"
                    SELECT
                        d.FullName,
                        d.BloodGroup,
                        d.Mobile,
                        d.Email,
                        d.Address,
                        d.City,
                        d.State,
                        d.Pincode,
                        d.IsAvailable,
                        d.CreatedDate,
                        u.Username,
                        u.Email AS AccountEmail
                    FROM dbo.Donors d
                    LEFT JOIN dbo.Users u
                        ON d.UserId = u.UserId
                    WHERE d.UserId = @UserId;";


                using (SqlConnection connection = Db.OpenConnection())
                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);


                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        // -------------------------------------------------
                        // NO DONOR PROFILE
                        // -------------------------------------------------

                        if (!reader.Read())
                        {
                            ShowNoProfileState();

                            return;
                        }


                        // -------------------------------------------------
                        // PROFILE EXISTS
                        // -------------------------------------------------

                        ShowDashboardState();


                        string name =
                            V(reader, "FullName");

                        string blood =
                            V(reader, "BloodGroup");

                        string mobile =
                            V(reader, "Mobile");

                        string email =
                            V(reader, "Email");

                        string address =
                            V(reader, "Address");

                        string city =
                            V(reader, "City");

                        string state =
                            V(reader, "State");

                        string pincode =
                            V(reader, "Pincode");


                        bool available =
                            reader["IsAvailable"] != DBNull.Value &&
                            Convert.ToBoolean(
                                reader["IsAvailable"]);


                        DateTime createdDate =
                            reader["CreatedDate"] == DBNull.Value
                                ? DateTime.Now
                                : Convert.ToDateTime(
                                    reader["CreatedDate"]);


                        string username =
                            V(reader, "Username");

                        string accountEmail =
                            V(reader, "AccountEmail");


                        // -------------------------------------------------
                        // WELCOME
                        // -------------------------------------------------

                        string displayName =
                            string.IsNullOrWhiteSpace(name)
                                ? AuthHelper.Username
                                : name;


                        if (string.IsNullOrWhiteSpace(displayName))
                        {
                            displayName = "Donor";
                        }


                        litWelcomeName.Text =
                            E(displayName);


                        // -------------------------------------------------
                        // PROFILE HEADER
                        // -------------------------------------------------

                        litFullName.Text =
                            E(string.IsNullOrWhiteSpace(name)
                                ? "Donor"
                                : name);


                        litInitial.Text =
                            E(GetInitial(name));


                        // -------------------------------------------------
                        // PROFILE DETAILS
                        // -------------------------------------------------

                        litBloodGroup.Text =
                            E(string.IsNullOrWhiteSpace(blood)
                                ? "Not Set"
                                : blood);


                        litMobile.Text =
                            E(string.IsNullOrWhiteSpace(mobile)
                                ? "Not Set"
                                : mobile);


                        litAddress.Text =
                            E(string.IsNullOrWhiteSpace(address)
                                ? "Not Set"
                                : address);


                        litCity.Text =
                            E(string.IsNullOrWhiteSpace(city)
                                ? "Not Set"
                                : city);


                        litState.Text =
                            E(BuildStateAndPincode(
                                state,
                                pincode));


                        // -------------------------------------------------
                        // AVAILABILITY
                        // -------------------------------------------------

                        if (available)
                        {
                            litAvailability.Text =
                                "<span class=\"status-badge status-available\">" +
                                "Available" +
                                "</span>";
                        }
                        else
                        {
                            litAvailability.Text =
                                "<span class=\"status-badge status-unavailable\">" +
                                "Not Available" +
                                "</span>";
                        }


                        // -------------------------------------------------
                        // KPI CARDS
                        // -------------------------------------------------

                        litKpiBlood.Text =
                            E(string.IsNullOrWhiteSpace(blood)
                                ? "—"
                                : blood);


                        litKpiAvailability.Text =
                            available
                                ? "Available"
                                : "Unavailable";


                        litKpiCity.Text =
                            E(string.IsNullOrWhiteSpace(city)
                                ? "—"
                                : city);


                        litKpiState.Text =
                            E(string.IsNullOrWhiteSpace(state)
                                ? "—"
                                : state);


                        litKpiDate.Text =
                            E(createdDate.ToString(
                                "MMM yyyy"));


                        // -------------------------------------------------
                        // ACCOUNT INFORMATION
                        // -------------------------------------------------

                        litUsername.Text =
                            E(string.IsNullOrWhiteSpace(username)
                                ? AuthHelper.Username
                                : username);


                        litEmail.Text =
                            E(string.IsNullOrWhiteSpace(email)
                                ? accountEmail
                                : email);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowDashboardError(ex);
            }
        }


        // =============================================================
        // PAGE STATES
        // =============================================================

        private void ShowDashboardState()
        {
            pnlNoProfile.Visible = false;
            pnlDashboard.Visible = true;

            pnlMessage.Visible = false;

            lnkWelcomeAction.Text =
                "✎ Update Profile";

            lnkWelcomeAction.NavigateUrl =
                "~/DonorRegistration.aspx";

            lnkCompleteRegistration.Visible =
                false;
        }


        private void ShowNoProfileState()
        {
            pnlDashboard.Visible = false;
            pnlNoProfile.Visible = true;

            pnlMessage.Visible = false;

            string username =
                AuthHelper.Username;

            if (string.IsNullOrWhiteSpace(username))
            {
                username = "Donor";
            }

            litWelcomeName.Text =
                E(username);


            lnkWelcomeAction.Text =
                "＋ Complete Registration";

            lnkWelcomeAction.NavigateUrl =
                "~/DonorRegistration.aspx";


            lnkCompleteRegistration.Visible =
                true;


            litUsername.Text =
                E(username);


            litEmail.Text =
                "Not Set";
        }


        private void ShowDashboardError(Exception ex)
        {
            pnlDashboard.Visible = false;
            pnlNoProfile.Visible = false;

            pnlMessage.Visible = true;
            pnlMessage.CssClass =
                "message message-error";


            // Do not expose database/server details to users.
            litMessage.Text =
                "Unable to load your donor dashboard. " +
                "Please try again. If the problem continues, " +
                "contact the administrator.";
        }


        // =============================================================
        // DATABASE VALUE HELPER
        // =============================================================

        private string V(
            SqlDataReader reader,
            string columnName)
        {
            if (reader[columnName] == DBNull.Value)
            {
                return "";
            }

            return Convert.ToString(
                reader[columnName]);
        }


        // =============================================================
        // HTML ENCODING
        // =============================================================

        private string E(string value)
        {
            return Server.HtmlEncode(
                value ?? "");
        }


        // =============================================================
        // INITIAL
        // =============================================================

        private string GetInitial(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "?";
            }


            return name.Trim()
                .Substring(0, 1)
                .ToUpperInvariant();
        }


        // =============================================================
        // STATE + PINCODE
        // =============================================================

        private string BuildStateAndPincode(
            string state,
            string pincode)
        {
            string result = "";


            if (!string.IsNullOrWhiteSpace(state))
            {
                result = state.Trim();
            }


            if (!string.IsNullOrWhiteSpace(pincode))
            {
                if (!string.IsNullOrWhiteSpace(result))
                {
                    result += " • ";
                }

                result += pincode.Trim();
            }


            return string.IsNullOrWhiteSpace(result)
                ? "Not Set"
                : result;
        }
    }
}
