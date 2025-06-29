<%@ Page Title="Manage Courses" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Courses.aspx.vb" Inherits="supabaseconnector.Courses" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-4">Course Management</h2>

    <!-- Form for adding/updating -->
    <div class="row mb-4">
        <div class="col-md-6">
            <div class="card shadow-sm">
                <div class="card-body">
                    <h5 class="card-title">Add / Edit Course</h5>

                    <div class="form-group">
                        <label>Course Name</label>
                        <asp:TextBox ID="txtCourseName" runat="server" CssClass="form-control fixed-width" />
                    </div>
                    <div class="form-group">
                        <label>ECTS</label>
                        <asp:TextBox ID="txtECTS" runat="server" CssClass="form-control fixed-width" TextMode="Number" />
                    </div>
                    <div class="form-group">
                        <label>Hours</label>
                        <asp:TextBox ID="txtHours" runat="server" CssClass="form-control fixed-width" TextMode="Number" />
                    </div>
                    <div class="form-group">
                        <label>Format</label>
                        <asp:DropDownList ID="ddlFormat" runat="server" CssClass="form-control fixed-width">
                            <asp:ListItem Text="Select format" Value="" />
                            <asp:ListItem Text="Online" Value="online" />
                            <asp:ListItem Text="Blended" Value="blended" />
                            <asp:ListItem Text="Campus" Value="campus" />
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Instructor</label>
                        <asp:DropDownList ID="ddlInstructor" runat="server" CssClass="form-control fixed-width" />
                    </div>

                    <asp:HiddenField ID="hfCourseId" runat="server" />

                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success mt-3" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary mt-3" Visible="false" OnClick="btnCancel_Click" />
                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-2 text-success" />
                </div>
            </div>
        </div>
    </div>

    <!-- Course table -->
    <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
        DataKeyNames="course_id"
        OnRowEditing="gvCourses_RowEditing"
        OnRowCancelingEdit="gvCourses_RowCancelingEdit"
        OnRowUpdating="gvCourses_RowUpdating"
        OnRowDataBound="gvCourses_RowDataBound">
        <Columns>
            <asp:BoundField DataField="course_id" HeaderText="ID" ReadOnly="True" />

            <asp:TemplateField HeaderText="Course Name">
                <ItemTemplate>
                    <%# Eval("course_name") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditCourseName" runat="server" CssClass="form-control" Text='<%# Bind("course_name") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="ECTS">
                <ItemTemplate>
                    <%# Eval("ects") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditEcts" runat="server" CssClass="form-control" Text='<%# Bind("ects") %>' TextMode="Number" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Hours">
                <ItemTemplate>
                    <%# Eval("hours") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditHours" runat="server" CssClass="form-control" Text='<%# Bind("hours") %>' TextMode="Number" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Format">
                <ItemTemplate>
                    <%# Eval("format") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlEditFormat" runat="server" CssClass="form-select" SelectedValue='<%# Bind("format") %>'>
                        <asp:ListItem Text="Online" Value="online" />
                        <asp:ListItem Text="Blended" Value="blended" />
                        <asp:ListItem Text="Campus" Value="campus" />
                    </asp:DropDownList>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Instructor">
                <ItemTemplate>
                    <%# Eval("instructor") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlEditInstructor" runat="server" CssClass="form-select" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Actions">
                <ItemStyle Width="170px" />
                <HeaderStyle Width="170px" />
                <ItemTemplate>
                    <div class="d-flex gap-1">
                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit"
                            CssClass="btn btn-warning enroll-btn btn-sm" Text="✏️ Edit" />
                        <asp:LinkButton ID="lnkDelete" runat="server"
                            Text="🗑 Delete"
                            CssClass="btn btn-danger drop-btn btn-sm"
                            CausesValidation="false" />
                    </div>
                </ItemTemplate>
                <EditItemTemplate>
                    <div class="d-flex gap-1">
                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update"
                            CssClass="btn btn-success enroll-btn btn-sm" Text="💾 Save" />
                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel"
                            CssClass="btn btn-secondary drop-btn btn-sm" Text="❌ Cancel" />
                    </div>
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <!-- Modal -->
    <div class="modal fade" id="confirmDeleteCourseModal" tabindex="-1" aria-labelledby="confirmDeleteLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-danger">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title" id="confirmDeleteLabel">Confirm Delete</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p id="confirmDeleteText"></p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmDelete" runat="server" CssClass="btn btn-danger" Text="Confirm" OnClick="btnConfirmDelete_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfCourseIdToDelete" runat="server" />

    <script type="text/javascript">
        function showCourseDeleteModal(courseName, courseId) {
            const text = `Are you sure you want to delete the course <strong>${courseName}</strong>?`;
            document.getElementById("confirmDeleteText").innerHTML = text;
            document.getElementById("<%= hfCourseIdToDelete.ClientID %>").value = courseId;
            const modal = new bootstrap.Modal(document.getElementById('confirmDeleteCourseModal'));
            modal.show();
        }
    </script>

</asp:Content>
