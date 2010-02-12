<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowApprover.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowApprover" %>
<table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row">
    <tr>
        <td width="22"><input type="checkbox" id="_approverCheck" runat="server" class="csm_input_checkradio" /></td>
        <td width="386"><span class="csm_inline" ID="_approverTitle" runat="server" /></td>
        <td width="20" class="oospa_delete_icon" id="_deleteApprover" runat="server">&nbsp;</td>
        <td width="20" class="oospa_edit_icon" id="_editApprover" runat="server">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="4"><span class="group_description" ID="_approverDescription" runat="server" /></td>
    </tr>
	
   <asp:PlaceHolder ID="_defaultApprover" runat="server" />

</table>							

<%--<td class="csm_input_form_label_column">
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
</td>--%>