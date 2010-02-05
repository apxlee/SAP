<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SNAPFooter.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.SNAPFooter" %>
<!-- BEGIN FOOTER -->
<div class="csm_container_16 csm_footer_float">
	<div class="csm_footer">
		<div class="csm_grid_3 csm_suffix_1 csm_footer_logo"></div>
		<div class="csm_grid_4">
			<h1 id="_footerLinkHeading" runat="server">Application Links</h1>
			
			<asp:Repeater ID="_footerLinksRepeater" DataMember="Item" runat="server">
				<HeaderTemplate>
					<ul>
				</HeaderTemplate>
				<ItemTemplate>
					<li><asp:LinkButton ID="_footerLinkItem" runat="server" OnCommand="_footerLinkItem_OnClick" 
						CommandArgument='<%# Eval("viewId") %>' Text='<%# Eval("text") %>' /></li>
				</ItemTemplate>
				<FooterTemplate>
					</ul>
				</FooterTemplate>
			</asp:Repeater>

		</div>
		<div class="csm_prefix_1 csm_grid_3">
			<h1>Feedback</h1>
			<ul>
				<li><a href="#">Comments</a></li>
				<li><a href="#">Report Problem</a></li>
				<li><a href="#">Statistics</a></li>
			</ul>
		</div>
	</div>
</div>
<!-- END FOOTER -->