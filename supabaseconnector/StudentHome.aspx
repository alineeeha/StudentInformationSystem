<%@ Page Language="VB" AutoEventWireup="true" Title="Student home" MasterPageFile="~/Site.Student.Master" CodeBehind="StudentHome.aspx.vb" Inherits="supabaseconnector.StudentHome" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron bg-light p-5 rounded-3 mb-5 shadow-sm text-center">
        <div class="container py-5">
            <div class="row align-items-center">
                <div class="col-md-6 text-center text-md-start">
                    <h1 class="display-5 fw-bold lh-sm">
                        🎓 Welcome to the<br />
                        <span style="margin-left: 82px;">Student Portal</span>
                    </h1>
                    <p class="fs-5 text-muted">
                        Explore your academic options and enroll in courses.
                    </p>
                </div>
                <div class="col-md-6 text-center">
                    <asp:Image ID="imgWelcome" runat="server"
                        ImageUrl="~/images/welcome.png"
                        AlternateText="Students collaborating"
                        CssClass="img-fluid"
                        Style="max-width: 600px;" />
                </div>
            </div>
        </div>
    </div> 

    <div class="row row-cols-1 row-cols-md-2 g-4 text-center">
        <div class="col">
            <a href="StudentCourses.aspx" class="text-decoration-none text-dark" role="button">
                <div class="card border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="card-title">📚 <strong>Courses</strong></h5>
                        <p class="card-text">Browse and enroll in available courses.</p>
                    </div>
                </div>
            </a>
        </div>

        <div class="col">
            <a href="MyEnrollments.aspx" class="text-decoration-none text-dark" role="button">
                <div class="card border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="card-title">📝 <strong>My Enrollments</strong></h5>
                        <p class="card-text">Review and manage your course registrations.</p>
                    </div>
                </div>
            </a>
        </div>
    </div>
</asp:Content>
