<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Dbord.View.Admin.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table {
            width: 100%;
            border-collapse: collapse;
            table-layout: fixed;
        }
        .table th {
            background-color: #4CAF50;
            color: white;
            text-align: center;
            padding: 8px;
            word-wrap: break-word;
        }
        .table td {
            padding: 8px;
            text-align: center;
            border-bottom: 1px solid #ddd;
            word-wrap: break-word;
            white-space: normal;
        }
        .table td.actions {
            white-space: nowrap;
            word-wrap: normal;
        }
        .table tr:nth-child(even) { background-color: #f2f2f2; }
        .table tr:hover { background-color: #ddd; }
        .btn-icon { margin-right: 5px; }
        .form-control { width: 100%; padding: 5px; box-sizing: border-box; }
        .d-none { display: none; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1>Dashboard</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/View/Admin/Dashboard.aspx") %>">Home</a></li>
                        <li class="breadcrumb-item active">Dashboard</li>
                    </ol>
                </div>
            </div>
        </div>
    </section>

    <section class="content">
        <div class="container-fluid">

            <div id="loading" style="display:none; text-align:center; margin:10px;">
    <img src="~/Content/images/loading.gif" alt="Loading..." width="40" height="40" />
    <p>Loading, please wait...</p>
</div>
            <!-- Summary Cards -->
            <div class="row">
                <div class="col-lg-4 sm-12">
                    <div class="small-box bg-info">
                        <div class="inner">
                            <h3><asp:Label ID="lbltotal" runat="server" Text="0"></asp:Label></h3>
                            <p>Total Policies</p>
                        </div>
                        <div class="icon">
                            <i class="far fa-envelope"></i>
                        </div>
                        <a href="<%= ResolveUrl("~/View/User/report.aspx") %>" class="small-box-footer">See Details <i class="fas fa-arrow-circle-right"></i></a>
                    </div>
                </div>

                <div class="col-lg-4 sm-12">
                    <div class="small-box bg-success">
                        <div class="inner">
                            <h3><asp:Label ID="lblowner" runat="server" Text="0"></asp:Label></h3>
                            <p>More Than One Policy Holder</p>
                        </div>
                        <div class="icon">
                            <i class="far fa-user"></i>
                        </div>
                        <a href="#" class="small-box-footer">See Details <i class="fas fa-arrow-circle-right"></i></a>
                    </div>
                </div>

                <div class="col-lg-4 sm-12">
                    <div class="small-box bg-danger">
                        <div class="inner">
                            <h3><asp:Label ID="lblexpired" runat="server" Text="0"></asp:Label></h3>
                            <p>Expire in one Month</p>
                        </div>
                        <div class="icon">
                            <i class="far fa-file"></i>
                        </div>

                
                        <a href="<%= ResolveUrl("~/View/Common/detailreport.aspx") %>" class="small-box-footer">See Details <i class="fas fa-arrow-circle-right"></i></a>
                    </div>
                </div>
            </div>

            <!-- Charts -->
            <div class="row">
                <div class="col-lg-6 sm-12">
                    <div class="card card-danger">
                        <div class="card-header">
                            <h3 class="card-title">Company Wise Policy</h3>
                        </div>
                        <div class="card-body">
                            <canvas id="companyChart" style="min-height: 250px; height: 250px; max-height: 250px; max-width: 100%;"></canvas>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6 sm-12">
                    <div class="card card-warning">
                        <div class="card-header">
                            <h3 class="card-title">Category Wise Policy</h3>
                        </div>
                        <div class="card-body">
                            <canvas id="CategoryChart" style="min-height: 250px; height: 250px; max-height: 250px; max-width: 100%;"></canvas>
                        </div>
                    </div>
                </div>
            </div>

            <!-- GridView -->
            <div class="row">
                <div class="col-lg-12 sm-12">
                    <div class="card card-danger">
                        <div class="card-header">
                            <h3 class="card-title">Detail Policy Holders</h3>
                        </div>

                        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="card-body">
                                    <div style="margin-bottom:10px; display: flex; align-items: center; gap: 5px;">
                                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" />
                                        <asp:Button ID="btnSearch_dash" runat="server" Text="Search" OnClick="btnSearch_dash_Click" CssClass="btn btn-primary" />
                                        <asp:Button ID="btnClearSearch_dash" runat="server" Text="Clear" OnClick="btnClearSearch_dash_Click" CssClass="btn btn-secondary" />
                                    </div>

                                     <asp:GridView ID="gvdashboard" runat="server" AutoGenerateColumns="False"
    DataKeyNames="PolicyID" CssClass="table"
    AllowPaging="True" PageSize="5"
    PagerSettings-Mode="NumericFirstLast"
    ShowFooter="true"
    PagerStyle-CssClass="grid-pager"
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
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2"></script>

    <script type="text/javascript">
        var companies = [];
        var totals = [];

        <% 
            System.Data.DataTable dt = ViewState["CompanyData"] as System.Data.DataTable;
            if (dt != null)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
        %>
        companies.push('<%= row["CompanyName"].ToString() %>');
        totals.push(<%= row["Count"] %>);
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
                }
            },
            plugins: [ChartDataLabels]
        });
    </script>

    <script type="text/javascript">
        var Category = [];
        var totals = [];

        <% 
            System.Data.DataTable dtcategory = ViewState["CategoryData"] as System.Data.DataTable;
            if (dtcategory != null)
            {
                foreach (System.Data.DataRow row in dtcategory.Rows)
                {
        %>
        Category.push('<%= row["CategoryName"].ToString() %>');
        totals.push(<%= row["Count"] %>);
        <% 
                }
            }
        %>

        var ctx = document.getElementById('CategoryChart').getContext('2d');
        var CategoryChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: Category,
                datasets: [{
                    label: 'Total Policies',
                    data: totals,
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
                }
            },
            plugins: [ChartDataLabels]
        });
    </script>
    <script type="text/javascript">
        function ShowLoading() {
            document.getElementById("loading").style.display = "block";
        }
        function HideLoading() {
            document.getElementById("loading").style.display = "none";
        }

        // Attach to UpdatePanel events
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
            ShowLoading();
        });
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            HideLoading();
        });
    </script>

</asp:Content>
