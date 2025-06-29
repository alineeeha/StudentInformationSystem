<%@ Page Title="My Enrollments" Language="VB" MasterPageFile="~/Site.Student.Master"
         AutoEventWireup="true" CodeBehind="MyEnrollments.aspx.vb"
         Inherits="supabaseconnector.MyEnrollments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><i class="bi bi-journal-check"></i> My Course Enrollments</h2>
    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger fw-bold"></asp:Label>

    <!-- ********  GRIDVIEW  ******** -->
<asp:GridView ID="gvMyEnrollments" runat="server"
              AutoGenerateColumns="False"
              DataKeyNames="enrollment_id"
              CssClass="table table-bordered"
              OnRowDataBound="gvMyEnrollments_RowDataBound"> 

    <Columns>
        <asp:BoundField DataField="course_name"      HeaderText="Course Name" />
        <asp:BoundField DataField="instructor"       HeaderText="Instructor"   />
        <asp:BoundField DataField="enrollment_date"  HeaderText="Enrollment Date"
                        DataFormatString="{0:yyyy-MM-dd}" />

        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="90px">
            <ItemTemplate>
                <asp:LinkButton ID="lnkDrop" runat="server"
                    Text="🗑 Drop"
                    CssClass="btn btn-danger drop-btn"
                    CausesValidation="false" />

            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>


    <!-- ********  CONFIRM MODAL  ******** -->
    <div class="modal fade" id="studentDropModal" tabindex="-1"
         aria-labelledby="studentDropModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-danger">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title" id="studentDropModalLabel">Confirm Drop</h5>
                    <button type="button" class="btn-close btn-close-white"
                            data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p id="studentDropText"></p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnStudentConfirmDrop" runat="server"
                                CssClass="btn btn-danger"
                                Text="Confirm"
                                OnClick="btnStudentConfirmDrop_Click" />
                    <button type="button" class="btn btn-secondary"
                            data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <!--  used by Confirm Drop button -->
    <asp:HiddenField ID="hfEnrollmentIdToDrop" runat="server" />

    <!-- ********  SCRIPT  ******** -->
    <script>
        // called by every Drop link
        function showStudentDropModal(course, id) {
            document.getElementById("studentDropText").innerHTML =
                `Are you sure you want to drop <strong>${course}</strong>?`;

            document.getElementById("<%= hfEnrollmentIdToDrop.ClientID %>").value = id;

            new bootstrap.Modal(document.getElementById('studentDropModal')).show();
        }
    </script>
</asp:Content>
