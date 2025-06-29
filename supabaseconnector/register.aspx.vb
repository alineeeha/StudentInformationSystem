Imports System.Data
Imports Npgsql
Imports System.Configuration
Imports System.Security.Cryptography
Imports System.Text

Partial Class Register
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    ' 🔐 Hashing function for password
    Private Function HashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function

    Protected Sub btnRegister_Click(sender As Object, e As EventArgs)
        Dim email As String = txtEmail.Text.Trim()
        Dim password As String = txtPassword.Text.Trim()
        Dim firstName As String = txtFirstName.Text.Trim()
        Dim lastName As String = txtLastName.Text.Trim()
        Dim enrollmentDate As Date = Date.Today
        Dim role As String = "student"

        Dim confirmPassword As String = txtConfirmPassword.Text.Trim()

        If email = "" OrElse password = "" OrElse confirmPassword = "" OrElse firstName = "" OrElse lastName = "" Then
            lblMessage.Text = "⚠ Please fill in all fields."
            Return
        End If

        If password <> confirmPassword Then
            lblMessage.Text = "⚠ Passwords do not match."
            Return
        End If

        Try
            Using conn As New NpgsqlConnection(connStr)
                conn.Open()

                ' 🔍 Check if user already exists
                Dim checkUserCmd As New NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @u", conn)
                checkUserCmd.Parameters.AddWithValue("@u", email)
                If Convert.ToInt32(checkUserCmd.ExecuteScalar()) > 0 Then
                    lblMessage.Text = "⚠ Email already in use."
                    Return
                End If

                ' 🔍 Check if student already exists
                Dim checkStudentCmd As New NpgsqlCommand("SELECT id FROM students WHERE email = @em", conn)
                checkStudentCmd.Parameters.AddWithValue("@em", email)
                Dim existingStudentId As Object = checkStudentCmd.ExecuteScalar()

                Dim studentId As Integer

                If existingStudentId IsNot Nothing Then
                    studentId = Convert.ToInt32(existingStudentId)
                Else
                    ' ➕ Insert new student
                    Dim insertStudentCmd As New NpgsqlCommand("
                        INSERT INTO students (first_name, last_name, email, enrollment_date)
                        VALUES (@fn, @ln, @em, @dt) RETURNING id", conn)
                    insertStudentCmd.Parameters.AddWithValue("@fn", firstName)
                    insertStudentCmd.Parameters.AddWithValue("@ln", lastName)
                    insertStudentCmd.Parameters.AddWithValue("@em", email)
                    insertStudentCmd.Parameters.AddWithValue("@dt", enrollmentDate)
                    studentId = Convert.ToInt32(insertStudentCmd.ExecuteScalar())
                End If

                ' 🔐 Hash the password
                Dim hashedPassword As String = HashPassword(password)

                ' ➕ Insert into users table
                Dim insertUserCmd As New NpgsqlCommand("
                    INSERT INTO users (username, password, role, student_id)
                    VALUES (@u, @p, @r, @sid)", conn)
                insertUserCmd.Parameters.AddWithValue("@u", email)
                insertUserCmd.Parameters.AddWithValue("@p", hashedPassword)
                insertUserCmd.Parameters.AddWithValue("@r", role)
                insertUserCmd.Parameters.AddWithValue("@sid", studentId)
                insertUserCmd.ExecuteNonQuery()

                Response.Redirect("login.aspx")
            End Using

        Catch ex As Exception
            lblMessage.Text = "❌ Error: " & Server.HtmlEncode(ex.Message)
        End Try
    End Sub
End Class
