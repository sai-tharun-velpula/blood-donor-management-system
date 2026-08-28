<%@ Page Title="Donor Directory"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Donors.aspx.cs"
    Inherits="BloodDonorManagementSystem.Donors" %>

<asp:Content
    ID="TitleContent"
    ContentPlaceHolderID="TitleContent"
    runat="server">

    Donor Directory | BloodCare

</asp:Content>

<asp:Content
    ID="PageTitleContent"
    ContentPlaceHolderID="PageTitleContent"
    runat="server">

    Donor Directory

</asp:Content>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>

        /* =========================================================
           SEARCH FILTERS
           ========================================================= */

        .directory-search {
            display: grid;

            grid-template-columns:
                minmax(200px, 1.7fr)
                minmax(145px, 1fr)
                minmax(145px, 1fr)
                minmax(145px, 1fr)
                minmax(145px, 1fr)
                auto;

            gap: 10px;

            align-items: end;

            width: 100%;
        }

        .directory-search .field {
            min-width: 0;
        }

        .search-actions {
            display: flex;
            gap: 8px;
            align-items: center;
            min-width: 0;
        }

        .search-actions .btn {
            white-space: nowrap;
        }


        /* =========================================================
           DONOR TABLE
           ========================================================= */

        .donor-table td:first-child {
            font-weight: 400 !important;
        }
        
        .donor-name {
            font-size: 10px !important;
            font-weight: 400 !important;
            font-style: normal !important;
            text-decoration: none !important;
            color: #536174 !important;
        }


        /* =========================================================
           ACCOUNT STATUS
           ========================================================= */

        .account-active {
            display: inline-flex;
            align-items: center;
            justify-content: center;

            padding: 4px 9px;

            border-radius: 999px;

            background: #eaf9f0;
            color: #167a49;

            font-size: 8px;
            font-weight: 800;

            white-space: nowrap;
        }

        .account-inactive {
            display: inline-flex;
            align-items: center;
            justify-content: center;

            padding: 4px 9px;

            border-radius: 999px;

            background: #fff0f0;
            color: #a33c3c;

            font-size: 8px;
            font-weight: 800;

            white-space: nowrap;
        }


        /* =========================================================
           ACTION BUTTONS
           ========================================================= */

        .action-buttons {
            display: flex;
            align-items: center;
            gap: 6px;

            white-space: nowrap;
        }

        .action-buttons a,
        .action-buttons .action-button {
            display: inline-flex;

            align-items: center;
            justify-content: center;

            min-height: 32px;

            padding: 6px 10px;

            border-radius: 7px;
            border: 0;

            text-decoration: none;

            font-size: 9px;
            font-weight: 700;

            cursor: pointer;

            white-space: nowrap;
        }

        .action-edit {
            background: #18b66a;
            color: #fff !important;
        }

        .action-view {
            background: #eef2f5;
            color: #344256 !important;
        }

        .action-toggle {
            background: #fff0f0;
            color: #a33c3c !important;
        }

        .action-toggle.activate {
            background: #eaf9f0;
            color: #167a49 !important;
        }


        /* =========================================================
           DETAILS
           ========================================================= */

        .detail-panel {
            margin-top: 15px;
        }


        /* =========================================================
           EMPTY STATE
           ========================================================= */

        .donor-empty {
            margin-top: 15px;
        }


        /* =========================================================
           RESPONSIVE
           ========================================================= */

        @media (max-width: 1150px) {

            .directory-search {
                grid-template-columns:
                    repeat(3, minmax(0, 1fr));
            }

            .search-actions {
                grid-column: 1 / -1;
            }
        }

        @media (max-width: 750px) {

            .directory-search {
                grid-template-columns: 1fr;
            }

            .search-actions {
                grid-column: auto;
            }

            .search-actions .btn {
                flex: 1;
            }
        }

        @media (max-width: 500px) {

            .search-actions {
                flex-direction: column;
                width: 100%;
            }

            .search-actions .btn {
                width: 100%;
            }
        }

        @media (max-width: 700px) {

            .action-buttons {
                flex-wrap: wrap;
            }
        }

    </style>


    <!-- =========================================================
         PAGE HEADING
         ========================================================= -->

    <div class="page-heading">

        <div>

            <h1>

                <asp:Literal
                    ID="litPageTitle"
                    runat="server"
                    Text="Donor Directory" />

            </h1>

            <p>

                <asp:Literal
                    ID="litPageSubtitle"
                    runat="server"
                    Text="Search and manage registered blood donors." />

            </p>

        </div>


        <asp:HyperLink
            ID="lnkAddDonor"
            runat="server"
            NavigateUrl="~/DonorRegistration.aspx"
            CssClass="btn btn-primary"
            Visible="false">

            + Register Donor

        </asp:HyperLink>

    </div>


    <!-- =========================================================
         MESSAGE
         ========================================================= -->

    <asp:Panel
        ID="pnlMessage"
        runat="server"
        Visible="false"
        CssClass="message">

        <asp:Literal
            ID="litMessage"
            runat="server" />

    </asp:Panel>


    <!-- =========================================================
         SEARCH CARD
         ========================================================= -->

    <div class="card search-card">

        <div class="card-header">

            <div>

                <div class="card-title">
                    Find Donors
                </div>

                <div class="card-subtitle">
                    Search donors by name, contact, location,
                    blood group or availability.
                </div>

            </div>

            <div class="card-header-icon icon-red">
                🩸
            </div>

        </div>


        <div class="card-body">

            <div class="directory-search">


                <!-- =================================================
                     1. DONOR NAME
                     TEXTBOX
                     ================================================= -->

                <div class="field">

                    <label for="<%= txtSearch.ClientID %>">
                        Donor Name
                    </label>

                    <asp:TextBox
                        ID="txtSearch"
                        runat="server"
                        CssClass="input"
                        MaxLength="150"
                        placeholder="Name, mobile or email..." />

                </div>


                <!-- =================================================
                     2. BLOOD GROUP
                     DROPDOWN
                     ================================================= -->

                <div class="field">

                    <label for="<%= ddlFilterBloodGroup.ClientID %>">
                        Blood Group
                    </label>

                    <asp:DropDownList
                        ID="ddlFilterBloodGroup"
                        runat="server"
                        CssClass="select">

                        <asp:ListItem
                            Value=""
                            Text="All blood groups" />

                        <asp:ListItem
                            Value="A+"
                            Text="A+" />

                        <asp:ListItem
                            Value="A-"
                            Text="A-" />

                        <asp:ListItem
                            Value="B+"
                            Text="B+" />

                        <asp:ListItem
                            Value="B-"
                            Text="B-" />

                        <asp:ListItem
                            Value="AB+"
                            Text="AB+" />

                        <asp:ListItem
                            Value="AB-"
                            Text="AB-" />

                        <asp:ListItem
                            Value="O+"
                            Text="O+" />

                        <asp:ListItem
                            Value="O-"
                            Text="O-" />

                    </asp:DropDownList>

                </div>


                <!-- =================================================
                     3. CITY
                     TEXTBOX
                     ================================================= -->

                <div class="field">

                    <label for="<%= txtFilterCity.ClientID %>">
                        City
                    </label>

                    <asp:TextBox
                        ID="txtFilterCity"
                        runat="server"
                        CssClass="input"
                        MaxLength="100"
                        placeholder="e.g. Hyderabad" />

                </div>


                <!-- =================================================
                     4. STATE
                     TEXTBOX
                     ================================================= -->

                <div class="field">

                    <label for="<%= txtFilterState.ClientID %>">
                        State
                    </label>

                    <asp:TextBox
                        ID="txtFilterState"
                        runat="server"
                        CssClass="input"
                        MaxLength="100"
                        placeholder="e.g. Andhra Pradesh" />

                </div>


                <!-- =================================================
                     5. AVAILABILITY
                     DROPDOWN
                     ================================================= -->

                <div class="field">

                    <label for="<%= ddlAvailability.ClientID %>">
                        Availability
                    </label>

                    <asp:DropDownList
                        ID="ddlAvailability"
                        runat="server"
                        CssClass="select">

                        <asp:ListItem
                            Value=""
                            Text="All donors" />

                        <asp:ListItem
                            Value="1"
                            Text="Available" />

                        <asp:ListItem
                            Value="0"
                            Text="Unavailable" />

                    </asp:DropDownList>

                </div>


                <!-- =================================================
                     6 + 7. SEARCH / CLEAR
                     ================================================= -->

                <div class="search-actions">

                    <asp:Button
                        ID="btnSearch"
                        runat="server"
                        Text="Search"
                        CssClass="btn btn-primary"
                        OnClick="btnSearch_Click" />

                    <asp:Button
                        ID="btnClear"
                        runat="server"
                        Text="Clear"
                        CssClass="btn btn-light"
                        OnClick="btnClear_Click" />

                </div>


            </div>

        </div>

    </div>


    <!-- =========================================================
         REGISTERED DONORS
         ========================================================= -->

    <div class="card">

        <div class="card-header">

            <div>

                <div class="card-title">
                    Registered Donors
                </div>

                <div class="card-subtitle">

                    <asp:Literal
                        ID="litResultSummary"
                        runat="server"
                        Text="Registered donor records." />

                </div>

            </div>

            <div class="card-header-icon icon-red">
                🩸
            </div>

        </div>


        <div class="table-responsive">

            <asp:GridView
                ID="gvDonors"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="donor-table"
                GridLines="None"
                UseAccessibleHeader="true"
                DataKeyNames="DonorId"
                AllowPaging="true"
                PageSize="10"
                OnPageIndexChanging="gvDonors_PageIndexChanging"
                OnRowCommand="gvDonors_RowCommand"
                OnRowDataBound="gvDonors_RowDataBound">

                <Columns>



                    <asp:TemplateField
                        HeaderText="Donor">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblDonorName"
                                runat="server"
                                CssClass="donor-name" />

                        </ItemTemplate>

                    </asp:TemplateField>


                   

                    <asp:TemplateField
                        HeaderText="Blood">

                        <ItemTemplate>

                            <span class="blood-badge">

                                <asp:Label
                                    ID="lblBloodGroup"
                                    runat="server" />

                            </span>

                        </ItemTemplate>

                    </asp:TemplateField>


                    

                    <asp:TemplateField
                        HeaderText="Mobile">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblMobile"
                                runat="server" />

                        </ItemTemplate>

                    </asp:TemplateField>



                    <asp:TemplateField
                        HeaderText="Email">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblEmail"
                                runat="server" />

                        </ItemTemplate>

                    </asp:TemplateField>


                    

                    <asp:TemplateField
                        HeaderText="City">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblCity"
                                runat="server" />

                        </ItemTemplate>

                    </asp:TemplateField>


                    

                    <asp:TemplateField
                        HeaderText="State">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblState"
                                runat="server" />

                        </ItemTemplate>

                    </asp:TemplateField>


                    

                    <asp:TemplateField
                        HeaderText="Age">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblAge"
                                runat="server" />

                        </ItemTemplate>

                    </asp:TemplateField>


                    

                    <asp:TemplateField
                        HeaderText="Donation">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblAvailability"
                                runat="server" />

                        </ItemTemplate>

                    </asp:TemplateField>


                    

                    <asp:TemplateField
                        HeaderText="Account">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblAccountStatus"
                                runat="server" />

                        </ItemTemplate>

                    </asp:TemplateField>


                   

                    <asp:TemplateField
                        HeaderText="Actions">

                        <ItemTemplate>

                            <div class="action-buttons">


                                <asp:HyperLink
                                    ID="lnkEdit"
                                    runat="server"
                                    CssClass="action-edit">

                                    Edit

                                </asp:HyperLink>


                                <asp:LinkButton
                                    ID="btnView"
                                    runat="server"
                                    CommandName="ViewDonor"
                                    CssClass="action-button action-view">

                                    View

                                </asp:LinkButton>


                                <asp:LinkButton
                                    ID="btnToggleAccount"
                                    runat="server"
                                    CommandName="ToggleAccount"
                                    CssClass="action-button action-toggle">

                                    Toggle

                                </asp:LinkButton>


                            </div>

                        </ItemTemplate>

                    </asp:TemplateField>


                </Columns>

            </asp:GridView>

        </div>

    </div>


    

    <asp:Panel
        ID="pnlNoDonors"
        runat="server"
        Visible="false"
        CssClass="empty-state donor-empty">

        <div class="empty-icon">
            🩸
        </div>

        <strong>
            No donor records found.
        </strong>

        <span>
            Try changing the search or filters.
        </span>

    </asp:Panel>


   

    <asp:Panel
        ID="pnlDetails"
        runat="server"
        Visible="false"
        CssClass="card detail-panel">

        <div class="card-header">

            <div>

                <div class="card-title">
                    Donor Details
                </div>

                <div class="card-subtitle">
                    Selected donor information.
                </div>

            </div>


            <asp:Button
                ID="btnCloseDetails"
                runat="server"
                Text="Close"
                CssClass="btn btn-light"
                OnClick="btnCloseDetails_Click" />

        </div>


        <div class="card-body">

            <div class="form-grid">


                <div class="field">

                    <label>
                        Donor ID
                    </label>

                    <asp:Literal
                        ID="litDetailDonorId"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Full Name
                    </label>

                    <asp:Literal
                        ID="litDetailFullName"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Blood Group
                    </label>

                    <asp:Literal
                        ID="litDetailBloodGroup"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Gender
                    </label>

                    <asp:Literal
                        ID="litDetailGender"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Date of Birth
                    </label>

                    <asp:Literal
                        ID="litDetailDateOfBirth"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Age
                    </label>

                    <asp:Literal
                        ID="litDetailAge"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Mobile
                    </label>

                    <asp:Literal
                        ID="litDetailMobile"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Email
                    </label>

                    <asp:Literal
                        ID="litDetailEmail"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        City
                    </label>

                    <asp:Literal
                        ID="litDetailCity"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        State
                    </label>

                    <asp:Literal
                        ID="litDetailState"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Pincode
                    </label>

                    <asp:Literal
                        ID="litDetailPincode"
                        runat="server" />

                </div>


                <div class="field">

                    <label>
                        Availability
                    </label>

                    <asp:Literal
                        ID="litDetailAvailability"
                        runat="server" />

                </div>


                <div
                    class="field"
                    style="grid-column: 1 / -1;">

                    <label>
                        Address
                    </label>

                    <asp:Literal
                        ID="litDetailAddress"
                        runat="server" />

                </div>


                <div class="field">

                    <asp:HyperLink
                        ID="lnkDetailEdit"
                        runat="server"
                        CssClass="btn btn-primary">

                        Edit Donor

                    </asp:HyperLink>

                </div>


            </div>

        </div>

    </asp:Panel>

</asp:Content>
