<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessCommentsPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.AccessCommentsPanel" %>
<!-- TODO: make server side user controls -->
<div class="csm_text_container">
	<div class="csm_icon_heading_container oospa_aim_acknowledgement">
		<h2>Comments</h2>
		<p>Enter comments that relate to the current request.  Optionally select the community who will be able to see the comments.</p>
	</div>
    		     
    <fieldset><!-- use fieldset for standards & compliance -->
	<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
		<tr>
			<td class="csm_input_form_label_column">
				<label>Comments</label>
				<p>Select the community who will be able to view the comments.  Comments will appear in the 'Request Details' section.</p>
			</td>
			<td class="csm_input_form_control_column">
				<input type="radio" name="_audience" class="csm_input_checkradio" value="7" onclick="audienceClick(this);" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">AIM</span>
				<input type="radio" name="_audience" class="csm_input_checkradio" value="6" onclick="audienceClick(this);" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Approvers</span>
				<input type="radio" name="_audience" class="csm_input_checkradio" value="5" onclick="audienceClick(this);" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Everyone</span>
				<div class="csm_clear">&nbsp;</div>
				<textarea rows="10" cols="" class="csm_text_input csm_textarea_short"></textarea>
			</td>
		</tr>					

	</table>
	<div class="csm_input_buttons_container" style="margin-right:6px;">
		<input type="button" id="_submitComments" value="Submit Comments" class="csm_html_button" runat="server" />
	</div>				
    </fieldset>	        
</div>