<%@ Page Title="Admin Dashboard" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.vb" Inherits="supabaseconnector.AdminDashboard" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-4">📊 Admin Dashboard</h2>

<!-- Summary Cards -->
<div class="row text-center mb-4">
    <div class="col-md-4">
        <div class="card shadow-sm h-100">
            <div class="card-body d-flex flex-column justify-content-center">
                <h5 class="card-title">Total Students</h5>
                <asp:Label ID="lblTotalStudents" runat="server" CssClass="display-6" />
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card shadow-sm h-100">
            <div class="card-body d-flex flex-column justify-content-center">
                <h5 class="card-title">Total Courses</h5>
                <asp:Label ID="lblTotalCourses" runat="server" CssClass="display-6" />
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card shadow-sm h-100">
            <div class="card-body d-flex flex-column justify-content-center">
                <h5 class="card-title">Most Popular Course</h5>
                <asp:Label ID="lblTopCourse" runat="server" CssClass="h5 text-success" />
            </div>
        </div>
    </div>
</div>


    <!-- Charts Container -->
    <div class="container mt-5">

        <!-- 📈 Students per Course -->
        <div class="card shadow-sm p-4 mb-5">
            <h5 class="text-center mb-4">📈 Students per Course</h5>
            <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped mb-4" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                    <asp:BoundField DataField="total_students" HeaderText="Enrolled Students" />
                </Columns>
            </asp:GridView>
            <canvas id="studentsPerCourseChart" height="250"></canvas>
        </div>

        <!-- 🧾 Format & 📅 Timeline -->
        <div class="row mb-5">
            <div class="col-lg-6 mb-4">
                <div class="card shadow-sm p-4 h-100">
                    <h5 class="text-center mb-3">🧾 Course Format Distribution</h5>
                    <canvas id="courseFormatChart" height="300"></canvas>
                </div>
            </div>
            <div class="col-lg-6 mb-4">
                <div class="card shadow-sm p-4 h-100">
                    <h5 class="text-center mb-3">📅 Enrollments Over Time</h5>
                    <canvas id="enrollmentsOverTimeChart" height="300"></canvas>
                </div>
            </div>
        </div>

    </div>

    <!-- Required JS -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="/Scripts/adminDashboardChart.js"></script>

    <!-- Script injection -->
    <asp:Literal ID="ltChartData" runat="server" />
</asp:Content>
