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
        .btn-success { background-color: #28a745; color: #fff; }
        .no-data { color: red; font-weight: bold; text-align: center; padding: 15px; }

        /* Loader overlay */
        #loader {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            z-index: 9999;
            display: none;
            justify-content: center;
            align-items: center;
            flex-direction: column;
            text-align: center;
        }
        .loader {
            border: 8px solid #f3f3f3;
            border-top: 8px solid #007bff;
            border-radius: 50%;
            width: 60px;
            height: 60px;
            animation: spin 1s linear infinite;
            margin-bottom: 15px;
        }
        @keyframes spin { 0% { transform: rotate(0deg);} 100% { transform: rotate(360deg);} }
        .loading-text { font-size: 18px; font-weight: bold; color: #fff; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />

    <section class="content">
        <div class="container-fluid">
            <div class="card card-info">
                <div class="card-header d-flex justify-content-between align-items-center">
    <h3 class="card-title mb-0">Policy Report</h3>
    <div class="ml-auto">         

       <asp:LinkButton ID="btnExportExcel" runat="server" 
    CssClass="btn btn-success" 
    OnClientClick="startDownload(); return false;">
    <i class="fas fa-file-excel"></i>
</asp:LinkButton>
    </div>
</div>
                <div class="card-body">

                    <asp:ScriptManager ID="ScriptManager1" runat="server" />

                    <!-- Export Button -->
                    

                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
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
                                            <ItemTemplate><asp:Label ID="lblSerial" runat="server" /></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PolicyID" HeaderText="Policy ID" Visible="false" />
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
                                    <FooterStyle CssClass="grid-footer" />
                                    <EmptyDataTemplate>
                                        <tr><td colspan="14" class="no-data">No records found.</td></tr>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <!-- Loader -->
        <div id="loader">
            <div class="loader"></div>
            <div class="loading-text">Please wait, processing...</div>
        </div>

        <!-- Hidden iframe -->
        <iframe id="downloadFrame" style="display:none;"></iframe>

        <script type="text/javascript">
            function startDownload() {
                var loader = document.getElementById("loader");
                loader.style.display = "flex";

                var token = new Date().getTime();

                // Poll for cookie
                var checkInterval = setInterval(function () {
                    if (document.cookie.indexOf("downloadToken=" + token) !== -1) {
                        loader.style.display = "none";
                        clearInterval(checkInterval);
                        document.cookie = "downloadToken=" + token + "; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
                    }
                }, 1000);

                var iframe = document.getElementById("downloadFrame");
                iframe.src = '<%= ResolveUrl("~/View/Common/detailreport.aspx") %>?download=1&token=' + token +
                             '&company=' + encodeURIComponent('<%= ViewState["Company"] ?? "" %>') +
                             '&category=' + encodeURIComponent('<%= ViewState["Category"] ?? "" %>');
            }

            // For GridView async postbacks
            if (typeof (Sys) !== "undefined") {
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_beginRequest(function () {
                    document.getElementById("loader").style.display = "flex";
                });
                prm.add_endRequest(function () {
                    document.getElementById("loader").style.display = "none";
                });
            }
        </script>
    </section>
</asp:Content>
