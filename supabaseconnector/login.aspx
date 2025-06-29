<%@ Page Language="VB" AutoEventWireup="true" CodeBehind="login.aspx.vb" Inherits="supabaseconnector.Login" %>




<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - Student SIS</title>

    <!-- Bootstrap -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet" />
    <style>
        body {
            background: linear-gradient(to right, #007D52, #009966);
            font-family: 'Segoe UI', sans-serif;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container d-flex justify-content-center align-items-center" style="min-height: 100vh;">
            <div class="card p-4 shadow-lg" style="width: 100%; max-width: 400px; border-radius: 1rem;">
                <h3 class="text-center mb-4">🔐 Welcome Back</h3>

                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-3" />

                <div class="form-group mb-3">
                    <div class="input-group">
                        <span class="input-group-text"><i class="bi bi-person-fill"></i></span>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Email" />
                    </div>
                </div>

                <div class="form-group mb-4">
                    <div class="input-group">
                        <span class="input-group-text"><i class="bi bi-lock-fill"></i></span>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password" />
                    </div>
                </div>

                <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary w-100" Text="Login" OnClick="btnLogin_Click" />

                <p class="text-center mt-3" style="font-size: 0.9rem;">
                    <a href="register.aspx" class="text-decoration-none text-success">➕ Create a new account</a>
                </p>

            </div>
        </div>
    </form>

    <!-- Bootstrap Bundle JS -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
