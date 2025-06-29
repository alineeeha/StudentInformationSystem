Imports System
Imports System.Data
Imports System.Configuration
Imports Npgsql
Imports System.Web.UI.WebControls

Partial Class Courses
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("role")?.ToString()?.ToLower() <> "admin" Then
            Response.Redirect("~/login.aspx")
        End If

        If Not IsPostBack Then
            LoadInstructors()
            LoadCoursesForAdmin()
        End If
    End Sub

    Private Sub LoadInstructors()
        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("SELECT instructor_id, title || ' ' || first_name || ' ' || last_name AS full_name FROM instructors ORDER BY last_name", conn)
            Dim rdr As NpgsqlDataReader = cmd.ExecuteReader()
            ddlInstructor.DataSource = rdr
            ddlInstructor.DataTextField = "full_name"
            ddlInstructor.DataValueField = "instructor_id"
            ddlInstructor.DataBind()
        End Using

        ddlInstructor.Items.Insert(0, New ListItem("Select instructor", ""))
    End Sub

    Private Sub LoadCoursesForAdmin()
        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("
                SELECT 
                    c.course_id, 
                    c.course_name, 
                    c.ects, 
                    c.hours, 
                    c.format, 
                    i.title || ' ' || i.first_name || ' ' || i.last_name AS instructor
                FROM courses c
                LEFT JOIN instructors i ON c.instructor_id = i.instructor_id
                ORDER BY c.course_id", conn)
            Dim da As New NpgsqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            gvCourses.DataSource = dt
            gvCourses.DataBind()
        End Using
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim id = hfCourseId.Value
        Dim name = txtCourseName.Text.Trim()
        Dim ects = Convert.ToInt32(txtECTS.Text)
        Dim hours = Convert.ToInt32(txtHours.Text)
        Dim format = ddlFormat.SelectedValue
        Dim instructorId = Convert.ToInt32(ddlInstructor.SelectedValue)

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As NpgsqlCommand

            If String.IsNullOrEmpty(id) Then
                cmd = New NpgsqlCommand("INSERT INTO courses (course_name, ects, hours, format, instructor_id) VALUES (@n, @e, @h, @f, @i)", conn)
            Else
                cmd = New NpgsqlCommand("UPDATE courses SET course_name = @n, ects = @e, hours = @h, format = @f, instructor_id = @i WHERE course_id = @id", conn)
                cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id))
            End If

            cmd.Parameters.AddWithValue("@n", name)
            cmd.Parameters.AddWithValue("@e", ects)
            cmd.Parameters.AddWithValue("@h", hours)
            cmd.Parameters.AddWithValue("@f", format)
            cmd.Parameters.AddWithValue("@i", instructorId)
            cmd.ExecuteNonQuery()
        End Using

        lblMessage.Text = If(String.IsNullOrEmpty(id), "✅ Course added.", "✅ Course updated.")
        ClearForm()
        LoadCoursesForAdmin()
    End Sub

    Protected Sub gvCourses_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvCourses.EditIndex = e.NewEditIndex
        LoadCoursesForAdmin()
    End Sub

    Protected Sub gvCourses_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvCourses.EditIndex = -1
        LoadCoursesForAdmin()
    End Sub

    Protected Sub gvCourses_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        Dim row As GridViewRow = gvCourses.Rows(e.RowIndex)
        Dim id As Integer = Convert.ToInt32(gvCourses.DataKeys(e.RowIndex).Value)

        Dim txtEditName As TextBox = CType(row.FindControl("txtEditCourseName"), TextBox)
        Dim txtEditEcts As TextBox = CType(row.FindControl("txtEditEcts"), TextBox)
        Dim txtEditHours As TextBox = CType(row.FindControl("txtEditHours"), TextBox)
        Dim ddlEditFormat As DropDownList = CType(row.FindControl("ddlEditFormat"), DropDownList)
        Dim ddlEditInstructor As DropDownList = CType(row.FindControl("ddlEditInstructor"), DropDownList)

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("UPDATE courses SET course_name = @n, ects = @e, hours = @h, format = @f, instructor_id = @i WHERE course_id = @id", conn)
            cmd.Parameters.AddWithValue("@n", txtEditName.Text.Trim())
            cmd.Parameters.AddWithValue("@e", Convert.ToInt32(txtEditEcts.Text))
            cmd.Parameters.AddWithValue("@h", Convert.ToInt32(txtEditHours.Text))
            cmd.Parameters.AddWithValue("@f", ddlEditFormat.SelectedValue)
            cmd.Parameters.AddWithValue("@i", Convert.ToInt32(ddlEditInstructor.SelectedValue))
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
        End Using

        gvCourses.EditIndex = -1
        LoadCoursesForAdmin()
        lblMessage.Text = "✅ Course updated inline."
    End Sub

    Protected Sub btnConfirmDelete_Click(sender As Object, e As EventArgs)
        Dim courseId As Integer = Convert.ToInt32(hfCourseIdToDelete.Value)

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim checkCmd As New NpgsqlCommand("SELECT COUNT(*) FROM enrollments WHERE course_id = @id", conn)
            checkCmd.Parameters.AddWithValue("@id", courseId)
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

            If count > 0 Then
                lblMessage.Text = "❌ Cannot delete: this course is currently assigned to one or more students."
                Return
            End If

        End Using

        lblMessage.Text = "🗑️ Course deleted."
        LoadCoursesForAdmin()
    End Sub

    Protected Sub gvCourses_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvCourses.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState.HasFlag(DataControlRowState.Edit) Then
                Dim ddlEditInstructor As DropDownList = CType(e.Row.FindControl("ddlEditInstructor"), DropDownList)

                Using conn As New NpgsqlConnection(connStr)
                    conn.Open()
                    Dim cmd As New NpgsqlCommand("SELECT instructor_id, title || ' ' || first_name || ' ' || last_name AS full_name FROM instructors ORDER BY last_name", conn)
                    Dim rdr As NpgsqlDataReader = cmd.ExecuteReader()
                    ddlEditInstructor.DataSource = rdr
                    ddlEditInstructor.DataTextField = "full_name"
                    ddlEditInstructor.DataValueField = "instructor_id"
                    ddlEditInstructor.DataBind()
                End Using
            Else
                ' Inject JS modal confirm delete
                Dim lnkDelete As LinkButton = TryCast(e.Row.FindControl("lnkDelete"), LinkButton)
                If lnkDelete IsNot Nothing Then
                    Dim courseName = DataBinder.Eval(e.Row.DataItem, "course_name").ToString()
                    Dim courseId = DataBinder.Eval(e.Row.DataItem, "course_id").ToString()
                    lnkDelete.Attributes("onclick") = $"showCourseDeleteModal({Js(courseName)}, {courseId}); return false;"
                End If
            End If
        End If
    End Sub

    Private Function Js(value As String) As String
        Return $"'{HttpUtility.JavaScriptStringEncode(value)}'"
    End Function

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        ClearForm()
    End Sub

    Private Sub ClearForm()
        txtCourseName.Text = ""
        txtECTS.Text = ""
        txtHours.Text = ""
        ddlFormat.SelectedIndex = 0
        ddlInstructor.SelectedIndex = 0
        hfCourseId.Value = ""
        btnSave.Text = "Save"
        btnCancel.Visible = False
    End Sub
End Class
