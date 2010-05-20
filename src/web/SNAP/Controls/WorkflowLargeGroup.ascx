<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowLargeGroup.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowLargeGroup" %>
<tr>
	<td class="csm_input_form_label_column">
		<asp:PlaceHolder ID="_largeGroupSectionName" runat="server" />
	</td>
	<td class="csm_input_form_control_column">
		<table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row">
			<tr>
		        <td class="csm_workflow_builder_control_column">
		            <asp:DropDownList ID="_dropdownActors" AutoPostBack="false" runat="server">
		                <asp:ListItem Selected="True" Value="0">Please select...</asp:ListItem>
		            </asp:DropDownList>
		            <br />
	                <input type="text" id="_actorDisplayName" runat="server" class="csm_text_input_short" />
	                <input type="hidden" id="_actorUserId" style="display:none;" runat="server"/>
	                <input type="hidden" id="_actorActorId" style="display:none;" runat="server"/>
	                <input type="hidden" id="_actorGroupId" style="display:none;" runat="server"/>
	                <input type='button' id='_checkActor' class="csm_html_button" value="Check" runat="server">
	                <input type='button' id='_addActor' class="csm_html_button" value="Add" runat="server">
		        </td>
			</tr>
			<tr>
			    <td class="csm_workflow_builder_control_column">
                    <asp:PlaceHolder ID="_largeGroupSelectedList" runat="server" />
			    </td>
			</tr>
		</table>							
	</td>
</tr>