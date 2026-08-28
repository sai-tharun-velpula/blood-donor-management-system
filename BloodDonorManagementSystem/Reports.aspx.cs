using System;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireLogin();

            if (!IsPostBack)
            {
                LoadReports();
            }
        }

        private void LoadReports()
        {
            try
            {
                gvBlood.DataSource = Db.GetDataTable(@"
                    SELECT
                        BloodGroup,
                        COUNT(1) AS TotalDonors,
                        SUM(CASE WHEN IsAvailable = 1 THEN 1 ELSE 0 END) AS AvailableDonors
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
                        END;");

                gvBlood.DataBind();

                gvCity.DataSource = Db.GetDataTable(@"
                    SELECT
                        City,
                        State,
                        COUNT(1) AS TotalDonors,
                        SUM(CASE WHEN IsAvailable = 1 THEN 1 ELSE 0 END) AS AvailableDonors
                    FROM dbo.Donors
                    WHERE City IS NOT NULL
                      AND LTRIM(RTRIM(City)) <> ''
                    GROUP BY City, State
                    ORDER BY COUNT(1) DESC, City;");

                gvCity.DataBind();

                lblFemale.Text = Convert.ToString(
                    Db.ExecuteScalar("SELECT COUNT(1) FROM dbo.Donors"));

                lblMale.Text = Convert.ToString(
                    Db.ExecuteScalar(
                        "SELECT COUNT(1) FROM dbo.Donors WHERE IsAvailable = 1"));

                lblRecent.Text = Convert.ToString(
                    Db.ExecuteScalar(
                        @"SELECT COUNT(DISTINCT BloodGroup)
                          FROM dbo.Donors
                          WHERE BloodGroup IS NOT NULL
                            AND LTRIM(RTRIM(BloodGroup)) <> ''"));

                lblInactive.Text = Convert.ToString(
                    Db.ExecuteScalar(
                        @"SELECT COUNT(DISTINCT City)
                          FROM dbo.Donors
                          WHERE City IS NOT NULL
                            AND LTRIM(RTRIM(City)) <> ''"));
            }
            catch
            {
                lblFemale.Text = "—";
                lblMale.Text = "—";
                lblRecent.Text = "—";
                lblInactive.Text = "—";
            }
        }
    }
}
