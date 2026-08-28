<%@ Page Title="Find Donor"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="DonorSearch.aspx.cs"
    Inherits="BloodDonorManagementSystem.DonorSearch" %>

<asp:Content
    ID="TitleContent"
    ContentPlaceHolderID="TitleContent"
    runat="server">

    Find Donor | BloodCare

</asp:Content>


<asp:Content
    ID="PageTitleContent"
    ContentPlaceHolderID="PageTitleContent"
    runat="server">

    Find Donor

</asp:Content>


<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>

        /* =========================================================
           DONOR SEARCH PAGE
           ========================================================= */

        .donor-search-filters {
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


        .donor-search-filters .field {
            min-width: 0;
        }


        .donor-search-actions {
            display: flex;
            gap: 8px;
            align-items: center;
            min-width: 0;
        }


        .donor-search-actions .btn {
            white-space: nowrap;
        }


        /* =========================================================
           DONOR NAME
           Keep names simple and normal-weight.
           ========================================================= */

        .donor-search-table td:first-child {
            font-weight: 400 !important;
        }


        .donor-search-name {
            font-weight: 400 !important;
            font-style: normal !important;
            text-decoration: none !important;
            color: #536174;
        }


        /* =========================================================
           BLOOD GROUP
           ========================================================= */

        .donor-search-table .blood-badge {
            white-space: nowrap;
        }


        /* =========================================================
           AVAILABILITY
           ========================================================= */

        .donor-search-table .availability {
            white-space: nowrap;
        }


        /* =========================================================
           EMPTY STATE
           ========================================================= */

        .donor-search-empty {
            margin-top: 15px;
        }


        /* =========================================================
           RESPONSIVE
           ========================================================= */

        @media (max-width: 1150px) {

            .donor-search-filters {
                grid-template-columns:
                    repeat(3, minmax(0, 1fr));
            }


            .donor-search-actions {
                grid-column: 1 / -1;
            }

        }


        @media (max-width: 750px) {

            .donor-search-filters {
                grid-template-columns: 1fr;
            }


            .donor-search-actions {
                grid-column: auto;
            }


            .donor-search-actions .btn {
                flex: 1;
            }

        }


        @media (max-width: 500px) {

            .donor-search-actions {
                flex-direction: column;
                width: 100%;
            }


            .donor-search-actions .btn {
                width: 100%;
            }

        }

    </style>


    <!-- =========================================================
         PAGE HEADING
         ========================================================= -->

    <div class="page-heading">

        <div>

            <h1>
                Find a Blood Donor
            </h1>

            <p>
                Search registered donors by blood group, location,
                name and availability.
            </p>

        </div>

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
                    Search Donors
                </div>

                <div class="card-subtitle">
                    Use any combination of filters to find donors.
                </div>

            </div>

            <div class="card-header-icon icon-red">
                🩸
            </div>

        </div>


        <div class="card-body">

            <div class="donor-search-filters">


                <!-- =================================================
                     DONOR NAME
                     ================================================= -->

                <div class="field">

                    <label for="<%= txtName.ClientID %>">
                        Donor Name
                    </label>

                    <asp:TextBox
                        ID="txtName"
                        runat="server"
                        CssClass="input"
                        MaxLength="150"
                        placeholder="Name, mobile or email..." />

                </div>


                <!-- =================================================
                     BLOOD GROUP
                     ================================================= -->

                <div class="field">

                    <label for="<%= ddlBloodGroup.ClientID %>">
                        Blood Group
                    </label>

                    <asp:DropDownList
                        ID="ddlBloodGroup"
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
                     CITY
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
                     STATE
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
                     AVAILABILITY
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
                     ACTION BUTTONS
                     ================================================= -->

                <div class="donor-search-actions">

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
                ID="gvSearchResults"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="donor-table donor-search-table"
                GridLines="None"
                UseAccessibleHeader="true"
                DataKeyNames="DonorId"
                AllowPaging="true"
                PageSize="10"
                OnPageIndexChanging="gvSearchResults_PageIndexChanging">

                <Columns>



                    <asp:TemplateField
                        HeaderText="Donor">

                        <ItemTemplate>

                            <span class="donor-search-name">

                                <%# Server.HtmlEncode(
                                    Convert.ToString(
                                        Eval("FullName")
                                    )
                                ) %>

                            </span>

                        </ItemTemplate>

                    </asp:TemplateField>


                    <asp:TemplateField
                        HeaderText="Blood">

                        <ItemTemplate>

                            <span class="blood-badge">

                                <%# Server.HtmlEncode(
                                    Convert.ToString(
                                        Eval("BloodGroup")
                                    )
                                ) %>

                            </span>

                        </ItemTemplate>

                    </asp:TemplateField>




                    <asp:BoundField
                        DataField="Mobile"
                        HeaderText="Mobile" />



                    <asp:BoundField
                        DataField="City"
                        HeaderText="City" />



                    <asp:BoundField
                        DataField="State"
                        HeaderText="State" />




                    <asp:TemplateField
                        HeaderText="Availability">

                        <ItemTemplate>

                            <span class='<%#
                                Convert.ToBoolean(
                                    Eval("IsAvailable")
                                )
                                    ? "availability available"
                                    : "availability unavailable"
                            %>'>

                                <span class="availability-dot"></span>

                                <%#
                                    Convert.ToBoolean(
                                        Eval("IsAvailable")
                                    )
                                        ? "Available"
                                        : "Unavailable"
                                %>

                            </span>

                        </ItemTemplate>

                    </asp:TemplateField>



                    <asp:BoundField
                        DataField="CreatedDate"
                        HeaderText="Registered"
                        DataFormatString="{0:dd MMM yyyy}"
                        HtmlEncode="true" />

                </Columns>

            </asp:GridView>

        </div>

    </div>



    <asp:Panel
        ID="pnlNoDonors"
        runat="server"
        Visible="false"
        CssClass="empty-state donor-search-empty">

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


</asp:Content>
