<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestForm.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.RequestForm" %>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>RequestForm.js" type="text/javascript"></script>
<script type="text/javascript"> $(document).ready(DocReady);</script>
<style type="text/css" media="screen">
	a.request_form_no_show {display:none;}
</style>
<div class="csm_container_center_700">
    
	<div class="csm_content_container">
		
		<div class="csm_text_container">
		    <p>The Supplemental Access Process application augments the rights being 
			provisioned through<br /> the <a href="http://access.apollogrp.edu/cap/" style="color:Blue;">Computer Access Process (CAP)</a>, 
			which handles Production systems.<br /><br />A Service Desk ticket will be created once the approvals are received from the
			Supplemental Access Process application.  Once the ticket is assigned, the goal is 2 to 3 business days for provisioning.<br /><br />
			Please fill out the form below and follow the prescribed formats when applicable.  Examples have been provided to assist in your request.
			</p>
		</div>
		
		<fieldset><!-- use fieldset for standards & compliance -->
        
        <div id="_formValidationTop" class="csm_input_validation_summary"></div>
        
        <asp:Label ID="_changeComments" runat="server" CssClass="request_form_change_comments" />
 		
		<div class="csm_input_buttons_container">
			<input type="button" value="Clear Form" class="csm_html_button" id='_clearForm'/>
			<asp:Button ID="_submitForm" Text="Submit" CssClass="csm_html_button" 
                runat="server" onclick="_submitForm_Click" />
		</div>				
		
		<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
			<tr>
				<td class="csm_input_form_label_column csm_input_required_field" style="vertical-align:inherit">
					<label for="_requestFormControl__requestorId">Affected End User:</label>
				</td>
				<td class="csm_input_form_control_column">
					<asp:TextBox ID="_requestorId" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
					<button type='button' id='_checkRequestorId' class="csm_html_button">Check</button>
					<p><em>Enter the Display Name or User ID of the Affected End User.</em></p>
				</td>
                <td>
                    <asp:TextBox ID="_requestorLoginId" runat="server" style="display:none" ></asp:TextBox>
                    <input type="hidden" id="_hiddenAffectedEndUserId" runat="server" value="" />
                </td>

			</tr>

			<tr>
				<td class="csm_input_form_label_column csm_input_required_field" style="vertical-align:inherit">
					<label for="_requestFormControl__managerName">Affected User's Manager:</label>
				</td>
				<td class="csm_input_form_control_column">
					<asp:TextBox ID="_managerName" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
			        <button type='button' id='_checkManagerName' class="csm_html_button">Check</button>
			        <button type='button' id="_editManagerName" class="csm_html_button">Edit</button>
			        <div id="_notificationManager" style="display:none;">
                        <img alt='loading...' src="images/ajax_indicator.gif" width="16" height="16" />
                    </div>
                    <p><em>Verify the Affected User's Manager</em></p>			        
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
<script type="text/javascript">
	try { pageTracker._trackPageview("/RequestForm"); }
	catch (err) { alert(err.toString()); }
</script>     