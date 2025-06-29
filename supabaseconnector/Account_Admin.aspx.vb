Imports System.Data
Imports System.Security.Cryptography
Imports System.Text
Imports Npgsql

Partial Class Account_Admin
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadUsername()
        End If
    End Sub

    Private Sub LoadUsername()
        Dim username = Session("username")?.ToString()
        If String.IsNullOrEmpty(username) Then
            lblMessage.Text = "❌ Session expired."
            Return
        End If
        txtUsername.Text = username
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

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
        Dim oldUsername = Session("username")?.ToString()
        Dim newUsername = txtUsername.Text.Trim()
        Dim currentPwd = txtCurrentPassword.Text.Trim()
        Dim newPwd = txtNewPassword.Text.Trim()
        Dim confirmPwd = txtConfirmPassword.Text.Trim()

        If String.IsNullOrEmpty(oldUsername) OrElse String.IsNullOrEmpty(newUsername) Then
            lblMessage.Text = "❌ Invalid session or empty username."
            Return
        End If

        Try
            Using conn As New NpgsqlConnection(connStr)
                conn.Open()

                ' Get current password hash
                Dim cmdCheck As New NpgsqlCommand("SELECT password FROM users WHERE username = @u", conn)
                cmdCheck.Parameters.AddWithValue("@u", oldUsername)
                Dim storedHash = TryCast(cmdCheck.ExecuteScalar(), String)

                If storedHash Is Nothing OrElse Not VerifyPassword(currentPwd, storedHash) Then
                    lblMessage.Text = "❌ Current password is incorrect."
                    Return
                End If

                ' Change password if fields are filled
                If Not String.IsNullOrEmpty(newPwd) OrElse Not String.IsNullOrEmpty(confirmPwd) Then
                    If newPwd <> confirmPwd Then
                        lblMessage.Text = "❌ Passwords do not match."
                        Return
                    End If

                    Dim newHash = HashPassword(newPwd)
                    Dim updatePwdCmd As New NpgsqlCommand("UPDATE users SET password = @p WHERE username = @u", conn)
                    updatePwdCmd.Parameters.AddWithValue("@p", newHash)
                    updatePwdCmd.Parameters.AddWithValue("@u", oldUsername)
                    updatePwdCmd.ExecuteNonQuery()
                End If

                ' If username changed, update and refresh session
                If newUsername <> oldUsername Then
                    ' Check if new username already exists
                    Dim checkCmd As New NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @n", conn)
                    checkCmd.Parameters.AddWithValue("@n", newUsername)
                    Dim exists = CInt(checkCmd.ExecuteScalar()) > 0
                    If exists Then
                        lblMessage.Text = "❌ Username already taken."
                        Return
                    End If

                    Dim updateUserCmd As New NpgsqlCommand("UPDATE users SET username = @newUsername WHERE username = @oldUsername", conn)
                    updateUserCmd.Parameters.AddWithValue("@newUsername", newUsername)
                    updateUserCmd.Parameters.AddWithValue("@oldUsername", oldUsername)
                    updateUserCmd.ExecuteNonQuery()

                    Session("username") = newUsername
                End If

                lblMessage.CssClass = "text-success"
                lblMessage.Text = "✅ Changes saved successfully."

            End Using
        Catch ex As Exception
            lblMessage.Text = "⚠ Error: " & Server.HtmlEncode(ex.Message)
        End Try
    End Sub
End Class
