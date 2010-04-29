<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultIncludes.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.DefaultIncludes" %>
<!-- NOTE: Style sheets must appear in this order: 1) reset, 2) grid, 3) template -->
<script type="text/javascript">
	var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
	document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
	try { var pageTracker = _gat._getTracker("UA-15693359-2"); }
	catch (err) { }
</script>    

<link href="styles/Csm_Reset.css" rel="stylesheet" type="text/css" />
<link href="styles/Csm_Grid976.css" rel="stylesheet" type="text/css" />
<link href="styles/Csm_Template_v1.css" rel="stylesheet" type="text/css" />
<link href="styles/SNAP_Brand.css" rel="stylesheet" type="text/css" />
<link href="styles/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>jquery-1.4.2.min.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>ApproverActions.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>Acknowledgement.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>AccessComments.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>WorkflowBuilder.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>Search.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>RequestFilter.js" type="text/javascript"></script>