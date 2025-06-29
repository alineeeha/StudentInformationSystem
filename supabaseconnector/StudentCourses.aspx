<%@ Page Title="Courses for Students" Language="VB" MasterPageFile="~/Site.Student.Master" AutoEventWireup="true" CodeBehind="StudentCourses.aspx.vb" Inherits="supabaseconnector.StudentCourses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Available Courses</h2>
    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" />

    <!-- Searchbar with Clear -->
    <div class="mb-3">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control w-50 d-inline me-2" placeholder="Search courses..." />
        <asp:Button ID="btnSearch" runat="server" Text="🔍 Search" CssClass="btn btn-primary me-2" OnClick="btnSearch_Click" />
        <asp:Button ID="btnClear" runat="server" Text="❌ Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
    </div>

    <asp:GridView ID="gvStudentCourses" runat="server" AutoGenerateColumns="False"
                  CssClass="table table-bordered"
                  DataKeyNames="course_id"
                  OnRowCommand="gvStudentCourses_RowCommand">
        <Columns>
            <asp:BoundField DataField="course_name" HeaderText="Course Name" />
            <asp:BoundField DataField="ects" HeaderText="ECTS" />
            <asp:BoundField DataField="hours" HeaderText="Hours" />
            <asp:BoundField DataField="format" HeaderText="Format" />
            <asp:BoundField DataField="instructor" HeaderText="Instructor" />
            <asp:TemplateField HeaderText="Actions">
                <ItemStyle Width="100px" />
                <HeaderStyle Width="100px" />
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEnroll" runat="server"
                        CommandName="Enroll"
                        CommandArgument='<%# Container.DataItemIndex %>'
                        Text="➕ Enroll"
                        CssClass="btn btn-success enroll-btn"
                        CausesValidation="false" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
