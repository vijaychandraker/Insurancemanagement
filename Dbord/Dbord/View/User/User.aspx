<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="Dbord.View.User.User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        .error {
            border: 2px solid red !important;
            background-color: #fff0f0 !important;
        }
        #loadingOverlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            z-index: 9999;
            display: none;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <br />

    <!-- Loading Overlay -->
    <div id="loadingOverlay">
        <div>
            <div class="spinner-border text-light" role="status"></div>
            <p class="mt-2">Saving, please wait...</p>
        </div>
    </div>

    <div class="card card-info">
        <div class="card-header">
            <h3 class="card-title">Add New Insurance Policy</h3>
        </div>
        <div class="card-body">

            <!-- Row 1 -->
            <div class="row mb-2">
                <div class="col-sm-6">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend"><span class="input-group-text">Policy No</span></div>
                        <asp:TextBox CssClass="form-control validate-text" ID="txtPolicyNo" runat="server"></asp:TextBox>
                    </div>
                </div>

                <div class="col-sm-6">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend"><span class="input-group-text">Name</span></div>
                        <asp:TextBox CssClass="form-control validate-string" ID="txtName" runat="server"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Row 2 -->
            <div class="row mb-2">
                <div class="col-sm-6">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend"><span class="input-group-text">Owner Name</span></div>
                        <asp:TextBox CssClass="form-control validate-string" ID="txtOwnerName" runat="server"></asp:TextBox>
                    </div>
                </div>

                <div class="col-sm-6">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend"><span class="input-group-text">Address</span></div>
                        <asp:TextBox CssClass="form-control" ID="txtAddress" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Row 3 -->
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

            <!-- Row 4 -->
            <div class="row mb-2">
                <div class="col-sm-6">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend"><span class="input-group-text">Sum Insured</span></div>
                        <asp:TextBox CssClass="form-control validate-decimal" ID="txtSumInsured" runat="server"></asp:TextBox>
                    </div>
                </div>

                <div class="col-sm-6">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend"><span class="input-group-text">Premium</span></div>
                        <asp:TextBox CssClass="form-control validate-decimal" ID="txtPremium" runat="server"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Row 5 -->
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
                        <asp:DropDownList CssClass="form-control" ID="ddlCompany" runat="server"></asp:DropDownList>
                    </div>
                </div>
            </div>

            <!-- Row 6 -->
            <div class="row mb-2">
                <div class="col-sm-6">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend"><span class="input-group-text">Category ID</span></div>
                        <asp:DropDownList CssClass="form-control" ID="ddlCategory" runat="server"></asp:DropDownList>
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
                    <asp:Button ID="btnSubmit" CssClass="btn btn-success" runat="server" Text="Save Policy"
                        OnClick="btnSubmit_Click1" OnClientClick="return validateForm();" />
                    <asp:Button ID="btnClear" CssClass="btn btn-warning ml-2" runat="server" Text="Clear" OnClick="btnClear_Click" OnClientClick="return confirmClear();" />
                </div>
            </div>

            <!-- Message -->
            <asp:Label ID="lblMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
        </div>
    </div>

    <script>
        function validateForm() {
            let valid = true;
            document.querySelectorAll(".form-control").forEach(el => el.classList.remove("error"));

            // Validate string fields
            document.querySelectorAll(".validate-string").forEach(el => {
                if (!/^[A-Za-z\s]+$/.test(el.value)) {
                    el.classList.add("error");
                    valid = false;
                }
            });

            // Validate decimal fields
            document.querySelectorAll(".validate-decimal").forEach(el => {
                if (!/^\d+(\.\d+)?$/.test(el.value)) {
                    el.classList.add("error");
                    valid = false;
                }
            });

            // Required check
            document.querySelectorAll(".form-control").forEach(el => {
                if (el.value.trim() === "" && el.hasAttribute("id")) {
                    el.classList.add("error");
                    valid = false;
                }
            });

            if (!valid) {
                Swal.fire("Validation Failed", "Please correct the highlighted fields.", "error");
                return false;
            }

            // Show loading spinner
            document.getElementById("loadingOverlay").style.display = "flex";
            return true;
        }

        // Hide spinner after postback completes
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            document.getElementById("loadingOverlay").style.display = "none";
        });

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
            return false;
        }
    </script>
</asp:Content>
