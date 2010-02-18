<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestForm.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.RequestForm" %>

<script src='/scripts/RequestFormUser.js' type="text/javascript" ></script>

<div class="csm_container_center_700">
    
	<h1>Request Form</h1>
	<div class="csm_content_container">
		
		<div class="csm_text_container">
		    <asp:Label ID="_requestFormDescription" runat="server">[TODO - REQUEST FORM DESCRIPTION PLACEHOLDER]</asp:Label>
		</div>
		
		<fieldset><!-- use fieldset for standards & compliance -->
		
		<div class="csm_input_buttons_container">
			<input type="button" value="Clear Form" class="csm_html_button" id='_clearForm'/>
			<asp:Button ID="_submitForm" Text="Submit" CssClass="csm_html_button" 
                runat="server" onclick="_submitForm_Click" />
		</div>				
		
		<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
			<tr>
				<td class="csm_input_form_label_column csm_input_required_field">
					<label for="_requestFormControl__requestorId">User Name</label>
					<p>Please enter your network user name</p>
				</td>
				<td class="csm_input_form_control_column">
					<asp:TextBox ID="_requestorId" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
					<!--<Button ID="_checkRequestorId" Class="csm_html_button" Text="Check" />-->
					<button type='button' id='_checkRequestorId' class="csm_html_button">Check</button>

			        <p><em>Example: axuser (domain name is not needed)</em></p>
				</td>
				<td style="vertical-align: top">
                    <div id="_notification" style="display:none;">
                        <img alt='loading...' src="/images/ajax_indicator.gif" width="16" height="16" />
                    </div>
				</td>
                <td>
                    <asp:TextBox ID="_requestorLoginId" runat="server" style="display:none" ></asp:TextBox>
                </td>

			</tr>

			<tr>
				<td class="csm_input_form_label_column">
					<label for="_requestFormControl__managerName">Manager Name</label>
					<p>Please verify your manager name</p>
				</td>
				<td class="csm_input_form_control_column">
					<asp:TextBox ID="_managerName" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
			        <!--<asp:Button ID="_checkManagerName" runat="server" CssClass="csm_html_button" Text="Check" /> -->
			        <button type='button' id='_checkManagerName' class="csm_html_button">Check</button>
			        <button type='button' id="_editManagerName" class="csm_html_button">Edit</button>
				</td>
				<td>&nbsp</td>
	            <td>
                  <asp:TextBox ID="_managerLoginId" runat="server" style="display:none" ></asp:TextBox>
                </td>
                
			</tr>

			<asp:PlaceHolder ID="_requestForm" runat="server" ></asp:PlaceHolder>		
				
		</table>
		
		<div class="csm_input_buttons_container">
			<input type="button" value="Clear Form" class="csm_html_button" id='_clearForm_lower'/>
			<asp:Button ID="_submitForm_lower" Text="Submit" CssClass="csm_html_button" runat="server" onclick="_submitForm_Click" />
		</div>				
		
		</fieldset>
	</div>
</div>

      <div id='_managerSelectionDiv' style="display:none"; >
          <p />
          <select size="3" name="managerSelection" id="_managerSelection"></select>
      </div>
      <div id='_nameSelectionDiv' style="display:none"; >
           <p />
          <select size="3" name="nameSelection" id="_nameSelection"></select>
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