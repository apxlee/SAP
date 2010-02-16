<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowBlade.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowBlade" %>
<div class="csm_padded_windowshade">
	<asp:Panel ID="_workflowBladeContainer" runat="server" CssClass="csm_data_row">
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td style="width:250px;"><asp:Label ID="_workflowActorName" runat="server"></asp:Label></td>
                <td style="width:150px;"><asp:Label ID="_workflowStatus" runat="server"></asp:Label></td>
                <td style="width:120px;"><asp:Label ID="_workflowDueDate" runat="server"></asp:Label> </td>
                <td style="width:120px;"><asp:Label ID="_workflowCompletedDate" runat="server"></asp:Label></td>
            </tr>
        </table>
     </asp:Panel>
     <asp:Panel ID="_workflowBladeCommentsContainer" runat="server" CssClass="csm_text_container_nodrop">
		<!--
		Replace with Literal Control(s) conforming to the following formatting:
		
		<p><u>%Action% by %WorkflowActorName%</u><br />Due Date:&nbsp;%LongDate%&nbsp;|&nbsp;Completed Date:&nbsp;%LongDate%</p>
		
		-OR-
		
		<p><u>%Action% by %WorkflowActorName% on %LongDate%</u><br />%Comments%</p>
		
		-OR-
		
		<p><u>%LongDate%</u><br />%Comments%</p>
		-->
     </asp:Panel>
</div>
<div class="csm_clear">&nbsp;</div>