﻿<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.Search" %>
<asp:Content ID="_headerContainer" ContentPlaceHolderID="_headPlaceHolder" runat="server"></asp:Content>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">

<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>SearchView.js" type="text/javascript"></script>
<div class="csm_container_center_700">
	<div class="csm_content_container">	
		<div class="csm_text_container csm_bottom5">
			<p>Search accepts <strong>User ID</strong>, <strong>Username</strong> and <strong>Request ID</strong> as search criteria. 
			The results only return the AFFECTED END USER (or REQUEST ID) matching the criteria for requests created in this application.</p>
			<p>Searches for requests created within the Sharepoint environment need to be made through filtering 
			from the legacy <a href="http://its/dcs/aim/pa/Lists/Paper3/Robust%20View.aspx">Sharepoint Privileged Access</a> site.</p>
		</div>
		<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
				<tr>
					<td class="csm_input_form_label_column csm_input_required_field">
						<label>Search Criteria:</label>
					</td>
					<td class="csm_input_form_control_column">
						<input type="text" id="__searchInput" class="csm_text_input_short" />
						<input type="text" style="visibility:hidden" />
						<p><em>Enter: User ID, Username, or Request ID</em></p>
					</td>
				</tr>
		</table>
		<div class="csm_input_buttons_container" style="margin-right:6px;">
			<input type="submit" id="__searchButton" value="Search" class="csm_html_button" onclick="ValidateInput(); return false;" />
		</div>
	</div>
	<h1>Search Results</h1>
	<div id="_searchResultsContainer"></div>
</div>

<script type="text/javascript">
    try { pageTracker._trackPageview("SearchView"); }
    catch (err) { }
</script>

</asp:Content>
