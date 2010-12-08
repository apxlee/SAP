<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.Search" %>
<asp:Content ID="_headerContainer" ContentPlaceHolderID="_headPlaceHolder" runat="server"></asp:Content>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">

<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>SearchView.js" type="text/javascript"></script>
<div class="csm_container_center_700">
	<div class="csm_content_container">	
		<div class="csm_text_container csm_bottom5">
			<p>Search accepts <strong>User ID</strong> for Requestor, or <strong>User ID</strong> and <strong>Display Name</strong> for the Affected End User (AEU), as well as <strong>Request ID</strong> as search criteria.
			<p><strong>Advanced Search</strong> supports both Content Search (for Access Details and Justification) and Date Range.</p>
			<p>Searches for requests created within the Sharepoint environment need to be made through filtering 
			from the legacy <a href="http://its/dcs/aim/pa/Lists/Paper3/Robust%20View.aspx">Sharepoint Privileged Access</a> site.</p>
		</div>
		<table border="0" cellpadding="0" cellspacing="0" style="width:678px;" class="csm_input_form_container">
				<tr>
					<td style="width:180px;" class="csm_input_form_label_column csm_input_required_field">
						<label>Search Criteria:</label>
						<div style="text-align:right;padding-top:5px;"><span class="csm_search_toggle" id="__advacnedSearchToggle" onclick="ToggleSearch();">Show Advanced Search</span></div>
					</td>
					<td style="width:498px;" class="csm_input_form_control_column">
						<input type="text" id="__searchInput" class="csm_text_input_short" />
						<p><em>Enter: Requestor, Affected End User, or Request ID</em></p>
					</td>
				</tr>
				<tr>
				    <td  id="__advancedSearchContainer" colspan="2" class="csm_input_spanning_column" style="width:678px;display:none;">
				        <div class="csm_input_form_label_container">
				            <label>Advanced Search</label>
				            <div><p>You may enter any combination of Search Criteria to fully refine your search to yield the most applicable </p><p>search results.</p></div>
				        </div>
				        <div class="csm_input_form_control_container">
				            <div>
				                <label>Contents contain the word(s):</label>
				            </div>
	                        <div style="text-align:center;padding-top:5px;">
	                            <textarea id="__searchContents" cols="100" class="csm_textarea_short"></textarea>
	                        </div>
	                        <div>
	                            <label>Date Range for Initial Request:</label>
	                        </div>	
                            <div style="text-align:left;padding-left:40px;padding-top:5px;border-top:1px solid #b7b7b7;">
                                <table border="0" cellpadding="0" cellspacing="0" style="width:auto;">
                                    <tr>
                                        <td style="text-align:right;width:140px;display: block;margin: 0;padding: 2px;font-size: .85em;line-height: 1.5em;font-weight: bold;color: #595959;">Choose Start Date:</td>
                                        <td style="width:16px;vertical-align:middle;"><input type="hidden" id="startDatepicker" /></td>
                                        <td style="width:15px;">&nbsp;</td>
                                        <td style="width:140px;text-align:center;border:1px solid #b7b7b7; background-color:#FFF;" id="__startDate"></td>
                                    </tr>
                                    <tr><td style="height:5px;" colspan="4">&nbsp;</td></tr>
                                    <tr>
                                        <td style="text-align:right;width:140px;display: block;margin: 0;padding: 2px;font-size: .85em;line-height: 1.5em;font-weight: bold;color: #595959;">Choose End Date:</td>
                                        <td style="width:16px;vertical-align:middle;"><input type="hidden" id="endDatepicker" /></td>
                                        <td style="width:15px;">&nbsp;</td>
                                        <td style="width:140px;text-align:center;border:1px solid #b7b7b7; background-color:#FFF;" id="__endDate"></td>
                                    </tr>
                                </table>
                            </div>
		                </div>
				    </td>
				</tr>
		</table>
		<div class="csm_input_buttons_container" style="margin-right:6px;">
		    <input type="button" id="__clearSearch" value="Clear" class="csm_html_button" onclick="Clear();" />
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
