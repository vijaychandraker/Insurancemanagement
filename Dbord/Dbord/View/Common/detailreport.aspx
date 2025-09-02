<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="detailreport.aspx.cs" Inherits="Dbord.View.Common.detailreport" %>
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
    <h3 class="card-title">Expire in one Month</h3>
</div>
<div class="card-body">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
                 
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="table-responsive">
                        <asp:GridView ID="Gvrepot"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table grid-wrap"
                            AllowPaging="true"
                            PageSize="5"
                            ShowFooter="true"
                            OnPageIndexChanging="Gvrepot_PageIndexChanging"
                            OnRowDataBound="Gvrepot_RowDataBound"
                            PagerSettings-Mode="NumericFirstLast"
                            PagerStyle-CssClass="grid-pager"
                            DataKeyNames="PolicyID">

                            <Columns>
                                <asp:TemplateField HeaderText="S.No">
                                    <ItemTemplate>
                                        <%# ((GridViewRow)Container).RowIndex + 1 + (Gvrepot.PageIndex * Gvrepot.PageSize) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="PolicyID" HeaderText="Policy ID" Visible="false" />

                                <asp:TemplateField HeaderText="Customer Name">
                                    <HeaderTemplate>
                                        Name
                                        
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Owner">
                                    <HeaderTemplate>
                                        Owner
                                        
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("OwnerName") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Address">
                                    <HeaderTemplate>
                                        Address
                                        
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Address") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Vehicle No">
                                    <HeaderTemplate>
                                        Vehicle No|
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("VehicleNo") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Particular">
                                    <HeaderTemplate>
                                        Particular
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Particular") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Sum Insured">
                                    <HeaderTemplate>
                                        Sum Insured
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("SumInsured") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Premium">
                                    <HeaderTemplate>
                                        Premium<br /><br />
                                       
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Premium") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="NCB">
                                    <HeaderTemplate>
                                        NCB<br /><br />
                                       
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("NCB") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Policy No">
                                    <HeaderTemplate>
                                        Policy No<br /><br />
                                        
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("PolicyNo") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Start Date">
                                    <HeaderTemplate>
                                        Start Date<br />
                                    
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("InsuredDate", "{0:dd/MM/yyyy}") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="End Date">
                                    <HeaderTemplate>
                                        End Date<br />  
                                       
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("ExpireDate", "{0:dd/MM/yyyy}") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Company">
                                    <HeaderTemplate>
                                        Company<br /><br />
                                       
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("CompanyName") %></ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Category">
                                    <HeaderTemplate>
                                        Category<br /><br />
                                       
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("CategoryName") %></ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                            <FooterStyle CssClass="grid-footer" />

                            
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="<%# Gvrepot.Columns.Count %>" class="no-data">
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
