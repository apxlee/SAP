﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.Footer" %>
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
							<li><a href="mailto:DG C-APO-Corporate-IT-Access%20%26%20Identity%20Management%20SNAPFU?subject=SNAP%20Comments">Comments</a></li>
							<li><a href="mailto:DG C-APO-Corporate-IT-Access%20%26%20Identity%20Management%20SNAPFU?subject=SNAP%20Problem">Report Problem</a></li>
							<li><a href="http://www.google.com/analytics">Statistics</a></li>
						</ul>
					</div>
					<div class="csm_clear">&nbsp;</div>
				</div>
				<div class="snap_copyright">
					<span>&copy;2010 Apollo Group, Inc.&nbsp;|&nbsp;Access & Identity Management
					<br />Supplemental Access Process&nbsp;<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.AppVersion %>
					&nbsp;on&nbsp;<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.CurrentServer %></span>
				</div>
				<div class="csm_clear">&nbsp;</div>									
			</div>			
		</div>
	</div>
<div class="csm_clear">&nbsp;</div>
</div>
<!-- END FOOTER -->