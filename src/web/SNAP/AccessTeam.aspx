﻿<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="AccessTeam.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.AccessTeam" %>
<asp:Content ID="_headerContainer" ContentPlaceHolderID="_headPlaceHolder" runat="server"></asp:Content>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">

<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>AccessTeamView.js" type="text/javascript"></script>

<div class="csm_container_center_700">
	<h1 style="margin-bottom:2px;">Open Requests</h1>
	
	<div id="access_filter_container" class="filter_view_all">
		<div id="access_filter_outer">
			<ul id="access_filter_inner">
				<li style="width:57px;"><span>Pending:</span></li>
				<li id="filter_pending_acknowledgement" class="active_carrot" snap="Pending Acknowledgement"><span>Acknowledgement (<span class="inline_counter" id="filter_pending_acknowledgement_count">00</span>)</span></li>
				<li id="filter_pending_workflow" class="active_carrot" snap="Pending Workflow"><span>Workflow (<span class="inline_counter"  id="filter_pending_workflow_count">00</span>)</span></li>
				<li id="filter_pending_provisioning" class="active_carrot" snap="Pending Provisioning"><span>Provisioning (<span class="inline_counter"  id="filter_pending_provisioning_count">00</span>)</span></li>
				<li style="width:4px;padding-left:2px;margin-right:4px;font-weight:bold;"><span>|</span></li>
				<li id="filter_in_workflow" class="active_carrot" snap="In Workflow"><span>In Workflow (<span class="inline_counter"  id="filter_in_workflow_count">00</span>)</span></li>
				<li style="width:4px;padding-left:2px;margin-right:4px;font-weight:bold;"><span>|</span></li>
				<li id="filter_view_all" onmouseover="FilterHover(this);" onmouseout="FilterHover(this);" onclick="FilterClick(this);" class="active_carrot" snap="All"><span>View All</span></li>
			</ul>
		</div>
	</div>	
	<div id="_openRequestsContainer"></div>	
	<h1>Closed Requests</h1>
	<div id="_closedRequestsContainer"></div>	
</div>

<script type="text/javascript">

    try { GetRequests(ViewIndexEnum.Access_Team, ""); }
    catch (err) { }

	try { pageTracker._trackPageview("AccessTeamView"); }
	catch (err) {}

</script>

</asp:Content>
