<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestTrackingPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.RequestTrackingPanel" %>
<div class="csm_text_container csm_top5">
    <div class="csm_icon_heading_container border oospa_request_status">
        <h2>Request Tracking</h2>
        <p class="">This section tracks the status of your request and includes all the people and groups 
		that are involved in the approval process.  Status "Not Active" indicates the person or team will be notified after prerequisite approvals
		have been made.  Due Dates do not include weekends or holidays.&nbsp;&nbsp;<span class="csm_legend_toggle" onclick="toggleLegend(this);">[Show Legend]</span></p>
    </div>
    <div snap="_legend" style="display:none;"><p>[TODO: Legend]</p></div>
    <asp:PlaceHolder ID="_workflowBladeContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage_NoWorkflows" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p>There are no active workflows at this time.</p>
		</div>
	</asp:Panel>    
</div>