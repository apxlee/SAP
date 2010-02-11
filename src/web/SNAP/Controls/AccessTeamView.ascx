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
		    <div class="csm_text_container csm_top5">

		        <div class="csm_icon_heading_container oospa_aim_builder">
		            <h2>Access & Identity Management - Workflow Builder</h2>
		        </div>
				
				<fieldset>	
				<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
					<tr>
						<td class="csm_input_form_label_column">
							<label>Required Approver</label>
						</td>
						<td class="csm_input_form_control_column">
							<table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row">
								<tr>
									<td width="22"><input type="checkbox" checked="checked" disabled="disabled" class="csm_input_checkradio" /></td>
									<td width="386"><span class="csm_inline" style="">Manager - Greg Belanger</span></td>
									<td width="20" class="">&nbsp;</td>
									<td width="20" class="oospa_edit_icon">&nbsp;</td>
								</tr>
							</table>							
						</td>
					</tr>
					<tr>
						<td class="csm_input_form_label_column">
							<label>Primary Approver</label>
						</td>
						<td class="csm_input_form_control_column">
							<table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row">
								<tr>
									<td width="22"><input type="checkbox" class="csm_input_checkradio" /></td>
									<td width="386"><span class="csm_inline" style="">Software Group</span></td>
									<td width="20" class="oospa_delete_icon">&nbsp;</td>
									<td width="20" class="oospa_edit_icon" onclick="throwModifyGroupModal();">&nbsp;</td>
								</tr>
								<tr>
									<td colspan="4"><span class="group_description">The Software Group includes all the people who make the softwares.</span></td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td colspan="3">
										<input type="radio" name="radio1" checked="checked" class="csm_input_checkradio" />
										<span class="csm_inline">Chris Schwimmer (Default)</span>
									</td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td colspan="3">
										<input type="radio" name="radio1" class="csm_input_checkradio" />
										<span class="csm_inline">Pat Robertson (Secondary)</span>
									</td>
								</tr>								
							</table>
							<hr />
							<div class="csm_input_buttons_container">
								<input type="button" value="Add New Primary Group" class="csm_html_button" onclick="throwModifyGroupModal();"/>
							</div>												        
						</td>
					</tr>
					<tr>
						<td class="csm_input_form_label_column">
							<label>Secondary Approver</label>
						</td>
						<td class="csm_input_form_control_column">
							<table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row">
								<tr>
									<td width="22"><input type="checkbox" class="csm_input_checkradio" /></td>
									<td width="386"><span class="csm_inline" style="">Technology Name</span></td>
									<td width="20" class="oospa_delete_icon">&nbsp;</td>
									<td width="20" class="oospa_edit_icon">&nbsp;</td>
								</tr>
								<tr>
									<td colspan="4"><span class="group_description">Exchange, Windows Network Shares, Collaboration</span></td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td colspan="3">
										<input type="radio" name="radio2" checked="checked" class="csm_input_checkradio" />
										<span class="csm_inline">Josh Welch (Default)</span>
									</td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td colspan="3">
										<input type="radio" name="radio2" class="csm_input_checkradio" />
										<span class="csm_inline">Flush Limbowel (Secondary)</span>
									</td>
								</tr>								
							</table>
							<table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row csm_alternating_bg">
								<tr>
									<td width="22"><input type="checkbox" class="csm_input_checkradio" /></td>
									<td width="386"><span class="csm_inline" style="">Another Technology</span></td>
									<td width="20" class="oospa_delete_icon">&nbsp;</td>
									<td width="20" class="oospa_edit_icon">&nbsp;</td>
								</tr>
								<tr>
									<td colspan="4"><span class="group_description">Oracle Databases, Galaxy Applications</span></td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td colspan="3">
										<input type="radio" name="radio3" checked="checked" class="csm_input_checkradio" />
										<span class="csm_inline">Billy Coriggan (Default)</span>
									</td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td colspan="3">
										<input type="radio" name="radio3" class="csm_input_checkradio" />
										<span class="csm_inline">Shame Hannity (Secondary)</span>
									</td>
								</tr>								
							</table>							
							<hr />
					        
					        <div class="csm_input_buttons_container">
								<input type="button" value="Add New Secondary Group" class="csm_html_button"/>
							</div>
						</td>
					</tr>																				
				</table>
				
				<div class="csm_input_buttons_container">
					<input type="button" value="Pause" class="csm_html_button"/>
					<input type="button" value="Build It | Continue" class="csm_html_button"/>
				</div>			
				
				</fieldset>		        
		        
		    </div>	
		    
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
