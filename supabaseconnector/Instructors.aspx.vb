Imports System.Data
Imports System.Configuration
Imports Npgsql
Imports System.Web.UI.WebControls

Partial Class Instructors
    Inherits System.Web.UI.Page

    Private ReadOnly connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("role")?.ToString()?.ToLower() <> "admin" Then
            Response.Redirect("~/login.aspx")
        End If

        If Not IsPostBack Then
            LoadInstructors()
        End If
    End Sub

    Private Sub LoadInstructors()
        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("SELECT instructor_id, title, first_name, last_name FROM instructors ORDER BY last_name", conn)
            Dim da As New NpgsqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            gvInstructors.DataSource = dt
            gvInstructors.DataBind()
        End Using
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim id = hfInstructorId.Value
        Dim title = ddlTitle.SelectedValue
        Dim firstName = txtFirstName.Text.Trim()
        Dim lastName = txtLastName.Text.Trim()

        If title = "" Or firstName = "" Or lastName = "" Then
            lblMessage.Text = "⚠ Please fill in all fields."
            Return
        End If

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As NpgsqlCommand

            If String.IsNullOrEmpty(id) Then
                cmd = New NpgsqlCommand("INSERT INTO instructors (title, first_name, last_name) VALUES (@t, @f, @l)", conn)
            Else
                cmd = New NpgsqlCommand("UPDATE instructors SET title = @t, first_name = @f, last_name = @l WHERE instructor_id = @id", conn)
                cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id))
            End If

            cmd.Parameters.AddWithValue("@t", title)
            cmd.Parameters.AddWithValue("@f", firstName)
            cmd.Parameters.AddWithValue("@l", lastName)
            cmd.ExecuteNonQuery()
        End Using

        lblMessage.Text = If(String.IsNullOrEmpty(id), "✅ Instructor added.", "✅ Instructor updated.")
        ClearForm()
        LoadInstructors()
    End Sub

    Protected Sub btnConfirmDelete_Click(sender As Object, e As EventArgs)
        Dim id As Integer = Convert.ToInt32(hfInstructorIdToDelete.Value)

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("DELETE FROM instructors WHERE instructor_id = @id", conn)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
        End Using

        lblMessage.Text = "🗑️ Instructor deleted."
        LoadInstructors()
    End Sub

    Protected Sub gvInstructors_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvInstructors.EditIndex = e.NewEditIndex
        LoadInstructors()
    End Sub

    Protected Sub gvInstructors_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvInstructors.EditIndex = -1
        LoadInstructors()
    End Sub

    Protected Sub gvInstructors_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        Dim row As GridViewRow = gvInstructors.Rows(e.RowIndex)
        Dim id As Integer = Convert.ToInt32(gvInstructors.DataKeys(e.RowIndex).Value)

        Dim ddlTitle As DropDownList = CType(row.FindControl("ddlEditTitle"), DropDownList)
        Dim txtFirstName As TextBox = CType(row.FindControl("txtEditFirstName"), TextBox)
        Dim txtLastName As TextBox = CType(row.FindControl("txtEditLastName"), TextBox)

        Dim title As String = ddlTitle.SelectedValue.Trim()
        Dim firstName As String = txtFirstName.Text.Trim()
        Dim lastName As String = txtLastName.Text.Trim()

        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim cmd As New NpgsqlCommand("UPDATE instructors SET title = @t, first_name = @f, last_name = @l WHERE instructor_id = @id", conn)
            cmd.Parameters.AddWithValue("@t", title)
            cmd.Parameters.AddWithValue("@f", firstName)
            cmd.Parameters.AddWithValue("@l", lastName)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
        End Using

        gvInstructors.EditIndex = -1
        LoadInstructors()
        lblMessage.Text = "✅ Instructor updated."
    End Sub

    Protected Sub gvInstructors_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvInstructors.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState.HasFlag(DataControlRowState.Edit) Then
                Dim currentTitle As String = DataBinder.Eval(e.Row.DataItem, "title").ToString()
                Dim ddlEditTitle As DropDownList = TryCast(e.Row.FindControl("ddlEditTitle"), DropDownList)
                If ddlEditTitle IsNot Nothing AndAlso ddlEditTitle.Items.FindByValue(currentTitle) IsNot Nothing Then
                    ddlEditTitle.SelectedValue = currentTitle
                End If
            Else
                ' Inject JS confirm delete
                Dim lnkDelete As LinkButton = TryCast(e.Row.FindControl("lnkDelete"), LinkButton)
                If lnkDelete IsNot Nothing Then
                    Dim firstName = DataBinder.Eval(e.Row.DataItem, "first_name").ToString()
                    Dim lastName = DataBinder.Eval(e.Row.DataItem, "last_name").ToString()
                    Dim id = DataBinder.Eval(e.Row.DataItem, "instructor_id").ToString()
                    lnkDelete.Attributes("onclick") = $"showConfirmDeleteModal('{firstName} {lastName}', {id}); return false;"
                End If
            End If
        End If
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        ClearForm()
    End Sub

    Private Sub ClearForm()
        ddlTitle.SelectedIndex = 0
        txtFirstName.Text = ""
        txtLastName.Text = ""
        hfInstructorId.Value = ""
        btnCancel.Visible = False
        btnSave.Text = "Save"
    End Sub
End Class
