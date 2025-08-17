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

        <!-- Row 1: Policy No & Name -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Policy No</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtPolicyNo" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ControlToValidate="txtPolicyNo" runat="server"
                    ErrorMessage="Policy No is required." ForeColor="Red" Display="Dynamic"/>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Name</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtName" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ControlToValidate="txtName" runat="server"
                    ErrorMessage="Name is required." ForeColor="Red" Display="Dynamic"/>
            </div>
        </div>

        <!-- Row 2: Owner Name & Address -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Owner Name</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtOwnerName" runat="server"></asp:TextBox>
                </div>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Address</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtAddress" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Row 3: Vehicle No & Particular -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Vehicle No</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtVehicleNo" runat="server"></asp:TextBox>
                </div>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Particular</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtParticular" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Row 4: Sum Insured & Premium -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Sum Insured</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtSumInsured" runat="server"></asp:TextBox>
                </div>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Premium</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtPremium" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Row 5: NCB & CompanyID -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">NCB</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtNCB" runat="server"></asp:TextBox>
                </div>
            </div>

            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Company ID</span></div>
                      <asp:DropDownList CssClass="form-control" ID="ddlCompany" runat="server">
            </asp:DropDownList>
                </div>
            </div>
        </div>

        <!-- Row 6: CategoryID & Dates -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Category ID</span></div>
                    <asp:DropDownList CssClass="form-control" ID="ddlCategory" runat="server">
            </asp:DropDownList>
                </div>
            </div>

            <div class="col-sm-3">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">Start Date</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtStartDate" runat="server" TextMode="Date"></asp:TextBox>
                </div>
            </div>

            <div class="col-sm-3">
                <div class="input-group mb-3">
                    <div class="input-group-prepend"><span class="input-group-text">End Date</span></div>
                    <asp:TextBox CssClass="form-control" ID="txtEndDate" runat="server" TextMode="Date"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Buttons -->
        <div class="row mb-2">
            <div class="col-sm-6">
                <asp:Button ID="btnSubmit" CssClass="btn btn-success" runat="server" Text="Save Policy" OnClick="btnSubmit_Click1"/>
                <asp:Button ID="btnClear" CssClass="btn btn-warning ml-2" runat="server" Text="Clear" OnClick="btnClear_Click" OnClientClick="return confirmClear();"/>
            </div>
            <div class="col-sm-6"></div>
        </div>

        <!-- Message -->
        <asp:Label ID="lblMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
    </div>
</div>

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

</asp:Content>
