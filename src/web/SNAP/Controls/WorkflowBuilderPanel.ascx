<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowBuilderPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowBuilderPanel" %>

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
		    <asp:PlaceHolder ID="_requiredApprover" runat="server" />
	    </tr>
	    <tr>
	        <td class="csm_input_form_label_column">
                <label>Team Approver</label>
            </td>
		    <asp:PlaceHolder ID="_teamApprover" runat="server" />
	    </tr>
	    <tr>
	        <td class="csm_input_form_label_column">
                <label>Technical Approver</label>
            </td>
		    <asp:PlaceHolder ID="_technicalApprover" runat="server" />																				
    </table>

    <div class="csm_input_buttons_container">
	    <input type="button" value="Pause" class="csm_html_button"/>
	    <input type="button" value="Build It | Continue" class="csm_html_button"/>
    </div>			

    </fieldset>		        

    </div>