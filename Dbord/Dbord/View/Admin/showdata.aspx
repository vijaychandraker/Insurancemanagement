<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="showdata.aspx.cs" Inherits="Dbord.View.Admin.showdata" %>
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
        .table tr:nth-child(even) {background-color: #f2f2f2;}
        .table tr:hover {background-color: #ddd;}
        .btn-icon {margin-right: 5px;}
        .form-control {width: 100%; padding: 5px; box-sizing: border-box;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:ScriptManager runat="server" ID="ScriptManager1" />

    <hr />
    <div>
        <div style="margin-bottom:10px;">
    <div style="margin-bottom: 10px; display: flex; align-items: center; gap: 5px;">
    <div style="position: relative; display: inline-block;">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" 
                     placeholder="Search by Policy Number, Customer Name, or Status" 
                     Width="250px"></asp:TextBox>
     
    </div>
    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
    <asp:Button ID="btnClearSearch" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClearSearch_Click" />
</div>

</div>

        <asp:GridView ID="gvPolicies" runat="server" AutoGenerateColumns="False"
            DataKeyNames="PolicyId" CssClass="table"
            OnRowEditing="gvPolicies_RowEditing"
            OnRowCancelingEdit="gvPolicies_RowCancelingEdit"
            OnRowUpdating="gvPolicies_RowUpdating"
            OnRowDeleting="gvPolicies_RowDeleting"
            OnRowDataBound="gvPolicies_RowDataBound"
            AllowPaging="True"
            PageSize="5"
            OnPageIndexChanging="gvPolicies_PageIndexChanging">

            <Columns>
                <asp:TemplateField HeaderText="ID">
                    <ItemTemplate><%# Eval("PolicyId") %></ItemTemplate>
                    <EditItemTemplate><asp:Label ID="lblPolicyId" runat="server" Text='<%# Bind("PolicyId") %>'></asp:Label></EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Policy Number">
                    <ItemTemplate><%# Eval("PolicyNumber") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtPolicyNumber" runat="server" Text='<%# Bind("PolicyNumber") %>' CssClass="form-control"></asp:TextBox></EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Customer Name">
                    <ItemTemplate><%# Eval("CustomerName") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtCustomerName" runat="server" Text='<%# Bind("CustomerName") %>' CssClass="form-control"></asp:TextBox></EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Premium">
                    <ItemTemplate><%# Eval("PremiumAmount") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtPremium" runat="server" Text='<%# Bind("PremiumAmount") %>' CssClass="form-control" TextMode="Number"></asp:TextBox></EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Start Date">
                    <ItemTemplate><%# Eval("StartDate", "{0:dd-MM-yyyy}") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtStartDate" runat="server" Text='<%# Bind("StartDate", "{0:yyyy-MM-dd}") %>' CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="End Date">
                    <ItemTemplate><%# Eval("EndDate", "{0:dd-MM-yyyy}") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEndDate" runat="server" Text='<%# Bind("EndDate", "{0:yyyy-MM-dd}") %>' CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Type">
                    <ItemTemplate><%# Eval("PolicyType") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtPolicyType" runat="server" Text='<%# Bind("PolicyType") %>' CssClass="form-control"></asp:TextBox></EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Agent">
                    <ItemTemplate><%# Eval("AgentName") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtAgentName" runat="server" Text='<%# Bind("AgentName") %>' CssClass="form-control"></asp:TextBox></EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Coverage">
                    <ItemTemplate><%# Eval("CoverageAmount") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtCoverage" runat="server" Text='<%# Bind("CoverageAmount") %>' CssClass="form-control" TextMode="Number"></asp:TextBox></EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate><%# Eval("Status") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control"></asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Remarks">
                    <ItemTemplate><%# Eval("Remarks") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtRemarks" runat="server" Text='<%# Bind("Remarks") %>' CssClass="form-control"></asp:TextBox></EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                            <span class="btn-icon fa fa-edit"></span>
                        </asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteRecord" CommandArgument="<%# Container.DataItemIndex %>" OnClientClick="return confirm('Are you sure you want to delete this record?');">
                            <span class="btn-icon fa fa-trash"></span>
                        </asp:LinkButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update">
                            <span class="btn-icon fa fa-check"></span>
                        </asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel">
                            <span class="btn-icon fa fa-times"></span>
                        </asp:LinkButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
