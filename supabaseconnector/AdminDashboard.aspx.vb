Imports System.Data
Imports Npgsql
Imports Newtonsoft.Json

Partial Class AdminDashboard
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' 🚫 Restrict access to admins only
        If Session("role") Is Nothing OrElse Session("role").ToString().ToLower() <> "admin" Then
            Response.Redirect("~/login.aspx")
            Return
        End If

        If Not IsPostBack Then
            LoadDashboardStatsAndChart()
        End If
    End Sub

    Private Sub LoadDashboardStatsAndChart()
        Using conn As New NpgsqlConnection(connStr)
            conn.Open()

            ' Total students
            Using cmd As New NpgsqlCommand("SELECT COUNT(*) FROM students", conn)
                lblTotalStudents.Text = cmd.ExecuteScalar().ToString()
            End Using

            ' Total courses
            Using cmd As New NpgsqlCommand("SELECT COUNT(*) FROM courses", conn)
                lblTotalCourses.Text = cmd.ExecuteScalar().ToString()
            End Using

            ' First get the maximum number of enrollments
            Dim maxEnrollments As Integer = 0
            Using cmd As New NpgsqlCommand("
    SELECT MAX(course_count) FROM (
        SELECT COUNT(e.student_id) AS course_count
        FROM enrollments e
        GROUP BY e.course_id
    ) AS subquery", conn)
                Dim result = cmd.ExecuteScalar()
                If result IsNot DBNull.Value Then
                    maxEnrollments = Convert.ToInt32(result)
                End If
            End Using

            ' Now get all courses with that max enrollment count
            Using cmd As New NpgsqlCommand("
    SELECT c.course_name, COUNT(e.student_id) AS total
    FROM enrollments e
    INNER JOIN courses c ON c.course_id = e.course_id
    GROUP BY c.course_name
    HAVING COUNT(e.student_id) = @maxCount
    ORDER BY c.course_name", conn)

                cmd.Parameters.AddWithValue("@maxCount", maxEnrollments)

                Using reader = cmd.ExecuteReader()
                    Dim topCourses As New List(Of String)()
                    While reader.Read()
                        topCourses.Add($"{reader("course_name")} ({reader("total")} students)")
                    End While

                    If topCourses.Count > 0 Then
                        lblTopCourse.Text = String.Join("<br/>", topCourses)
                    Else
                        lblTopCourse.Text = "N/A"
                    End If
                End Using
            End Using


            ' Table + chart: students per course
            Using cmd As New NpgsqlCommand("
                SELECT c.course_name, COUNT(e.student_id) AS total_students
                FROM enrollments e
                INNER JOIN courses c ON e.course_id = c.course_id
                GROUP BY c.course_name
                ORDER BY c.course_name", conn)

                Using da As New NpgsqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)



                    ' Chart data lists
                    Dim labels As New List(Of String)()
                    Dim data As New List(Of Integer)()

                    For Each row As DataRow In dt.Rows
                        labels.Add(row("course_name").ToString())
                        data.Add(Convert.ToInt32(row("total_students")))
                    Next

                    ' Convert to JSON
                    Dim jsonLabels = JsonConvert.SerializeObject(labels)
                    Dim jsonValues = JsonConvert.SerializeObject(data)

                    ' Inject chart JS
                    ltChartData.Text = $"<script>renderStudentsPerCourseChart({jsonLabels}, {jsonValues});</script>"
                End Using
            End Using
            Using cmd As New NpgsqlCommand("
    SELECT format, COUNT(*) AS total
    FROM courses
    GROUP BY format
    ORDER BY format", conn)

                Using reader = cmd.ExecuteReader()
                    Dim formats As New List(Of String)()
                    Dim counts As New List(Of Integer)()

                    While reader.Read()
                        formats.Add(reader("format").ToString())
                        counts.Add(Convert.ToInt32(reader("total")))
                    End While

                    Dim jsonLabels = JsonConvert.SerializeObject(formats)
                    Dim jsonValues = JsonConvert.SerializeObject(counts)

                    ltChartData.Text &= $"<script>renderCourseFormatPieChart({jsonLabels}, {jsonValues});</script>"
                End Using
            End Using
            Using cmd As New NpgsqlCommand("
    SELECT TO_CHAR(enrollment_date, 'YYYY-MM') AS month, COUNT(*) AS total
    FROM enrollments
    GROUP BY month
    ORDER BY month", conn)

                Using reader = cmd.ExecuteReader()
                    Dim months As New List(Of String)()
                    Dim totals As New List(Of Integer)()

                    While reader.Read()
                        months.Add(reader("month").ToString())
                        totals.Add(Convert.ToInt32(reader("total")))
                    End While

                    Dim jsonLabels = JsonConvert.SerializeObject(months)
                    Dim jsonValues = JsonConvert.SerializeObject(totals)

                    ltChartData.Text &= $"<script>renderEnrollmentsOverTimeChart({jsonLabels}, {jsonValues});</script>"
                End Using
            End Using

        End Using
    End Sub
End Class
