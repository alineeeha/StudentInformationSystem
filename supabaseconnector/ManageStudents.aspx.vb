Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports Npgsql

Partial Class Students
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    ' Force US-culture so yyyy-MM-dd matches server expectations
    Protected Overrides Sub InitializeCulture()
        Culture = "en-US"
        UICulture = "en-US"
        MyBase.InitializeCulture()
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("role") Is Nothing OrElse Session("role").ToString().ToLower() <> "admin" Then
            Response.Redirect("~/login.aspx")
            Return
        End If

        If Not IsPostBack Then
            txtEnrollmentDate.Attributes("placeholder") = "yyyy-MM-dd"
            LoadStudents()
        End If
    End Sub

    ' ---------------- Utility ----------------
    Private Sub LoadStudents()
        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("
                SELECT id, first_name, last_name, email, enrollment_date
                FROM students
                ORDER BY id", conn)
            Dim da As New NpgsqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            gvStudents.DataSource = dt
            gvStudents.DataBind()
        End Using

        ' Add placeholder to inline-edit textbox (if any)
        If gvStudents.EditIndex >= 0 Then
            Dim row As GridViewRow = gvStudents.Rows(gvStudents.EditIndex)
            Dim txtEditDate As TextBox = CType(row.FindControl("txtEditDate"), TextBox)
            If txtEditDate IsNot Nothing Then
                txtEditDate.Attributes("placeholder") = "yyyy-MM-dd"
            End If
        End If
    End Sub

    ' ---------------- Add / Update via Form ----------------
    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim id = hfStudentId.Value
        Dim firstName = txtFirstName.Text.Trim()
        Dim lastName = txtLastName.Text.Trim()
        Dim email = txtEmail.Text.Trim()

        Dim enrollDate As DateTime
        If Not DateTime.TryParseExact(
                txtEnrollmentDate.Text.Trim(),
                "yyyy-MM-dd",
                Globalization.CultureInfo.InvariantCulture,
                Globalization.DateTimeStyles.None,
                enrollDate) Then
            lblMessage.Text = "❌ Please enter a valid enrollment date (yyyy-MM-dd)."
            Return
        End If

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As NpgsqlCommand

            If String.IsNullOrEmpty(id) Then
                cmd = New NpgsqlCommand("
                    INSERT INTO students (first_name, last_name, email, enrollment_date)
                    VALUES (@f, @l, @m, @e)", conn)
            Else
                cmd = New NpgsqlCommand("
                    UPDATE students
                    SET first_name = @f, last_name = @l, email = @m, enrollment_date = @e
                    WHERE id = @id", conn)
                cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id))
            End If

            cmd.Parameters.AddWithValue("@f", firstName)
            cmd.Parameters.AddWithValue("@l", lastName)
            cmd.Parameters.AddWithValue("@m", email)
            cmd.Parameters.AddWithValue("@e", enrollDate)
            cmd.ExecuteNonQuery()
        End Using

        lblMessage.Text = If(String.IsNullOrEmpty(id), "✅ Student created.", "✅ Student updated.")
        ClearForm()
        LoadStudents()
    End Sub

    ' ---------------- GridView Events ----------------
    Protected Sub gvStudents_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvStudents.EditIndex = e.NewEditIndex
        LoadStudents()
    End Sub

    Protected Sub gvStudents_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        Dim id As Long = Convert.ToInt64(gvStudents.DataKeys(e.RowIndex).Value)
        Dim row As GridViewRow = gvStudents.Rows(e.RowIndex)

        Dim txtEditFirstName As TextBox = CType(row.FindControl("txtEditFirstName"), TextBox)
        Dim txtEditLastName As TextBox = CType(row.FindControl("txtEditLastName"), TextBox)
        Dim txtEditEmail As TextBox = CType(row.FindControl("txtEditEmail"), TextBox)
        Dim txtEditDate As TextBox = CType(row.FindControl("txtEditDate"), TextBox)

        Dim firstName As String = txtEditFirstName.Text.Trim()
        Dim lastName As String = txtEditLastName.Text.Trim()
        Dim email As String = txtEditEmail.Text.Trim()

        Dim enrollDate As DateTime
        If Not DateTime.TryParseExact(
                txtEditDate.Text.Trim(),
                "yyyy-MM-dd",
                Globalization.CultureInfo.InvariantCulture,
                Globalization.DateTimeStyles.None,
                enrollDate) Then
            lblMessage.Text = "❌ Please enter a valid enrollment date (yyyy-MM-dd)."
            Return
        End If

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("
                UPDATE students
                SET first_name = @f, last_name = @l, email = @m, enrollment_date = @e
                WHERE id = @id", conn)

            cmd.Parameters.AddWithValue("@f", firstName)
            cmd.Parameters.AddWithValue("@l", lastName)
            cmd.Parameters.AddWithValue("@m", email)
            cmd.Parameters.AddWithValue("@e", enrollDate)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
        End Using

        gvStudents.EditIndex = -1
        LoadStudents()
        lblMessage.Text = "✅ Student updated inline successfully!"
    End Sub

    Protected Sub gvStudents_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvStudents.EditIndex = -1
        LoadStudents()
    End Sub

    ' ---------------- Modal Delete (with confirmation) ----------------
    Protected Sub gvStudents_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim btnDelete As LinkButton = TryCast(e.Row.FindControl("lnkDelete"), LinkButton)
        If btnDelete Is Nothing Then Exit Sub

        Dim studentId = DataBinder.Eval(e.Row.DataItem, "id").ToString()
        Dim fullName = DataBinder.Eval(e.Row.DataItem, "first_name").ToString() & " " & DataBinder.Eval(e.Row.DataItem, "last_name").ToString()
        btnDelete.Attributes("onclick") = $"showConfirmStudentModal({studentId}, {Js(fullName)}); return false;"
    End Sub

    Public Shared Function Js(value As String) As String
        Return "'" & HttpUtility.JavaScriptStringEncode(value) & "'"
    End Function

    Protected Sub btnConfirmDeleteStudent_Click(sender As Object, e As EventArgs)
        Dim id As Long = Convert.ToInt64(hfStudentIdToDelete.Value)

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()

            ' Cascade deletes
            Dim cmdEnroll As New NpgsqlCommand("DELETE FROM enrollments WHERE student_id = @id", conn)
            cmdEnroll.Parameters.AddWithValue("@id", id)
            cmdEnroll.ExecuteNonQuery()

            Dim cmdUser As New NpgsqlCommand("DELETE FROM users WHERE student_id = @id", conn)
            cmdUser.Parameters.AddWithValue("@id", id)
            cmdUser.ExecuteNonQuery()

            Dim cmdStu As New NpgsqlCommand("DELETE FROM students WHERE id = @id", conn)
            cmdStu.Parameters.AddWithValue("@id", id)
            cmdStu.ExecuteNonQuery()
        End Using

        lblMessage.Text = "✅ Student and related data deleted successfully."
        LoadStudents()
    End Sub

    ' ---------------- Helpers ----------------
    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        ClearForm()
    End Sub

    Private Sub ClearForm()
        txtFirstName.Text = ""
        txtLastName.Text = ""
        txtEmail.Text = ""
        txtEnrollmentDate.Text = ""
        hfStudentId.Value = ""
        btnSave.Text = "Save"
        btnCancel.Visible = False
    End Sub
End Class
