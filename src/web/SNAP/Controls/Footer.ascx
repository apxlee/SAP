<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.Footer" %>
<!-- BEGIN FOOTER -->
<div class="csm_container_100 csm_footer_float">
	<div class="csm_footer">
		<div class="csm_container_16">
			<div class="snap_container_center_788 csm_footer_logo">
				<div class="csm_alpha csm_grid_10 csm_top10 ">
					<!-- TODO: future iteration -->
					<div class="csm_grid_3">
						<h1>Application Links</h1>
						<asp:PlaceHolder ID="_applicationLinksContainer" runat="server"></asp:PlaceHolder>
					</div>
					<div class="csm_grid_3">
						<h1>Feedback</h1>
						<ul>
							<li><a href="mailto:DG C-APO-Corporate-ITS Access Management?subject=SAP%20Contact%20Team">Contact Team</a></li>
							<li><a href="mailto:DG C-APO-Corporate-ITS Access Management SNAPFU?subject=SAP%20Comments%20or%20Problem">Comments or Report Problem</a></li>
							<li><a href="http://www.google.com/analytics">Statistics</a></li>
						</ul>
					</div>
					<div class="csm_clear">&nbsp;</div>
				</div>
				<div class="snap_copyright">
					<span>&copy;2010 Apollo Group, Inc.&nbsp;|&nbsp;Access & Identity Management
					<br />Supplemental Access Process&nbsp;<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.AppVersion %>
					&nbsp;on&nbsp;<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.CurrentServer %>
					<span id="aim_IE6_nag"><br /><br />We've noticed you're using Internet Explorer 6.  Consider upgrading to one of<br />these modern browsers:&nbsp;<a href="http://www.microsoft.com/windows/internet-explorer/worldwide-sites.aspx">Internet Explorer 7+</a> | <a href="http://www.google.com/chrome">Chrome</a> | <a href="http://www.mozilla.com/en-US/firefox/firefox.html">Firefox</a> | <a href="http://www.apple.com/safari/download/">Safari</a></span></span>
				</div>
				<div class="csm_clear">&nbsp;</div>									
			</div>			
		</div>
	</div>
<div class="csm_clear">&nbsp;</div>
</div>
<!-- END FOOTER -->