<%@ Page Language="VB" CodeBehind="register.aspx.vb" Inherits="supabaseconnector.Register" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Account</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
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
                <h3 class="text-center mb-4">➕ Create Account</h3>

                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-3" />

                    <div class="mb-3">
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="First Name" />
                    </div>
                    <div class="mb-3">
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Last Name" />
                    </div>
                    <div class="mb-3">
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Email" />
                    </div>


                <div class="form-group mb-3">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password" />
                </div>

                <div class="form-group mb-3">
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Confirm Password" />
                </div>

                <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-success w-100" OnClick="btnRegister_Click" />

                <p class="text-center mt-3">
                    <a href="login.aspx" class="text-decoration-none text-muted">← Back to login</a>
                </p>
            </div>
        </div>
    </form>
</body>
</html>
