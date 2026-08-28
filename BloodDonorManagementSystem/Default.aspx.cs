using System;
using BloodDonorManagementSystem.Infrastructure;
namespace BloodDonorManagementSystem
{
    public partial class DefaultPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect(AuthHelper.IsLoggedIn ? "~/Dashboard.aspx" : "~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
