Public Class logout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Sign out and clear session
        FormsAuthentication.SignOut()
        Session.Abandon()

        ' Let the logout message render, then redirect after short delay (via JavaScript)
        Dim script As String = "<script>setTimeout(function() { window.location.href = 'login.aspx'; }, 3000);</script>"
        ClientScript.RegisterStartupScript(Me.GetType(), "RedirectScript", script)
    End Sub
End Class
