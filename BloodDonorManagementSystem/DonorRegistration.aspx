<%@ Page
Title="Donor Registration"
Language="C#"
MasterPageFile="~/Site.Master"
AutoEventWireup="true"
CodeBehind="DonorRegistration.aspx.cs"
Inherits="BloodDonorManagementSystem.DonorRegistration" %>

<asp:Content
ID="TitleContent"
ContentPlaceHolderID="TitleContent"
runat="server">
Donor Registration | BloodCare
</asp:Content>

<asp:Content
ID="PageTitle"
ContentPlaceHolderID="PageTitleContent"
runat="server">
Donor Registration
</asp:Content>

<asp:Content
ID="Main"
ContentPlaceHolderID="MainContent"
runat="server">

<style type="text/css"> 
 
    .registration-page { 
        width: 100%; 
        max-width: 1120px; 
        margin: 0 auto; 
        padding: 10px 0 35px; 
        box-sizing: border-box; 
    } 
 
    .registration-header { 
        display: flex; 
        align-items: center; 
        justify-content: space-between; 
        gap: 20px; 
        margin-bottom: 20px; 
    } 
 
    .registration-heading { 
        display: flex; 
        align-items: center; 
        gap: 13px; 
    } 
 
    .registration-heading-icon { 
        width: 46px; 
        height: 46px; 
        flex: 0 0 46px; 
        display: flex; 
        align-items: center; 
        justify-content: center; 
        border-radius: 12px; 
        background: #fef2f2; 
        color: #dc2626; 
        font-size: 21px; 
        font-weight: 700; 
    } 
 
    .registration-heading h1 { 
        margin: 0; 
        color: #111827; 
        font-size: 24px; 
        line-height: 1.25; 
        font-weight: 700; 
    } 
 
    .registration-heading p { 
        margin: 4px 0 0; 
        color: #6b7280; 
        font-size: 13px; 
        line-height: 1.4; 
    } 
 
    .registration-message { 
        margin-bottom: 16px; 
        padding: 11px 14px; 
        border: 1px solid #fecaca; 
        border-radius: 8px; 
        background: #fef2f2; 
        color: #991b1b; 
        font-size: 13px; 
        line-height: 1.45; 
    } 
 
    .registration-card { 
        overflow: hidden; 
        border: 1px solid #e5e7eb; 
        border-radius: 12px; 
        background: #ffffff; 
        box-shadow: 0 2px 10px rgba(15, 23, 42, 0.05); 
    } 
 
    .registration-card-header { 
        display: flex; 
        align-items: center; 
        justify-content: space-between; 
        gap: 15px; 
        padding: 17px 20px; 
        border-bottom: 1px solid #eef0f3; 
        background: #ffffff; 
    } 
 
    .registration-card-header-text { 
        min-width: 0; 
    } 
 
    .registration-card-title { 
        margin: 0; 
        color: #111827; 
        font-size: 16px; 
        line-height: 1.3; 
        font-weight: 700; 
    } 
 
    .registration-card-description { 
        margin-top: 4px; 
        color: #6b7280; 
        font-size: 12px; 
        line-height: 1.4; 
    } 
 
    .registration-card-icon { 
        width: 37px; 
        height: 37px; 
        flex: 0 0 37px; 
        display: flex; 
        align-items: center; 
        justify-content: center; 
        border-radius: 9px; 
        background: #fef2f2; 
        color: #dc2626; 
        font-size: 17px; 
    } 
 
    .registration-card-body { 
        padding: 20px; 
    } 
 
    .form-section { 
        margin-bottom: 23px; 
    } 
 
    .form-section:last-child { 
        margin-bottom: 0; 
    } 
 
    .form-section-header { 
        display: flex; 
        align-items: center; 
        gap: 9px; 
        margin-bottom: 14px; 
        padding-bottom: 8px; 
        border-bottom: 1px solid #eef0f3; 
    } 
 
    .form-section-marker { 
        width: 3px; 
        height: 17px; 
        flex: 0 0 3px; 
        border-radius: 3px; 
        background: #dc2626; 
    } 
 
    .form-section-title { 
        margin: 0; 
        color: #1f2937; 
        font-size: 14px; 
        font-weight: 700; 
    } 
 
    .form-section-description { 
        margin-left: auto; 
        color: #9ca3af; 
        font-size: 11px; 
    } 
 
    .form-grid { 
        display: grid; 
        grid-template-columns: repeat(2, minmax(0, 1fr)); 
        column-gap: 18px; 
        row-gap: 14px; 
    } 
 
    .form-field { 
        min-width: 0; 
    } 
 
    .form-field-full { 
        grid-column: 1 / -1; 
    } 
 
    .form-label { 
        display: block; 
        margin: 0 0 6px; 
        color: #374151; 
        font-size: 12px; 
        line-height: 1.3; 
        font-weight: 600; 
    } 
 
    .required-mark { 
        margin-left: 2px; 
        color: #dc2626; 
    } 
 
    .form-input { 
        width: 100%; 
        height: 40px; 
        box-sizing: border-box; 
        padding: 8px 11px; 
        border: 1px solid #d1d5db; 
        border-radius: 7px; 
        outline: none; 
        background: #ffffff; 
        color: #111827; 
        font-family: inherit; 
        font-size: 13px; 
        line-height: 1.3; 
        transition: 
            border-color 0.15s ease, 
            box-shadow 0.15s ease, 
            background 0.15s ease; 
    } 
 
    .form-input:hover { 
        border-color: #9ca3af; 
    } 
 
    .form-input:focus { 
        border-color: #dc2626; 
        box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.08); 
    } 
 
    .form-input::placeholder { 
        color: #9ca3af; 
    } 
 
    .form-input:disabled { 
        background: #f9fafb; 
        color: #9ca3af; 
        cursor: not-allowed; 
    } 
 
    select.form-input { 
        cursor: pointer; 
    } 
 
    .address-input { 
        height: 82px !important; 
        min-height: 82px; 
        max-height: 130px; 
        resize: vertical; 
        line-height: 1.45; 
    } 
 
    .field-help { 
        margin-top: 4px; 
        color: #9ca3af; 
        font-size: 10px; 
        line-height: 1.35; 
    } 
 
    .validation-message { 
        display: block; 
        margin-top: 4px; 
        color: #dc2626; 
        font-size: 11px; 
        line-height: 1.3; 
    } 
 
    .availability-box { 
        padding: 13px 14px; 
        border: 1px solid #e5e7eb; 
        border-radius: 8px; 
        background: #f9fafb; 
    } 
 
    .availability-checkbox { 
        display: inline-flex; 
        align-items: center; 
        gap: 7px; 
        color: #374151; 
        font-size: 13px; 
        font-weight: 600; 
        cursor: pointer; 
    } 
 
    .availability-checkbox input[type="checkbox"] { 
        width: 16px; 
        height: 16px; 
        margin: 0; 
        accent-color: #dc2626; 
        cursor: pointer; 
    } 
 
    .availability-help { 
        margin-top: 5px; 
        margin-left: 23px; 
        color: #6b7280; 
        font-size: 11px; 
        line-height: 1.4; 
    } 
 
    .form-actions { 
        display: flex; 
        align-items: center; 
        justify-content: flex-end; 
        gap: 9px; 
        margin-top: 22px; 
        padding-top: 17px; 
        border-top: 1px solid #eef0f3; 
    } 
 
    .form-button { 
        min-width: 108px; 
        height: 39px; 
        padding: 0 17px; 
        display: inline-flex; 
        align-items: center; 
        justify-content: center; 
        box-sizing: border-box; 
        border: 1px solid transparent; 
        border-radius: 7px; 
        font-family: inherit; 
        font-size: 13px; 
        font-weight: 600; 
        text-decoration: none; 
        cursor: pointer; 
    } 
 
    .button-primary { 
        border-color: #dc2626; 
        background: #dc2626; 
        color: #ffffff !important; 
    } 
 
    .button-primary:hover { 
        border-color: #b91c1c; 
        background: #b91c1c; 
        color: #ffffff !important; 
    } 
 
    .button-secondary { 
        border-color: #d1d5db; 
        background: #ffffff; 
        color: #374151 !important; 
    } 
 
    .button-secondary:hover { 
        border-color: #9ca3af; 
        background: #f9fafb; 
        color: #111827 !important; 
    } 
 
    @media (max-width: 760px) { 
 
        .registration-page { 
            padding: 6px 0 25px; 
        } 
 
        .registration-heading { 
            gap: 10px; 
        } 
 
        .registration-heading-icon { 
            width: 40px; 
            height: 40px; 
            flex-basis: 40px; 
            border-radius: 10px; 
            font-size: 18px; 
        } 
 
        .registration-heading h1 { 
            font-size: 21px; 
        } 
 
        .registration-heading p { 
            font-size: 12px; 
        } 
 
        .registration-card-header { 
            padding: 15px 16px; 
        } 
 
        .registration-card-body { 
            padding: 16px; 
        } 
 
        .form-grid { 
            grid-template-columns: 1fr; 
            row-gap: 13px; 
        } 
 
        .form-field-full { 
            grid-column: auto; 
        } 
 
        .form-section-description { 
            display: none; 
        } 
 
        .form-actions { 
            justify-content: stretch; 
        } 
 
        .form-button { 
            flex: 1; 
        } 
    } 
 
    @media (max-width: 430px) { 
 
        .registration-heading h1 { 
            font-size: 19px; 
        } 
 
        .registration-heading p { 
            display: none; 
        } 
 
        .registration-card-icon { 
            display: none; 
        } 
 
        .form-actions { 
            flex-direction: column-reverse; 
        } 
 
        .form-button { 
            width: 100%; 
            flex: none; 
        } 
    } 
 
</style> 

<div class="registration-page"> 

<!-- PAGE HEADER --> 

<div class="registration-header"> 

    <div class="registration-heading"> 

        <div class="registration-heading-icon"> 
            ♥ 
        </div> 

        <div> 

            <h1> 
                <asp:Literal 
                    ID="litPageTitle" 
                    runat="server" 
                    Text="Donor Registration" /> 
            </h1> 

            <p> 
                <asp:Literal 
                    ID="litPageSubtitle" 
                    runat="server" 
                    Text="Create a new donor registration." /> 
            </p> 

        </div> 

    </div> 

</div> 


<!-- MESSAGE --> 

<asp:Panel 
    ID="pnlMessage" 
    runat="server" 
    CssClass="registration-message" 
    Visible="false"> 

    <asp:Literal 
        ID="litMessage" 
        runat="server" /> 

</asp:Panel> 


<!-- CREDENTIALS --> 

<asp:Panel ID="pnlCredentials" 
    runat="server" 
    Visible="false" 
    CssClass="credential-panel"> 

    <div class="credential-title"> 
        Donor Account Created 
    </div> 

    <div class="credential-row"> 
        <span class="credential-label">Donor Name:</span> 
        <asp:Literal ID="litCredentialDonorName" 
            runat="server" /> 
    </div> 

    <div class="credential-row"> 
        <span class="credential-label">Username:</span> 
        <asp:Literal ID="litCredentialUsername" 
            runat="server" /> 
    </div> 

    <div class="credential-row"> 
        <span class="credential-label">Temporary Password:</span> 
        <asp:Literal ID="litCredentialPassword" 
            runat="server" /> 
    </div> 

    <div class="credential-warning"> 
        These credentials are displayed for 1 minute. 
        Please save them securely. 
    </div> 

    <div id="credentialCountdown" 
        class="credential-countdown"> 
        Redirecting in 60 seconds... 
    </div> 

</asp:Panel> 


<!-- MAIN CARD --> 

<div class="registration-card"> 

    <div class="registration-card-header"> 

        <div class="registration-card-header-text"> 

            <div class="registration-card-title"> 
                Donor Information 
            </div> 

            <div class="registration-card-description"> 
                Enter all required donor information. 
            </div> 

        </div> 

        <div class="registration-card-icon"> 
            ♥ 
        </div> 

    </div> 


    <div class="registration-card-body"> 


        <!-- PERSONAL INFORMATION --> 

        <div class="form-section"> 

            <div class="form-section-header"> 

                <span class="form-section-marker"></span> 

                <h2 class="form-section-title"> 
                    Personal Information 
                </h2> 

                <span class="form-section-description"> 
                    Basic donor details 
                </span> 

            </div> 


            <div class="form-grid"> 


                <!-- FULL NAME --> 

                <div class="form-field form-field-full"> 

                    <label 
                        class="form-label" 
                        for="<%= txtFullName.ClientID %>"> 

                        Full Name 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:TextBox 
                        ID="txtFullName" 
                        runat="server" 
                        CssClass="form-input" 
                        MaxLength="150" 
                        placeholder="Enter donor's full name" /> 

                    <asp:RequiredFieldValidator 
                        ID="valFullName" 
                        runat="server" 
                        ControlToValidate="txtFullName" 
                        ErrorMessage="Full name is required." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <asp:RegularExpressionValidator 
                        ID="valFullNameFormat" 
                        runat="server" 
                        ControlToValidate="txtFullName" 
                        ValidationExpression="^[A-Za-z]+(?:[ .'-][A-Za-z]+)*$" 
                        ErrorMessage="Full name must contain letters only with valid spaces, dots, apostrophes or hyphens." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                </div> 


                <!-- BLOOD GROUP --> 

                <div class="form-field"> 

                    <label 
                        class="form-label" 
                        for="<%= ddlBloodGroup.ClientID %>"> 

                        Blood Group 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:DropDownList 
                        ID="ddlBloodGroup" 
                        runat="server" 
                        CssClass="form-input"> 

                        <asp:ListItem 
                            Text="Select blood group" 
                            Value="" /> 

                        <asp:ListItem Text="A+" Value="A+" /> 
                        <asp:ListItem Text="A-" Value="A-" /> 
                        <asp:ListItem Text="B+" Value="B+" /> 
                        <asp:ListItem Text="B-" Value="B-" /> 
                        <asp:ListItem Text="AB+" Value="AB+" /> 
                        <asp:ListItem Text="AB-" Value="AB-" /> 
                        <asp:ListItem Text="O+" Value="O+" /> 
                        <asp:ListItem Text="O-" Value="O-" /> 

                    </asp:DropDownList> 

                    <asp:RequiredFieldValidator 
                        ID="valBloodGroup" 
                        runat="server" 
                        ControlToValidate="ddlBloodGroup" 
                        InitialValue="" 
                        ErrorMessage="Please select a blood group." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                </div> 


                <!-- GENDER --> 

                <div class="form-field"> 

                    <label 
                        class="form-label" 
                        for="<%= ddlGender.ClientID %>"> 

                        Gender 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:DropDownList 
                        ID="ddlGender" 
                        runat="server" 
                        CssClass="form-input"> 

                        <asp:ListItem 
                            Text="Select gender" 
                            Value="" /> 

                        <asp:ListItem 
                            Text="Male" 
                            Value="Male" /> 

                        <asp:ListItem 
                            Text="Female" 
                            Value="Female" /> 

                        <asp:ListItem 
                            Text="Other" 
                            Value="Other" /> 

                    </asp:DropDownList> 

                    <asp:RequiredFieldValidator 
                        ID="valGender" 
                        runat="server" 
                        ControlToValidate="ddlGender" 
                        InitialValue="" 
                        ErrorMessage="Please select a gender." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                </div> 


                <!-- DATE OF BIRTH --> 

                <div class="form-field"> 

                    <label 
                        class="form-label" 
                        for="<%= txtDateOfBirth.ClientID %>"> 

                        Date of Birth 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:TextBox 
                        ID="txtDateOfBirth" 
                        runat="server" 
                        CssClass="form-input" 
                        TextMode="Date" /> 

                    <asp:RequiredFieldValidator 
                        ID="valDateOfBirth" 
                        runat="server" 
                        ControlToValidate="txtDateOfBirth" 
                        ErrorMessage="Date of birth is required." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <asp:CustomValidator 
                        ID="valDateOfBirthAge" 
                        runat="server" 
                        ControlToValidate="txtDateOfBirth" 
                        ValidateEmptyText="true" 
                        ErrorMessage="Donor age must be between 18 and 65 years." 
                        CssClass="validation-message" 
                        Display="Dynamic" 
                        OnServerValidate="ValidateDateOfBirth" /> 

                    <div class="field-help"> 
                        Donor must be between 18 and 65 years of age. 
                    </div> 

                </div> 


                <!-- MOBILE --> 

                <div class="form-field"> 

                    <label 
                        class="form-label" 
                        for="<%= txtMobile.ClientID %>"> 

                        Mobile Number 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:TextBox 
                        ID="txtMobile" 
                        runat="server" 
                        CssClass="form-input" 
                        MaxLength="10" 
                        placeholder="Enter 10-digit mobile number" 
                        inputmode="numeric" /> 

                    <asp:RequiredFieldValidator 
                        ID="valMobile" 
                        runat="server" 
                        ControlToValidate="txtMobile" 
                        ErrorMessage="Mobile number is required." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <asp:RegularExpressionValidator 
                        ID="valMobileFormat" 
                        runat="server" 
                        ControlToValidate="txtMobile" 
                        ValidationExpression="^[6-9][0-9]{9}$" 
                        ErrorMessage="Mobile number must contain exactly 10 digits and start with 6, 7, 8, or 9." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <div class="field-help"> 
                        Numbers only. Exactly 10 digits. Must start with 6, 7, 8, or 9. 
                    </div> 

                </div> 


                <!-- EMAIL --> 

                <div class="form-field"> 

                    <label 
                        class="form-label" 
                        for="<%= txtEmail.ClientID %>"> 

                        Email Address 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:TextBox 
                        ID="txtEmail" 
                        runat="server" 
                        CssClass="form-input" 
                        MaxLength="255" 
                        TextMode="Email" 
                        placeholder="name@example.com" /> 

                    <asp:RequiredFieldValidator 
                        ID="valEmailRequired" 
                        runat="server" 
                        ControlToValidate="txtEmail" 
                        ErrorMessage="Email address is required." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <asp:RegularExpressionValidator 
                        ID="valEmail" 
                        runat="server" 
                        ControlToValidate="txtEmail" 
                        ValidationExpression="^[A-Za-z0-9.!#$%&amp;'*+/=?^_`{|}~-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)+$" 
                        ErrorMessage="Enter a valid email address, for example david.john01@gmail.com." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <div class="field-help"> 
                        Letters, numbers and valid email symbols are allowed. 
                    </div> 

                </div> 

            </div> 

        </div> 


        <!-- CONTACT INFORMATION --> 

        <div class="form-section"> 

            <div class="form-section-header"> 

                <span class="form-section-marker"></span> 

                <h2 class="form-section-title"> 
                    Contact Information 
                </h2> 

                <span class="form-section-description"> 
                    Donor location 
                </span> 

            </div> 


            <div class="form-grid"> 


                <!-- ADDRESS --> 

                <div class="form-field form-field-full"> 

                    <label 
                        class="form-label" 
                        for="<%= txtAddress.ClientID %>"> 

                        Address 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:TextBox 
                        ID="txtAddress" 
                        runat="server" 
                        CssClass="form-input address-input" 
                        MaxLength="500" 
                        TextMode="MultiLine" 
                        Rows="3" 
                        placeholder="Enter donor's complete address" /> 

                    <asp:RequiredFieldValidator 
                        ID="valAddress" 
                        runat="server" 
                        ControlToValidate="txtAddress" 
                        ErrorMessage="Address is required." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <asp:RegularExpressionValidator 
                        ID="valAddressFormat" 
                        runat="server" 
                        ControlToValidate="txtAddress" 
                        ValidationExpression="^[A-Za-z0-9][A-Za-z0-9\s,./#'()\-]*$" 
                        ErrorMessage="Enter a valid address using letters, numbers, spaces and normal address symbols." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <div class="field-help"> 
                        Example: 23 Main Street, New York, NY 10501, USA or Room No 101, 2/12 Floor, ASSASD, NASD, JAMAIKA. 
                    </div> 

                </div> 


                <!-- CITY --> 

                <div class="form-field"> 

                    <label 
                        class="form-label" 
                        for="<%= txtCity.ClientID %>"> 

                        City 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:TextBox 
                        ID="txtCity" 
                        runat="server" 
                        CssClass="form-input" 
                        MaxLength="100" 
                        placeholder="Enter city" /> 

                    <asp:RequiredFieldValidator 
                        ID="valCity" 
                        runat="server" 
                        ControlToValidate="txtCity" 
                        ErrorMessage="City is required." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <asp:RegularExpressionValidator 
                        ID="valCityFormat" 
                        runat="server" 
                        ControlToValidate="txtCity" 
                        ValidationExpression="^[A-Za-z]+(?:[ .'-][A-Za-z]+)*$" 
                        ErrorMessage="City must contain text only." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                </div> 


                <!-- STATE --> 

                <div class="form-field"> 

                    <label 
                        class="form-label" 
                        for="<%= txtState.ClientID %>"> 

                        State 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:TextBox 
                        ID="txtState" 
                        runat="server" 
                        CssClass="form-input" 
                        MaxLength="100" 
                        placeholder="Enter state" /> 

                    <asp:RequiredFieldValidator 
                        ID="valState" 
                        runat="server" 
                        ControlToValidate="txtState" 
                        ErrorMessage="State is required." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <asp:RegularExpressionValidator 
                        ID="valStateFormat" 
                        runat="server" 
                        ControlToValidate="txtState" 
                        ValidationExpression="^[A-Za-z]+(?:[ .'-][A-Za-z]+)*$" 
                        ErrorMessage="State must contain text only." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                </div> 


                <!-- PINCODE --> 

                <div class="form-field"> 

                    <label 
                        class="form-label" 
                        for="<%= txtPincode.ClientID %>"> 

                        Pincode 
                        <span class="required-mark">*</span> 

                    </label> 

                    <asp:TextBox 
                        ID="txtPincode" 
                        runat="server" 
                        CssClass="form-input" 
                        MaxLength="6" 
                        placeholder="Enter 6-digit pincode" 
                        inputmode="numeric" /> 

                    <asp:RequiredFieldValidator 
                        ID="valPincode" 
                        runat="server" 
                        ControlToValidate="txtPincode" 
                        ErrorMessage="Pincode is required." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <asp:RegularExpressionValidator 
                        ID="valPincodeFormat" 
                        runat="server" 
                        ControlToValidate="txtPincode" 
                        ValidationExpression="^[0-9]{6}$" 
                        ErrorMessage="Pincode must contain exactly 6 digits." 
                        CssClass="validation-message" 
                        Display="Dynamic" /> 

                    <div class="field-help"> 
                        Numbers only. Exactly 6 digits. 
                    </div> 

                </div> 

            </div> 

        </div> 


        <!-- AVAILABILITY --> 

        <div class="form-section"> 

            <div class="form-section-header"> 

                <span class="form-section-marker"></span> 

                <h2 class="form-section-title"> 
                    Availability 
                </h2> 

                <span class="form-section-description"> 
                    Donor availability status 
                </span> 

            </div> 


            <div class="availability-box"> 

                <asp:CheckBox 
                    ID="chkAvailable" 
                    runat="server" 
                    Text="Currently available to donate blood" 
                    CssClass="availability-checkbox" 
                    Checked="false" /> 

                <div class="availability-help"> 
                    Availability is optional and is OFF by default. 
                    Leave unchecked if the donor is currently unavailable. 
                </div> 

            </div> 

        </div> 


        <!-- ACTIONS --> 

        <div class="form-actions"> 

            <asp:HyperLink 
                ID="lnkCancel" 
                runat="server" 
                NavigateUrl="~/Donors.aspx" 
                CssClass="form-button button-secondary"> 

                Cancel 

            </asp:HyperLink> 

            <asp:Button 
                ID="btnSave" 
                runat="server" 
                Text="Create Donor" 
                CssClass="form-button button-primary" 
                OnClick="btnSave_Click" /> 

        </div> 


    </div> 

</div> 

</div> 

</asp:Content>
