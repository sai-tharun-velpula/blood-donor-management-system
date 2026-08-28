<%@ Page Title="Reports &amp; Insights"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Reports.aspx.cs"
    Inherits="BloodDonorManagementSystem.Reports" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Reports &amp; Insights | BloodCare
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    Reports &amp; Insights
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-heading reports-heading">
        <div>
            <div class="eyebrow eyebrow-dark"><span class="eyebrow-icon">▥</span> Analytics</div>
            <h1>Reports &amp; Insights</h1>
            <p>Understand donor distribution, availability and location coverage using live database data.</p>
        </div>
        <a href="<%= ResolveUrl("~/Donors.aspx") %>" class="btn btn-light">Open Directory →</a>
    </div>

    <section class="report-kpi-grid">

        <div class="report-kpi report-red">
            <span class="report-kpi-icon">♥</span>
            <div><small>Total Donors</small><strong><asp:Label ID="lblFemale" runat="server" Text="0" /></strong></div>
        </div>

        <div class="report-kpi report-green">
            <span class="report-kpi-icon">✓</span>
            <div><small>Available Donors</small><strong><asp:Label ID="lblMale" runat="server" Text="0" /></strong></div>
        </div>

        <div class="report-kpi report-purple">
            <span class="report-kpi-icon">✚</span>
            <div><small>Blood Groups</small><strong><asp:Label ID="lblRecent" runat="server" Text="0" /></strong></div>
        </div>

        <div class="report-kpi report-blue">
            <span class="report-kpi-icon">⌖</span>
            <div><small>Cities Covered</small><strong><asp:Label ID="lblInactive" runat="server" Text="0" /></strong></div>
        </div>

    </section>

    <section class="report-grid">

        <div class="card">
            <div class="card-header">
                <div>
                    <div class="card-title">Blood Group Report</div>
                    <div class="card-subtitle">Total and available donors per blood group</div>
                </div>
                <span class="card-header-icon icon-red">♥</span>
            </div>

            <div class="table-responsive">
                <asp:GridView
                    ID="gvBlood"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="donor-table"
                    GridLines="None"
                    EmptyDataText="No blood group data available.">
                    <Columns>
                        <asp:TemplateField HeaderText="Blood Group">
                            <ItemTemplate>
                                <span class="blood-badge"><%# Eval("BloodGroup") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="TotalDonors" HeaderText="Total Donors" />
                        <asp:BoundField DataField="AvailableDonors" HeaderText="Available" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="card">
            <div class="card-header">
                <div>
                    <div class="card-title">Location Report</div>
                    <div class="card-subtitle">Donor concentration by city</div>
                </div>
                <span class="card-header-icon icon-blue">⌖</span>
            </div>

            <div class="table-responsive">
                <asp:GridView
                    ID="gvCity"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="donor-table"
                    GridLines="None"
                    EmptyDataText="No location data available.">
                    <Columns>
                        <asp:BoundField DataField="City" HeaderText="City" />
                        <asp:BoundField DataField="State" HeaderText="State" />
                        <asp:BoundField DataField="TotalDonors" HeaderText="Donors" />
                        <asp:BoundField DataField="AvailableDonors" HeaderText="Available" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </section>

    <section class="card report-summary-card">
        <div class="card-header">
            <div>
                <div class="card-title">Operational Summary</div>
                <div class="card-subtitle">Live metrics calculated from dbo.Donors</div>
            </div>
        </div>

        <div class="card-body">
            <div class="insight-strip">
                <div>
                    <span class="insight-icon red">♥</span>
                    <span><strong>Real-time donor data</strong><small>Values are loaded directly from SQL Server.</small></span>
                </div>
                <div>
                    <span class="insight-icon green">✓</span>
                    <span><strong>Availability tracking</strong><small>Available donors are counted from IsAvailable.</small></span>
                </div>
                <div>
                    <span class="insight-icon purple">✚</span>
                    <span><strong>Blood coverage</strong><small>Distinct blood groups represented by donors.</small></span>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
