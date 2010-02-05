<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SNAPHome.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.SNAPHome" %>

<%@ Register src="~/Controls/SNAPDefaultIncludes.ascx" tagname="SNAPIncludes" tagprefix="uc" %>
<%@ Register src="~/Controls/SNAPFooter.ascx" tagname="SNAPFooter" tagprefix="uc" %>
<%@ Register src="~/Controls/SNAPRequestForm.ascx" tagname="RequestForm" tagprefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="_head" runat="server">
    <title>Out-Of-Scope Privileged Access</title>
</head>
<body> 
	<form runat="server" id="_defaultForm">
	
	<uc:SNAPIncludes id="_includes" runat="server" />

	<!-- BEGIN MASTER CONTAINER -->
	<div class="csm_container_100 csm_template_1">

		<!-- BEGIN CONTENT AREA -->
		<asp:Panel ID="_contentContainer" CssClass="csm_container_16 csm_500" runat="server">
			
			<!-- BEGIN MULTIVIEW CONTROL -->
			<asp:MultiView ID="_csmMultiView" runat="server" ActiveViewIndex="0" >

				<!-- NOTE: If using the _csmTabbedMenu, the View items 'ID' (below) must match the 'Value' of the associated MenuItem. -->
				<!-- NOTE: Default Behavior: When page initially loads, view item in ordinal position '0' loads as default,
					so make sure that your 'home' page View is first in-line (either programatically or static). -->
				
				<asp:View ID="_requestForm" runat="server">
					<uc:RequestForm id="_requestFormControl" runat="server" />
				</asp:View>
				
				
			</asp:MultiView>

			<div class="csm_clear">&nbsp;</div>
			<!-- END MUTLIVIEW CONTROL -->				
		
		</asp:Panel>
		<div class="csm_clear">&nbsp;</div>
		<!-- END CONTENT AREA -->

		<uc:SNAPFooter id="_footer" runat="server" />
	
	</div>
	<!-- END MASTER CONTAINER -->
	</form>
</body>
</html>