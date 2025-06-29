Imports System
Imports System.Configuration
Imports System.Data
Imports Npgsql

Partial Class StudentCourses
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("role") Is Nothing OrElse Session("role").ToString().ToLower() <> "student" Then
            Response.Redirect("~/login.aspx")
        End If

        If Not IsPostBack Then
            LoadCoursesForStudent()
        End If
    End Sub

    Private Sub LoadCoursesForStudent(Optional searchQuery As String = "")
        Using conn As New NpgsqlConnection(connStr)
            conn.Open()

            ' Hole die student_id über die Session-E-Mail
            Dim getStudentIdCmd As New NpgsqlCommand("SELECT id FROM students WHERE email = @e", conn)
            getStudentIdCmd.Parameters.AddWithValue("@e", Session("username").ToString())
            Dim studentIdObj = getStudentIdCmd.ExecuteScalar()

            If studentIdObj Is Nothing Then
                lblMessage.Text = "⚠ Student record not found."
                Return
            End If

            Dim studentId As Integer = Convert.ToInt32(studentIdObj)

            ' Hauptabfrage
            Dim sql As String = "
            SELECT 
                c.course_id, 
                c.course_name, 
                c.ects, 
                c.hours, 
                c.format,
                i.title || ' ' || i.first_name || ' ' || i.last_name AS instructor
            FROM courses c
            LEFT JOIN instructors i ON c.instructor_id = i.instructor_id
            WHERE c.course_id NOT IN (
                SELECT course_id FROM enrollments WHERE student_id = @sid
            )
        "

            If Not String.IsNullOrWhiteSpace(searchQuery) Then
                sql &= " AND LOWER(c.course_name) LIKE LOWER(@search)"
            End If

            sql &= " ORDER BY c.course_id"

            Dim cmd As New NpgsqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@sid", studentId)

            If Not String.IsNullOrWhiteSpace(searchQuery) Then
                cmd.Parameters.AddWithValue("@search", "%" & searchQuery & "%")
            End If

            Dim da As New NpgsqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            gvStudentCourses.DataSource = dt
            gvStudentCourses.DataBind()
        End Using
    End Sub


    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Dim query As String = txtSearch.Text.Trim()
        LoadCoursesForStudent(query)
    End Sub

    Protected Sub btnClear_Click(sender As Object, e As EventArgs)
        txtSearch.Text = ""
        LoadCoursesForStudent()
    End Sub

    Protected Sub gvStudentCourses_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "Enroll" Then
            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            Dim courseId As Integer = Convert.ToInt32(gvStudentCourses.DataKeys(rowIndex).Value)
            Dim username As String = Session("username").ToString()

            Using conn As New NpgsqlConnection(connStr)
                conn.Open()

                ' Get student_id by email
                Dim getStudentIdCmd As New NpgsqlCommand("SELECT id FROM students WHERE email = @e", conn)
                getStudentIdCmd.Parameters.AddWithValue("@e", username)
                Dim studentIdObj = getStudentIdCmd.ExecuteScalar()

                If studentIdObj Is Nothing Then
                    lblMessage.Text = "⚠ Student record not found."
                    Return
                End If

                Dim studentId As Integer = Convert.ToInt32(studentIdObj)

                ' Check for existing enrollment
                Dim checkCmd As New NpgsqlCommand("SELECT COUNT(*) FROM enrollments WHERE student_id = @sid AND course_id = @cid", conn)
                checkCmd.Parameters.AddWithValue("@sid", studentId)
                checkCmd.Parameters.AddWithValue("@cid", courseId)

                If Convert.ToInt32(checkCmd.ExecuteScalar()) > 0 Then
                    lblMessage.Text = "⚠ You are already enrolled in this course."
                    Return
                End If

                ' Insert enrollment
                Dim enrollCmd As New NpgsqlCommand("INSERT INTO enrollments (student_id, course_id, enrollment_date) VALUES (@s, @c, CURRENT_DATE)", conn)
                enrollCmd.Parameters.AddWithValue("@s", studentId)
                enrollCmd.Parameters.AddWithValue("@c", courseId)
                enrollCmd.ExecuteNonQuery()

                lblMessage.Text = "✅ Enrollment successful!"
            End Using

            LoadCoursesForStudent()
        End If
    End Sub
End Class
