<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="MyApprovals.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.MyApprovals" %>
<asp:Content ID="_headerContainer" ContentPlaceHolderID="_headPlaceHolder" runat="server"></asp:Content>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">

<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>ApprovingManagerView.js" type="text/javascript"></script>

<div class="csm_container_center_700">
	
	<asp:Panel ID="_statusChangedMessage" runat="server" CssClass="csm_content_container" Visible="false">
		<div class="csm_text_container">
			<p class="csm_error_text"><strong>Request Status Changed</strong><br />The Access Request <asp:Label ID="_badStatusRequestId" runat="server">you have requested</asp:Label> has had its status changed.  
			At this time, you cannot perform Approver Actions.<br />See Request Tracking for details.</p>
		</div>
	</asp:Panel>

	<input type="hidden" id="__multiplePendingApprovals" value="no" />

	<h1>Pending Approval</h1>
	<div id="_pendingApprovalsContainer"></div>
	<h1>Open Requests</h1>
	<div id="_openRequestsContainer"></div>
	<h1>Closed Requests</h1>
	<div id="_closedRequestsContainer"></div>
</div>

<script type="text/javascript">
    try { GetRequests(ViewIndexEnum.My_Approvals,""); }
    catch (err) {}
	
	try { pageTracker._trackPageview("ApprovingManagerView"); }
	catch (err) {}
</script>

</asp:Content>