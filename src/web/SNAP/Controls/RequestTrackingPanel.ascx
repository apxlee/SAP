<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestTrackingPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.RequestTrackingPanel" %>
<div class="csm_text_container csm_top5">
    
    <div class="csm_icon_heading_container oospa_request_status">
        <h2>Request Tracking</h2>
    </div>

    <p class="csm_prefix_bump3">This section tracks the status of your request and includes all the people and groups 
    that are involved in the approval process.  Status "Inactive" indicates the person or team will be notified after prerequisite approvals
    have been made.  Due Dates do not include weekends or holidays.</p>
    <table class="csm_top15" border="0" cellpadding="0" cellspacing="0" style="margin-left:12px;"><!-- width 654 -->
        <tr class="csm_stacked_heading_title">
            <td style="width:248px;">TEAM / APPROVER</td>
            <td style="width:148px;">STATUS</td>
            <td style="width:120px;">DUE DATE</td>
            <td style="width:120px;">COMPLETED DATE</td>
        </tr>
    </table>
    <asp:PlaceHolder ID="_workflowBladeContainer" runat="server"></asp:PlaceHolder>
</div>