<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowBuilderPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowBuilderPanel" %>

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
						<td width="386">
			<%--			    <asp:Label ID="_requiredApproverFullName" runat="server" CssClass="csm_inline"></asp:Label>
						    <asp:Label ID="_requiredApproverUserId" Visible="false" runat="server" CssClass="csm_inline"></asp:Label>--%>
						    <asp:PlaceHolder ID="_managerInfoSection" runat="server" />
						</td>
						<td width="20" class="">&nbsp;</td>
						<td width="20" class="oospa_edit_icon" onclick="">&nbsp;</td><!-- TODO: onclick trigger ajax? -->
					</tr>
				</table>							
			</td>
		</tr>
		
		<!-- APPROVERS SECTION -->
		<asp:PlaceHolder ID="_dynamicApproversContainer" runat="server"></asp:PlaceHolder>
																						
	</table>
	
	<div class="csm_input_buttons_container">
	    <asp:PlaceHolder ID="_dynamicButtonsContainer" runat="server"></asp:PlaceHolder>
	</div>			
	
	</fieldset>		        
    
</div>	