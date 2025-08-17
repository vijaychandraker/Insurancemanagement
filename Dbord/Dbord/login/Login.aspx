<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Dbord.login.Login" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <!-- Google Font: Source Sans Pro -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,400i,700&display=fallback"/>
    <!-- Font Awesome -->
    <link rel="stylesheet" href="~/Assets/plugins/fontawesome-free/css/all.min.css"/>
    <!-- icheck bootstrap -->
    <link rel="stylesheet" href="~/Assets/plugins/icheck-bootstrap/icheck-bootstrap.min.css"/>
    <!-- Theme style -->
    <link rel="stylesheet" href="~/Assets/dist/css/adminlte.min.css"/>
    <link href="~/Assets/Scripts/sweetalert.css" rel="stylesheet" />
    <style>
    #loader {
        position: fixed;
        left: 0;
        top: 0;
        width: 100%;
        height: 100%;
        z-index: 9999;
        background: rgba(255, 255, 255, 0.7);
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .loader {
        border: 16px solid #f3f3f3;
        border-radius: 50%;
        border-top: 16px solid #3498db;
        width: 120px;
        height: 120px;
        animation: spin 2s linear infinite;
    }

    @@keyframes spin {
        0% {
            transform: rotate(0deg);
        }

        100% {
            transform: rotate(360deg);
        }
    }
</style>
</head>
<body class="hold-transition login-page">
    <form id="form1" runat="server">
      <div class="login-box">
  <!-- /.login-logo -->
  <div class="card">
    <div class="card-body login-card-body">
      <p class="login-box-msg">Login</p>
        <div class="input-group mb-3">
          <asp:TextBox ID="txtEmail" runat="server" class="form-control" placeholder="Email"></asp:TextBox>
          <div class="input-group-append">
            <div class="input-group-text">
              <span class="fas fa-envelope"></span>
            </div>
          </div>
        </div>
        <div class="input-group mb-3">
                   <asp:TextBox ID="txtPassword" runat="server" class="form-control" placeholder="Password"></asp:TextBox>
          <div class="input-group-append">
            <div class="input-group-text">
              <span class="fas fa-lock"></span>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-12">
            <div class="icheck-primary">
            <!-- Captcha -->
<img src='<%= ResolveUrl("~/helpers/Captcha.ashx") %>' id="imgCaptcha" alt="captcha" />
<a href="javascript:void(0);" onclick="refreshCaptcha()">Refresh</a>
<asp:TextBox ID="txtCaptcha" runat="server" CssClass="form-control" Placeholder="Enter Captcha"></asp:TextBox>
<script type="text/javascript">
    function refreshCaptcha() {
        document.getElementById("imgCaptcha").src =
            '<%= ResolveUrl("~/helpers/Captcha.ashx") %>?t=' + new Date().getTime();
    }
</script>
</div>
          </div>
          <div class="col-4">
              <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn btn-primary" OnClick="btnSignIn_Click" />
          </div>
        </div>
   
    </div>
  </div>
</div>
        <div id="loader" style="display:none;">
    <div class="loader"></div>
</div>
  <!-- Bootstrap 4 -->
  <!-- AdminLTE App -->
  <script src="~/Assets/Scripts/jquery.unobtrusive-ajax.min.js"></script>
  <script src="~/Assets/Scripts/sweetalert.js"></script>
  <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
  <script src="~/Assets/plugins/jquery-validation/jquery.validate.min.js"></script>
  <script src="~/Assets/plugins/jquery-validation/additional-methods.min.js"></script>
  <script src="<%= ResolveUrl("~/Assets/plugins/jquery/jquery.min.js") %>"></script>
<script src="<%= ResolveUrl("~/Assets/plugins/bootstrap/js/bootstrap.bundle.min.js") %>"></script>
<script src="<%= ResolveUrl("~/Assets/dist/js/adminlte.min.js") %>"></script>

         <script>
             $(document).ready(function () {
                 $("form").on("submit", function () {
                     $("#loader").show();
                 });
             });
         </script>
    </form>

</body>
</html>
