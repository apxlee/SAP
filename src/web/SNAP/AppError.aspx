<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppError.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.AppError" %>
<%@ Register src="~/Controls/DefaultIncludes.ascx" tagname="DefaultIncludes" tagprefix="uc" %>
<%@ Register src="~/Controls/Header.ascx" tagname="Header" tagprefix="uc" %>
<%@ Register src="~/Controls/Footer.ascx" tagname="Footer" tagprefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="_head" runat="server">
    <title>Supplemental Network Access Process</title>
</head>
<body> 
	<form runat="server" id="_defaultForm">
	
	<uc:DefaultIncludes id="_includes" runat="server" />

	<!-- BEGIN MASTER CONTAINER -->
	<div class="csm_container_100 csm_template_1">
	
		<uc:Header ID="_headerControl" runat="server" IsStandalonePage="true" />		
		
		<!-- BEGIN CONTENT AREA -->
		<div class="csm_container_100 csm_500">
			<div class="snap_container_center_844">
				<div class="csm_container_center_700">
					<h1>Application Error</h1>
					<div class="csm_content_container">
						<p>The application encountered an error.</p>
						<asp:Label ID="_errorMessage" runat="server"></asp:Label>
					</div>
				</div>
			</div>
		</div>
		<div class="csm_clear">&nbsp;</div>
		<!-- END CONTENT AREA -->

		<uc:Footer ID="_footerControl" runat="server" />		
	
	</div>
	<!-- END MASTER CONTAINER -->
	</form>
</body>
	<script type="text/javascript">
	//<![CDATA[
		var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
		document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
		try { var pageTracker = _gat._getTracker("UA-15693359-2"); }
		catch (err) { } //]]>
	</script> 
<script type="text/javascript">
	//<![CDATA[
	$(document).ready(
	function() {
		try { pageTracker._trackPageview("/AppError"); }
		catch (err) { }
	}
	);
	//]]>
</script>
</html>
