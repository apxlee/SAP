﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestForm.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.RequestForm" %>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>RequestForm.js" type="text/javascript"></script>
<script type="text/javascript"> $(document).ready(DocReady);</script>
<div class="csm_container_center_700">
    
	<div class="csm_content_container">
		
		<div class="csm_text_container">
		    <p>Use this form to request Out-of-Scope Privileged Access to
		    servers only.  "Out-of-Scope" is defined as resources that are not subject to PCI/SOX compliance.  In-Scope access is
		    handled with a <a href="http://microsoft.com">Computer Access Process Request.</a></p>
		    <p>Please Note: This form is NOT used for requesting application or database access.  Windows Sql Server database
		    access requires an "a.dot" account (you may request "a.dot" accounts with this application).  Database access is
		    completed using a Service Desk ticket assigned to the database group.</p>
		</div>
		
		<fieldset><!-- use fieldset for standards & compliance -->
        
        <div id="_formValidationTop"></div>
        
        <asp:PlaceHolder ID="_changeComments" runat="server" />
		
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
					<button type='button' id='_checkRequestorId' class="csm_html_button">Check</button>
			        <p><em>Example: axuser (domain name is not needed)</em></p>
				</td>
                <td>
                    <asp:TextBox ID="_requestorLoginId" runat="server" style="display:none" ></asp:TextBox>
                </td>

			</tr>

			<tr>
				<td class="csm_input_form_label_column csm_input_required_field">
					<label for="_requestFormControl__managerName">Manager Name</label>
					<p>Please verify your manager name</p>
				</td>
				<td class="csm_input_form_control_column">
					<asp:TextBox ID="_managerName" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
			        <button type='button' id='_checkManagerName' class="csm_html_button">Check</button>
			        <button type='button' id="_editManagerName" class="csm_html_button">Edit</button>
			        <div id="_notificationManager" style="display:none;">
                        <img alt='loading...' src="images/ajax_indicator.gif" width="16" height="16" />
                    </div>			        
				</td>
				<td>&nbsp</td>
	            <td>
                  <asp:TextBox ID="_managerLoginId" runat="server" style="display:none" ></asp:TextBox>
                </td>
                
			</tr>

			<asp:PlaceHolder ID="_requestForm" runat="server" ></asp:PlaceHolder>		
				
		</table>
		
		<div id="_formValidationBottom"></div>
		
		<div class="csm_input_buttons_container">
			<input type="button" value="Clear Form" class="csm_html_button" id='_clearForm_lower'/>
			<asp:Button ID="_submitForm_lower" Text="Submit" CssClass="csm_html_button" runat="server"/>
		</div>				
		
		</fieldset>
	</div>
</div>

      <div id='_managerSelectionDiv' style="display:none"; >
          <p />
            <div style="display:none;" class="oospa_ajax_indicator">
                <img alt="loading..." src="images/ajax_indicator.gif" width="16" height="16" />
            </div>
          <select size="3" style="display:none;" class="oospa_select_user" name="managerSelection" id="_managerSelection"></select>
      </div>
      <div id='_nameSelectionDiv' style="display:none"; >
           <p />
           <div style="display:none;" class="oospa_ajax_indicator">
                <img alt="loading..." src="images/ajax_indicator.gif" width="16" height="16" />
            </div>
          <select size="3" style="display:none;" class="oospa_select_user" name="nameSelection" id="_nameSelection"></select>
      </div>

     <div id="_acknowledgmentDiv" class="modal_li" style="display:none;">
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
        <div style="display:none;">
            <asp:Button ID="_submit_form" Text="submit form" CssClass="csm_html_button" runat="server" onclick="_submitForm_Click" />
        </div>
     </div>