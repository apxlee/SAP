<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MasterRequestBlade.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.MasterRequestBlade" %>
	<!-- BEGIN REQUEST ROW -->
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
			            <td>Chris Schwimmer</td>
			            <td>Open</td>
			            <td>Jan. 12, 2010</td>
			            <td>1015</td>
			        </tr>
			    </table>
			</div>
			<div class="csm_float_right">
				<!-- add additional text/icons here using divs and spans, requires custom Css (remember float:right rules!) -->
				<div class="csm_toggle_container" onclick="csmToggle(this, 'ppnn');">
					<span>Toggle</span>
					<div class="csm_toggle_icon_up"></div>
				</div>
			</div>
		</div>
		<div class="csm_clear">&nbsp;</div>
		<!-- END FLOATING HEADING -->
		
		<!-- TOGGLED CONTENT AREA -->
		<div style="display:block;">
		    
		    <!-- REQUEST DETAILS SECTION -->
		    <asp:PlaceHolder ID="_readOnlyRequestView" runat="server"></asp:PlaceHolder>

            <!-- REQUEST TRACKING SECTION -->
            <asp:PlaceHolder ID="_requestTrackingView" runat="server"></asp:PlaceHolder>
	      
		</div>
	</div>
	<div class="csm_clear">&nbsp;</div>
	<!-- END REQUEST ROW ------------------------------------------------------------------------------->
	