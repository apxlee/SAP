<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="Support.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.Support" %>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
	<div class="csm_container_center_700">
		<img src="./images/marketing_support_banner.png" />
		<div class="csm_clear">&nbsp;</div>
		
		<div class="csm_grid_8">
			<h1>Access News</h1>
			<div class="csm_content_container">
				<p><strong>Summer 2010</strong><br />
				Sharepoint Paperless Privileged Access will no longer support new access requests.  Please enter new requests by accessing 
				the "Request Form" link from the top navigation bar when it appears after login.
				</p>
			</div>
			
			<h1>Frequently Asked Questions</h1>
			<div class="csm_content_container">
				<p><strong>Question:</strong><br />Can I fill out a form for another employee?</p>
				<p><strong>Answer:</strong><br />Yes. It is your responsibility to communicate the Acknowledgement information to the employee. They
				will receive email updates including Request for Change or details if the request is Denied.</p>
				<hr />
				
				<p><strong>Question:</strong><br />I am an approver. How do I review a request?</p>
				<p><strong>Answer:</strong><br />You can review a request directly from the link in the email you received. You may approve directly 
				from the application by selecting "My Approvals" in the user interface (UI) within the top navigation. <i>Note: Only individuals 
				identified as Approvers will see the "My Approvals" section.</i></p>
				<hr />
				
				<p><strong>Question:</strong><br />Are there tickets generated for my request?</p>
				<p><strong>Answer:</strong><br />Yes. Once you have gained all the appropriate approvals, the request will return to the Access Team 
				so they may review and create a ticket for provisioning.</p>
				<hr />

				<p><strong>Question:</strong><br />How do I know when I have my access?</p>
				<p><strong>Answer:</strong><br />You will receive an email and you will see a status of Closed Completed in your access 
				request tracking, viewable in "My Requests".</p>
			</div>
			
			<h1>Contact The Team</h1>
			<p style="font-size:.85em;line-height:1.2em;">Supplemental Access Process Feedback and Usability:<br />
			<strong><a href="mailto:DG C-APO-Corporate-ITS Access Management SNAPFU?subject=SNAP%20Comments">DG C-APO-Corporate-ITS Access Management SNAPFU</a></strong>
			</p>
			
			<table border="0" cellpadding="0" cellspacing="0" style="margin-top:5px;">
				<tr>
					<td><div id="support_contact_logo"></div></td>
					<td style="vertical-align:middle;padding-left:10px;">
						<p style="font-size:.85em;line-height:1.2em;">The Supplemental Access Process application is a product of <strong>Access & Identity Managment</strong> under the direction of<br />Greg Belanger.</p>
						<p style="font-size:.85em;line-height:1.8em;"><strong><a href="mailto:DG C-APO-Corporate-ITS Access Management?subject=SNAP%20Comments">DG C-APO-Corporate-ITS Access Management</a></strong></p>
					</td>
				</tr>
			</table>

		</div>
		
		<div class="csm_grid_3" style="padding-left:10px;">
			<h1>Tutorials</h1>
			<table border="0" cellpadding="0" cellspacing="5" style="margin-top:5px;" width="100%">
				<tr>
					<td ><img src="images/icon_quicktime.png" /></td>
					<td style="font-size:.85em;line-height:1.2em;vertical-align:top;padding-bottom:5px;"><a href="../snap/SupportFiles/MyApprovals_1024x576.mov">My Approvals</a><br />Quicktime 6.6MB</td>
				</tr>
				<tr>
					<td><img src="images/icon_WMV.png" /></td>
					<td style="font-size:.85em;line-height:1.2em;vertical-align:top;"><a href="../snap/SupportFiles/MyApprovals.wmv" onclick="return pageTracker._trackEvent('video','play','MyApprovals WMV');">My Approvals</a><br />WMV 11.7MB</td>
				</tr>
			</table>
			<hr />
			<table border="0" cellpadding="0" cellspacing="0" style="margin-top:5px;" width="100%">
				<tr>
					<td><img src="images/icon_quicktime.png" /></td>
					<td style="font-size:.85em;line-height:1.2em;vertical-align:top;padding-bottom:5px;"><a href="../snap/SupportFiles/RequestForm.mov">Request Form</a><br />Quicktime 1.2MB</td>
				</tr>
				<tr>
					<td><img src="images/icon_WMV.png" /></td>
					<td style="font-size:.85em;line-height:1.2em;vertical-align:top;"><a href="../snap/SupportFiles/RequestForm.wmv">Request Form</a><br />WMV 890K</td>
				</tr>

			</table>
		</div>
	</div>
	<script type="text/javascript">
		try { pageTracker._trackPageview("SupportView"); }
		catch (err) {}
	</script>
</asp:Content>
