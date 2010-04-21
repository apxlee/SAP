<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AcknowledgementPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.AcknowledgementPanel" %>
<!-- TODO: make server side user controls -->
<div class="csm_text_container">
	<div class="csm_icon_heading_container snap_my_approvals">
		<h2>Access & Identity Management - Acknowledgement</h2>
	</div>
	<p class="csm_prefix_bump3">Please review the access details above and select an appropriate action below.  Denials
		and Request for Change require a detailed description.</p>
    		     
    <fieldset><!-- use fieldset for standards & compliance -->
	<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
		<tr>
			<td class="csm_input_form_label_column">
				<label>Acknowledgement</label>
			</td>
			<td class="csm_input_form_control_column">
				<asp:Button ID="_acknowledge" Text="Acknowledge" CssClass="csm_html_button" runat="server" />
				<asp:Button ID="_acknowledgeAndMoveNext" Text="Acknowledge and Review Next" CssClass="csm_html_button" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="csm_input_form_label_column">
				<label>Request Change, Deny or Cancel</label>
			</td>
			<td class="csm_input_form_control_column">
				<input type="radio" name="_changeDeny" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Request Change</span>
				<input type="radio" name="_changeDeny" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Deny</span>
				<input type="radio" name="_changeDeny" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Cancel</span>				
				<div class="csm_clear">&nbsp;</div>
				<textarea rows="10" cols="" class="csm_text_input csm_textarea_short"></textarea>
				<p><em><strong>Note:&nbsp;</strong>Specific reasons for Request Changes, Denial or Cancellation are required.</em></p>
			</td>
		</tr>					

	</table>
	<div class="csm_input_buttons_container" style="margin-right:6px;">
		<asp:Button ID="_requestChangeDeny" Text="Submit" class="csm_html_button" runat="server" />
	</div>				
    </fieldset>	        
</div>