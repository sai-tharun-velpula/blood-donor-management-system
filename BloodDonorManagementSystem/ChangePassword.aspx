<%@ Page Title="Change Password"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs"
    Inherits="BloodDonorManagementSystem.ChangePassword" %>

<asp:Content
    ID="TitleContent"
    ContentPlaceHolderID="TitleContent"
    runat="server">

    Change Password | BloodCare

</asp:Content>


<asp:Content
    ID="PageTitle"
    ContentPlaceHolderID="PageTitleContent"
    runat="server">

    Change Password

</asp:Content>


<asp:Content
    ID="Main"
    ContentPlaceHolderID="MainContent"
    runat="server">


    <div class="page-heading">

        <div>

            <div class="eyebrow eyebrow-dark">
                <span class="eyebrow-icon">🔐</span>
                Account Security
            </div>

            <h1>
                Change Password
            </h1>

            <p>
                Protect your BloodCare account with a
                strong private password.
            </p>

        </div>

    </div>


    <!-- =========================================================
         MESSAGE
         ========================================================= -->

    <asp:Panel
        ID="pnlNotice"
        runat="server"
        CssClass="message message-warning"
        Visible="false">

        <asp:Literal
            ID="litNotice"
            runat="server" />

    </asp:Panel>


    <!-- =========================================================
         PASSWORD CARD
         ========================================================= -->

    <div class="card password-card">


        <div class="card-header">

            <div>

                <div class="card-title">
                    Set a New Password
                </div>

                <div class="card-subtitle">
                    Create a password that only you know.
                </div>

            </div>

            <div class="card-header-icon icon-green">
                🔐
            </div>

        </div>


        <div class="card-body">


            <div class="password-help">

                <strong>
                    Password requirements
                </strong>

                <span>
                    At least 8 characters containing
                    letters, numbers and a special character.
                </span>

            </div>


            <div class="form-grid password-form">


                <!-- CURRENT PASSWORD -->

                <div class="field field-full">

                    <label>
                        Current / Temporary Password
                        <span class="required">*</span>
                    </label>

                    <asp:TextBox
                        ID="txtCurrentPassword"
                        runat="server"
                        CssClass="input"
                        TextMode="Password"
                        MaxLength="100"
                        autocomplete="current-password" />

                    <asp:RequiredFieldValidator
                        ID="valCurrent"
                        runat="server"
                        ControlToValidate="txtCurrentPassword"
                        ErrorMessage="Current password is required."
                        CssClass="validation"
                        Display="Dynamic" />

                </div>


                <!-- NEW PASSWORD -->

                <div class="field">

                    <label>
                        New Password
                        <span class="required">*</span>
                    </label>

                    <asp:TextBox
                        ID="txtNewPassword"
                        runat="server"
                        CssClass="input"
                        TextMode="Password"
                        MaxLength="100"
                        autocomplete="new-password" />

                    <asp:RequiredFieldValidator
                        ID="valNew"
                        runat="server"
                        ControlToValidate="txtNewPassword"
                        ErrorMessage="New password is required."
                        CssClass="validation"
                        Display="Dynamic" />

                </div>


                <!-- CONFIRM PASSWORD -->

                <div class="field">

                    <label>
                        Confirm New Password
                        <span class="required">*</span>
                    </label>

                    <asp:TextBox
                        ID="txtConfirmPassword"
                        runat="server"
                        CssClass="input"
                        TextMode="Password"
                        MaxLength="100"
                        autocomplete="new-password" />

                    <asp:RequiredFieldValidator
                        ID="valConfirm"
                        runat="server"
                        ControlToValidate="txtConfirmPassword"
                        ErrorMessage="Confirmation is required."
                        CssClass="validation"
                        Display="Dynamic" />

                    <asp:CompareValidator
                        ID="valMatch"
                        runat="server"
                        ControlToValidate="txtConfirmPassword"
                        ControlToCompare="txtNewPassword"
                        ErrorMessage="Passwords do not match."
                        CssClass="validation"
                        Display="Dynamic" />

                </div>


            </div>


            <div class="form-actions">

                <asp:Button
                    ID="btnChange"
                    runat="server"
                    Text="Change Password"
                    CssClass="btn btn-primary"
                    OnClick="btnChange_Click" />

            </div>


        </div>

    </div>

</asp:Content>