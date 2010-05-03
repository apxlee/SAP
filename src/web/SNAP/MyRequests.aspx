<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="MyRequests.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.MyRequests" %>
<%@ Register src="~/Controls/UserView.ascx" tagname="UserView" tagprefix="uc" %>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
	<uc:UserView id="_userViewControl" runat="server" />
</asp:Content>
