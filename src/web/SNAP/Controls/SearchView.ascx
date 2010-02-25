<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.SearchView" %>
<div class="csm_container_center_700">
	
	<h1>Search Panel</h1>
	
	<h1>Search Results</h1>
	<asp:PlaceHolder ID="_searchResultsContainer" runat="server"></asp:PlaceHolder>
	<asp:Panel ID="_nullDataMessage" runat="server" CssClass="csm_content_container" Visible="true">
		<div class="csm_text_container">
			<p>The search criteria specified did not return any results.</p>
		</div>
	</asp:Panel>

</div>