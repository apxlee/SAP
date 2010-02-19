<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowApprover.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowApprover" %>
<td class="csm_input_form_label_column">
	<asp:Label ID="_approverSectionName" runat="server">[TEAM APPROVERS]</asp:Label>
</td>
<td class="csm_input_form_control_column">
	<table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row">
		<tr>
			<td width="22"><input type="checkbox" class="csm_input_checkradio" /></td>
			<td width="386"><asp:Label ID="_approverGroupName" runat="server" CssClass="csm_inline">[SOFTWARE GROUP]</asp:Label></td>
			<td width="20" class="oospa_delete_icon"><!-- TODO: move icons to asp:Panel -->&nbsp;</td>
			<td width="20" class="oospa_edit_icon" onclick="throwModifyGroupModal(GROUP_ID);">&nbsp;</td>
		</tr>
		<tr>
			<td colspan="4"><asp:Label ID="_approverGroupDescription" runat="server">[GROUP DESCRIPTION]</asp:Label></td>
		</tr>
		<asp:Repeater ID="_workflowApproverMembers" runat="server">
			<ItemTemplate>
				<tr>
					<td>&nbsp;</td>
					<td colspan="3">
						<asp:RadioButton ID="_selectMember" runat="server" GroupName="ASSIGNED_PROGRAMATICALLY" Checked="true" CssClass="csm_input_checkradio" />
						<asp:Label ID="_memberFullName" runat="server"></asp:Label>
						
						<!--<input type="radio" name="radio1" checked="checked" class="csm_input_checkradio" />
						<span class="csm_inline">Chris Schwimmer (Default)</span>-->
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>
	<hr />
	<div class="csm_input_buttons_container">
		<input type="button" value="Add New Team Group" class="csm_html_button" onclick="throwModifyGroupModal();"/>
	</div>												        
</td>