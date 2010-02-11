<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessTeamView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.AccessTeamView" %>
		    <!-- ACKNOWLEDGEMENT SECTION -->
		    <div class="csm_text_container csm_top5">
		        
		        <div class="csm_icon_heading_container oospa_aim_acknowledgement">
		            <h2>Access & Identity Management - Acknowledgement</h2>
		        </div>
		        <!-- Access Team Acknowledgement -->
				<fieldset><!-- use fieldset for standards & compliance -->
				
				<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
					<tr>
						<td class="csm_input_form_label_column">
							<label>Acknowledgement</label>
						</td>
						<td class="csm_input_form_control_column">
							<input type="button" value="Acknowledge" class="csm_html_button"/>
						</td>
					</tr>
					<tr>
						<td class="csm_input_form_label_column">
							<label>Request Change,Deny or Cancel</label>
						</td>
						<td class="csm_input_form_control_column">
								<input type="radio" name="radio5" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Request Change</span>
								<input type="radio" name="radio5" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Deny</span>
								<input type="radio" name="radio5" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Cancel</span>
								<div class="csm_clear">&nbsp;</div>
								<textarea rows="10" cols="" class="csm_text_input csm_textarea_short"></textarea>
								<p><em><strong>Note:&nbsp;</strong>Specific reasons for Request Changes, Denial or Cancellation are required.</em></p>
						</td>
					</tr>					

				</table>
								<div class="csm_input_buttons_container" style="margin-right:6px;">
									<input type="button" value="Submit | Cancel" class="csm_html_button" />
								</div>							
				

				</fieldset>
			</div>

		    <!-- WORKFLOW BUILDER SECTION -->
		    
		    <asp:PlaceHolder ID="_workflowBuilderPanel" runat="server" />	
		    
		    <!-- COMMENTS SECTION -->
		    <div class="csm_text_container csm_top5">
		        
		        <div class="csm_icon_heading_container oospa_aim_comments">
		            <h2>Access & Identity Management - Comments</h2>
		        </div>
		        
				<fieldset><!-- use fieldset for standards & compliance -->
				
				<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
				
					<tr>
						<td class="csm_input_form_label_column">
							<label>Comments</label>
							<p>Select the community who will be able to view comments.  Comments will appear in the 'Request Details' section.</p>
						</td>
						<td class="csm_input_form_control_column">
								<input type="radio" name="radio4" class="csm_input_checkradio" checked="checked" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">AIM</span>
								<input type="radio" name="radio4" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Approvers</span>
								<input type="radio" name="radio4" class="csm_input_checkradio" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Everyone</span>							
								<textarea rows="4" cols="" class="csm_text_input csm_textarea_short"></textarea>							
						</td>
					</tr>
				</table>
				
				<div class="csm_input_buttons_container">
					<input type="button" value="Submit Comments" class="csm_html_button"/>
				</div>
								
				</fieldset>
			</div>		        
