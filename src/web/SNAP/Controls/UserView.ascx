<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.UserView" %>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MyRequestsView.js" type="text/javascript"></script>

<div class="csm_container_center_700">
	<h1>Open Requests</h1>
	<div id="_openRequestsContainer"></div>
	<div id="_nullDataMessage_OpenRequests" class="csm_content_container" style="display:none;">
		<div class="csm_text_container">
			<p>There are no Open Requests at this time.</p>
		</div>
	</div>
	
	<h1>Closed Requests</h1>
	<div id="_closedRequestsContainer"></div>
	<div id="_nullDataMessage_ClosedRequests" class="csm_content_container" style="display:none;">
		<div class="csm_text_container">
			<p>There are no Closed Requests at this time.</p>
		</div>
	</div>	
</div>
<script type="text/javascript">
	//<![CDATA[
	//]]>
</script>
<script type="text/javascript">
    try {GetRequests(2);}
    catch (err) {}
	try { pageTracker._trackPageview("UserView"); }
	catch (err) {}
</script>