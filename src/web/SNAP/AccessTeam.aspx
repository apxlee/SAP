<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="AccessTeam.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.AccessTeam" %>
<%@ Register src="~/Controls/AccessTeamView.ascx" tagname="AccessTeamView" tagprefix="uc" %>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
	<uc:AccessTeamView id="_accessTeamViewControl" runat="server" />
</asp:Content>
