﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReadOnlyRequestPanel.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.ReadOnlyRequestPanel" %>
<div class="csm_text_container csm_top5">
	<div class="csm_icon_heading_container border oospa_request_details">
		<h2>Request Details</h2>
		<p class="">This section contains the detailed description of the access being requested, including the 
		justification for the request.  Please review this section for accuracy.</p><!-- csm_prefix_bump3 -->
	</div>
	
	<!-- TODO: style this table with Css -->              
	<table style="font-size:1em;margin-left:10px;" cellpadding="0" cellspacing="0" width="100%">
		<!-- TODO: add static rows for title/mgr/requestor to style easier? -->
		<tr style="line-height:1em;">
			<td style="text-align:right;width:140px;padding-right:4px;"><p>Title&#58;</p></td>
			<td style="padding:4px;padding-right:10px;">
				<p><asp:Label ID="_affectedEndUserTitle" runat="server"></asp:Label></p>
			</td>
		</tr>
		<tr style="line-height:1em;">
			<td style="text-align:right;width:140px;padding-right:4px;"><p>Manager Name&#58;</p></td>
			<td style="padding:4px;padding-right:10px;">
				<p><asp:Label ID="_managerName" runat="server"></asp:Label>&nbsp;&nbsp;<asp:Label ID="_adManagerName" runat="server"></asp:Label></p>
			</td>
		</tr>
		<tr style="line-height:1em;">
			<td style="text-align:right;width:140px;padding-right:4px;"><p>Requestor&#58;</p></td>
			<td style="padding:4px;padding-right:10px;">
				<p><asp:Label ID="_requestorName" runat="server"></asp:Label></p>
			</td>
		</tr>
		<asp:Repeater ID="_readOnlyRequestDetails" runat="server">
			<ItemTemplate>
				<tr style="line-height:1em;">
					<td style="text-align:right;width:140px;padding-right:4px;"><p><%# Eval("label") %>&#58;</p></td>
					<td style="padding:4px;padding-right:10px;"><p><%# Eval("value") %></p></td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<asp:PlaceHolder ID="_accessNotesContainer" runat="server" Visible="false">
			<tr style="line-height:1.2em;color:Red;">
				<td style="text-align:right;width:140px;padding-right:4px;"><p>Access Notes&#58;</p></td>
				<td style="padding:4px;padding-right:10px;"><asp:Literal ID="_accessNotes" runat="server"></asp:Literal></td>
			</tr>			
		</asp:PlaceHolder>
	</table>
</div>