<%@ Page
    Title="Donor Dashboard"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="DonorDashboard.aspx.cs"
    Inherits="BloodDonorManagementSystem.DonorDashboard" %>

<asp:Content
    ID="TitleContent"
    ContentPlaceHolderID="TitleContent"
    runat="server">
    Donor Dashboard | BloodCare
</asp:Content>

<asp:Content
    ID="PageTitle"
    ContentPlaceHolderID="PageTitleContent"
    runat="server">
    Donor Dashboard
</asp:Content>

<asp:Content
    ID="Main"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <!-- =========================================================
         WELCOME
         ========================================================= -->

    <div class="dashboard-welcome donor-welcome">

        <div>

            <div class="eyebrow">
                <span class="status-dot"></span>
                Donor workspace
            </div>

            <h1>
                Welcome back,
                <strong>
                    <asp:Literal
                        ID="litWelcomeName"
                        runat="server" />
                </strong>
                👋
            </h1>

            <p>
                Manage your donor profile, availability and account securely.
            </p>

        </div>

        <asp:HyperLink
            ID="lnkWelcomeAction"
            runat="server"
            CssClass="btn btn-primary btn-lg"
            NavigateUrl="~/DonorRegistration.aspx">

            ✎ Update Profile

        </asp:HyperLink>

    </div>


    <!-- =========================================================
         MESSAGE
         ========================================================= -->

    <asp:Panel
        ID="pnlMessage"
        runat="server"
        CssClass="message"
        Visible="false">

        <asp:Literal
            ID="litMessage"
            runat="server" />

    </asp:Panel>


    <!-- =========================================================
         NO PROFILE STATE
         ========================================================= -->

    <asp:Panel
        ID="pnlNoProfile"
        runat="server"
        Visible="false"
        CssClass="card donor-empty-state">

        <div class="card-body">

            <div class="donor-empty-icon">
                ♥
            </div>

            <div class="donor-empty-content">

                <h2>
                    Your donor profile is not registered yet
                </h2>

                <p>
                    Your account is ready, but we could not find a donor
                    profile linked to it. Complete your donor registration
                    to add your blood group, contact information,
                    location and availability.
                </p>

                <asp:HyperLink
                    ID="lnkCompleteRegistration"
                    runat="server"
                    CssClass="btn btn-primary"
                    NavigateUrl="~/DonorRegistration.aspx">

                    Complete Donor Registration →

                </asp:HyperLink>

            </div>

        </div>

    </asp:Panel>


    <!-- =========================================================
         REGISTERED DONOR DASHBOARD
         ========================================================= -->

    <asp:Panel
        ID="pnlDashboard"
        runat="server"
        Visible="false">


        <!-- =====================================================
             KPI CARDS
             ===================================================== -->

        <div class="kpi-grid donor-kpi-grid">

            <!-- BLOOD GROUP -->

            <div class="kpi-card kpi-red">

                <div class="kpi-icon">
                    ♥
                </div>

                <div>

                    <span class="kpi-label">
                        Blood Group
                    </span>

                    <strong>
                        <asp:Literal
                            ID="litKpiBlood"
                            runat="server" />
                    </strong>

                    <small>
                        Your registered blood type
                    </small>

                </div>

            </div>


            <!-- AVAILABILITY -->

            <div class="kpi-card kpi-green">

                <div class="kpi-icon">
                    ✓
                </div>

                <div>

                    <span class="kpi-label">
                        Availability
                    </span>

                    <strong>
                        <asp:Literal
                            ID="litKpiAvailability"
                            runat="server" />
                    </strong>

                    <small>
                        Current donation status
                    </small>

                </div>

            </div>


            <!-- LOCATION -->

            <div class="kpi-card kpi-blue">

                <div class="kpi-icon">
                    ⌂
                </div>

                <div>

                    <span class="kpi-label">
                        Location
                    </span>

                    <strong>
                        <asp:Literal
                            ID="litKpiCity"
                            runat="server" />
                    </strong>

                    <small>
                        <asp:Literal
                            ID="litKpiState"
                            runat="server" />
                    </small>

                </div>

            </div>


            <!-- MEMBER SINCE -->

            <div class="kpi-card kpi-purple">

                <div class="kpi-icon">
                    ID
                </div>

                <div>

                    <span class="kpi-label">
                        Member Since
                    </span>

                    <strong>
                        <asp:Literal
                            ID="litKpiDate"
                            runat="server" />
                    </strong>

                    <small>
                        BloodCare donor account
                    </small>

                </div>

            </div>

        </div>


        <!-- =====================================================
             MAIN DASHBOARD GRID
             ===================================================== -->

        <div class="dashboard-grid dashboard-grid-top donor-dashboard-grid">


            <!-- =================================================
                 DONOR PROFILE
                 ================================================= -->

            <div class="card">

                <div class="card-header">

                    <div>

                        <div class="card-title">
                            My Donor Profile
                        </div>

                        <div class="card-subtitle">
                            Your complete registered donor information
                        </div>

                    </div>

                    <div class="card-header-icon icon-red">
                        ♥
                    </div>

                </div>


                <div class="card-body">


                    <!-- PROFILE HEADER -->

                    <div class="donor-profile-header">

                        <div class="donor-avatar">

                            <asp:Literal
                                ID="litInitial"
                                runat="server" />

                        </div>

                        <div>

                            <div class="donor-name">

                                <asp:Literal
                                    ID="litFullName"
                                    runat="server" />

                            </div>

                            <div class="donor-role">
                                Registered Blood Donor
                            </div>

                        </div>

                    </div>


                    <!-- PROFILE DETAILS -->

                    <div class="donor-details-grid">


                        <!-- BLOOD GROUP -->

                        <div class="donor-detail">

                            <div class="donor-detail-label">
                                Blood Group
                            </div>

                            <div class="donor-detail-value">

                                <span class="blood-badge">

                                    <asp:Literal
                                        ID="litBloodGroup"
                                        runat="server" />

                                </span>

                            </div>

                        </div>


                        <!-- MOBILE -->

                        <div class="donor-detail">

                            <div class="donor-detail-label">
                                Mobile
                            </div>

                            <div class="donor-detail-value">

                                <asp:Literal
                                    ID="litMobile"
                                    runat="server" />

                            </div>

                        </div>


                        <!-- ADDRESS -->

                        <div class="donor-detail donor-address">

                            <div class="donor-detail-label">
                                Full Address
                            </div>

                            <div class="donor-detail-value">

                                <asp:Literal
                                    ID="litAddress"
                                    runat="server" />

                            </div>

                        </div>


                        <!-- CITY -->

                        <div class="donor-detail">

                            <div class="donor-detail-label">
                                City
                            </div>

                            <div class="donor-detail-value">

                                <asp:Literal
                                    ID="litCity"
                                    runat="server" />

                            </div>

                        </div>


                        <!-- STATE / PINCODE -->

                        <div class="donor-detail">

                            <div class="donor-detail-label">
                                State / Pincode
                            </div>

                            <div class="donor-detail-value">

                                <asp:Literal
                                    ID="litState"
                                    runat="server" />

                            </div>

                        </div>


                        <!-- AVAILABILITY -->

                        <div class="donor-detail donor-availability">

                            <div class="donor-detail-label">
                                Donation Availability
                            </div>

                            <div class="donor-detail-value">

                                <asp:Literal
                                    ID="litAvailability"
                                    runat="server" />

                            </div>

                        </div>

                    </div>

                </div>

            </div>


            <!-- =================================================
                 QUICK ACTIONS
                 ================================================= -->

            <div class="card">

                <div class="card-header">

                    <div>

                        <div class="card-title">
                            Quick Actions
                        </div>

                        <div class="card-subtitle">
                            Common donor tasks
                        </div>

                    </div>

                    <div class="card-header-icon icon-green">
                        ⚡
                    </div>

                </div>


                <div class="card-body">

                    <div class="quick-actions-modern">


                        <!-- PROFILE -->

                        <asp:HyperLink
                            ID="lnkQuickProfile"
                            runat="server"
                            CssClass="quick-action-card quick-red"
                            NavigateUrl="~/DonorRegistration.aspx">

                            <span class="quick-icon">
                                ♥
                            </span>

                            <span>

                                <strong>
                                    My Donor Profile
                                </strong>

                                <small>
                                    Update personal and contact details
                                </small>

                            </span>

                            <b>
                                ›
                            </b>

                        </asp:HyperLink>


                        <!-- SEARCH -->

                        <asp:HyperLink
                            ID="lnkQuickSearch"
                            runat="server"
                            CssClass="quick-action-card quick-blue"
                            NavigateUrl="~/DonorSearch.aspx">

                            <span class="quick-icon">
                                ⌕
                            </span>

                            <span>

                                <strong>
                                    Find Blood Donor
                                </strong>

                                <small>
                                    Search available blood donors
                                </small>

                            </span>

                            <b>
                                ›
                            </b>

                        </asp:HyperLink>


                        <!-- PASSWORD -->

                        <asp:HyperLink
                            ID="lnkQuickPassword"
                            runat="server"
                            CssClass="quick-action-card quick-purple"
                            NavigateUrl="~/ChangePassword.aspx">

                            <span class="quick-icon">
                                🔐
                            </span>

                            <span>

                                <strong>
                                    Change Password
                                </strong>

                                <small>
                                    Keep your account protected
                                </small>

                            </span>

                            <b>
                                ›
                            </b>

                        </asp:HyperLink>

                    </div>

                </div>

            </div>

        </div>


        <!-- =====================================================
             ACCOUNT INFORMATION
             ===================================================== -->

        <div class="card recent-panel">

            <div class="card-header">

                <div>

                    <div class="card-title">
                        Account Information
                    </div>

                    <div class="card-subtitle">
                        Your BloodCare login and account details
                    </div>

                </div>

                <div class="card-header-icon icon-purple">
                    ID
                </div>

            </div>


            <div class="card-body">

                <div class="account-info-grid">


                    <!-- USERNAME -->

                    <div class="account-info-item">

                        <span class="account-info-label">
                            Username
                        </span>

                        <strong>

                            <asp:Literal
                                ID="litUsername"
                                runat="server" />

                        </strong>

                    </div>


                    <!-- EMAIL -->

                    <div class="account-info-item">

                        <span class="account-info-label">
                            Email
                        </span>

                        <strong>

                            <asp:Literal
                                ID="litEmail"
                                runat="server" />

                        </strong>

                    </div>


                    <!-- ROLE -->

                    <div class="account-info-item">

                        <span class="account-info-label">
                            Role
                        </span>

                        <strong>
                            Donor
                        </strong>

                    </div>


                    <!-- STATUS -->

                    <div class="account-info-item">

                        <span class="account-info-label">
                            Account Status
                        </span>

                        <span class="status-badge status-available">
                            Active
                        </span>

                    </div>

                </div>

            </div>

        </div>


        <!-- =====================================================
             LIFE MESSAGE
             ===================================================== -->

        <div class="life-message">

            <div class="life-message-icon">
                ♥
            </div>

            <div class="life-message-content">

                <h3>
                    Your donation can make a difference.
                </h3>

                <p>
                    Keeping your blood group, address and availability
                    current helps people find the right donor when it matters.
                </p>

            </div>

            <asp:HyperLink
                ID="lnkLifeUpdate"
                runat="server"
                CssClass="life-message-button"
                NavigateUrl="~/DonorRegistration.aspx">

                Update Profile →

            </asp:HyperLink>

        </div>

    </asp:Panel>

</asp:Content>
