<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowBlade.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowBlade" %>
<div class="csm_padded_windowshade">
	<asp:Panel ID="_workflowBladeData" runat="server" CssClass="csm_data_row <%=AlternatingCss %>">
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td style="width:230px;"><%=ActorName %></td>
                <td style="width:170px;"><%=Status %></td>
                <td style="width:120px;"><%=DueDate %></td>
                <td style="width:120px;"><%=CompletedDate %></td>
            </tr>
        </table>
     </asp:Panel>
     <asp:Panel ID="_workflowBladeCommentsContainer" runat="server" CssClass="csm_text_container_nodrop" Visible="false">
		<!--
		Replace with Literal Control(s) conforming to the following formatting:
		<p><u>%Action% by %WorkflowActorName% on %LongDate%</u><br />%Comments%</p>
		-->
     </asp:Panel>
</div>
<div class="csm_clear">&nbsp;</div>