<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="BloodDonorManagementSystem.Login" %>

<!DOCTYPE html>

<html lang="en">

<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
        content="width=device-width, initial-scale=1.0" />

    <title>Sign In | BloodCare</title>

    <link href="<%= ResolveUrl("~/Content/site.css") %>"
        rel="stylesheet" />

</head>

<body class="login-body">

<form id="form1"
    runat="server">

    <div class="login-page">

        <!-- =====================================================
             LEFT VISUAL PANEL
             ===================================================== -->

        <section class="login-visual">

            <div class="login-visual-inner">

                <div class="login-brand-row">

                    <div class="brand-mark">
                        ♥
                    </div>

                    <div>
                        <strong>BloodCare</strong>

                        <small>
                            Donor Management System
                        </small>
                    </div>

                </div>


                <div class="login-kicker">

                    <span></span>

                    Secure donor operations

                </div>


                <h1>
                    Every donor can
                    <br />
                    <strong>make a difference.</strong>
                </h1>


                <p>
                    Manage donor registrations, blood groups,
                    locations and availability from one secure,
                    professional workspace.
                </p>


                <div class="login-feature-grid">

                    <div>

                        <span>✓</span>

                        <strong>
                            Live donor data
                        </strong>

                        <small>
                            Connected to SQL Server
                        </small>

                    </div>


                    <div>

                        <span>✓</span>

                        <strong>
                            Role-based access
                        </strong>

                        <small>
                            Admin and donor workspaces
                        </small>

                    </div>


                    <div>

                        <span>✓</span>

                        <strong>
                            Protected authentication
                        </strong>

                        <small>
                            ASP.NET Forms Authentication
                        </small>

                    </div>

                </div>

            </div>

        </section>


        <!-- =====================================================
             LOGIN PANEL
             ===================================================== -->

        <section class="login-panel">

            <div class="login-box">

                <div class="login-box-logo">
                    ♥
                </div>


                <div class="login-box-kicker">
                    WELCOME BACK
                </div>


                <h2>
                    Sign in to BloodCare
                </h2>


                <p class="hint">
                    Use your authorized account to continue.
                </p>


                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="message"
                    Visible="false" />


                <!-- USERNAME -->

                <div class="field">

                    <label
                        for="<%= txtUsername.ClientID %>">

                        Username

                    </label>

                    <asp:TextBox
                        ID="txtUsername"
                        runat="server"
                        CssClass="input"
                        MaxLength="100"
                        autocomplete="username"
                        placeholder="Enter username" />

                </div>


                <!-- PASSWORD -->

                <div class="field">

                    <label
                        for="<%= txtPassword.ClientID %>">

                        Password

                    </label>

                    <asp:TextBox
                        ID="txtPassword"
                        runat="server"
                        CssClass="input"
                        TextMode="Password"
                        MaxLength="100"
                        autocomplete="current-password"
                        placeholder="Enter password" />

                </div>


                <!-- LOGIN -->

                <asp:Button
                    ID="btnLogin"
                    runat="server"
                    Text="Sign In →"
                    CssClass="btn btn-primary btn-login"
                    OnClick="btnLogin_Click" />


                <div class="login-security-note">

                    <span>✓</span>

                    <span>
                        Your credentials are protected using
                        secure password hashing and ASP.NET
                        Forms Authentication.
                    </span>

                </div>

            </div>

        </section>

    </div>

</form>

</body>

</html>