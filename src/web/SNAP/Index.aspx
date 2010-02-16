<%@ Page CodeBehind="Index.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="Apollo.AIM.SNAP.Web.Index"  %>
<%@ Register src="~/Controls/DefaultIncludes.ascx" tagname="DefaultIncludes" tagprefix="uc" %>
<%@ Register src="~/Controls/Footer.ascx" tagname="Footer" tagprefix="uc" %>
<%@ Register src="~/Controls/RequestForm.ascx" tagname="RequestForm" tagprefix="uc" %>
<%@ Register src="~/Controls/LoginForm.ascx" tagname="LoginForm" tagprefix="uc" %>
<%@ Register src="~/Controls/UserView.ascx" tagname="UserView" tagprefix="uc" %>
<%@ Register src="~/Controls/ApprovingManagerView.ascx" tagname="ApprovingManagerView" tagprefix="uc" %>
<%@ Register src="~/Controls/AccessTeamView.ascx" tagname="AccessTeamView" tagprefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="_head" runat="server">
    <title>Out-Of-Scope Privileged Access</title>
</head>
<body> 
	<form runat="server" id="_defaultForm">
	
	<uc:DefaultIncludes id="_includes" runat="server" />

	<!-- BEGIN MASTER CONTAINER -->
	<div class="csm_container_100 csm_template_1">
	
		<asp:PlaceHolder ID="_topNavigation" runat="server">
			<p>TODO: TOP NAVIGATION</p>
		</asp:PlaceHolder>

		<!-- BEGIN CONTENT AREA -->
		<asp:Panel ID="_contentContainer" CssClass="csm_container_16 csm_500" runat="server">
		<!-- TODO: do we really need this container server-side? -->
			
			<!-- BEGIN MULTIVIEW CONTROL -->
			<asp:MultiView ID="_csmMultiView" runat="server" ActiveViewIndex="1" >

				<!-- TODO: ADD LOGIN FORM VIEW -->

				<asp:View ID="_requestFormView" runat="server">
					<uc:RequestForm id="_requestFormControl" runat="server" />
				</asp:View>
				
				<asp:View ID="_userView" runat="server">
					<uc:UserView id="_userViewControl" runat="server" />
				</asp:View>

				<asp:View ID="_approvingManagerView" runat="server">
					<uc:ApprovingManagerView id="_approvingManagerViewControl" runat="server" />
				</asp:View>
				
				<asp:View ID="_accessTeamView" runat="server">
					<uc:AccessTeamView id="_accessTeamViewControls" runat="server" />
				</asp:View>
				
				<asp:View ID="_searchFormView" runat="server">
					<p>TODO: SEARCH FORM VIEW</p>
				</asp:View>
				
			</asp:MultiView>

			<div class="csm_clear">&nbsp;</div>
			<!-- END MUTLIVIEW CONTROL -->				
		
		</asp:Panel>
		<div class="csm_clear">&nbsp;</div>
		<!-- END CONTENT AREA -->

		<!-- TODO: FOOTER CONTROL -->
	
	</div>
	<!-- END MASTER CONTAINER -->
	</form>
</body>
</html>