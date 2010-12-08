<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="MyRequests.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.MyRequests" %>
<asp:Content ID="_headerContainer" ContentPlaceHolderID="_headPlaceHolder" runat="server"></asp:Content>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">

<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MyRequestsView.js" type="text/javascript"></script>

<div class="csm_container_center_700">
	<h1>Open Requests</h1>
	<div id="_openRequestsContainer"></div>
	<h1>Closed Requests</h1>
	<div id="_closedRequestsContainer"></div>
</div>

<script type="text/javascript">
    try { GetRequests(ViewIndexEnum.My_Requests, null); }
    catch (err) {}
    
	try { pageTracker._trackPageview("UserView"); }
	catch (err) {}
</script>

</asp:Content>