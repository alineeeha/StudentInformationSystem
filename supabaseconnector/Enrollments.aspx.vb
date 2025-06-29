Imports System.Data
Imports System.Configuration
Imports Npgsql

Partial Class Enrollments
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("role") Is Nothing OrElse Session("role").ToString().ToLower() <> "admin" Then
            Response.Redirect("~/login.aspx")
            Return
        End If

        If Not IsPostBack Then
            LoadCourseDropdown()
            LoadEnrollments()
        End If
    End Sub

    Private Sub LoadEnrollments()
        Dim selectedCourse As String = ddlCourseFilter.SelectedValue

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()

            Dim query As String = "
                SELECT e.enrollment_id, 
                       s.first_name || ' ' || s.last_name AS student_name,
                       c.course_name, 
                       e.enrollment_date
                FROM enrollments e
                INNER JOIN students s ON e.student_id = s.id
                INNER JOIN courses c ON e.course_id = c.course_id
            "

            If Not String.IsNullOrEmpty(selectedCourse) Then
                query &= " WHERE c.course_name = @courseName"
            End If

            query &= " ORDER BY e.enrollment_id"

            Dim cmd As New NpgsqlCommand(query, conn)
            If Not String.IsNullOrEmpty(selectedCourse) Then
                cmd.Parameters.AddWithValue("@courseName", selectedCourse)
            End If

            Dim da As New NpgsqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)

            gvEnrollments.DataSource = dt
            gvEnrollments.DataBind()
        End Using
    End Sub

    Private Sub LoadCourseDropdown()
        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("SELECT DISTINCT course_name FROM courses ORDER BY course_name", conn)
            Dim reader = cmd.ExecuteReader()

            ddlCourseFilter.Items.Clear()
            ddlCourseFilter.Items.Add(New ListItem("All Courses", ""))

            While reader.Read()
                ddlCourseFilter.Items.Add(New ListItem(reader("course_name").ToString(), reader("course_name").ToString()))
            End While
        End Using
    End Sub

    Protected Sub ddlCourseFilter_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadEnrollments()
    End Sub

    Protected Sub btnClearFilter_Click(sender As Object, e As EventArgs)
        ddlCourseFilter.SelectedIndex = 0
        LoadEnrollments()
    End Sub

    Protected Sub gvEnrollments_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvEnrollments.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim btn = TryCast(e.Row.FindControl("lnkDrop"), LinkButton)
        If btn Is Nothing Then Exit Sub

        Dim student = DataBinder.Eval(e.Row.DataItem, "student_name").ToString()
        Dim course = DataBinder.Eval(e.Row.DataItem, "course_name").ToString()
        Dim id = DataBinder.Eval(e.Row.DataItem, "enrollment_id").ToString()

        btn.Attributes("onclick") = $"showConfirmModal({Js(student)}, {Js(course)}, {id}); return false;"
    End Sub

    Private Shared Function Js(value As String) As String
        Return $"'{HttpUtility.JavaScriptStringEncode(value)}'"
    End Function

    Protected Sub btnConfirmDrop_Click(sender As Object, e As EventArgs)
        Dim enrollmentId As Integer = Convert.ToInt32(hfEnrollmentIdToDrop.Value)

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("DELETE FROM enrollments WHERE enrollment_id = @id", conn)
            cmd.Parameters.AddWithValue("@id", enrollmentId)
            cmd.ExecuteNonQuery()
        End Using

        LoadEnrollments()
    End Sub
End Class
