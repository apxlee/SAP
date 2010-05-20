﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchView.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.SearchView" %>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>SearchView.js" type="text/javascript"></script>
<div class="csm_container_center_700">
	<div class="csm_content_container">	
		<div class="csm_text_container csm_bottom5">
			<p>Search accepts <strong>User ID</strong>, <strong>Username</strong> and <strong>Request ID</strong> as search criteria. 
			The results only return the AFFECTED END USER (or REQUEST ID) matching the criteria for requests created in this application.</p>
			<p>Searches for requests created within the Sharepoint environment need to be made through filtering 
			from the legacy <a href="http://apolloiandt/SiteDirectory/infraops/ITOC/Lists/Paper3/Robust%20View.aspx">Sharepoint Privileged Access</a> site.</p>
		</div>
		<table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container">
				<tr>
					<td class="csm_input_form_label_column csm_input_required_field">
						<label>Search Criteria:</label>
					</td>
					<td class="csm_input_form_control_column">
						<input type="text" id="_searchInput" runat="server" class="csm_text_input_short" />
						<p><em>Enter: User ID, Username, or Request ID</em></p>
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
			<p>There are no Search Results.</p>
		</div>
	</asp:Panel>
	
	<div id="_actionMessageDiv" style="display:none;">
        <div class="messageBox"> 
            <h2>header</h2>
            <div id="_indicatorDiv" style="display:none;text-align:center;padding-top:15px;">
                <img alt="creating ticket..." src="images/ajax_indicator.gif" width="16" height="16" />
            </div>
            <p>message</p>
            <div id="_closeMessageDiv" style="display:none;">
                <input type="button" value="Close" onclick="$('#_actionMessageDiv').hide();$('#_closeMessageDiv').hide();" />
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
	try { pageTracker._trackPageview("SearchView"); }
	catch (err) {}
</script>
