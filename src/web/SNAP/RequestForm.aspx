<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="RequestForm.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.RequestForm" %>
<%@ Register src="~/Controls/RequestForm.ascx" tagname="RequestForm" tagprefix="uc" %>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
	<uc:RequestForm id='_requestFormControl' runat="server" />
</asp:Content>