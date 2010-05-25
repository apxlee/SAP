<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestTrackingPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.RequestTrackingPanel" %>
<div class="csm_text_container csm_top5">
    <div class="csm_icon_heading_container border oospa_request_status">
        <h2>Request Tracking</h2>
        <p class="">This section tracks the status of your request and includes all the people and groups 
		that are involved in the approval process.  Status "Not Active" indicates the person or team will be notified after prerequisite approvals
		have been made.  Due Dates do not include weekends or holidays.&nbsp;&nbsp;<span class="csm_legend_toggle" onclick="toggleLegend(this);">[Show Legend]</span></p>
    </div>
    
    <div snap="_legend" style="display:none;">
		<style type="text/css">
			.legendPadding {margin-left:15px;}
		</style>
		<div class="csm_text_container_single_row csm_alternating_bg" style="font-size:12px;padding:5px 5px 5px 10px;">
			<p><strong>Pending:</strong>&nbsp;The request workflow is active.</p>
			<p class="legendPadding"><strong>Pending Acknowledgement:</strong>&nbsp;The Access Team has yet to review it for custom workflow creation.</p>
			<p class="legendPadding"><strong>Pending Workflow:</strong>&nbsp;The Access Team has acknowledged the request, and it is currently being reviewed<br />before workflow creation.</p>
			<p class="legendPadding"><strong>Pending Approval:</strong>&nbsp;The request is now in workflow and is waiting for all associated approvals.</p>					
		</div>
		<div class="csm_text_container_single_row" style="font-size:12px;padding:5px 5px 5px 10px;">
			<p><strong>Approved:</strong>&nbsp;An individual or group has approved this request for their speciality as part of the request workflow.</p>
		</div>
		<div class="csm_text_container_single_row csm_alternating_bg" style="font-size:12px;padding:5px 5px 5px 10px;">
			<p><strong>In Workflow:</strong>&nbsp;The request is active, mid-workflow and being reviewed for access approval.</p>
		</div>
		<div class="csm_text_container_single_row" style="font-size:12px;padding:5px 5px 5px 10px;">					
			<p><strong>Request Change:</strong>&nbsp;The Affected End User needs to make changes as detailed by an Approver or Access Team.</p>
		</div>
		<div class="csm_text_container_single_row_last csm_alternating_bg" style="font-size:12px;padding:5px 5px 5px 10px;">
			<p><strong>Closed:</strong>&nbsp;A workflow is no longer active.</p>
			<p class="legendPadding"><strong>Closed Cancelled:</strong>&nbsp;A workflow is no longer active because of a technical cancellation or change to policy.</p>
			<p class="legendPadding"><strong>Closed Denied:</strong>&nbsp;A workflow is no longer active because an approver denied the request with reasons<br />specificed in the comments.</p>
			<p class="legendPadding"><strong>Closed Complete:</strong>&nbsp;A workflow has gained all approvals and access has been provisioned with<br />Service Desk ticket reference.</p>					
		</div>
	</div>
    
    <asp:PlaceHolder ID="_workflowBladeContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage_NoWorkflows" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p>There are no active workflows at this time.</p>
		</div>
	</asp:Panel>    
</div>