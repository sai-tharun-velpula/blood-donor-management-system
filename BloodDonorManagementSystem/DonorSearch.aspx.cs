using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class DonorSearch : Page
    {
        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthHelper.IsLoggedIn)
            {
                Response.Redirect(
                    "~/Login.aspx",
                    false);

                Context.ApplicationInstance
                    .CompleteRequest();

                return;
            }

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

            HideNoDonors();

            gvSearchResults.PageIndex = 0;

            LoadDonors();
        }


        // =========================================================
        // SEARCH BUTTON
        // =========================================================

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            HideMessage();

            gvSearchResults.PageIndex = 0;

            LoadDonors();
        }


        // =========================================================
        // CLEAR BUTTON
        // =========================================================

        protected void btnClear_Click(
            object sender,
            EventArgs e)
        {
            txtName.Text = string.Empty;

            ddlBloodGroup.SelectedIndex = 0;

            txtFilterCity.Text = string.Empty;

            txtFilterState.Text = string.Empty;

            ddlAvailability.SelectedIndex = 0;

            gvSearchResults.PageIndex = 0;

            HideMessage();

            LoadDonors();
        }


        // =========================================================
        // PAGE INDEX
        // =========================================================

        protected void gvSearchResults_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            if (e.NewPageIndex < 0)
            {
                return;
            }

            gvSearchResults.PageIndex =
                e.NewPageIndex;

            LoadDonors();
        }


        // =========================================================
        // LOAD DONORS
        // =========================================================

        private void LoadDonors()
        {
            try
            {
                DataTable table =
                    GetDonors();

                gvSearchResults.DataSource =
                    table;

                gvSearchResults.DataBind();

                bool hasRows =
                    table != null &&
                    table.Rows.Count > 0;

                gvSearchResults.Visible =
                    hasRows;

                pnlNoDonors.Visible =
                    !hasRows;

                if (hasRows)
                {
                    litResultSummary.Text =
                        table.Rows.Count +
                        " donor record(s) found.";
                }
                else
                {
                    litResultSummary.Text =
                        "No donor records match your search.";
                }

                HideMessage();
            }
            catch (Exception ex)
            {
                gvSearchResults.DataSource =
                    null;

                gvSearchResults.DataBind();

                gvSearchResults.Visible =
                    false;

                pnlNoDonors.Visible =
                    true;

                litResultSummary.Text =
                    "No donor records found.";

                ShowError(
                    "Unable to load donor records. " +
                    ex.Message);
            }
        }


        // =========================================================
        // GET DONORS
        // =========================================================

        private DataTable GetDonors()
        {
            const string sql = @"
                SELECT
                    d.DonorId,
                    d.FullName,
                    d.BloodGroup,
                    d.Mobile,
                    d.City,
                    d.State,
                    d.IsAvailable,
                    d.CreatedDate

                FROM dbo.Donors AS d

                WHERE

                    (
                        @Name = ''
                        OR ISNULL(d.FullName, '') LIKE
                            '%' + @Name + '%'
                        OR ISNULL(d.Mobile, '') LIKE
                            '%' + @Name + '%'
                        OR ISNULL(d.Email, '') LIKE
                            '%' + @Name + '%'
                    )

                    AND

                    (
                        @BloodGroup = ''
                        OR d.BloodGroup = @BloodGroup
                    )

                    AND

                    (
                        @City = ''
                        OR LTRIM(RTRIM(ISNULL(d.City, ''))) LIKE
                            '%' + @City + '%'
                    )

                    AND

                    (
                        @State = ''
                        OR LTRIM(RTRIM(ISNULL(d.State, ''))) LIKE
                            '%' + @State + '%'
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


            string name =
                txtName.Text == null
                    ? string.Empty
                    : txtName.Text.Trim();


            string bloodGroup =
                ddlBloodGroup.SelectedValue == null
                    ? string.Empty
                    : ddlBloodGroup.SelectedValue.Trim();


            string city =
                txtFilterCity.Text == null
                    ? string.Empty
                    : txtFilterCity.Text.Trim();


            string state =
                txtFilterState.Text == null
                    ? string.Empty
                    : txtFilterState.Text.Trim();


            string availability =
                ddlAvailability.SelectedValue == null
                    ? string.Empty
                    : ddlAvailability.SelectedValue.Trim();


            using (SqlConnection connection =
                Db.OpenConnection())

            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection))
            {
                command.Parameters.Add(
                    "@Name",
                    SqlDbType.NVarChar,
                    150).Value =
                    name;


                command.Parameters.Add(
                    "@BloodGroup",
                    SqlDbType.NVarChar,
                    5).Value =
                    bloodGroup;


                command.Parameters.Add(
                    "@City",
                    SqlDbType.NVarChar,
                    100).Value =
                    city;


                command.Parameters.Add(
                    "@State",
                    SqlDbType.NVarChar,
                    100).Value =
                    state;


                command.Parameters.Add(
                    "@Availability",
                    SqlDbType.VarChar,
                    1).Value =
                    availability;


                using (SqlDataAdapter adapter =
                    new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }


            return table;
        }


        // =========================================================
        // HIDE NO-DONOR STATE
        // =========================================================

        private void HideNoDonors()
        {
            pnlNoDonors.Visible = false;

            gvSearchResults.Visible = true;
        }


        // =========================================================
        // HIDE MESSAGE
        // =========================================================

        private void HideMessage()
        {
            pnlMessage.Visible = false;

            pnlMessage.CssClass =
                "message";

            litMessage.Text =
                string.Empty;
        }


        // =========================================================
        // SHOW ERROR
        // =========================================================

        private void ShowError(
            string message)
        {
            pnlMessage.Visible = true;

            pnlMessage.CssClass =
                "message message-error";

            litMessage.Text =
                Server.HtmlEncode(
                    message ?? string.Empty);
        }
    }
}
