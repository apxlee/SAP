<%@ Page CodeBehind="Index.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="Apollo.AIM.SNAP.Web.Index"  %>
<%@ Register src="~/Controls/DefaultIncludes.ascx" tagname="DefaultIncludes" tagprefix="uc" %>
<%@ Register src="~/Controls/Header.ascx" tagname="Header" tagprefix="uc" %>
<%@ Register src="~/Controls/Footer.ascx" tagname="Footer" tagprefix="uc" %>
<%@ Register src="~/Controls/LoginView.ascx" tagname="LoginView" tagprefix="uc" %>
<%@ Register src="~/Controls/UserView.ascx" tagname="UserView" tagprefix="uc" %>
<%@ Register src="~/Controls/ApprovingManagerView.ascx" tagname="ApprovingManagerView" tagprefix="uc" %>
<%@ Register src="~/Controls/AccessTeamView.ascx" tagname="AccessTeamView" tagprefix="uc" %>
<%@ Register src="~/Controls/SearchView.ascx" tagname="SearchView" tagprefix="uc" %>
<%@ Register src="~/Controls/RequestForm.ascx" tagname="RequestForm" tagprefix="uc" %>
<%@ Register src="~/Controls/SupportView.ascx" tagname="SupportView" tagprefix="uc" %>
	
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="_head" runat="server">
    <title>Supplemental Access Process</title>
<script src="http://www.google-analytics.com/ga.js" type="text/javascript"></script>

<script type="text/javascript">
	//	var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
	//	document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
	try { var pageTracker = _gat._getTracker("UA-15693359-2"); }
	catch (err) { alert(err.toString()); }
</script>  
</head>
<body> 
	<form runat="server" id="_defaultForm">
	
	<uc:DefaultIncludes id="_includes" runat="server" />
    <!-- HIDDEN INPUTS -->
    <input type="hidden" id="_currentUserDisplayName" runat="server" />
    <input type="hidden" id="_currentUserId" runat="server" />
	<!-- BEGIN MASTER CONTAINER -->
	<div class="csm_container_100 csm_template_1">
	
		<uc:Header ID="_headerControl" runat="server" />		
		
		<!-- BEGIN CONTENT AREA -->
		<asp:Panel ID="_contentContainer" CssClass="csm_container_100 csm_500" runat="server">
		<!-- TODO: do we really need this container server-side? csm_prefix_1 csm_suffix_1 csm_grid_14-->
		<div class="snap_container_center_844">
			
			<!-- BEGIN MULTIVIEW CONTROL -->
			<asp:MultiView ID="_masterMultiView" runat="server" ActiveViewIndex="0">

				<asp:View ID="_loginView" runat="server"><!-- 0 -->
					<uc:LoginView id='_loginViewControl' runat="server" />
				</asp:View>

				<asp:View ID="_requestFormView" runat="server"><!-- 1 -->
					<uc:RequestForm id='_requestFormControl' runat="server" />
				</asp:View>
				
				<asp:View ID="_userView" runat="server"><!-- 2 -->
					<uc:UserView id="_userViewControl" runat="server" />
				</asp:View>

				<asp:View ID="_approvingManagerView" runat="server"><!-- 3 -->
					<uc:ApprovingManagerView id="_approvingManagerViewControl" runat="server" />
				</asp:View>
				
				<asp:View ID="_accessTeamView" runat="server"><!-- 4 -->
					<uc:AccessTeamView id="_accessTeamViewControl" runat="server" />
				</asp:View>
				
				<asp:View ID="_searchFormView" runat="server"><!-- 5 -->
					<uc:SearchView id="_searchControl" runat="server" />
				</asp:View>
				
				<asp:View ID="_supportView" runat="server"><!-- 6 -->
					<uc:SupportView id="_supportControl" runat="server" />
				</asp:View>
				
			</asp:MultiView>

			<div class="csm_clear">&nbsp;</div>
			<!-- END MUTLIVIEW CONTROL -->				
		
		</div>
		</asp:Panel>
		<div class="csm_clear">&nbsp;</div>
		<!-- END CONTENT AREA -->

		<uc:Footer ID="_footerControl" runat="server" />		
	
	</div>
	<!-- END MASTER CONTAINER -->
	</form>

</body>
</html>