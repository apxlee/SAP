<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowBuilderPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowBuilderPanel" %>
<!-- WORKFLOW BUILDER SECTION -->
<div class="csm_text_container csm_top5">

    <div class="csm_icon_heading_container oospa_aim_builder">
        <h2>Access & Identity Management - Workflow Builder</h2>
        <p class="">Please review the access details above and select an appropriate action below.</p>
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
						<td width="22"><input type="checkbox" id="_requiredCheck" checked="checked" disabled="disabled" class="csm_input_checkradio" /></td>
						<td width="386">
						    <asp:PlaceHolder ID="_managerInfoSection" runat="server" />		        
				        </td>
				        <td width="386" style="display:none;" class="csm_input_form_control_column">
				            <asp:TextBox ID="_managerName" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
			                <button type='button' id='_checkManagerName' class="csm_html_button">Check</button>
				        </td>
				        <td>&nbsp</td>
						<td width="20" class="">&nbsp;</td>
						<td width="20" class="oospa_edit_icon_disabled">&nbsp;</td>
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