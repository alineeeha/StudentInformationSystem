Imports System.Data
Imports System.Security.Cryptography
Imports System.Text
Imports Npgsql

Partial Class Account_Student
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStudentData()
        End If
    End Sub

    Private Sub LoadStudentData()
        Dim username = Session("username")?.ToString()
        If String.IsNullOrEmpty(username) Then
            lblMessage.Text = "❌ Session expired."
            Return
        End If

        Try
            Using conn As New NpgsqlConnection(connStr)
                conn.Open()
                Dim cmd As New NpgsqlCommand("SELECT first_name, last_name, email FROM students WHERE email = @e", conn)
                cmd.Parameters.AddWithValue("@e", username)
                Dim reader = cmd.ExecuteReader()

                If reader.Read() Then
                    txtFirstName.Text = reader("first_name").ToString()
                    txtLastName.Text = reader("last_name").ToString()
                    txtEmail.Text = reader("email").ToString()
                End If
                reader.Close()
            End Using
        Catch ex As Exception
            lblMessage.Text = "⚠ Fehler beim Laden der Daten: " & Server.HtmlEncode(ex.Message)
        End Try
    End Sub

    Private Function HashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function

    Private Function VerifyPassword(inputPassword As String, storedHash As String) As Boolean
        Return HashPassword(inputPassword) = storedHash
    End Function

    Protected Sub btnSaveChanges_Click(sender As Object, e As EventArgs)
        Dim username = Session("username")?.ToString()
        If String.IsNullOrEmpty(username) Then
            lblMessage.Text = "❌ Session abgelaufen."
            Return
        End If

        Dim newEmail = txtEmail.Text.Trim()
        Dim newFirst = txtFirstName.Text.Trim()
        Dim newLast = txtLastName.Text.Trim()
        Dim currentPwd = txtCurrentPassword.Text.Trim()
        Dim newPwd = txtNewPassword.Text.Trim()
        Dim confirmPwd = txtConfirmPassword.Text.Trim()

        If String.IsNullOrEmpty(newEmail) OrElse String.IsNullOrEmpty(newFirst) OrElse String.IsNullOrEmpty(newLast) Then
            lblMessage.Text = "❌ Please fill in all personal fields."
            Return
        End If

        Try
            Using conn As New NpgsqlConnection(connStr)
                conn.Open()

                ' Update personal data
                Dim updateInfoCmd As New NpgsqlCommand("UPDATE students SET first_name = @f, last_name = @l, email = @newEmail WHERE email = @oldEmail", conn)
                updateInfoCmd.Parameters.AddWithValue("@f", newFirst)
                updateInfoCmd.Parameters.AddWithValue("@l", newLast)
                updateInfoCmd.Parameters.AddWithValue("@newEmail", newEmail)
                updateInfoCmd.Parameters.AddWithValue("@oldEmail", username)
                updateInfoCmd.ExecuteNonQuery()

                ' Also update email in users table if changed
                If newEmail <> username Then
                    Dim updateUserCmd As New NpgsqlCommand("UPDATE users SET username = @newEmail WHERE username = @oldEmail", conn)
                    updateUserCmd.Parameters.AddWithValue("@newEmail", newEmail)
                    updateUserCmd.Parameters.AddWithValue("@oldEmail", username)
                    updateUserCmd.ExecuteNonQuery()
                    Session("username") = newEmail
                End If

                ' Password change if fields are filled
                If Not String.IsNullOrEmpty(currentPwd) OrElse Not String.IsNullOrEmpty(newPwd) OrElse Not String.IsNullOrEmpty(confirmPwd) Then
                    If String.IsNullOrEmpty(currentPwd) OrElse String.IsNullOrEmpty(newPwd) OrElse String.IsNullOrEmpty(confirmPwd) Then
                        lblMessage.Text = "❌ Please fill in all password fields to change password."
                        Return
                    End If

                    If newPwd <> confirmPwd Then
                        lblMessage.Text = "❌ Passwords do not match."
                        Return
                    End If

                    ' Get and verify current password
                    Dim cmdPwd As New NpgsqlCommand("SELECT password FROM users WHERE username = @u", conn)
                    cmdPwd.Parameters.AddWithValue("@u", username)
                    Dim storedHash = TryCast(cmdPwd.ExecuteScalar(), String)

                    If storedHash Is Nothing OrElse Not VerifyPassword(currentPwd, storedHash) Then
                        lblMessage.Text = "❌ Current password is incorrect."
                        Return
                    End If

                    ' Update password
                    Dim newHash = HashPassword(newPwd)
                    Dim updatePwdCmd As New NpgsqlCommand("UPDATE users SET password = @p WHERE username = @u", conn)
                    updatePwdCmd.Parameters.AddWithValue("@p", newHash)
                    updatePwdCmd.Parameters.AddWithValue("@u", username)
                    updatePwdCmd.ExecuteNonQuery()
                End If

                lblMessage.CssClass = "text-success"
                lblMessage.Text = "✅ Changes saved successfully."

            End Using
        Catch ex As Exception
            lblMessage.Text = "⚠ Error: " & Server.HtmlEncode(ex.Message)
        End Try
    End Sub
End Class
