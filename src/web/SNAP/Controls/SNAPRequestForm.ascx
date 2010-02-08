<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SNAPRequestForm.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.SNAPRequestForm" %>
<div class="csm_container_center_700">
    
	<h1>Request Form</h1>
	<div class="csm_content_container">
		
		<div class="csm_text_container">
		    <asp:Label ID="_requestFormDescription" runat="server">[TODO - REQUEST FORM DESCRIPTION PLACEHOLDER]</asp:Label>
		</div>
		
		<fieldset><!-- use fieldset for standards & compliance -->
		
		<div class="csm_input_buttons_container">
			<input type="button" value="Clear Form" class="csm_html_button" />
			<asp:Button ID="_submitForm" Text="Submit" CssClass="csm_html_button" runat="server" />
		</div>				
		
		<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
			<tr>
				<td class="csm_input_form_label_column csm_input_required_field">
					<label for="_requestFormControl__requestorId">User Name</label>
					<p>Please enter your network user name</p>
				</td>
				<td class="csm_input_form_control_column">
					<asp:TextBox ID="_requestorId" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
					<asp:Button ID="_checkRequestorId" runat="server" CssClass="csm_html_button" Text="Check" />
			        <p><em>Example: axuser -or- apollogroup\axuser</em></p>
				</td>
			</tr>
			<tr>
				<td class="csm_input_form_label_column">
					<label for="_requestFormControl__managerName">Manager Name</label>
					<p>Please verify your manager name</p>
				</td>
				<td class="csm_input_form_control_column">
					<asp:TextBox ID="_managerName" runat="server" CssClass="csm_text_input_short"></asp:TextBox>
			        <asp:Button ID="_checkManagerName" runat="server" CssClass="csm_html_button" Text="Check" />
			        <asp:Button ID="_editManagerName" runat="server" CssClass="csm_html_button" Text="Edit" />
				</td>
			</tr>
			
			<!-- TODO: replace the following with a repeater -->
			<!-- REPEATER START -->
			
			<!-- IF INPUT FIELD HAS CHILDREN THEN USE THIS METHOD... -->		
			<tr>
				<td colspan="2" class="csm_input_spanning_column">
					<div class="csm_input_form_label_container">
						<label class="csm_input_required_field"><asp:Label ID="_todo1" runat="server">TODO: OUTER SECTION LABEL</asp:Label></label>
						<asp:Label ID="_todo2" runat="server">TODO: OUTER SECTION DESCRIPTION</asp:Label>						
					</div>
					<div class="csm_input_form_control_container">
						<!-- INNER REPEATER START -->
					    <label><asp:Label ID="_todo3" runat="server">TODO: INNER SECTION LABEL</asp:Label></label>
						<asp:Label ID="_todo4" runat="server">TODO: INNER SECTION DESCRIPTION</asp:Label>
						<asp:TextBox ID="__aid2__unique1" runat="server" TextMode="MultiLine" Rows="10" CssClass="csm_text_input"></asp:TextBox>
						<!-- INNER REPEATER END -->
						<!-- INNER REPEATER START -->
					    <label><asp:Label ID="_todo6" runat="server">TODO: INNER SECTION LABEL</asp:Label></label>
						<asp:Label ID="_todo7" runat="server">TODO: INNER SECTION DESCRIPTION</asp:Label>
						<asp:TextBox ID="__aid3__unique2" runat="server" TextMode="MultiLine" Rows="10" CssClass="csm_text_input"></asp:TextBox>
						<!-- INNER REPEATER END -->
						
					</div>							
				</td>
			</tr>
			
			<!-- ELSE IF INPUT FIELD IS PARENT THEN USE THIS METHOD... -->
			<tr>
				<td colspan="2" class="csm_input_spanning_column">
					<div class="csm_input_form_label_container">
						<label class="csm_input_required_field"><asp:Label ID="_todo8" runat="server">TODO: OUTER SECTION LABEL</asp:Label></label>
						<asp:Label ID="_todo9" runat="server">TODO: OUTER SECTION DESCRIPTION</asp:Label>
					</div>
					<div class="csm_input_form_control_container">
						<asp:TextBox ID="_todo10" runat="server" TextMode="MultiLine" Rows="10" CssClass="csm_text_input"></asp:TextBox>
					</div>							
				</td>
			</tr>
			
			<!-- REPEATER STOP -->
					
		</table>
		
		<div class="csm_input_buttons_container">
			<input type="button" value="Clear Form" class="csm_html_button" />
			<asp:Button ID="_submitForm_lower" Text="Submit" CssClass="csm_html_button" runat="server" />
		</div>				
		
		</fieldset>
	</div>
</div>

     <div id="dialog" class="" style="display:none;">
        <p>As an individual whose position requires Privileged Administrative Access with any or all of Apollo Group, Inc. administrative 
        information systems I have the following responsibilities:</p>
        <ul>
            <li>I may be provided with direct access to confidential and valuable data and/or use of data systems.</li>
        <li>In the interest of maintaining the integrity of these systems and of ensuring the security and proper use of Apollo Group, Inc. 
        resources, I will maintain the confidentiality of my password for all systems to which I have access.</li>
         <li>I will maintain in strictest confidence the data to which I have access. Confidential information will not be shared in any manner with others 
        who are unauthorized to access such data.</li>
         <li>I will use my access to Apollo Group, Inc. systems for the sole purpose of 
        conducting official business of Apollo Group, Inc.</li>
         <li>I understand that the use of these systems and their data for personal 
        purposes is prohibited.</li>
        <li> I understand that any abuse of access to Apollo Group, Inc. systems and their data, any illegal 
        copying of software or data, and any misuse of Apollo Group, Inc. equipment may result in disciplinary action up to and 
        including termination and possible legal action.</li>
        </ul>
     </div>