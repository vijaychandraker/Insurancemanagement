<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Regristration.aspx.cs" Inherits="Dbord.login.Regristration" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register</title>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,400i,700&display=fallback"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <style>
        body {
            margin: 0;
            font-family: 'Source Sans Pro', sans-serif;
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            background: #1e1e2f;
        }
        .register-box {
            width: 400px;
            padding: 2rem;
            border-radius: 15px;
            background: rgba(255,255,255,0.1);
            backdrop-filter: blur(10px);
            box-shadow: 0 8px 32px rgba(0,0,0,0.37);
            color: #fff;
        }
        .register-box h2 { text-align: center; margin-bottom: 2rem; }
        .form-control { border-radius: 10px; border: none; padding: 0.75rem; background: rgba(255,255,255,0.2); color: #fff; margin-bottom:1rem; }
        .btn-primary { width: 100%; border-radius: 10px; background: rgba(255,255,255,0.2); border: none; color: #fff; padding: 0.75rem; font-weight:600; transition: 0.3s; }
        .btn-primary:hover { background: rgba(255,255,255,0.3); cursor: pointer; }
        a { color: #fff; text-decoration: underline; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="register-box">
            <h2>Register</h2>

            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" Placeholder="Username"></asp:TextBox>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Email"></asp:TextBox>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Password"></asp:TextBox>
            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Confirm Password"></asp:TextBox>

            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-primary" OnClick="btnRegister_Click" />

            <p style="text-align:center; margin-top:1rem;">
                <a href="Login.aspx">Already have an account? Sign In</a>
            </p>
        </div>
    </form>
</body>
</html>
