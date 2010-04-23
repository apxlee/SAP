﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovingManagerPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.ApprovingManagerPanel" %>
<div class="csm_text_container">
	<div class="csm_icon_heading_container snap_my_approvals">
		<h2>Approver Actions</h2>
		<p class="">As an Approver, please review the access details above and select an appropriate action below.  Denials
		and Request for Change require a detailed description.</p>
	</div>
	
    		     
    <fieldset><!-- use fieldset for standards & compliance -->
	<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
		<tr>
			<td class="csm_input_form_label_column">
				<label>Approval</label>
			</td>
			<td class="csm_input_form_control_column">
			    <input type="button" id="_approve" value="Approve" class="csm_html_button" runat="server" />
	            <input type="button" id="_approveAndMoveNext" value="Approve and Review Next" class="csm_html_button" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="csm_input_form_label_column">
				<label>Request Change or Deny</label>
			</td>
			<td class="csm_input_form_control_column">
				<input type="radio" name="_changeDeny" class="csm_input_checkradio" value="Request Change" onclick="changeDenyClick(this);" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Request Change</span>
				<input type="radio" name="_changeDeny" class="csm_input_checkradio" value="Deny" onclick="changeDenyClick(this);" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Deny</span>				
				<div class="csm_clear">&nbsp;</div>
				<textarea rows="10" cols="" name="_changeDenyComment" class="csm_text_input csm_textarea_short"></textarea>
				<p><em><strong>Note:&nbsp;</strong>Specific reasons for Request Changes, Denial are required.</em></p>
			</td>
		</tr>					

	</table>
	<div class="csm_input_buttons_container" style="margin-right:6px;">
	    <input type="button" id="_requestChange" value="Request Change" class="csm_html_button" runat="server" />
	    <input type="button" id="_deny" value="Deny" class="csm_html_button" runat="server" />
	</div>				
    </fieldset>	        
</div>