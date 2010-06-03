<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessTeamView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.AccessTeamView" %>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>AccessTeamView.js" type="text/javascript"></script>
	<style type="text/css">

	</style>
<div class="csm_container_center_700">

	<h1 style="margin-bottom:2px;">Open Requests</h1>
	
	<div id="access_filter_container" class="filter_view_all">
		<div id="access_filter_outer">
			<ul id="access_filter_inner">
				<li style="width:57px;"><span>Pending:</span></li>
				<li id="filter_pending_acknowledgement" class="active_carrot" snap="Pending Acknowledgement"><span>Acknowledgement (<span class="inline_counter" id="filter_pending_acknowledgement_count">00</span>)</span></li>
				<li id="filter_pending_workflow" class="active_carrot" snap="Pending Workflow"><span>Workflow (<span class="inline_counter"  id="filter_pending_workflow_count">00</span>)</span></li>
				<li id="filter_pending_provisioning" class="active_carrot" snap="Pending Provisioning"><span>Provisioning (<span class="inline_counter"  id="filter_pending_provisioning_count">00</span>)</span></li>
				<li style="width:4px;padding-left:2px;margin-right:4px;font-weight:bold;"><span>|</span></li>
				<li id="filter_in_workflow" class="active_carrot" snap="In Workflow"><span>In Workflow (<span class="inline_counter"  id="filter_in_workflow_count">00</span>)</span></li>
				<li style="width:4px;padding-left:2px;margin-right:4px;font-weight:bold;"><span>|</span></li>
				<li id="filter_view_all" onmouseover="filterHover(this);" onmouseout="filterHover(this);" onclick="filterClick(this);" class="active_carrot" snap="All"><span>View All</span></li>
			</ul>
		</div>
	</div>	
	
	<asp:PlaceHolder ID="_openRequestsContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage_OpenRequests" snap="_nullDataMessage" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p>There are no Open Requests at this time.</p>
		</div>
	</asp:Panel>
	
	<h1>Closed Requests</h1>
	<asp:PlaceHolder ID="_closedRequestsContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage_ClosedRequests" snap="_nullDataMessage" runat="server" CssClass="csm_content_container" Visible="false">
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
    
    <div id="_actionMessageDiv" style="display:none;">
        <div class="messageBox"> 
            <h2>header</h2>
            <div id="_indicatorDiv" style="display:none;text-align:center;padding-top:15px;">
                <img alt="creating ticket..." src="images/ajax_indicator.gif" width="16" height="16" />
            </div>
            <p style="margin-left:5px;margin-right:5px;">message</p>
            <div id="_closeMessageDiv" style="display:none;">
                <input type="button" value="Close" onclick="$('#_actionMessageDiv').hide();$('#_closeMessageDiv').hide();" />
            </div>
        </div>
    </div>
    <div style="display:none;">
            <asp:Button ID="_submit_form" Text="submit form" CommandName="AccessTeam" CssClass="csm_html_button" runat="server" OnClick="_submitForm_Click" />
        </div>
</div>
<script type="text/javascript">
	try { pageTracker._trackPageview("AccessTeamView"); }
	catch (err) {}
</script>