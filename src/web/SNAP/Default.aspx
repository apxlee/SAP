<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_headPlaceHolder" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
<script type="text/javascript">

	$(document).ready(

		// Login path checkbox image swaps & populate hidden form field
		function() {
			$("div[id*='_loginCheck']").click(function() {
				$("div[id*='_loginCheck']").removeClass("aim_checkbox_checked").addClass("aim_checkbox_unchecked");
				$(this).removeClass("aim_checkbox_unchecked").addClass("aim_checkbox_checked");
				$("input[id*='_loginPathSelection']").attr("value", $(this).attr('value'));
			});
		}

		);
</script>

<div class="csm_container_center_700">
	<div class="csm_content_container">
		
		<div class="csm_text_container csm_alternating_bg">
			<table border="0" cellpadding="0" cellspacing="0">
				<tr>
					<td><div id="login_description_logo"></div></td>
					<td style="vertical-align:middle;padding-left:10px;">
						<p style="padding-right:15px;">The Supplemental Access Process application augments the rights being 
						provisioned through the <a href="http://access.apollogrp.edu/cap/" style="color:Blue;">Computer Access Process (CAP)</a>, which 
						handles Production systems.<br /><br />A Service Desk ticket will be created once the approvals are received from the
						Supplemental Access Process application.  Once the ticket is assigned, the goal is 2 to 3 business days for provisioning.<br /><br />
						Below you may specify the reason for your visit and the application will drop you at that point with minimal user effort.</p>
					</td>
				</tr>
			</table>
		</div>
		
		<asp:Panel ID="_loginMessageContainer" runat="server" CssClass="csm_content_container" Visible="false">
			<asp:Label ID="_loginMessage" runat="server"></asp:Label>
		</asp:Panel>		
	
		<fieldset class="csm_top5">
		
			<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
				<tr>
					<td class="csm_input_form_label_column csm_input_required_field">
						<label for="_loginViewControl__networkId">Network Login:</label>
					</td>
					<td class="csm_input_form_control_column">
						<asp:TextBox ID="_networkId" runat="server" CssClass="csm_text_input_short"></asp:TextBox>
					</td>
				</tr>

				<tr>
					<td class="csm_input_form_label_column csm_input_required_field">
						<label for="_loginViewControl__password">Password:</label>
					</td>
					<td class="csm_input_form_control_column">
						<asp:TextBox TextMode="SingleLine" ID="_password" runat="server" CssClass="csm_text_input_short"></asp:TextBox>
					</td>
				</tr>

				<tr>
					<td colspan="2" class="csm_input_spanning_column">
						<div class="csm_input_form_label_container">
							<table cellpadding="0" cellspacing="0" border="0" class="snap_login">
								<tr>
									<td><div id="_loginCheck1" class="aim_checkbox_checked" value="request_form" runat="server"></div></td>
									<td>
										<h2>I am requesting access for myself.</h2>
										<p>You will be taken to the request form with credentials automatically 
										verified in the system.<br />In the event the username or manager are incorrect, 
										please use the EDIT then CHECK controls to correct the request to reflect 
										your current personal details.</p>
									</td>
								</tr>
								<tr>
									<td><div id="_loginCheck2" class="aim_checkbox_unchecked" value="proxy_request"></div></td>
									<td>
										<h2>I am requesting access for someone else.</h2>
										<p>You will be required to enter the person's username on the request form.</p>
									</td>
								</tr>
								<tr>
									<td><div id="_loginCheck3" class="aim_checkbox_unchecked" value="role_default" runat="server"></div></td>
									<td>
										<h2>I am managing my tasks and approvals, updating a requested change,<br />or checking my status.</h2>
										<p><asp:Label ID="_followLinkMessage" runat="server" Visible="false"><em>The application has determined you are 
										following an email link to Request Id:&nbsp;<asp:Label ID="_followLinkRequestId" runat="server"></asp:Label>.</em>&nbsp;</asp:Label>
										You will be taken to 'My Requests', 'My Approvals' or the request form automatically based on your role.</p>
									</td>
								</tr>
							</table>
							<div class="csm_input_buttons_container_centered csm_top10">
								<asp:Button ID="_submitLogin" Text="Login" CssClass="csm_html_button" runat="server" onclick="_submitLogin_Click" />
							</div>
						</div>
					</td>
				</tr>
			</table>
			<input id="_loginPathSelection" type="hidden" value="request_form" runat="server" />
		</fieldset>			

	</div>
</div>
<script type="text/javascript">
	try { pageTracker._trackPageview("/LoginView"); }
	catch (err) { alert(err.toString()); }
</script>
</asp:Content>
