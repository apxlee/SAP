<%@ Page CodeBehind="Index.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="Apollo.AIM.SNAP.Web.Index"  %>
<%@ Register src="~/Controls/SNAPDefaultIncludes.ascx" tagname="SNAPIncludes" tagprefix="uc" %>
<%@ Register src="~/Controls/SNAPFooter.ascx" tagname="SNAPFooter" tagprefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="_head" runat="server">
    <title>Out-Of-Scope Privileged Access</title>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
	<meta name="Topaz Monitor" content="OOSPA - Login Page">    
</head>
<body onLoad="document.getElementById('txtLogin').focus();">
	<form runat="server" id="_defaultForm">
	
	<uc:SNAPIncludes id="_includes" runat="server" />

	<!-- BEGIN MASTER CONTAINER -->
	<div class="csm_container_100 csm_template_1">

		<!-- BEGIN CONTENT AREA -->
		<asp:Panel ID="_contentContainer" CssClass="csm_container_16 csm_500" runat="server">
			
			<p id="ErrorMsg" runat="server" />
			<h2>LOGIN</h2>
			<div>
				<p><label for="txtLogin">Network&nbsp;Login</label><asp:textbox Runat="server" name="loginID" type="text" id="txtLogin" size="20" maxlength="30"/></p>
				<p><label for="txtPassword">Password</label><asp:TextBox runat="server" id="txtPassword" type="password" size="20" maxlength="30" TextMode="Password"/></p>
			</div>
			<asp:Button id="btnLogin" runat="server" Text="Submit" class="button"/>
				
			<div>
				<p>This application allows employees of Apollo Group, Inc., University of Phoenix, 
                    Institute for Professional Development, Western International University, and 
                    College for Financial Planning to request access to applications such as Campus 
                    Tracking, OSIRIS, PeopleSoft, and Remote Access. In addition, this application 
                    is used to create access accounts for new users who require Outlook (email) and 
                    network access.</p>
	 
				<p><a href="<%=Request.ApplicationPath %>/Help/ApplicationList.htm" class="" target="_blank">
                    Click here</a> for a list of company applications. Instructions are provided to 
                    request access for non-CAP applications.</p>
			
				<p>What's new in CAP?  <a href="<%=Request.ApplicationPath %>/Help/WhatsNew.htm" class="" target="_blank">
                    Click here</a> to review recent changes to CAP.</p>
				
				<div>
					<h2>Questions and Technical Support</h2>
					<p>Campus employees should contact their local Campus Technician for assistance. All 
						other users should contact <br/>Apollo Tech Support at (480) 929-4100.</p>
				</div>						
		
		</asp:Panel>
		<div class="csm_clear">&nbsp;</div>
		<!-- END CONTENT AREA -->

		<uc:SNAPFooter id="_footer" runat="server" />
	
	</div>
	<!-- END MASTER CONTAINER -->
	</form>
</body>
</html>