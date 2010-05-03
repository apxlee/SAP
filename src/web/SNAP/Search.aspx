<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.Search" %>
<%@ Register src="~/Controls/SearchView.ascx" tagname="SearchView" tagprefix="uc" %>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
	<uc:SearchView id="_searchControl" runat="server" />
</asp:Content>
