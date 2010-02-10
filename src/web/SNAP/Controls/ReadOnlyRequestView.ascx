<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReadOnlyRequestView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.ReadOnlyRequestView" %>
<div class="csm_text_container csm_top5">
	<div class="csm_icon_heading_container oospa_request_details">
		<h2>Request Details</h2>
		<asp:Label ID="jtest" runat="server">BLANK</asp:Label>
	</div>
	<!-- TODO: style this table with Css -->              
	<table style="font-size:.85em;margin-left:10px;" cellpadding="0" cellspacing="0" width="100%">
		<asp:Repeater ID="_readOnlyRequestDetails" runat="server">
			<ItemTemplate>
				<tr>
					<td align="right">[%REPLACE WITH LABEL%]:&nbsp;&nbsp;</td>
					<td>[%REPLACE WITH TEXT%]</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>
</div>