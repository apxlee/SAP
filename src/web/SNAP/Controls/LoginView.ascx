<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.LoginView" %>
<div class="csm_container_center_700">
	<div class="csm_content_container">
		
		<asp:Panel ID="_loginMessageContainer" runat="server" CssClass="csm_content_container" Visible="false">
			<!--<div class="csm_text_container">-->
				<asp:Label ID="_loginMessage" runat="server"></asp:Label>
			<!--</div>-->
		</asp:Panel>
	
		<fieldset>
		
			<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
				<tr>
					<td class="csm_input_form_label_column csm_input_required_field">
						<label for="_loginViewControl__networkId">Network Login</label>
					</td>
					<td class="csm_input_form_control_column">
						<asp:TextBox ID="_networkId" runat="server" CssClass="csm_text_input_short"></asp:TextBox>
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
			</table>
		
			<div class="csm_text_container">
				<p>[TODO: Action checkboxes and verbiage]</p>
			</div>	
		
			<div class="csm_input_buttons_container">
				<asp:Button ID="_submitLogin" Text="Submit" CssClass="csm_html_button" runat="server" onclick="_submitLogin_Click" />
			</div>
		
		</fieldset>				
	
	</div>
</div>