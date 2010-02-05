<%@ Register TagPrefix="uc1" TagName="ucFooter" Src="includes/ucFooter.ascx" %>
<%@ Page CodeBehind="Index.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="Apollo.AIM.SNAP.Web.Index2"  %>
<%@ Register TagPrefix="uc1" TagName="ucHeader" Src="includes/ucHeader.ascx" %>
<html>
<head>
	<title>Apollo Group, Inc.</title>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
	<link rel="stylesheet" href="styles/login.css">
	<meta name="Topaz Monitor" content="CAP - Login page">
</head>
<body onLoad="document.getElementById('txtLogin').focus();">
	<div id="iecontainer">
		<div id="main">
			<div id="header"></div>
			<div id="content" class="alt-align">
				<p id="ErrorMsg" runat="server" class="error"/>
				<div id="login">
					<form runat="server">
						<h2>LOGIN</h2>
						<div class="fields">
							<p><label for="txtLogin">Network&nbsp;Login</label><asp:textbox Runat="server" name="loginID" type="text" id="txtLogin" size="20" maxlength="30"/></p>
							<p><label for="txtPassword">Password</label><asp:TextBox runat="server" id="txtPassword" type="password" size="20" maxlength="30" TextMode="Password"/></p>
						</div>
						<asp:Button id="btnLogin" runat="server" Text="Submit" class="button"/>
					</form>
				</div>
				<div id="usage">
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
					
					<div class="questions">
						<h2>Questions and Technical Support</h2>
						<p>Campus employees should contact their local Campus Technician for assistance. All 
                            other users should contact 
							<br/>Apollo Tech Support at (480) 929-4100.</p>
					</div>
				</div>
				<div id="holder"></div>
			</div>
		</div>
		<uc1:ucFooter id="UcFooter1" runat="server"/>
	</div>		
</body>
</html>
