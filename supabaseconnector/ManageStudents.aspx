<%@ Page Title="Manage Students" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageStudents.aspx.vb" Inherits="supabaseconnector.Students" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-4">Student Management</h2>

    <!-- Input form -->
    <div class="row mb-4">
        <div class="col-md-6">
            <div class="card shadow-sm">
                <div class="card-body">
                    <h5 class="card-title">Add / Edit Student</h5>

                    <div class="form-group">
                        <label>First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <label>Last Name</label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <label>Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                    </div>
                    <div class="form-group">
                        <label>Enrollment Date</label>
                        <asp:TextBox ID="txtEnrollmentDate" runat="server" CssClass="form-control datepicker" />
                    </div>

                    <asp:HiddenField ID="hfStudentId" runat="server" />
                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success mt-3" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary mt-3" Visible="False" OnClick="btnCancel_Click" />
                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-2 text-success" />
                </div>
            </div>
        </div>
    </div>

    <!-- Student Grid -->
    <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
        DataKeyNames="id"
        OnRowEditing="gvStudents_RowEditing"
        OnRowUpdating="gvStudents_RowUpdating"
        OnRowCancelingEdit="gvStudents_RowCancelingEdit"
        OnRowDataBound="gvStudents_RowDataBound">
        <Columns>
            <asp:BoundField DataField="id" HeaderText="ID" ReadOnly="True" />

            <asp:TemplateField HeaderText="First Name">
                <ItemTemplate><%# Eval("first_name") %></ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditFirstName" runat="server" CssClass="form-control" Text='<%# Bind("first_name") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Last Name">
                <ItemTemplate><%# Eval("last_name") %></ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditLastName" runat="server" CssClass="form-control" Text='<%# Bind("last_name") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Email">
                <ItemTemplate><%# Eval("email") %></ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditEmail" runat="server" CssClass="form-control" Text='<%# Bind("email") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Enrollment Date">
                <ItemTemplate><%# Eval("enrollment_date", "{0:yyyy-MM-dd}") %></ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditDate" runat="server" CssClass="form-control datepicker"
                        Text='<%# Bind("enrollment_date", "{0:yyyy-MM-dd}") %>' />
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
                            CssClass="btn btn-danger drop-btn btn-sm" Text="🗑 Delete"
                            OnClientClick='<%# "showConfirmStudentModal(" & Eval("id") & ", " & Js(Eval("first_name") & " " & Eval("last_name")) & "); return false;" %>' />
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

    <!-- Delete Confirm Modal -->
    <div class="modal fade" id="confirmStudentModal" tabindex="-1" aria-labelledby="confirmStudentLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-danger">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title" id="confirmStudentLabel">Confirm Deletion</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p id="confirmStudentText"></p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmDeleteStudent" runat="server" CssClass="btn btn-danger" Text="Delete"
                        OnClick="btnConfirmDeleteStudent_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfStudentIdToDelete" runat="server" />

    <!-- Scripts -->
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

    <script type="text/javascript">
        function showConfirmStudentModal(studentId, fullName) {
            document.getElementById("confirmStudentText").innerHTML =
                `Are you sure you want to delete student <strong>${fullName}</strong>? This will also remove related enrollments and user account.`;
            document.getElementById("<%= hfStudentIdToDelete.ClientID %>").value = studentId;
            const modal = new bootstrap.Modal(document.getElementById('confirmStudentModal'));
            modal.show();
        }

        $(function () {
            $(".datepicker").datepicker({
                dateFormat: "yy-mm-dd"
            });
        });
    </script>
</asp:Content>
