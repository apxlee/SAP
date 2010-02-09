<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowBlade.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.WorkflowBlade" %>
<div class="csm_padded_windowshade">
	<asp:Panel ID="_workflowBladeContainer" runat="server" CssClass="csm_data_row csm_alternating_bg"><!-- TODO: replace alternating_bg style as needed -->
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td style="width:250px;"><asp:Label ID="_workflowActor" runat="server">Access & Identity Management</asp:Label></td>
                <td style="width:150px;"><asp:Label ID="_workflowStatus" runat="server"> Acknowledged</asp:Label></td>
                <td style="width:120px;"><asp:Label ID="_workflowDueDate" runat="server">Jan. 13, 2010</asp:Label> </td>
                <td style="width:120px;"><asp:Label ID="_workflowCompletedDate" runat="server"></asp:Label></td>
            </tr>
        </table>
     </asp:Panel>
</div>
<div class="csm_clear">&nbsp;</div>