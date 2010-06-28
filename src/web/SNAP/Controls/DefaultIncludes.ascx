<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultIncludes.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.DefaultIncludes" %>
<!-- NOTE: Style sheets must appear in this order: 1) reset, 2) grid, 3) template -->
<link href="styles/Csm_Reset.css" rel="stylesheet" type="text/css" />
<link href="styles/Csm_Grid976.css" rel="stylesheet" type="text/css" />
<link href="styles/SNAP.css" rel="stylesheet" type="text/css" />
<!--[if IE 6]>
	<link href="styles/SNAP_IE6.css" rel="stylesheet" type="text/css" />
<![endif]-->
<link href="styles/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>jquery-1.4.2.min.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>
<script type="text/javascript">

	$(document).ready(

		// This function is to hide 'Edit Request Form' link unless currently logged-in user is also AEU
		function() {
			var hiddenSpanId = '_' + $("input[id*='_hiddenCurrentUserId']").attr("value") + '_request_link';
			$("span[id*=" + hiddenSpanId + "]").removeClass("csm_hidden_span");
		}

		);
</script>