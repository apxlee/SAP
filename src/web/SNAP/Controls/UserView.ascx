<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.UserView" %>

<div class="csm_container_center_700">
	
	<h1>Open Requests</h1>
	<asp:PlaceHolder ID="_openRequestsContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage_OpenRequests" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p>There are no Open Requests at this time.</p>
		</div>
	</asp:Panel>
	
	<h1>Closed Requests</h1>
	<asp:PlaceHolder ID="_closedRequestsContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage_ClosedRequests" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p>There are no Closed Requests at this time.</p>
		</div>
	</asp:Panel>	

</div>
<script type="text/javascript">
	//<![CDATA[
	$(document).ready(
	function() {
		try { pageTracker._trackPageview("/UserView.ascx"); }
		catch (err) { }
	}
	);
	//]]>
</script>