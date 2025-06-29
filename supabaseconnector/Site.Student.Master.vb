Imports System.Data
Imports Npgsql

Partial Class Site_Student
    Inherits System.Web.UI.MasterPage

    Public StudentFirstName As String = ""

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Session("username") IsNot Nothing Then
                Dim email As String = Session("username").ToString()
                Dim connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

                Using conn As New NpgsqlConnection(connStr)
                    conn.Open()
                    Dim cmd As New NpgsqlCommand("SELECT first_name FROM students WHERE email = @e", conn)
                    cmd.Parameters.AddWithValue("@e", email)

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        StudentFirstName = result.ToString()
                    Else
                        StudentFirstName = "Student"
                    End If
                End Using
            End If
        End If
    End Sub
End Class
