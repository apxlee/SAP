<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessTeamView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.AccessTeamView" %>
<script type="text/javascript">
	//<![CDATA[
	$(document).ready(
	function() {
		try { pageTracker._trackPageview("/AccessTeamView"); }
		catch (err) { }
	}
	);
	//]]>
</script>
<div class="csm_container_center_700">
	
	<h1>Access Team Request Filter</h1>
	<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
		<tr>
			<td class="csm_input_form_control_column">
				<input type="radio" name="_changeFilter" onclick="filterClick(this);" class="csm_input_checkradio" value="Pending Acknowledgement" />
				<asp:Label CssClass="csm_input_checkradio_span" ID="_pendingacknowledgementFilter" runat="server" />
				<input type="radio" name="_changeFilter" onclick="filterClick(this);" class="csm_input_checkradio" value="Pending Workflow" />
				<asp:Label CssClass="csm_input_checkradio_span" ID="_pendingworkflowFilter" runat="server" />	
				<input type="radio" name="_changeFilter" onclick="filterClick(this);" class="csm_input_checkradio" value="Pending Provisioning" />
				<asp:Label CssClass="csm_input_checkradio_span" ID="_pendingprovisioningFilter" runat="server" />
				<input type="radio" name="_changeFilter" onclick="filterClick(this);" checked="checked" class="csm_input_checkradio" value="Show All" />
				<asp:Label CssClass="csm_input_checkradio_span" ID="_showallFilter" runat="server" />
			</td>
		</tr>					
	</table>
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
    <div id="_managerSelectionDiv" style="display:none"; >
      <p />
      <div style="display:none;text-align:center;padding-top:100px;" class="oospa_ajax_indicator">
            <img alt="loading..." src="images/ajax_indicator.gif" width="16" height="16" />
      </div>
      <select style="display:none;" size="3" class="oospa_select_user" name="managerSelection" id="_managerSelection"></select>
        
    </div>
</div>