<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.LoginView" %>
<script type="text/javascript">

	$(document).ready(

		// Login path checkbox image swaps & populate hidden form field
		function() {
			$("div[id*='_loginCheck']").click(function() {
				$("div[id*='_loginCheck']").removeClass("aim_checkbox_checked").addClass("aim_checkbox_unchecked");
				$(this).removeClass("aim_checkbox_unchecked").addClass("aim_checkbox_checked");
				$("input[id*='_loginPathSelection']").attr("value", $(this).attr('value'));
				//alert($("input[id*='_loginPathSelection']").attr('value'));
			});
		}

		);
</script>

<div class="csm_container_center_700">
	<div class="csm_content_container">
		
		<div class="csm_text_container csm_alternating_bg">
			<p>this app is for the thing with the stuff</p>
		</div>
		
		<asp:Panel ID="_loginMessageContainer" runat="server" CssClass="csm_content_container" Visible="false">
			<!--<div class="csm_text_container">-->
				<asp:Label ID="_loginMessage" runat="server">This is the login error message</asp:Label>
			<!--</div>-->
		</asp:Panel>		
	
		<fieldset class="csm_top5">
		
			<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
				<tr>
					<td class="csm_input_form_label_column csm_input_required_field">
						<label for="_loginViewControl__networkId">Network Login</label>
					</td>
					<td class="csm_input_form_control_column">
						<asp:TextBox ID="_networkId" runat="server" CssClass="csm_text_input_short">jdsteele</asp:TextBox>
					</td>
				</tr>

				<tr>
					<td class="csm_input_form_label_column csm_input_required_field">
						<label for="_loginViewControl__password">Password</label>
					</td>
					<td class="csm_input_form_control_column">
						<asp:TextBox TextMode="Password" ID="_password" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
					</td>
				</tr>

				<tr>
					<td colspan="2" class="csm_input_spanning_column">
						<div class="csm_input_form_label_container">
							<table cellpadding="0" cellspacing="5px" border="0" class="snap_login">
								<tr>
									<td><div id="_loginCheck1" class="aim_checkbox_unchecked" value="request_form"></div></td>
									<td>
										<h2>I am requesting access for myself.</h2>
										<p>You will be taken to the request form with credentials automatically 
										verified in the system.<br />In the event the username or manager are incorrect, 
										please use the EDIT and CHECK controls to correct the request to reflect 
										your current status within the organization.</p>
									</td>
								</tr>
								<tr>
									<td><div id="_loginCheck2" class="aim_checkbox_unchecked" value="proxy_request"></div></td>
									<td>
										<h2>I am a manager or supervisor requesting access for a team member.</h2>
										<p>You will be required to enter your team members' username on the the request form.</p>
									</td>
								</tr>
								<tr>
									<td><div id="_loginCheck3" class="aim_checkbox_unchecked" value="role_default"></div></td>
									<td>
										<h2>I am checking my status, managing my tasks and approvals, or<br />updating a requested change.</h2>
										<p>You will be taken to 'My Requests', 'My Approvals' or the request form automatically based on your role.</p>
									</td>
								</tr>
							</table>
							<div class="csm_input_buttons_container_centered">
								<asp:Button ID="_submitLogin" Text="Login" CssClass="csm_html_button" runat="server" onclick="_submitLogin_Click" />
							</div>
						</div>
					</td>
				</tr>
			</table>
			<input id="_loginPathSelection" type="hidden" value="role_default" runat="server" />
		</fieldset>			

	</div>
</div>