<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderStandalone.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.HeaderStandalone" %>
<div class="snap_brand_header">
	<div class="csm_container_16">
		<div class="csm_grid_4 csm_prefix_2 csm_alpha csm_omega logo_itservices"></div>
		<div class="csm_grid_6 csm_prefix_3 csm_alpha csm_omega logo_oospa">
			<span class="aim_header_logout">
				<asp:Label ID="_userNameHeader" runat="server" EnableViewState="false"></asp:Label>
				<asp:LinkButton ID="_logout" runat="server" onclick="Logout_Click" CssClass="aim_logout_link">Logout</asp:LinkButton>
			</span>
		</div>
		<div class="csm_clear">&nbsp;</div>
		<asp:Panel ID="_ribbonContainerOuter" runat="server">
			<asp:PlaceHolder ID="_ribbonContainer" runat="server"></asp:PlaceHolder>
		</asp:Panel>
	</div>
</div>
<div class="csm_clear">&nbsp;</div>