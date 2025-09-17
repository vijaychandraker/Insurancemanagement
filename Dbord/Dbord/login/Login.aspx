<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Dbord.login.Login" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="System.Web.Optimization" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <!-- Bundled CSS -->
  <%: Styles.Render("~/bundles/css") %>

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

    @keyframes spin {
        0% {
            transform: rotate(0deg);
        }

        100% {
            transform: rotate(360deg);
        }
    }
</style>
</head>
<body class="hold-transition login-page" style="background: var(--primary-gradient);">
    <form id="form1" runat="server">
      <div class="login-box glass-fade-in">
        <div class="card glass-login">
          <div class="card-body login-card-body glass-login-body">
            <p class="login-box-msg">Login</p>

            <div class="input-group mb-3">
              <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control glass-input" placeholder="Email" TextMode="Email"></asp:TextBox>
              <div class="input-group-append">
                <div class="input-group-text glass-addon">
                  <span class="fas fa-envelope"></span>
                </div>
              </div>
            </div>

            <div class="input-group mb-3">
              <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control glass-input" placeholder="Password" TextMode="Password"></asp:TextBox>
              <div class="input-group-append">
                <div class="input-group-text glass-addon">
                  <span class="fas fa-lock"></span>
                </div>
              </div>
            </div>

            <div class="mb-3 text-center">
              <img src='<%= ResolveUrl("~/helpers/Captcha.ashx") %>' id="imgCaptcha" alt="captcha" class="mb-2" />
              <div>
                <a href="javascript:void(0);" onclick="refreshCaptcha()">Refresh</a>
              </div>
            </div>
            <div class="input-group mb-3">
              <asp:TextBox ID="txtCaptcha" runat="server" CssClass="form-control glass-input" Placeholder="Enter Captcha"></asp:TextBox>
              <div class="input-group-append">
                <div class="input-group-text glass-addon">
                  <span class="fas fa-shield-alt"></span>
                </div>
              </div>
            </div>

            <div class="row align-items-center">
              <div class="col-12 text-right">
                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn btn-primary glass-btn" OnClick="btnSignIn_Click" />
              </div>
            </div>

            <script type="text/javascript">
              function refreshCaptcha() {
                document.getElementById('imgCaptcha').src = '<%= ResolveUrl("~/helpers/Captcha.ashx") %>?t=' + new Date().getTime();
              }
            </script>

          </div>
        </div>
      </div>

        <div id="loader" style="display:none;">
    <div class="loader"></div>
</div>
   <%: Scripts.Render("~/bundles/jquery") %>
  <script>
      if (typeof window.jQuery === 'undefined') {
          document.write('<script src="<%= ResolveUrl("~/Assets/Admin/jquery/jquery.min.js") %>"><\/script>');
      }
  </script>
  <%: Scripts.Render("~/bundles/js") %>
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
