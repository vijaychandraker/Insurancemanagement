<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="report.aspx.cs" Inherits="Dbord.View.User.report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .grid-wrap {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
            table-layout: auto;
        }
        .grid-wrap th {
            background: #2d5f72;
            color: #fff;
            text-align: center;
            padding: 10px;
            font-size: 14px;
            text-transform: uppercase;
            letter-spacing: 1px;
            border-bottom: 3px solid #0078d4;
        }
        .grid-wrap td {
            padding: 10px;
            border-bottom: 1px solid #f0f0f0;
            word-wrap: break-word;
            text-align: center;
        }
        .grid-wrap tr:nth-child(even) td { background-color: #fdf6ff; }
        .grid-wrap tr:nth-child(odd) td { background-color: #f0faff; }
        .grid-wrap tr:hover td { background-color: #ffe9d6; transition: 0.3s ease; }
        .grid-footer td {
            background: linear-gradient(45deg, #00c6ff, #2d5f72);
            color: #fff;
            font-weight: bold;
            text-align: center;
            padding: 10px;
        }
        .table-responsive { width: 100%; overflow-x: auto; }
        .form-control { width: 100%; padding: 3px; margin-top: 2px; box-sizing: border-box; }
        .btn { padding: 6px 12px; margin-right: 5px; border-radius: 4px; border: none; cursor: pointer; }
        .btn-primary { background-color: #007bff; color: #fff; }
        .btn-secondary { background-color: #6c757d; color: #fff; }
        .no-data { color: red; font-weight: bold; text-align: center; padding: 15px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
            <section class="content-header">
    <div class="container-fluid">
        <div class="row mb-2">
            <div class="col-sm-6">
                <h1>Report</h1>
            </div>
            <div class="col-sm-6">
                <ol class="breadcrumb float-sm-right">
                    <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/View/Admin/Dashboard.aspx") %>">Home</a></li>
                    <li class="breadcrumb-item active">Report</li>
                </ol>
            </div>
        </div>
    </div>
</section>
    <section class="content">
        <div class="container-fluid">
            

            <div class="card card-info">
<div class="card-header">
    <h3 class="card-title">Policy Report</h3>
</div>
<div class="card-body">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Button ID="btnExportExcel" runat="server" Text="Download Excel" CssClass="btn btn-primary" OnClick="btnExportExcel_Click" />
                    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn btn-secondary" OnClick="btnRefresh_Click" />
                    <br /><br />
                    <div class="table-responsive">
                        <asp:GridView ID="GridView1"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table grid-wrap"
                            AllowPaging="true"
                            PageSize="10"
                            ShowFooter="true"
                            OnPageIndexChanging="GridView1_PageIndexChanging"
                            OnRowDataBound="GridView1_RowDataBound"
                            PagerSettings-Mode="NumericFirstLast"
                            PagerStyle-CssClass="grid-pager"
                            DataKeyNames="PolicyID">

                            <Columns>
                                <asp:TemplateField HeaderText="S.No">
                                    <ItemTemplate>
                                        <%# ((GridViewRow)Container).RowIndex + 1 + (GridView1.PageIndex * GridView1.PageSize) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="PolicyID" HeaderText="Policy ID" Visible="false" />

                                <asp:TemplateField HeaderText="Customer Name">
                                    <HeaderTemplate>
                                        Name<br /><br />
                                        <asp:TextBox ID="txtSearchName" Placeholder="Search" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Owner">
                                    <HeaderTemplate>
                                        Owner<br /><br />
                                        <asp:TextBox ID="txtSearchOwner" Placeholder="Search" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("OwnerName") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Address">
                                    <HeaderTemplate>
                                        Address<br /><br />
                                        <asp:TextBox ID="txtSearchAddress" Placeholder="Search" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Address") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Vehicle No">
                                    <HeaderTemplate>
                                        Vehicle No<br /><br />
                                        <asp:TextBox ID="txtSearchVehicle" Placeholder="Search" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("VehicleNo") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Particular">
                                    <HeaderTemplate>
                                        Particular<br /><br />
                                        <asp:TextBox ID="txtSearchParticular" Placeholder="Search" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Particular") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Sum Insured">
                                    <HeaderTemplate>
                                        Sum Insured<br />
                                        <asp:TextBox ID="txtSearchSumInsured" Placeholder="Search" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("SumInsured") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Premium">
                                    <HeaderTemplate>
                                        Premium<br /><br />
                                        <asp:TextBox ID="txtSearchPremium" Placeholder="Search" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Premium") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="NCB">
                                    <HeaderTemplate>
                                        NCB<br /><br />
                                        <asp:TextBox ID="txtSearchNCB" runat="server" Placeholder="Search" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("NCB") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Policy No">
                                    <HeaderTemplate>
                                        Policy No<br /><br />
                                        <asp:TextBox ID="txtSearchPolicyNo" runat="server" Placeholder="Search" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("PolicyNo") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Start Date">
                                    <HeaderTemplate>
                                        Start Date<br />
                                        <asp:TextBox ID="txtSearchStartDate" runat="server" Placeholder="YYYY-MM-DD" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("InsuredDate", "{0:dd/MM/yyyy}") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="End Date">
                                    <HeaderTemplate>
                                        End Date<br />  
                                        <asp:TextBox ID="txtSearchEndDate" runat="server" Placeholder="YYYY-MM-DD" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("ExpireDate", "{0:dd/MM/yyyy}") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Company">
                                    <HeaderTemplate>
                                        Company<br /><br />
                                        <asp:TextBox ID="txtSearchCompany" runat="server" Placeholder="Search" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("CompanyName") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Category">
                                    <HeaderTemplate>
                                        Category<br /><br />
                                        <asp:TextBox ID="txtSearchCategory" runat="server" Placeholder="Search" CssClass="form-control" AutoPostBack="true" OnTextChanged="SearchTextChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("CategoryName") %></ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                            <FooterStyle CssClass="grid-footer" />

                            
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="<%# GridView1.Columns.Count %>" class="no-data">
                                        No records found.
                                    </td>
                                </tr>
                            </EmptyDataTemplate>

                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
    </div>
                </div>
        </div>
    </section>
</asp:Content>
