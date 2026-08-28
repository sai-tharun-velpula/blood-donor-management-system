using System;
using System.Data;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireAdmin();

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        protected string GetCurrentUser()
        {
            return string.IsNullOrWhiteSpace(AuthHelper.Username)
                ? "there"
                : AuthHelper.Username;
        }

        protected string GetInitials(object value)
        {
            string name = Convert.ToString(value);

            if (string.IsNullOrWhiteSpace(name))
            {
                return "D";
            }

            string[] parts = name.Trim().Split(
                new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return parts[0].Substring(0, 1).ToUpperInvariant();
            }

            return (
                parts[0].Substring(0, 1) +
                parts[parts.Length - 1].Substring(0, 1)
            ).ToUpperInvariant();
        }

        private void LoadDashboard()
        {
            try
            {
                LoadStatistics();
                LoadBloodGroups();
                LoadRecentDonors();
            }
            catch
            {
                ShowDashboardError();
            }
        }

        private void LoadStatistics()
        {
            lblTotal.Text = Convert.ToString(
                Db.ExecuteScalar("SELECT COUNT(1) FROM dbo.Donors"));

            lblAvailable.Text = Convert.ToString(
                Db.ExecuteScalar(
                    @"SELECT COUNT(1)
                      FROM dbo.Donors
                      WHERE IsAvailable = 1"));

            lblGroups.Text = Convert.ToString(
                Db.ExecuteScalar(
                    @"SELECT COUNT(DISTINCT BloodGroup)
                      FROM dbo.Donors
                      WHERE BloodGroup IS NOT NULL
                        AND LTRIM(RTRIM(BloodGroup)) <> ''"));

            lblCities.Text = Convert.ToString(
                Db.ExecuteScalar(
                    @"SELECT COUNT(DISTINCT City)
                      FROM dbo.Donors
                      WHERE City IS NOT NULL
                        AND LTRIM(RTRIM(City)) <> ''"));
        }

        private void LoadBloodGroups()
        {
            const string sql = @"
                SELECT
                    BloodGroup,
                    COUNT(1) AS DonorCount,
                    CAST(
                        ROUND(
                            COUNT(1) * 100.0 /
                            NULLIF(
                                (
                                    SELECT COUNT(1)
                                    FROM dbo.Donors
                                    WHERE BloodGroup IS NOT NULL
                                      AND LTRIM(RTRIM(BloodGroup)) <> ''
                                ),
                                0
                            ),
                            0
                        ) AS INT
                    ) AS PercentValue
                FROM dbo.Donors
                WHERE BloodGroup IS NOT NULL
                  AND LTRIM(RTRIM(BloodGroup)) <> ''
                GROUP BY BloodGroup
                ORDER BY
                    CASE BloodGroup
                        WHEN 'A+' THEN 1
                        WHEN 'A-' THEN 2
                        WHEN 'B+' THEN 3
                        WHEN 'B-' THEN 4
                        WHEN 'AB+' THEN 5
                        WHEN 'AB-' THEN 6
                        WHEN 'O+' THEN 7
                        WHEN 'O-' THEN 8
                        ELSE 9
                    END;";

            DataTable groups = Db.GetDataTable(sql);

            rptBloodGroups.DataSource = groups;
            rptBloodGroups.DataBind();

            pnlNoBloodGroups.Visible = groups.Rows.Count == 0;
        }

        private void LoadRecentDonors()
        {
            const string sql = @"
                SELECT TOP 8
                    FullName,
                    BloodGroup,
                    City,
                    IsAvailable,
                    CreatedDate
                FROM dbo.Donors
                ORDER BY CreatedDate DESC;";

            DataTable recent = Db.GetDataTable(sql);

            gvRecent.DataSource = recent;
            gvRecent.DataBind();
        }

        private void ShowDashboardError()
        {
            lblTotal.Text = "—";
            lblAvailable.Text = "—";
            lblGroups.Text = "—";
            lblCities.Text = "—";

            rptBloodGroups.DataSource = null;
            rptBloodGroups.DataBind();
            pnlNoBloodGroups.Visible = true;

            gvRecent.DataSource = null;
            gvRecent.DataBind();
        }
    }
}
