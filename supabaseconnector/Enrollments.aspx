<%@ Page Title="Enrollments" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Enrollments.aspx.vb" Inherits="supabaseconnector.Enrollments" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-4">📋 Enrollments</h2>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-success mb-3 d-block" />

    <!-- Filter by course -->
        <div class="mb-3">
            <label for="ddlCourseFilter" class="form-label fw-bold">Filter by Course:</label>
            <asp:DropDownList ID="ddlCourseFilter" runat="server" AutoPostBack="true"
                              CssClass="form-select w-auto d-inline-block me-2"
                              OnSelectedIndexChanged="ddlCourseFilter_SelectedIndexChanged" />
            <asp:Button ID="btnClearFilter" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClearFilter_Click" />
        </div>


    <!-- GridView to show current enrollments -->
<asp:GridView ID="gvEnrollments" runat="server"
              AutoGenerateColumns="False"
              CssClass="table table-bordered"
              DataKeyNames="enrollment_id"
              OnRowDataBound="gvEnrollments_RowDataBound">

        <Columns>
            <asp:BoundField DataField="student_name" HeaderText="Student" />
            <asp:BoundField DataField="course_name" HeaderText="Course" />
            <asp:BoundField DataField="enrollment_date" HeaderText="Enrollment Date" DataFormatString="{0:yyyy-MM-dd}" />

            <asp:TemplateField HeaderText="Actions">
                <ItemStyle Width="90px" />
                <HeaderStyle Width="90px" />
                <ItemTemplate>
                <asp:LinkButton ID="lnkDrop" runat="server"
                    Text="🗑 Drop"
                    CssClass="btn btn-danger drop-btn"
                    CausesValidation="false" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <!-- Modal -->
    <div class="modal fade" id="confirmDropModal" tabindex="-1" aria-labelledby="confirmDropLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-danger">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title" id="confirmDropLabel">Confirm Drop</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p id="confirmDropText"></p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmDrop" runat="server" CssClass="btn btn-danger" Text="Confirm" OnClick="btnConfirmDrop_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfEnrollmentIdToDrop" runat="server" />

    <script type="text/javascript">
        function showConfirmModal(student, course, enrollmentId) {
            const text = `Are you sure you want to drop <strong>${student}</strong> from <strong>${course}</strong>?`;
            document.getElementById("confirmDropText").innerHTML = text;
            document.getElementById("<%= hfEnrollmentIdToDrop.ClientID %>").value = enrollmentId;
            const modal = new bootstrap.Modal(document.getElementById('confirmDropModal'));
            modal.show();
        }
    </script>
</asp:Content>
