<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MasterRequestBlade.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.MasterRequestBlade" %>
	<!-- BEGIN REQUEST BLADE ------------------------------------------------------------------------------->
	<div class="csm_content_container">
		<!-- BEGIN FLOATING HEADING -->
		<div class="csm_divided_heading">
			<div class="csm_float_left">
			    <table border="0" cellpadding="0" cellspacing="0" style="margin-left:3px;">
			        <tr class="csm_stacked_heading_title">
			            <td style="width:200px;">AFFECTED END USER</td>
			            <td style="width:170px;">STATUS</td>
			            <td style="width:110px;">LAST MODIFIED</td>
			            <td style="width:115px;">REQUEST ID</td>
			        </tr>
			        <tr class="csm_stacked_heading_label">
			            <td><asp:Label ID="_affectedEndUserName" runat="server"></asp:Label></td>
			            <td><asp:Label ID="_overallRequestStatus" runat="server"></asp:Label></td>
			            <td><asp:Label ID="_lastUpdatedDate" runat="server"></asp:Label></td>
			            <td><asp:Label ID="_requestId" runat="server"></asp:Label></td>
			        </tr>
			    </table>
			</div>
			<div class="csm_float_right">
				<div class="csm_toggle_container" onclick="csmToggle(this, 'ppnn');">
					<span>Toggle</span>
					<asp:Panel ID="_toggleIconContainer" runat="server" CssClass="csm_toggle_icon_up"></asp:Panel>
				</div>
			</div>
		</div>
		<div class="csm_clear">&nbsp;</div>
		<!-- END FLOATING HEADING -->
		
		<!-- TOGGLED CONTENT AREA -->
		<asp:Panel ID="_toggledContentContainer" runat="server" CssClass="csm_displayed_block">
		    
		    <!-- REQUEST DETAILS SECTION -->
		    <asp:PlaceHolder ID="_readOnlyRequestPanelContainer" runat="server"></asp:PlaceHolder>
		    
		    <!-- MANAGER APPROVAL SECTION -->
		    <asp:PlaceHolder ID="_approvingManagerPanelContainer" runat="server"></asp:PlaceHolder>
		    
		    <!-- ACCESS TEAM SECTION -->
		    <asp:PlaceHolder ID="_accessTeamPanelContainer" runat="server"></asp:PlaceHolder>

            <!-- REQUEST TRACKING SECTION -->
            <asp:PlaceHolder ID="_requestTrackingPanelContainer" runat="server"></asp:PlaceHolder>
	      
		</asp:Panel>
	</div>
	<div class="csm_clear">&nbsp;</div>
	<!-- END REQUEST BLADE ------------------------------------------------------------------------------->
	