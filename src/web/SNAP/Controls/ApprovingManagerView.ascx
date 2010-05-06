<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovingManagerView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.ApprovingManagerView" %>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>ApprovingManagerView.js" type="text/javascript"></script>
<div class="csm_container_center_700">
	
	<h1>Pending Approval</h1>
	<asp:PlaceHolder ID="_pendingApprovalsContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage_PendingApprovals" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p>There are no requests Pending Approval at this time.</p>
		</div>
	</asp:Panel>
	
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
		
    <div id="_actionMessageDiv" style="display:none;">
        <div class="messageBox"> 
            <h2>header</h2>
            <p>message</p>
        </div>
    </div>
</div>
<script type="text/javascript">
	try { pageTracker._trackPageview("/ApprovingManagerView"); }
	catch (err) { alert(err.toString()); }
</script>