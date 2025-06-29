<%@ Page Title="Manage Instructors" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Instructors.aspx.vb" Inherits="supabaseconnector.Instructors" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-4">Instructor Management</h2>

    <!-- Add/Edit Form -->
    <div class="row mb-4">
        <div class="col-md-6">
            <div class="card shadow-sm">
                <div class="card-body">
                    <h5 class="card-title">Add / Edit Instructor</h5>

                    <div class="form-group mb-2">
                        <label>Title</label>
                        <asp:DropDownList ID="ddlTitle" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Select title" Value="" />
                            <asp:ListItem Text="Dr." Value="Dr." />
                            <asp:ListItem Text="Prof." Value="Prof." />
                            <asp:ListItem Text="Mr." Value="Mr." />
                            <asp:ListItem Text="Ms." Value="Ms." />
                        </asp:DropDownList>
                    </div>
                    <div class="form-group mb-2">
                        <label>First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-group mb-2">
                        <label>Last Name</label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
                    </div>

                    <asp:HiddenField ID="hfInstructorId" runat="server" />
                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success mt-2" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary mt-2" Visible="false" OnClick="btnCancel_Click" />
                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-2 text-success" />
                </div>
            </div>
        </div>
    </div>

    <!-- GridView -->
    <asp:GridView ID="gvInstructors" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
        DataKeyNames="instructor_id"
        OnRowEditing="gvInstructors_RowEditing"
        OnRowCancelingEdit="gvInstructors_RowCancelingEdit"
        OnRowUpdating="gvInstructors_RowUpdating"
        OnRowDataBound="gvInstructors_RowDataBound">
        <Columns>
            <asp:BoundField DataField="instructor_id" HeaderText="ID" ReadOnly="True" />

            <asp:TemplateField HeaderText="Title">
                <ItemTemplate>
                    <%# Eval("title") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlEditTitle" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Dr." Value="Dr." />
                        <asp:ListItem Text="Prof." Value="Prof." />
                        <asp:ListItem Text="Mr." Value="Mr." />
                        <asp:ListItem Text="Ms." Value="Ms." />
                    </asp:DropDownList>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="First Name">
                <ItemTemplate>
                    <%# Eval("first_name") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditFirstName" runat="server" CssClass="form-control" Text='<%# Bind("first_name") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Last Name">
                <ItemTemplate>
                    <%# Eval("last_name") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditLastName" runat="server" CssClass="form-control" Text='<%# Bind("last_name") %>' />
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
    <div class="modal fade" id="confirmDeleteInstructorModal" tabindex="-1" aria-labelledby="confirmDeleteLabel" aria-hidden="true">
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

    <asp:HiddenField ID="hfInstructorIdToDelete" runat="server" />

    <script type="text/javascript">
        function showConfirmDeleteModal(name, id) {
            document.getElementById("confirmDeleteText").innerHTML =
                `Are you sure you want to delete <strong>${name}</strong>?`;
            document.getElementById("<%= hfInstructorIdToDelete.ClientID %>").value = id;
            const modal = new bootstrap.Modal(document.getElementById('confirmDeleteInstructorModal'));
            modal.show();
        }
    </script>
</asp:Content>
