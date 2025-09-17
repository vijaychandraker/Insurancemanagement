<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Dbord.View.Admin.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        .table { width: 100%; border-collapse: collapse; table-layout: auto; }
        .table th { background-color: #4CAF50; color: white; text-align: center; padding: 8px; word-wrap: break-word; }
        .table td { padding: 8px; text-align: center; border-bottom: 1px solid #ddd; word-wrap: break-word; white-space: normal; }
        .table td.actions { white-space: nowrap; word-wrap: normal; }
        .table tr:nth-child(even) { background-color: #f2f2f2; }
        .table tr:hover { background-color: #ddd; }
        .btn-icon { margin-right: 5px; }
        .form-control { width: 100%; padding: 5px; box-sizing: border-box; }
        .d-none { display: none; }

        /* CSS spinner */
        .spinner { width: 48px; height: 48px; border: 5px solid rgba(0,0,0,0.1); border-top-color: #4CAF50; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto; }
        @keyframes spin { to { transform: rotate(360deg); } }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- ✅ ScriptManager placed before any UpdatePanel -->
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

    <br />

    <section class="content">
        <div class="container-fluid">

            <!-- Loader -->
            <div id="loading" style="display:none; position:fixed; inset:0; background:rgba(255,255,255,0.7); z-index:9999; align-items:center; justify-content:center;">
                <div style="text-align:center;">
                    <div class="spinner"></div>
                    <p style="margin-top:8px; font-weight:600; color:#333;">Loading, please wait...</p>
                </div>
            </div>

            <!-- Summary Cards -->
            <div class="row">
                <!-- Total Policies -->
                <div class="col-lg-4 sm-12">
                    <div class="small-box bg-info">
                        <div class="inner">
                            <h3><asp:Label ID="lbltotal" runat="server" Text="0"></asp:Label></h3>
                            <p>Total Policies</p>
                        </div>
                        <div class="icon"><i class="far fa-envelope"></i></div>
                        <a href="<%= ResolveUrl("~/View/User/report.aspx") %>" onclick="ShowLoading();" class="small-box-footer">See Details <i class="fas fa-arrow-circle-right"></i></a>
                    </div>
                </div>

                <!-- More Than One Policy -->
                <div class="col-lg-4 sm-12">
                    <div class="small-box bg-success">
                        <div class="inner">
                            <h3><asp:Label ID="lblowner" runat="server" Text="0"></asp:Label></h3>
                            <p>More Than One Policy Holder</p>
                        </div>
                        <div class="icon"><i class="far fa-user"></i></div>
                        <a href="#" class="small-box-footer">See Details <i class="fas fa-arrow-circle-right"></i></a>
                    </div>
                </div>

                <!-- Expiring -->
                <div class="col-lg-4 sm-12">
                    <div class="small-box bg-danger">
                        <div class="inner">
                            <h3><asp:Label ID="lblexpired" runat="server" Text="0"></asp:Label></h3>
                            <p>Expire in one Month</p>
                        </div>
                        <div class="icon"><i class="far fa-file"></i></div>
                       <a href='<%= ResolveUrl("~/View/Common/detailreport.aspx?defaultvalue=1") %>'  onclick="ShowLoading();"
   class="small-box-footer">
    See Details <i class="fas fa-arrow-circle-right"></i>
</a>



                    </div>
                </div>
            </div>

            <!-- Charts -->
            <div class="row">
                <div class="col-lg-6 sm-12">
                    <!-- Company Wise -->
                    <div class="card card-danger">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h3 class="card-title mb-0">Company Wise Policy</h3>
                            <div class="ml-auto">
                                <asp:LinkButton ID="btnExportExcel" runat="server" OnClick="btnExportExcel_Click"><i class="fas fa-file-excel"></i></asp:LinkButton>
                            </div>
                        </div>
                        <div class="card-body">
                            <canvas id="companyChart" style="min-height:250px;height:250px;max-height:250px;max-width:100%;"></canvas>
                        </div>
                    </div>

                    <!-- Category Wise Company -->
                    <div class="card card-info">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h3 class="card-title mb-0">Category Wise Company</h3>
                            <div class="ml-auto">
                                <asp:LinkButton ID="lnkcompanywiseCategory" runat="server" OnClick="lnkcompanywiseCategory_Click"><i class="fas fa-file-excel"></i></asp:LinkButton>
                            </div>
                        </div>
                        <div class="card-body">
                            <canvas id="CategoryChart" style="min-height:250px;height:250px;max-height:250px;max-width:100%;"></canvas>
                        </div>
                    </div>
                </div>

                <!-- Category Wise Policy -->
                <div class="col-lg-6 sm-12">
                    <div class="card card-warning">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h3 class="card-title">Category Wise Policy</h3>
                            <div class="ml-auto">
                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="btnExportCategoryExcel_Click"><i class="fas fa-file-excel"></i></asp:LinkButton>
                            </div>
                        </div>
                        <div class="card-body">

                            <!-- ✅ UpdatePanel for gvCategoryCompany -->
                            <asp:UpdatePanel ID="upCategoryCompany" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvCategoryCompany" runat="server" AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-striped"
                                        AllowPaging="true" PageSize="12"
                                        PagerStyle-CssClass="grid-pager"
                                        EmptyDataText="No records found."
                                        OnPageIndexChanging="gvCategoryCompany_PageIndexChanging">

                                        <Columns>
                                            <asp:TemplateField HeaderText="S.No">
                                                <ItemTemplate>
                                                    <%# ((GridViewRow)Container).RowIndex + 1 + (gvCategoryCompany.PageIndex * gvCategoryCompany.PageSize) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                                            <asp:BoundField DataField="CompanyName" HeaderText="Company" />

                                            <asp:TemplateField HeaderText="Total Policies">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkTotalPolicies" runat="server"
                                                        Text='<%# Eval("TotalPolicies") %>'
                                                        OnClientClick='<%# "ShowLoading(); window.location=\"" 
                                                                        + ResolveUrl("~/View/Common/detailreport.aspx?CategoryID=" 
                                                                        + Eval("CategoryID") 
                                                                        + "&CompanyID=" 
                                                                        + Eval("CompanyID")) 
                                                                        + "\"; return false;" %>'>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                    </div>
                </div>
            </div>

            <!-- Policy Holders Grid -->
            <div class="row">
                <div class="col-lg-12 sm-12">
                    <div class="card card-danger">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h3 class="card-title">Details Policy Holders</h3>
                        </div>

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="card-body">
            <div style="margin-bottom:10px; display:flex; align-items:center; gap:5px;">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" />
                <asp:LinkButton ID="btnSearch_dash" runat="server" OnClick="btnSearch_dash_Click" OnClientClick="ShowLoading();" CssClass="btn btn-primary" ToolTip="Search">
                    <i class="fas fa-search"></i>
                </asp:LinkButton>
                <asp:LinkButton ID="btnClearSearch_dash" runat="server" OnClick="btnClearSearch_dash_Click" OnClientClick="ShowLoading();" CssClass="btn btn-secondary" ToolTip="Clear">
                    <i class="fas fa-eraser"></i>
                </asp:LinkButton>
            </div>

            <asp:GridView ID="gvdashboard" runat="server" AutoGenerateColumns="False"
                DataKeyNames="PolicyID"
                CssClass="table table-bordered table-striped"
                AllowPaging="True" PageSize="5"
                PagerSettings-Mode="NumericFirstLast"
                AllowCustomPaging="True"
                PagerSettings-Position="Bottom"
                PagerSettings-PageButtonCount="5"
                PagerStyle-CssClass="grid-pager"
                ShowFooter="true"
                OnPageIndexChanging="gvdashboard_PageIndexChanging"
                OnRowDataBound="gvdashboard_RowDataBound">

                <Columns>
                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate>
                            <%# ((GridViewRow)Container).RowIndex + 1 + (gvdashboard.PageIndex * gvdashboard.PageSize) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Customer Name" />
                    <asp:BoundField DataField="OwnerName" HeaderText="Owner" />
                    <asp:BoundField DataField="Address" HeaderText="Address" />
                    <asp:BoundField DataField="VehicleNo" HeaderText="Vehicle No" />
                    <asp:BoundField DataField="Particular" HeaderText="Particular" />
                    <asp:BoundField DataField="SumInsured" HeaderText="Sum Insured" />
                    <asp:BoundField DataField="Premium" HeaderText="Premium" />
                    <asp:BoundField DataField="NCB" HeaderText="NCB" />
                    <asp:BoundField DataField="PolicyNo" HeaderText="Policy No" />
                    <asp:BoundField DataField="InsuredDate" HeaderText="Start Date" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="ExpireDate" HeaderText="End Date" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="CompanyName" HeaderText="Company" />
                    <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                </Columns>
            </asp:GridView>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
        </div>
    </ContentTemplate>

    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSearch_dash" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="btnClearSearch_dash" EventName="Click" />
        <asp:PostBackTrigger ControlID="btnExportExcel" />
        <asp:PostBackTrigger ControlID="LinkButton1" />
    </Triggers>
</asp:UpdatePanel>


                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="0">
                            <ProgressTemplate>
                                <div style="text-align:center; margin:10px;">
                                    <span>Loading, please wait...</span>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js@3.9.1"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2.2.0"></script>
    <script>
        // Ensure the plugin is registered globally (in addition to per-chart usage)
        if (window.Chart && window.ChartDataLabels) {
            Chart.register(ChartDataLabels);
        }
    </script>

    <script type="text/javascript">
        var companies = [];
        var totals = [];
        var companyIds = [];

    <% 
        System.Data.DataTable dt = Session["CompanyData"] as System.Data.DataTable;
        if (dt != null)
        {
            foreach (System.Data.DataRow row in dt.Rows)
            {
    %>
        companies.push('<%= row["CompanyName"].ToString() %>');
        totals.push(<%= row["Count"] %>);
        companyIds.push(<%= row["CompanyID"] %>);
    <% 
            }
        }
    %>

    var ctx = document.getElementById('companyChart').getContext('2d');
    var companyChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: companies,
            datasets: [{
                label: 'Total Policies',
                data: totals,
                companyIds: companyIds, // ✅ attach IDs here
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                datalabels: {
                    anchor: 'end',
                    align: 'end',
                    color: '#000',
                    font: { weight: 'bold', size: 10 },
                    formatter: function (value) { return value; }
                },
                legend: { display: false }
            },
            scales: {
                x: {
                    title: { display: true, text: 'Company' },
                    ticks: { autoSkip: false, maxRotation: 45, minRotation: 45 },
                    grid: { display: false }
                },
                y: {
                    beginAtZero: true,
                    max: 3500,
                    title: { display: true, text: 'Total Policies' },
                    grid: { display: false }
                }
            },
           
            onClick: (evt, elements) => {
                if (elements.length > 0) {
                    var chartElem = elements[0];
                    var companyId = companyChart.data.datasets[0].companyIds[chartElem.index];
                    if (typeof ShowLoading === 'function') { ShowLoading(); }
                    window.location.href = "<%= ResolveUrl("~/View/Common/detailreport.aspx") %>?CompanyID=" + companyId;
                }
            }
        },
        plugins: [ChartDataLabels]
    });
    </script>


    <script type="text/javascript">
        var Category = [];
        var totals = [];
        var categoryIds = [];

    <% 
        System.Data.DataTable dtcategory = Session["CategoryData"] as System.Data.DataTable;
        if (dtcategory != null)
        {
            foreach (System.Data.DataRow row in dtcategory.Rows)
            {
    %>
        Category.push('<%= row["CategoryName"].ToString() %>');
        totals.push(<%= row["Count"] %>);
        categoryIds.push(<%= row["CategoryID"] %>);
    <% 
            }
        }
    %>

    var ctcat = document.getElementById('CategoryChart').getContext('2d');
    var CategoryChart = new Chart(ctcat, {
        type: 'bar',
        data: {
            labels: Category,
            datasets: [{
                label: 'Total Policies',
                data: totals,
                categoryIds: categoryIds, // ✅ attach IDs here
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                datalabels: {
                    anchor: 'end',
                    align: 'end',
                    color: '#000',
                    font: { weight: 'bold', size: 10 },
                    formatter: function (value) { return value; }
                },
                legend: { display: false }
            },
            scales: {
                x: {
                    title: { display: true, text: 'Category' },
                    ticks: { autoSkip: false, maxRotation: 45, minRotation: 45 },
                    grid: { display: false }
                },
                y: {
                    beginAtZero: true,
                    max: 2500,
                    title: { display: true, text: 'Total Policies' },
                    grid: { display: false }
                }
            },
            // ✅ Redirect when bar clicked
            onClick: (evt, elements) => {
                if (elements.length > 0) {
                    var chartElem = elements[0];
                    var categoryId = CategoryChart.data.datasets[0].categoryIds[chartElem.index];
                    if (typeof ShowLoading === 'function') { ShowLoading(); }
                    window.location.href = "<%= ResolveUrl("~/View/Common/detailreport.aspx") %>?CategoryID=" + categoryId;
                }
            }
        },
        plugins: [ChartDataLabels]
    });
    </script>

    <script type="text/javascript">
        function ShowLoading() { document.getElementById("loading").style.display = "flex"; }
        function HideLoading() { document.getElementById("loading").style.display = "none"; }

        // ✅ Automatically show/hide loader for UpdatePanels
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () { ShowLoading(); });
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () { HideLoading(); });
    </script>

    <!-- Hidden iframe for Excel download -->
    <iframe id="downloadFrame" name="downloadFrame" style="display:none;width:0;height:0;border:0;" title="download"></iframe>
    <script type="text/javascript">
        function startDownload(btn) {
            try {
                ShowLoading();
                var form = btn && btn.form ? btn.form : (document.forms && document.forms[0]);
                if (form) {
                    window._prevFormTarget = form.target || '';
                    form.target = 'downloadFrame';
                }
            } catch (e) { }
            return true;
        }

        (function () {
            var iframe = document.getElementById('downloadFrame');
            if (iframe) {
                iframe.addEventListener('load', function () {
                    try {
                        var form = document.forms && document.forms[0];
                        if (form && typeof window._prevFormTarget !== 'undefined') {
                            form.target = window._prevFormTarget;
                            window._prevFormTarget = undefined;
                        }
                    } catch (e) { }
                    setTimeout(HideLoading, 50);
                });
            }
        })();
    </script>


</asp:Content>