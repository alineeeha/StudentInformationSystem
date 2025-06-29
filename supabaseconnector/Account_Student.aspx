<%@ Page Title="Account Settings" Language="VB" MasterPageFile="~/Site.Student.Master" AutoEventWireup="true" CodeBehind="Account_Student.aspx.vb" Inherits="supabaseconnector.Account_Student" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><i class="bi bi-gear"></i> Account Settings</h2>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

    <!-- Personal Info -->
    <div class="form-group">
        <label>First Name</label>
        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control fixed-width" />
    </div>

    <div class="form-group">
        <label>Last Name</label>
        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control fixed-width" />
    </div>

    <div class="form-group">
        <label>Email</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control fixed-width" />
    </div>

    <!-- Password Change -->
    <hr />
    <h5>Change Password</h5>
    <div class="form-group">
        <label>Current Password</label>
        <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" CssClass="form-control fixed-width" />
    </div>

    <div class="form-group">
        <label>New Password</label>
        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="form-control fixed-width" />
    </div>

    <div class="form-group">
        <label>Confirm New Password</label>
        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control fixed-width" />
    </div>

    <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" CssClass="btn btn-primary mt-3" OnClick="btnSaveChanges_Click" />
</asp:Content>
