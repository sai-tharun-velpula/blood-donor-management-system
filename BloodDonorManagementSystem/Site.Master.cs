using BloodDonorManagementSystem.Infrastructure;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BloodDonorManagementSystem
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ConfigureNavigation();

            if (AuthHelper.IsLoggedIn &&
                AuthHelper.MustChangePassword &&
                !Request.AppRelativeCurrentExecutionFilePath.Equals(
                    "~/ChangePassword.aspx",
                    StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect(
                    "~/ChangePassword.aspx",
                    false);

                Context.ApplicationInstance
                    .CompleteRequest();

                return;
            }
        }


        private void ConfigureNavigation()
        {
            bool isLoggedIn =
                AuthHelper.IsLoggedIn;

            bool isAdmin =
                isLoggedIn &&
                AuthHelper.IsAdmin;

            bool isDonor =
                isLoggedIn &&
                AuthHelper.IsDonor;


            pnlAdminMenu.Visible =
                isAdmin;

            pnlDonorMenu.Visible =
                isDonor;


            lnkLogout.Visible =
                isLoggedIn;

            btnTopLogout.Visible =
                isLoggedIn;


            if (!isLoggedIn)
            {
                return;
            }


            if (isAdmin)
            {
                SetActive(
                    lnkDashboard,
                    "~/Dashboard.aspx");

                SetActive(
                    lnkDonors,
                    "~/Donors.aspx");

                SetActive(
                    lnkRegister,
                    "~/DonorRegistration.aspx");

                SetActive(
                    lnkReports,
                    "~/Reports.aspx");
            }
            else if (isDonor)
            {
                SetActive(
                    lnkDonorDashboard,
                    "~/DonorDashboard.aspx");

                SetActive(
                    lnkMyRegistration,
                    "~/DonorRegistration.aspx");

                SetActive(
                    lnkFindDonor,
                    "~/DonorSearch.aspx");

                SetActive(
                    lnkChangePassword,
                    "~/ChangePassword.aspx");
            }
        }


        private void SetActive(
            HyperLink link,
            string path)
        {
            string current =
                VirtualPathUtility.ToAbsolute(
                    Request.AppRelativeCurrentExecutionFilePath);

            string target =
                VirtualPathUtility.ToAbsolute(path);


            link.CssClass =
                current.Equals(
                    target,
                    StringComparison.OrdinalIgnoreCase)

                ? "nav-link active"

                : "nav-link";
        }


        protected string GetUserName()
        {
            if (!AuthHelper.IsLoggedIn)
            {
                return "Guest";
            }


            if (string.IsNullOrWhiteSpace(
                AuthHelper.Username))
            {
                return "Guest";
            }


            return AuthHelper.Username;
        }


        protected string GetRoleName()
        {
            if (!AuthHelper.IsLoggedIn)
            {
                return "Guest";
            }


            if (string.IsNullOrWhiteSpace(
                AuthHelper.Role))
            {
                return "User";
            }


            return AuthHelper.Role;
        }


        protected string GetInitials()
        {
            string username =
                GetUserName();


            if (string.IsNullOrWhiteSpace(username) ||
                username.Equals(
                    "Guest",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "?";
            }


            string[] parts =
                username.Trim().Split(
                    new[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);


            if (parts.Length == 1)
            {
                return parts[0]
                    .Substring(0, 1)
                    .ToUpperInvariant();
            }


            return (
                parts[0].Substring(0, 1) +
                parts[parts.Length - 1]
                    .Substring(0, 1)
            ).ToUpperInvariant();
        }
    }
}
