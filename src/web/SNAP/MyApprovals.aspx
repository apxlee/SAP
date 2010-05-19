<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="MyApprovals.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.MyApprovals" %>
<%@ Register src="~/Controls/ApprovingManagerView.ascx" tagname="ApprovingManagerView" tagprefix="uc" %>
<asp:Content ID="_headerContainer" ContentPlaceHolderID="_headPlaceHolder" runat="server"></asp:Content>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
	<uc:ApprovingManagerView id="_approvingManagerViewControl" runat="server" />
</asp:Content>