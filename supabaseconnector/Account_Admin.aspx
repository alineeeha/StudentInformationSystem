<%@ Page Title="Admin Account Settings" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Account_Admin.aspx.vb" Inherits="supabaseconnector.Account_Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><i class="bi bi-shield-lock"></i> Admin Account Settings</h2>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

    <!-- Username -->
    <div class="form-group">
        <label>Username</label>
        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control fixed-width" />
    </div>

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

    <asp:Button ID="btnUpdate" runat="server" Text="Save Changes" CssClass="btn btn-primary mt-3" OnClick="btnUpdate_Click" />
</asp:Content>
