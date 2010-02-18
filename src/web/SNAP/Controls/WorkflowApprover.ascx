<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowApprover.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowApprover" %>
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