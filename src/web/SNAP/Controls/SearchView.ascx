<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.SearchView" %>
<div class="csm_container_center_700">
	<div class="csm_content_container">	
		<div class="csm_text_container csm_bottom5">
			<p>TODO: Search verbiage</p>
		</div>
		<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
				<tr>
					<td class="csm_input_form_label_column csm_input_required_field">
						<label>User ID, User Name, or Request ID</label>
					</td>
					<td class="csm_input_form_control_column">
						<asp:TextBox ID="_searchInput" runat="server" CssClass="csm_text_input_short" ></asp:TextBox>
					</td>
				</tr>
		</table>
		<div class="csm_input_buttons_container" style="margin-right:6px;">
			<asp:Button ID="_searchButton" Text="Search" CssClass="csm_html_button" OnClientClick="return validateInput(this);" OnClick="Search_Click" runat="server" />
		</div>
	</div>
		<h1>Search Results</h1>
		<asp:PlaceHolder ID="_searchResultsContainer" runat="server"></asp:PlaceHolder>
		<asp:Panel ID="_nullDataMessage_SearchRequests" runat="server" CssClass="csm_content_container" Visible="false">
			<div class="csm_text_container">
				<p>There are no Search Requests at this time.</p>
			</div>
		</asp:Panel>
	
</div>
