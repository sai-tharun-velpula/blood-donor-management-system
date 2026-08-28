using System;
using System.Web;
using System.Web.Security;

namespace BloodDonorManagementSystem
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Application startup
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // New session started
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Request begins
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            // Request ends
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Keep application errors available for debugging.
            // Do not redirect here because it can hide the actual error.
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Session ended
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Application shutdown
        }
    }
}