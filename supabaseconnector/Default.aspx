<%@ Page Title="Home" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="supabaseconnector._Default" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron bg-light p-5 rounded-3 mb-5 shadow-sm">
        <div class="container-fluid py-3">
            <h1 class="display-5 fw-bold">🎓 Welcome to the Student Information System</h1>
            <p class="col-md-8 fs-5">
                Manage students, courses, and enrollments efficiently and securely.
            </p>
        </div>
    </div>

    <!-- 2x2 Grid of Feature Tiles -->
    <div class="row text-center mb-4">
        <div class="col-md-6 mb-4">
            <a href="ManageStudents.aspx" class="text-decoration-none text-dark">
                <div class="card border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="card-title">🎓 <strong>Students</strong></h5>
                        <p class="card-text">Create, update, or delete student records.</p>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-md-6 mb-4">
            <a href="Courses.aspx" class="text-decoration-none text-dark">
                <div class="card border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="card-title">📚 <strong>Courses</strong></h5>
                        <p class="card-text">View and manage course offerings.</p>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-md-6 mb-4">
            <a href="Enrollments.aspx" class="text-decoration-none text-dark">
                <div class="card border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="card-title">📝 <strong>Enrollments</strong></h5>
                        <p class="card-text">View and manage student enrollments.</p>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-md-6 mb-4">
            <a href="AdminDashboard.aspx" class="text-decoration-none text-dark">
                <div class="card border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="card-title">📊 <strong>Dashboard</strong></h5>
                        <p class="card-text">View key students statistics.</p>
                    </div>
                </div>
            </a>
        </div>
    </div>



</asp:Content>
