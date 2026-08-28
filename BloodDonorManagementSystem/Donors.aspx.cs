using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class Donors : Page
    {
        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireLogin();

            if (!IsPostBack)
            {
                InitialisePage();
            }
        }


        // =========================================================
        // INITIALISE PAGE
        // =========================================================

        private void InitialisePage()
        {
            HideMessage();
            HideDetails();

            if (AuthHelper.IsAdmin)
            {
                InitialiseAdminPage();
                return;
            }

            if (AuthHelper.IsDonor)
            {
                InitialiseDonorPage();
                return;
            }

            Response.Redirect("~/Dashboard.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }


        // =========================================================
        // ADMIN PAGE
        // =========================================================

        private void InitialiseAdminPage()
        {
            litPageTitle.Text = "Donor Directory";

            litPageSubtitle.Text =
                "Search and manage registered blood donors.";

            lnkAddDonor.Visible = true;

            txtSearch.Visible = true;
            ddlFilterBloodGroup.Visible = true;
            txtFilterCity.Visible = true;
            txtFilterState.Visible = true;
            ddlAvailability.Visible = true;
            btnSearch.Visible = true;
            btnClear.Visible = true;

            BindDonors();
        }


        // =========================================================
        // DONOR PAGE
        // =========================================================

        private void InitialiseDonorPage()
        {
            litPageTitle.Text = "My Donor Profile";

            litPageSubtitle.Text =
                "View your donor registration information.";

            lnkAddDonor.Visible = false;

            txtSearch.Visible = false;
            ddlFilterBloodGroup.Visible = false;
            txtFilterCity.Visible = false;
            txtFilterState.Visible = false;
            ddlAvailability.Visible = false;
            btnSearch.Visible = false;
            btnClear.Visible = false;

            DonorData donor =
                LoadDonorByUserId(AuthHelper.UserId);

            if (donor == null)
            {
                pnlNoDonors.Visible = true;
                gvDonors.Visible = false;

                litResultSummary.Text =
                    "Your donor registration has not been completed yet.";

                return;
            }

            DataTable table =
                CreateDonorTable();

            AddDonorRow(
                table,
                donor);

            gvDonors.DataSource = table;
            gvDonors.DataBind();

            gvDonors.Visible = true;
            pnlNoDonors.Visible = false;

            litResultSummary.Text =
                "Your donor registration.";

            ShowDonorDetails(donor);
        }


        // =========================================================
        // SEARCH
        // =========================================================

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            if (!AuthHelper.IsAdmin)
            {
                return;
            }

            HideMessage();
            HideDetails();

            gvDonors.PageIndex = 0;

            BindDonors();
        }


        // =========================================================
        // CLEAR
        // =========================================================

        protected void btnClear_Click(
            object sender,
            EventArgs e)
        {
            if (!AuthHelper.IsAdmin)
            {
                return;
            }

            txtSearch.Text = string.Empty;

            ddlFilterBloodGroup.SelectedIndex = 0;

            txtFilterCity.Text = string.Empty;

            txtFilterState.Text = string.Empty;

            ddlAvailability.SelectedIndex = 0;

            gvDonors.PageIndex = 0;

            HideMessage();
            HideDetails();

            BindDonors();
        }


        // =========================================================
        // BIND DONORS
        // =========================================================

        private void BindDonors()
        {
            if (!AuthHelper.IsAdmin)
            {
                return;
            }

            try
            {
                string search =
                    txtSearch.Text.Trim();

                string bloodGroup =
                    ddlFilterBloodGroup.SelectedValue;

                string city =
                    txtFilterCity.Text.Trim();

                string state =
                    txtFilterState.Text.Trim();

                string availability =
                    ddlAvailability.SelectedValue;

                DataTable donors =
                    GetDonors(
                        search,
                        bloodGroup,
                        city,
                        state,
                        availability);

                gvDonors.DataSource = donors;
                gvDonors.DataBind();

                bool hasRows =
                    donors != null &&
                    donors.Rows.Count > 0;

                gvDonors.Visible = hasRows;
                pnlNoDonors.Visible = !hasRows;

                if (hasRows)
                {
                    litResultSummary.Text =
                        donors.Rows.Count.ToString(
                            CultureInfo.InvariantCulture) +
                        " donor record(s) found.";
                }
                else
                {
                    litResultSummary.Text =
                        "No donor records match your search.";
                }
            }
            catch (Exception ex)
            {
                gvDonors.Visible = false;
                pnlNoDonors.Visible = true;

                litResultSummary.Text =
                    "Unable to load donor records.";

                ShowError(
                    "Unable to load donor records. " +
                    ex.Message);
            }
        }


        // =========================================================
        // GET DONORS
        // =========================================================

        private DataTable GetDonors(
            string search,
            string bloodGroup,
            string city,
            string state,
            string availability)
        {
            const string sql = @"
                SELECT
                    d.DonorId,
                    d.FullName,
                    d.BloodGroup,
                    d.Mobile,
                    d.Email,
                    d.Address,
                    d.City,
                    d.State,
                    d.Pincode,
                    d.Gender,
                    d.DateOfBirth,
                    d.IsAvailable,
                    d.UserId,

                    ISNULL(u.IsActive, 0) AS IsActive,

                    CASE
                        WHEN d.DateOfBirth IS NULL
                            THEN NULL
                        ELSE
                            DATEDIFF(
                                YEAR,
                                d.DateOfBirth,
                                CAST(GETDATE() AS DATE)
                            )
                            -
                            CASE
                                WHEN DATEADD(
                                    YEAR,
                                    DATEDIFF(
                                        YEAR,
                                        d.DateOfBirth,
                                        CAST(GETDATE() AS DATE)
                                    ),
                                    d.DateOfBirth
                                ) > CAST(GETDATE() AS DATE)
                                THEN 1
                                ELSE 0
                            END
                    END AS Age

                FROM dbo.Donors d

                LEFT JOIN dbo.Users u
                    ON u.UserId = d.UserId

                WHERE

                    (
                        @Search = ''
                        OR d.FullName LIKE '%' + @Search + '%'
                        OR d.Mobile LIKE '%' + @Search + '%'
                        OR d.Email LIKE '%' + @Search + '%'
                    )

                    AND

                    (
                        @BloodGroup = ''
                        OR d.BloodGroup = @BloodGroup
                    )

                    AND

                    (
                        @City = ''
                        OR ISNULL(d.City, '') LIKE '%' + @City + '%'
                    )

                    AND

                    (
                        @State = ''
                        OR ISNULL(d.State, '') LIKE '%' + @State + '%'
                    )

                    AND

                    (
                        @Availability = ''
                        OR d.IsAvailable =
                            CASE
                                WHEN @Availability = '1'
                                    THEN 1
                                ELSE 0
                            END
                    )

                ORDER BY
                    d.FullName ASC,
                    d.DonorId ASC;";


            DataTable table =
                new DataTable();


            using (SqlConnection connection =
                Db.OpenConnection())
            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection))
            {
                command.Parameters.Add(
                    "@Search",
                    SqlDbType.NVarChar,
                    150).Value =
                    search ?? string.Empty;

                command.Parameters.Add(
                    "@BloodGroup",
                    SqlDbType.NVarChar,
                    5).Value =
                    bloodGroup ?? string.Empty;

                command.Parameters.Add(
                    "@City",
                    SqlDbType.NVarChar,
                    100).Value =
                    city ?? string.Empty;

                command.Parameters.Add(
                    "@State",
                    SqlDbType.NVarChar,
                    100).Value =
                    state ?? string.Empty;

                command.Parameters.Add(
                    "@Availability",
                    SqlDbType.VarChar,
                    1).Value =
                    availability ?? string.Empty;


                using (SqlDataAdapter adapter =
                    new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }


        // =========================================================
        // PAGE INDEX
        // =========================================================

        protected void gvDonors_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            if (!AuthHelper.IsAdmin)
            {
                return;
            }

            gvDonors.PageIndex =
                e.NewPageIndex;

            BindDonors();
        }


        // =========================================================
        // ROW COMMAND
        // =========================================================

        protected void gvDonors_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (!AuthHelper.IsAdmin)
            {
                return;
            }


            // -----------------------------------------------------
            // VIEW DONOR
            // -----------------------------------------------------

            if (string.Equals(
                e.CommandName,
                "ViewDonor",
                StringComparison.OrdinalIgnoreCase))
            {
                int donorId;

                if (!int.TryParse(
                    Convert.ToString(
                        e.CommandArgument),
                    out donorId) ||
                    donorId <= 0)
                {
                    ShowError(
                        "Invalid donor selected.");

                    return;
                }

                DonorData donor =
                    LoadDonorById(donorId);

                if (donor == null)
                {
                    ShowError(
                        "The selected donor could not be found.");

                    return;
                }

                ShowDonorDetails(donor);

                return;
            }


            // -----------------------------------------------------
            // TOGGLE ACCOUNT
            // -----------------------------------------------------

            if (string.Equals(
                e.CommandName,
                "ToggleAccount",
                StringComparison.OrdinalIgnoreCase))
            {
                int userId;

                if (!int.TryParse(
                    Convert.ToString(
                        e.CommandArgument),
                    out userId) ||
                    userId <= 0)
                {
                    ShowError(
                        "Invalid donor account selected.");

                    return;
                }

                ToggleAccountStatus(userId);
            }
        }


        // =========================================================
        // TOGGLE ACCOUNT STATUS
        // =========================================================

        private void ToggleAccountStatus(
            int userId)
        {
            if (!AuthHelper.IsAdmin)
            {
                return;
            }

            try
            {
                const string sql = @"
                    UPDATE dbo.Users
                    SET
                        IsActive =
                            CASE
                                WHEN IsActive = 1
                                    THEN 0
                                ELSE 1
                            END,
                        UpdatedDate = GETDATE()
                    WHERE UserId = @UserId;";


                int affectedRows;


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
                        userId;

                    affectedRows =
                        command.ExecuteNonQuery();
                }


                if (affectedRows == 0)
                {
                    ShowError(
                        "The donor account could not be found.");

                    return;
                }


                HideDetails();

                BindDonors();

                ShowSuccess(
                    "Donor account status updated successfully.");
            }
            catch (Exception ex)
            {
                ShowError(
                    "Unable to update donor account status. " +
                    ex.Message);
            }
        }


        // =========================================================
        // ROW DATA BOUND
        // =========================================================
        //
        // IMPORTANT:
        // All display values and account button behaviour are
        // configured here instead of using complicated inline
        // <%# ... %> expressions in Donors.aspx.
        // =========================================================

        protected void gvDonors_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if (e.Row.RowType !=
                DataControlRowType.DataRow)
            {
                return;
            }


            // -----------------------------------------------------
            // DONOR VALUES
            // -----------------------------------------------------

            Label donorName =
                e.Row.FindControl(
                    "lblDonorName") as Label;

            Label bloodGroup =
                e.Row.FindControl(
                    "lblBloodGroup") as Label;

            Label mobile =
                e.Row.FindControl(
                    "lblMobile") as Label;

            Label email =
                e.Row.FindControl(
                    "lblEmail") as Label;

            Label city =
                e.Row.FindControl(
                    "lblCity") as Label;

            Label state =
                e.Row.FindControl(
                    "lblState") as Label;

            Label age =
                e.Row.FindControl(
                    "lblAge") as Label;

            Label availability =
                e.Row.FindControl(
                    "lblAvailability") as Label;

            Label accountStatus =
                e.Row.FindControl(
                    "lblAccountStatus") as Label;


            DataRowView row =
                e.Row.DataItem as DataRowView;


            if (row == null)
            {
                return;
            }


            if (donorName != null)
            {
                donorName.Text =
                    Server.HtmlEncode(
                        Convert.ToString(
                            row["FullName"]));
            }


            if (bloodGroup != null)
            {
                bloodGroup.Text =
                    Server.HtmlEncode(
                        Convert.ToString(
                            row["BloodGroup"]));
            }


            if (mobile != null)
            {
                mobile.Text =
                    Server.HtmlEncode(
                        Convert.ToString(
                            row["Mobile"]));
            }


            if (email != null)
            {
                email.Text =
                    Server.HtmlEncode(
                        Convert.ToString(
                            row["Email"]));
            }


            if (city != null)
            {
                city.Text =
                    Server.HtmlEncode(
                        Convert.ToString(
                            row["City"]));
            }


            if (state != null)
            {
                state.Text =
                    Server.HtmlEncode(
                        Convert.ToString(
                            row["State"]));
            }


            bool isAvailable =
                row["IsAvailable"] != DBNull.Value &&
                Convert.ToBoolean(
                    row["IsAvailable"]);


            if (availability != null)
            {
                availability.Text =
                    isAvailable
                        ? "Available"
                        : "Unavailable";

                availability.CssClass =
                    isAvailable
                        ? "availability available"
                        : "availability unavailable";
            }


            if (age != null)
            {
                if (row["Age"] == DBNull.Value)
                {
                    age.Text = "-";
                }
                else
                {
                    age.Text =
                        Convert.ToString(
                            row["Age"]) +
                        " yrs";
                }
            }


            bool isActive =
                row["IsActive"] != DBNull.Value &&
                Convert.ToBoolean(
                    row["IsActive"]);


            if (accountStatus != null)
            {
                accountStatus.Text =
                    isActive
                        ? "Active"
                        : "Inactive";

                accountStatus.CssClass =
                    isActive
                        ? "account-active"
                        : "account-inactive";
            }


            // -----------------------------------------------------
            // EDIT
            // -----------------------------------------------------

            HyperLink editLink =
                e.Row.FindControl(
                    "lnkEdit") as HyperLink;

            if (editLink != null)
            {
                int donorId =
                    Convert.ToInt32(
                        row["DonorId"]);

                editLink.NavigateUrl =
                    "~/DonorRegistration.aspx?id=" +
                    donorId.ToString(
                        CultureInfo.InvariantCulture);

                editLink.Visible =
                    AuthHelper.IsAdmin;
            }


            // -----------------------------------------------------
            // VIEW
            // -----------------------------------------------------

            LinkButton viewButton =
                e.Row.FindControl(
                    "btnView") as LinkButton;

            if (viewButton != null)
            {
                int donorId =
                    Convert.ToInt32(
                        row["DonorId"]);

                viewButton.CommandArgument =
                    donorId.ToString(
                        CultureInfo.InvariantCulture);
            }


            // -----------------------------------------------------
            // ACCOUNT TOGGLE
            // -----------------------------------------------------

            LinkButton toggleButton =
                e.Row.FindControl(
                    "btnToggleAccount") as LinkButton;

            if (toggleButton != null)
            {
                int userId =
                    row["UserId"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            row["UserId"]);

                toggleButton.CommandArgument =
                    userId.ToString(
                        CultureInfo.InvariantCulture);

                toggleButton.Visible =
                    AuthHelper.IsAdmin;


                if (isActive)
                {
                    toggleButton.Text =
                        "Deactivate";

                    toggleButton.CssClass =
                        "action-button action-toggle";

                    toggleButton.OnClientClick =
                        "return confirm('Deactivate this donor account?');";
                }
                else
                {
                    toggleButton.Text =
                        "Activate";

                    toggleButton.CssClass =
                        "action-button action-toggle activate";

                    toggleButton.OnClientClick =
                        "return confirm('Activate this donor account?');";
                }
            }
        }


        // =========================================================
        // LOAD DONOR BY ID
        // =========================================================

        private DonorData LoadDonorById(
            int donorId)
        {
            if (donorId <= 0)
            {
                return null;
            }


            const string sql = @"
                SELECT
                    d.DonorId,
                    d.FullName,
                    d.BloodGroup,
                    d.Mobile,
                    d.City,
                    d.State,
                    d.IsAvailable,
                    d.UserId,
                    d.Email,
                    d.Address,
                    d.Pincode,
                    d.Gender,
                    d.DateOfBirth
                FROM dbo.Donors d
                WHERE d.DonorId = @DonorId;";


            using (SqlConnection connection =
                Db.OpenConnection())
            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection))
            {
                command.Parameters.Add(
                    "@DonorId",
                    SqlDbType.Int).Value =
                    donorId;


                using (SqlDataReader reader =
                    command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return ReadDonor(reader);
                }
            }
        }


        // =========================================================
        // LOAD DONOR BY USER ID
        // =========================================================

        private DonorData LoadDonorByUserId(
            int userId)
        {
            if (userId <= 0)
            {
                return null;
            }


            const string sql = @"
                SELECT
                    d.DonorId,
                    d.FullName,
                    d.BloodGroup,
                    d.Mobile,
                    d.City,
                    d.State,
                    d.IsAvailable,
                    d.UserId,
                    d.Email,
                    d.Address,
                    d.Pincode,
                    d.Gender,
                    d.DateOfBirth
                FROM dbo.Donors d
                WHERE d.UserId = @UserId;";


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
                    userId;


                using (SqlDataReader reader =
                    command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return ReadDonor(reader);
                }
            }
        }


        // =========================================================
        // READ DONOR
        // =========================================================

        private DonorData ReadDonor(
            SqlDataReader reader)
        {
            DonorData donor =
                new DonorData();


            donor.DonorId =
                Convert.ToInt32(
                    reader["DonorId"]);


            donor.FullName =
                Convert.ToString(
                    reader["FullName"]);


            donor.BloodGroup =
                Convert.ToString(
                    reader["BloodGroup"]);


            donor.Mobile =
                Convert.ToString(
                    reader["Mobile"]);


            donor.City =
                reader["City"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(
                        reader["City"]);


            donor.State =
                reader["State"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(
                        reader["State"]);


            donor.IsAvailable =
                reader["IsAvailable"] != DBNull.Value &&
                Convert.ToBoolean(
                    reader["IsAvailable"]);


            donor.UserId =
                reader["UserId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(
                        reader["UserId"]);


            donor.Email =
                reader["Email"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(
                        reader["Email"]);


            donor.Address =
                reader["Address"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(
                        reader["Address"]);


            donor.Pincode =
                reader["Pincode"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(
                        reader["Pincode"]);


            donor.Gender =
                reader["Gender"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(
                        reader["Gender"]);


            donor.DateOfBirth =
                reader["DateOfBirth"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(
                        reader["DateOfBirth"]);


            return donor;
        }


        // =========================================================
        // CREATE DONOR TABLE
        // =========================================================

        private DataTable CreateDonorTable()
        {
            DataTable table =
                new DataTable();


            table.Columns.Add(
                "DonorId",
                typeof(int));

            table.Columns.Add(
                "FullName",
                typeof(string));

            table.Columns.Add(
                "BloodGroup",
                typeof(string));

            table.Columns.Add(
                "Mobile",
                typeof(string));

            table.Columns.Add(
                "City",
                typeof(string));

            table.Columns.Add(
                "State",
                typeof(string));

            table.Columns.Add(
                "IsAvailable",
                typeof(bool));

            table.Columns.Add(
                "IsActive",
                typeof(bool));

            table.Columns.Add(
                "UserId",
                typeof(int));

            table.Columns.Add(
                "Email",
                typeof(string));

            table.Columns.Add(
                "Gender",
                typeof(string));

            table.Columns.Add(
                "Age",
                typeof(int));


            return table;
        }


        // =========================================================
        // ADD DONOR ROW
        // =========================================================

        private void AddDonorRow(
            DataTable table,
            DonorData donor)
        {
            DataRow row =
                table.NewRow();


            row["DonorId"] =
                donor.DonorId;

            row["FullName"] =
                donor.FullName ?? string.Empty;

            row["BloodGroup"] =
                donor.BloodGroup ?? string.Empty;

            row["Mobile"] =
                donor.Mobile ?? string.Empty;

            row["City"] =
                donor.City ?? string.Empty;

            row["State"] =
                donor.State ?? string.Empty;

            row["IsAvailable"] =
                donor.IsAvailable;

            row["IsActive"] =
                donor.IsActive;

            row["UserId"] =
                donor.UserId;

            row["Email"] =
                donor.Email ?? string.Empty;

            row["Gender"] =
                donor.Gender ?? string.Empty;

            row["Age"] =
                donor.DateOfBirth.HasValue
                    ? CalculateAge(
                        donor.DateOfBirth.Value)
                    : 0;


            table.Rows.Add(row);
        }


        // =========================================================
        // SHOW DONOR DETAILS
        // =========================================================

        private void ShowDonorDetails(
            DonorData donor)
        {
            if (donor == null)
            {
                HideDetails();
                return;
            }


            litDetailDonorId.Text =
                donor.DonorId.ToString(
                    CultureInfo.InvariantCulture);


            litDetailFullName.Text =
                Encode(donor.FullName);


            litDetailBloodGroup.Text =
                Encode(donor.BloodGroup);


            litDetailGender.Text =
                Encode(donor.Gender);


            litDetailMobile.Text =
                Encode(donor.Mobile);


            litDetailEmail.Text =
                Encode(donor.Email);


            litDetailCity.Text =
                Encode(donor.City);


            litDetailState.Text =
                Encode(donor.State);


            litDetailPincode.Text =
                Encode(donor.Pincode);


            litDetailAddress.Text =
                EncodeMultiline(donor.Address);


            if (donor.DateOfBirth.HasValue)
            {
                litDetailDateOfBirth.Text =
                    donor.DateOfBirth.Value.ToString(
                        "dd MMM yyyy",
                        CultureInfo.InvariantCulture);

                litDetailAge.Text =
                    CalculateAge(
                        donor.DateOfBirth.Value)
                    .ToString(
                        CultureInfo.InvariantCulture);
            }
            else
            {
                litDetailDateOfBirth.Text =
                    "-";

                litDetailAge.Text =
                    "-";
            }


            if (donor.IsAvailable)
            {
                litDetailAvailability.Text =
                    "<span class=\"status-badge status-available\">Available</span>";
            }
            else
            {
                litDetailAvailability.Text =
                    "<span class=\"status-badge status-unavailable\">Unavailable</span>";
            }


            lnkDetailEdit.NavigateUrl =
                "~/DonorRegistration.aspx?id=" +
                donor.DonorId.ToString(
                    CultureInfo.InvariantCulture);


            lnkDetailEdit.Visible =
                AuthHelper.IsAdmin;


            pnlDetails.Visible =
                true;
        }


        // =========================================================
        // CLOSE DETAILS
        // =========================================================

        protected void btnCloseDetails_Click(
            object sender,
            EventArgs e)
        {
            HideDetails();
        }


        // =========================================================
        // HIDE DETAILS
        // =========================================================

        private void HideDetails()
        {
            pnlDetails.Visible =
                false;
        }


        // =========================================================
        // HIDE MESSAGE
        // =========================================================

        private void HideMessage()
        {
            pnlMessage.Visible =
                false;

            litMessage.Text =
                string.Empty;
        }


        // =========================================================
        // SHOW ERROR
        // =========================================================

        private void ShowError(
            string message)
        {
            pnlMessage.Visible =
                true;

            pnlMessage.CssClass =
                "message message-error";

            litMessage.Text =
                Server.HtmlEncode(
                    message);
        }


        // =========================================================
        // SHOW SUCCESS
        // =========================================================

        private void ShowSuccess(
            string message)
        {
            pnlMessage.Visible =
                true;

            pnlMessage.CssClass =
                "message message-success";

            litMessage.Text =
                Server.HtmlEncode(
                    message);
        }


        // =========================================================
        // ENCODE
        // =========================================================

        private string Encode(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            return Server.HtmlEncode(
                value);
        }


        // =========================================================
        // ENCODE MULTILINE
        // =========================================================

        private string EncodeMultiline(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            return Server.HtmlEncode(
                    value)
                .Replace(
                    Environment.NewLine,
                    "<br />")
                .Replace(
                    "\n",
                    "<br />")
                .Replace(
                    "\r",
                    string.Empty);
        }


        // =========================================================
        // CALCULATE AGE
        // =========================================================

        private int CalculateAge(
            DateTime dateOfBirth)
        {
            DateTime today =
                DateTime.Today;


            int age =
                today.Year -
                dateOfBirth.Year;


            if (dateOfBirth.Date >
                today.AddYears(-age))
            {
                age--;
            }


            return age;
        }


        // =========================================================
        // DONOR MODEL
        // =========================================================

        private class DonorData
        {
            public int DonorId { get; set; }

            public string FullName { get; set; }

            public string BloodGroup { get; set; }

            public string Mobile { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public bool IsAvailable { get; set; }

            public bool IsActive { get; set; }

            public int UserId { get; set; }

            public string Email { get; set; }

            public string Address { get; set; }

            public string Pincode { get; set; }

            public string Gender { get; set; }

            public DateTime? DateOfBirth { get; set; }
        }
    }
}
