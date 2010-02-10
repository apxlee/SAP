<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.UserView" %>
<div class="csm_container_center_700">

	<!-- 
	
	TODO: 
	
	1) figure out what kind of user you are to create major headings
	2) load 'MasterRequestBlades' and associated children based on role
	3) expand requestId blade if ID is available (from url)
	
	-->
	<asp:PlaceHolder ID="_requestFilter" runat="server"></asp:PlaceHolder>

</div>