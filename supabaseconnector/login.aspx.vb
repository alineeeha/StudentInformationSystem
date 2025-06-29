Imports System.Data
Imports System.Security.Cryptography
Imports System.Text
Imports Npgsql

Partial Class Login
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    Private Function VerifyPassword(inputPassword As String, storedHash As String) As Boolean
        ' Legacy SHA-256 only (for reset)
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(inputPassword)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Dim hex = BitConverter.ToString(hash).Replace("-", "").ToLower()
            Return storedHash = hex
        End Using
    End Function




    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text.Trim()

        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            lblMessage.Text = "⚠ Please fill in username and password"
            Return
        End If

        Try
            Using conn As New NpgsqlConnection(connStr)
                conn.Open()

                ' ✅ Get stored password hash + role
                Dim cmdHash As New NpgsqlCommand("SELECT password, role FROM users WHERE username = @u", conn)
                cmdHash.Parameters.AddWithValue("@u", username)

                Dim reader = cmdHash.ExecuteReader()

                If reader.Read() Then
                    Dim storedHash As String = reader.GetString(0)
                    Dim role As String = reader.GetString(1)
                    reader.Close()

                    If VerifyPassword(password, storedHash) Then
                        Session("username") = username
                        Session("role") = role.ToLower()

                        ' Optionally get student name
                        If role.ToLower() = "student" Then
                            Dim cmdStudent As New NpgsqlCommand("SELECT first_name FROM students WHERE email = @u", conn)
                            cmdStudent.Parameters.AddWithValue("@u", username)
                            Dim firstNameObj = cmdStudent.ExecuteScalar()
                            Session("student_first_name") = If(firstNameObj IsNot Nothing, firstNameObj.ToString(), "")
                        End If

                        ' Redirect based on role
                        If role.ToLower() = "admin" Then
                            Response.Redirect("~/Default.aspx")
                        ElseIf role.ToLower() = "student" Then
                            Response.Redirect("~/StudentHome.aspx")
                        Else
                            lblMessage.Text = "❌ Unknown role."
                        End If
                    Else
                        lblMessage.Text = "❌ Wrong username or password."
                    End If
                Else
                    lblMessage.Text = "❌ User does not exist."
                End If
            End Using

        Catch ex As Exception
            lblMessage.Text = "⚠ Login error: " & Server.HtmlEncode(ex.Message)
        End Try
    End Sub

End Class
