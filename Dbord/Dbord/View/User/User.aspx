<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="Dbord.View.User.User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

   <br />
    
<div class="card card-info">
    <div class="card-header">
        <h3 class="card-title">Add New Insurance Policy</h3>
    </div>
    <div class="card-body">

        <!-- Row 1: Policy Number & Customer Name -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Policy Number</span>
                    </div>
                    <asp:TextBox CssClass="form-control" ID="txtPolicyNumber" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvPolicyNumber" runat="server"
                    ControlToValidate="txtPolicyNumber" ErrorMessage="Policy Number is required."
                    ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Customer Name</span>
                    </div>
                    <asp:TextBox CssClass="form-control" ID="txtCustomerName" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvCustomerName" runat="server"
                    ControlToValidate="txtCustomerName" ErrorMessage="Customer Name is required."
                    ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
        </div>

        <!-- Row 2: Premium Amount & Coverage Amount -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Premium Amount</span>
                    </div>
                    <asp:TextBox CssClass="form-control" ID="txtPremiumAmount" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvPremium" runat="server"
                    ControlToValidate="txtPremiumAmount" ErrorMessage="Premium is required."
                    ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revPremium" runat="server"
                    ControlToValidate="txtPremiumAmount" ValidationExpression="^\d+(\.\d{1,2})?$"
                    ErrorMessage="Enter a valid amount." ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Coverage Amount</span>
                    </div>
                    <asp:TextBox CssClass="form-control" ID="txtCoverageAmount" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvCoverage" runat="server"
                    ControlToValidate="txtCoverageAmount" ErrorMessage="Coverage is required."
                    ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revCoverage" runat="server"
                    ControlToValidate="txtCoverageAmount" ValidationExpression="^\d+(\.\d{1,2})?$"
                    ErrorMessage="Enter a valid amount." ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>
        </div>

        <!-- Row 3: Start Date & End Date -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Start Date</span>
                    </div>
                    <asp:TextBox CssClass="form-control" ID="txtStartDate" runat="server" TextMode="Date"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server"
                    ControlToValidate="txtStartDate" ErrorMessage="Start Date is required."
                    ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">End Date</span>
                    </div>
                    <asp:TextBox CssClass="form-control" ID="txtEndDate" runat="server" TextMode="Date"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server"
                    ControlToValidate="txtEndDate" ErrorMessage="End Date is required."
                    ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
        </div>

        <!-- Row 4: Policy Type & Agent Name -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Policy Type</span>
                    </div>
                    <asp:DropDownList CssClass="form-control" ID="ddlPolicyType" runat="server">
                        <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                        <asp:ListItem Text="Life Insurance" Value="Life Insurance"></asp:ListItem>
                        <asp:ListItem Text="Health Insurance" Value="Health Insurance"></asp:ListItem>
                        <asp:ListItem Text="Vehicle Insurance" Value="Vehicle Insurance"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:RequiredFieldValidator ID="rfvPolicyType" runat="server"
                    ControlToValidate="ddlPolicyType" InitialValue=""
                    ErrorMessage="Select a policy type." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Agent Name</span>
                    </div>
                    <asp:TextBox CssClass="form-control" ID="txtAgentName" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvAgentName" runat="server"
                    ControlToValidate="txtAgentName" ErrorMessage="Agent Name is required."
                    ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
        </div>

        <!-- Row 5: Status & Remarks -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Status</span>
                    </div>
                    <asp:DropDownList CssClass="form-control" ID="ddlStatus" runat="server">
                        <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                        <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                        <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                        <asp:ListItem Text="Expired" Value="Expired"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:RequiredFieldValidator ID="rfvStatus" runat="server"
                    ControlToValidate="ddlStatus" InitialValue=""
                    ErrorMessage="Select a status." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text">Remarks</span>
                    </div>
                    <asp:TextBox CssClass="form-control" ID="txtRemarks" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Row 6: Buttons -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <asp:Button ID="btnSubmit" CssClass="btn btn-success" runat="server" Text="Save Policy" OnClick="btnSubmit_Click1"/>
                <asp:Button ID="btnClear" CssClass="btn btn-warning ml-2" runat="server" Text="Clear" OnClick="btnClear_Click" OnClientClick="return confirmClear();"/>
            <script>
    function confirmClear() {
        Swal.fire({
            title: 'Clear Form?',
            text: "This will remove all entered data.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, clear it!'
        }).then((result) => {
            if (result.isConfirmed) {
                document.getElementById('<%= this.Master.FindControl("form1").ClientID %>').reset();
            }
        });
        return false; // prevent default postback
    }
            </script>
            </div>
            <div class="col-sm-6"></div>
        </div>

        <!-- Message Label -->
        <asp:Label ID="lblMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
    </div>
</div>

</asp:Content>
