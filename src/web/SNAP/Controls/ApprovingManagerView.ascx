<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovingManagerView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.ApprovingManagerView" %>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>ApprovingManagerView.js" type="text/javascript"></script>
<div class="csm_container_center_700">

	<asp:Panel ID="_statusChangedMessage" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p class="csm_error_text"><strong>Request Status Changed</strong><br />The Access Request <asp:Label ID="_requestIdChanged" runat="server"></asp:Label> has had its status changed.  
			At this time, you cannot perform Approver Actions.<br />See Request Tracking for details.</p>
		</div>
	</asp:Panel>
	
	<h1>Pending Approval</h1>
	<asp:PlaceHolder ID="_pendingApprovalsContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage_PendingApprovals" snap="_nullDataMessage" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p>There are no requests Pending Approval at this time.</p>
		</div>
	</asp:Panel>
	
	<h1>Open Requests</h1>
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
		
    <div id="_actionMessageDiv" style="display:none;">
        <div class="messageBox"> 
            <h2>header</h2>
            <div id="_indicatorDiv" style="display:none;text-align:center;padding-top:15px;">
                <img alt="creating ticket..." src="images/ajax_indicator.gif" width="16" height="16" />
            </div>
            <p>message</p>
            <div id="_closeMessageDiv" style="display:none;">
                <input type="button" value="Close" onclick="$('#_actionMessageDiv').hide();$('#_closeMessageDiv').hide();" />
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
	try { pageTracker._trackPageview("ApprovingManagerView"); }
	catch (err) {}
</script>