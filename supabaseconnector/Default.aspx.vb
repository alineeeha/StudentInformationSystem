Imports System

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' 🔒 Zugriffsschutz: Redirect zu Login, falls keine Rolle gesetzt ist
        If Session("role") Is Nothing Then
            Response.Redirect("~/Login.aspx")
            Return
        End If

        ' Keine weiteren Aktionen nötig – statische Startseite
    End Sub
End Class
