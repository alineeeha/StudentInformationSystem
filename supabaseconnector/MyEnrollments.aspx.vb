Imports System.Data
Imports System.Web
Imports Npgsql

Partial Class MyEnrollments
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String =
        ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    '––––––––––––  PAGE LOAD  ––––––––––––'
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("role")?.ToString().ToLower() <> "student" Then
            Response.Redirect("~/login.aspx")
        End If

        If Not IsPostBack Then LoadMyEnrollments()
    End Sub

    '––––––––––––  FILL GRID  ––––––––––––'
    Private Sub LoadMyEnrollments()
        Const sql As String = "
            SELECT e.enrollment_id,
                   c.course_name,
                   i.title || ' ' || i.first_name || ' ' || i.last_name AS instructor,
                   e.enrollment_date
            FROM   enrollments e
            JOIN   students s   ON e.student_id  = s.id
            JOIN   courses  c   ON e.course_id   = c.course_id
            LEFT   JOIN instructors i ON c.instructor_id = i.instructor_id
            WHERE  s.email = @username
            ORDER  BY e.enrollment_date DESC;"

        Try
            Using conn As New NpgsqlConnection(connStr),
                  cmd As New NpgsqlCommand(sql, conn),
                  da As New NpgsqlDataAdapter(cmd)

                cmd.Parameters.AddWithValue("@username", Session("username").ToString())

                Dim dt As New DataTable()
                da.Fill(dt)

                gvMyEnrollments.DataSource = dt
                gvMyEnrollments.DataBind()
            End Using

        Catch ex As Exception
            lblMessage.Text = "⚠ Failed to load data: " & Server.HtmlEncode(ex.Message)
        End Try
    End Sub

    '––––––––––––  ADD DATA-ATTRIBUTES SAFELY  ––––––––––––'
    Protected Sub gvMyEnrollments_RowDataBound(
        sender As Object, e As GridViewRowEventArgs) Handles gvMyEnrollments.RowDataBound

        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim lnk = TryCast(e.Row.FindControl("lnkDrop"), LinkButton)
        If lnk Is Nothing Then Exit Sub                ' NEW safety check

        Dim course = DataBinder.Eval(e.Row.DataItem, "course_name").ToString()
        Dim id = DataBinder.Eval(e.Row.DataItem, "enrollment_id").ToString()

        lnk.Attributes("onclick") =
        $"showStudentDropModal({Js(course)}, {id}); return false;"
    End Sub


    'helper that wraps a string in single quotes and JS-escapes it
    Private Shared Function Js(value As String) As String
        Return $"'{HttpUtility.JavaScriptStringEncode(value)}'"
    End Function

    '––––––––––––  CONFIRM BUTTON  ––––––––––––'
    Protected Sub btnStudentConfirmDrop_Click(sender As Object, e As EventArgs)
        Dim id As Integer = Convert.ToInt32(hfEnrollmentIdToDrop.Value)

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Using cmd As New NpgsqlCommand("DELETE FROM enrollments WHERE enrollment_id = @id", conn)
                cmd.Parameters.AddWithValue("@id", id)

                Try
                    cmd.ExecuteNonQuery()
                    lblMessage.CssClass = "text-success fw-bold"
                    lblMessage.Text = "✅ Course dropped successfully."
                Catch ex As Exception
                    lblMessage.Text = "❌ Failed to drop course: " & ex.Message
                End Try
            End Using
        End Using

        LoadMyEnrollments()
    End Sub
End Class
