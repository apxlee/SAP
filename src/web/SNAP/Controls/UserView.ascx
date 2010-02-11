<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.UserView" %>
<div class="csm_container_center_700">
	
	<h1>Open Requests</h1>
	<asp:PlaceHolder ID="_openRequestsContainer" runat="server"></asp:PlaceHolder>
	
	<h1>Closed Requests</h1>
	<asp:PlaceHolder ID="_closedRequestsContainer" runat="server"></asp:PlaceHolder>

</div>