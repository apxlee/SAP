<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovingManagerPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.ApprovingManagerPanel" %>
<!-- TODO: make server side user controls -->
<div class="csm_text_container">
	<div class="csm_icon_heading_container snap_my_approvals">
		<h2>Approver Actions</h2>
	</div>
	<p class="csm_prefix_bump3">As an Approver, please review the access details above and select an appropriate action below.  Denials
		and Request for Change require a detailed description.</p>
    		     
    <fieldset><!-- use fieldset for standards & compliance -->
	<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
		<tr>
			<td class="csm_input_form_label_column">
				<label>Approval</label>
			</td>
			<td class="csm_input_form_control_column">
				<input type="button" value="Approve" class="csm_html_button"/>
				<input type="button" value="Approve and Review Next" class="csm_html_button"/>
			</td>
		</tr>
		<tr>
			<td class="csm_input_form_label_column">
				<label>Request Change or Deny</label>
			</td>
			<td class="csm_input_form_control_column">
					<input type="radio" name="radio5" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Request Change</span>
					<input type="radio" name="radio5" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Deny</span>
					<div class="csm_clear">&nbsp;</div>
					<textarea rows="10" cols="" class="csm_text_input csm_textarea_short"></textarea>
					<p><em><strong>Note:&nbsp;</strong>Specific reasons for Request Changes, Denial are required.</em></p>
			</td>
		</tr>					

	</table>
	<div class="csm_input_buttons_container" style="margin-right:6px;">
		<input type="button" value="Request Change | Deny" class="csm_html_button" />
	</div>				
    </fieldset>	        
</div>