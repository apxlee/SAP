<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowApprover.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowApprover" %>
<tr>
    <td class="csm_input_form_label_column">
	    <asp:PlaceHolder ID="_approverSectionName" runat="server" />
    </td>
    <td class="csm_input_form_control_column">
	    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row">
		    <tr>
			    <td width="22"><asp:PlaceHolder ID="_approverCheckBoxSection" runat="server" /></td>
			    <td width="386"><asp:Label ID="_approverGroupName" runat="server" CssClass="csm_inline">[SOFTWARE GROUP]</asp:Label></td>
			    <td style="display:none;" width="20" class="oospa_delete_icon" onclick="alert('Delete Group');">&nbsp;</td>
			    <td style="display:none;" width="20" class="oospa_edit_icon" onclick="alert('Edit Group');">&nbsp;</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:Label ID="_approverGroupDescription" class="group_description" runat="server">[GROUP DESCRIPTION]</asp:Label></td>
		    </tr>
    		
		    <asp:PlaceHolder ID="_approverGroupMemebers" runat="server" />
    		
	    </table>
	    <hr />
	    <div class="csm_input_buttons_container" style="display:none;">
		    <input type="button" value="Add New Approver Group" class="csm_html_button" onclick="alert('Create Group');"/>
	    </div>											        
    </td>
</tr>