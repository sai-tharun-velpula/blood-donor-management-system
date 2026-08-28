using System;
using System.Web;
using System.Web.Security;

namespace BloodDonorManagementSystem.Infrastructure
{
    public static class AuthHelper
    {
        private const char TicketSeparator = '|';

        // =========================================================
        // LOGIN STATUS
        // =========================================================

        public static bool IsLoggedIn
        {
            get
            {
                HttpContext context =
                    HttpContext.Current;

                if (context == null ||
                    context.User == null ||
                    context.User.Identity == null ||
                    !context.User.Identity.IsAuthenticated)
                {
                    return false;
                }

                FormsIdentity identity =
                    context.User.Identity as FormsIdentity;

                if (identity == null ||
                    identity.Ticket == null ||
                    identity.Ticket.Expired)
                {
                    return false;
                }

                return true;
            }
        }

        // =========================================================
        // USERNAME
        // =========================================================

        public static string Username
        {
            get
            {
                FormsAuthenticationTicket ticket =
                    GetTicket();

                if (ticket == null)
                {
                    return string.Empty;
                }

                return ticket.Name ?? string.Empty;
            }
        }

        // =========================================================
        // ROLE
        // =========================================================

        public static string Role
        {
            get
            {
                string[] values =
                    GetTicketValues();

                if (values.Length > 1)
                {
                    return values[1] ?? string.Empty;
                }

                return string.Empty;
            }
        }

        // =========================================================
        // USER ID
        // =========================================================

        public static int UserId
        {
            get
            {
                string[] values =
                    GetTicketValues();

                if (values.Length == 0)
                {
                    return 0;
                }

                int userId;

                if (int.TryParse(
                    values[0],
                    out userId))
                {
                    return userId;
                }

                return 0;
            }
        }

        // =========================================================
        // ADMIN
        // =========================================================

        public static bool IsAdmin
        {
            get
            {
                return string.Equals(
                    Role,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        // =========================================================
        // DONOR
        // =========================================================

        public static bool IsDonor
        {
            get
            {
                return string.Equals(
                    Role,
                    "Donor",
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        // =========================================================
        // MUST CHANGE PASSWORD
        // =========================================================

        public static bool MustChangePassword
        {
            get
            {
                string[] values =
                    GetTicketValues();

                if (values.Length <= 2)
                {
                    return false;
                }

                bool result;

                if (bool.TryParse(
                    values[2],
                    out result))
                {
                    return result;
                }

                return false;
            }
        }

        // =========================================================
        // SIGN IN
        // =========================================================

        public static void SignIn(
            int userId,
            string username,
            string role,
            bool mustChangePassword,
            bool persistent)
        {
            HttpContext context =
                HttpContext.Current;

            if (context == null)
            {
                return;
            }

            if (userId <= 0)
            {
                throw new ArgumentException(
                    "Invalid user ID.",
                    nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException(
                    "Username is required.",
                    nameof(username));
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException(
                    "Role is required.",
                    nameof(role));
            }

            string userData =
                userId.ToString() +
                TicketSeparator +
                role +
                TicketSeparator +
                mustChangePassword.ToString();

            DateTime issued =
                DateTime.Now;

            TimeSpan timeout =
                FormsAuthentication.Timeout;

            DateTime expiration =
                issued.Add(timeout);

            FormsAuthenticationTicket ticket =
                new FormsAuthenticationTicket(
                    1,
                    username,
                    issued,
                    expiration,
                    persistent,
                    userData,
                    FormsAuthentication.FormsCookiePath);

            string encryptedTicket =
                FormsAuthentication.Encrypt(ticket);

            if (string.IsNullOrEmpty(encryptedTicket))
            {
                throw new InvalidOperationException(
                    "Unable to create authentication ticket.");
            }

            HttpCookie cookie =
                new HttpCookie(
                    FormsAuthentication.FormsCookieName,
                    encryptedTicket);

            cookie.HttpOnly = true;

            cookie.Secure =
                context.Request.IsSecureConnection;

            cookie.Path =
                FormsAuthentication.FormsCookiePath;

            context.Response.Cookies.Remove(
                FormsAuthentication.FormsCookieName);

            context.Response.Cookies.Add(cookie);
        }

        // =========================================================
        // SIGN OUT
        // =========================================================

        public static void SignOut()
        {
            HttpContext context =
                HttpContext.Current;

            FormsAuthentication.SignOut();

            if (context == null)
            {
                return;
            }

            HttpCookie cookie =
                new HttpCookie(
                    FormsAuthentication.FormsCookieName,
                    string.Empty);

            cookie.Expires =
                DateTime.Now.AddDays(-1);

            cookie.HttpOnly = true;

            cookie.Secure =
                context.Request.IsSecureConnection;

            cookie.Path =
                FormsAuthentication.FormsCookiePath;

            context.Response.Cookies.Set(cookie);

            if (context.Session != null)
            {
                context.Session.Clear();
                context.Session.Abandon();
            }
        }

        // =========================================================
        // REQUIRE LOGIN
        // =========================================================

        public static void RequireLogin()
        {
            if (IsLoggedIn)
            {
                return;
            }

            Redirect("~/Login.aspx");
        }

        // =========================================================
        // REQUIRE ADMIN
        // =========================================================

        public static void RequireAdmin()
        {
            RequireLogin();

            if (IsAdmin)
            {
                return;
            }

            Redirect("~/Dashboard.aspx");
        }

        // =========================================================
        // REQUIRE DONOR
        // =========================================================

        public static void RequireDonor()
        {
            RequireLogin();

            if (IsDonor)
            {
                return;
            }

            Redirect("~/Dashboard.aspx");
        }

        // =========================================================
        // REDIRECT
        // =========================================================

        private static void Redirect(
            string url)
        {
            HttpContext context =
                HttpContext.Current;

            if (context == null)
            {
                return;
            }

            context.Response.Redirect(
                url,
                false);

            context.ApplicationInstance
                .CompleteRequest();
        }

        // =========================================================
        // GET AUTHENTICATION TICKET
        // =========================================================

        private static FormsAuthenticationTicket GetTicket()
        {
            HttpContext context =
                HttpContext.Current;

            if (context == null ||
                context.User == null ||
                context.User.Identity == null)
            {
                return null;
            }

            FormsIdentity identity =
                context.User.Identity as FormsIdentity;

            if (identity == null)
            {
                return null;
            }

            return identity.Ticket;
        }

        // =========================================================
        // GET TICKET DATA
        // =========================================================

        private static string[] GetTicketValues()
        {
            FormsAuthenticationTicket ticket =
                GetTicket();

            if (ticket == null ||
                string.IsNullOrWhiteSpace(
                    ticket.UserData))
            {
                return new string[0];
            }

            return ticket.UserData.Split(
                new[] { TicketSeparator },
                StringSplitOptions.None);
        }
    }
}