<%@ Page Title="Dashboard"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="BloodDonorManagementSystem.Dashboard" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Dashboard | BloodCare
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    Dashboard
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="dashboard-welcome">
        <div>
            <div class="eyebrow"><span class="status-dot"></span> BloodCare Management Portal</div>
            <h1>Welcome back, <strong><%= Server.HtmlEncode(GetCurrentUser()) %></strong> <span>👋</span></h1>
            <p>Monitor donor registrations, blood availability and donor activity from one secure workspace.</p>
        </div>
        <a href="<%= ResolveUrl("~/DonorRegistration.aspx") %>" class="btn btn-primary btn-lg">
            <span class="btn-icon">＋</span> Register Donor
        </a>
    </div>

    <section class="kpi-grid">

        <div class="kpi-card kpi-red">
            <div class="kpi-icon">♥</div>
            <div>
                <span class="kpi-label">Total Donors</span>
                <strong><asp:Label ID="lblTotal" runat="server" Text="0" /></strong>
                <small>Registered donor records</small>
            </div>
        </div>

        <div class="kpi-card kpi-green">
            <div class="kpi-icon">✓</div>
            <div>
                <span class="kpi-label">Available Donors</span>
                <strong><asp:Label ID="lblAvailable" runat="server" Text="0" /></strong>
                <small>Currently available</small>
            </div>
        </div>

        <div class="kpi-card kpi-purple">
            <div class="kpi-icon">✚</div>
            <div>
                <span class="kpi-label">Blood Groups</span>
                <strong><asp:Label ID="lblGroups" runat="server" Text="0" /></strong>
                <small>Groups represented</small>
            </div>
        </div>

        <div class="kpi-card kpi-blue">
            <div class="kpi-icon">⌖</div>
            <div>
                <span class="kpi-label">Cities Covered</span>
                <strong><asp:Label ID="lblCities" runat="server" Text="0" /></strong>
                <small>Locations with donors</small>
            </div>
        </div>

    </section>

    <section class="dashboard-grid dashboard-grid-top">

        <div class="card dashboard-panel">
            <div class="card-header">
                <div>
                    <div class="card-title">Blood Group Availability</div>
                    <div class="card-subtitle">Current distribution of registered donors</div>
                </div>
                <span class="card-header-icon icon-red">♥</span>
            </div>

            <div class="card-body">
                <div class="availability-list">
                    <asp:Repeater ID="rptBloodGroups" runat="server">
                        <ItemTemplate>
                            <div class="availability-row">
                                <span class="blood-badge blood-badge-lg"><%# Eval("BloodGroup") %></span>
                                <div class="availability-track">
                                    <div class="availability-fill" style='<%# "width:" + Eval("PercentValue") + "%;" %>'></div>
                                </div>
                                <strong class="availability-number"><%# Eval("DonorCount") %></strong>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Panel ID="pnlNoBloodGroups" runat="server" CssClass="empty-state" Visible="false">
                        <span class="empty-icon">♥</span>
                        <strong>No donor data yet</strong>
                        <span>Register a donor to populate blood group availability.</span>
                    </asp:Panel>
                </div>
            </div>
        </div>

        <div class="card dashboard-panel">
            <div class="card-header">
                <div>
                    <div class="card-title">Quick Actions</div>
                    <div class="card-subtitle">Common donor management operations</div>
                </div>
                <span class="card-header-icon icon-purple">⚡</span>
            </div>

            <div class="card-body quick-actions-modern">

                <a href="<%= ResolveUrl("~/DonorRegistration.aspx") %>" class="quick-action-card quick-red">
                    <span class="quick-icon">＋</span>
                    <span>
                        <strong>Register Donor</strong>
                        <small>Create a new donor record</small>
                    </span>
                    <b>→</b>
                </a>

                <a href="<%= ResolveUrl("~/Donors.aspx") %>" class="quick-action-card quick-blue">
                    <span class="quick-icon">◉</span>
                    <span>
                        <strong>Donor Directory</strong>
                        <small>Search, edit and manage records</small>
                    </span>
                    <b>→</b>
                </a>

                <a href="<%= ResolveUrl("~/Reports.aspx") %>" class="quick-action-card quick-purple">
                    <span class="quick-icon">▥</span>
                    <span>
                        <strong>Reports &amp; Insights</strong>
                        <small>View donor statistics and reports</small>
                    </span>
                    <b>→</b>
                </a>

            </div>
        </div>

    </section>

    <section class="card recent-panel">
        <div class="card-header">
            <div>
                <div class="card-title">Recent Registrations</div>
                <div class="card-subtitle">Latest donor records added to BloodCare</div>
            </div>
            <a href="<%= ResolveUrl("~/Donors.aspx") %>" class="card-header-link">View Directory <span>→</span></a>
        </div>

        <div class="table-responsive">
            <asp:GridView
                ID="gvRecent"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="donor-table dashboard-recent-table"
                GridLines="None"
                EmptyDataText="No donor registrations yet.">
                <Columns>
                    <asp:TemplateField HeaderText="Donor">
                        <ItemTemplate>
                            <div class="table-person">
                                <span class="table-avatar"><%# GetInitials(Eval("FullName")) %></span>
                                <strong><%# Eval("FullName") %></strong>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Blood Group">
                        <ItemTemplate>
                            <span class="blood-badge"><%# Eval("BloodGroup") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="City" HeaderText="City" />

                    <asp:TemplateField HeaderText="Availability">
                        <ItemTemplate>
                            <span class='<%# Convert.ToBoolean(Eval("IsAvailable")) ? "availability available" : "availability unavailable" %>'>
                                <span class="availability-dot"></span>
                                <%# Convert.ToBoolean(Eval("IsAvailable")) ? "Available" : "Unavailable" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="CreatedDate" HeaderText="Registered" DataFormatString="{0:dd MMM yyyy}" />
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <section class="life-message">
        <div class="life-message-icon">♥</div>
        <div class="life-message-content">
            <h3>Every donation can make a difference.</h3>
            <p>Keep donor information accurate and up to date so patients can find the right blood group when needed.</p>
        </div>
    </section>

</asp:Content>
