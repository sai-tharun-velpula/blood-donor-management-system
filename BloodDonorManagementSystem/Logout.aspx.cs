using System;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AuthHelper.SignOut();

                Response.Redirect(
                    "~/Login.aspx",
                    false);

                Context.ApplicationInstance
                    .CompleteRequest();
            }
        }
    }
}
