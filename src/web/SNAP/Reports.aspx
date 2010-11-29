﻿<%@ Page Title="" Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.Reports" %>
<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>Reports.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>MasterRequestBlade.js" type="text/javascript"></script>
	<div class="csm_container_center_700">
        <div class="csm_content_container">
            <div class="csm_text_container">
		        <p>The Supplemental Access Process application augments the rights being 
			    provisioned through<br /> the <a href="https://access-prod.apollogrp.edu/cap/" style="color:Blue;">Computer Access Process (CAP)</a>, 
			    which handles Production systems.<br /><br />A Service Desk ticket will be created once the approvals are received from the
			    Supplemental Access Process application.  Once the ticket is assigned, the goal is 2 to 3 business days for provisioning.<br /><br />
			    Please fill out the form below and follow the prescribed formats when applicable.  Examples have been provided to assist in your request.
			    </p>
		    </div>    
		    <br />
		    <h1>Bottleneck Dashboard</h1>        
            <div id="current_report">
                <div style="padding-top:3px;padding-right:10px;">
                    <div style="float:right;padding-left:10px;" >
                        <a href="#" id="_emailReport" style="display:none;" onclick="EmailReport(this);">
                            <img src="images/icon_email_zoom.jpg" />
                        </a>
                    </div>
                    <div style="float:right;" >
                        <a href="#" id="_zoomReport" style="display:none;" class="enlarge">
                            <img src="images/icon_glass_zoom.jpg" />
                        </a> 
                    </div>
                </div>
            </div>
            <br />
            <h1>Bottleneck Details Report (Data + Graphs)</h1>
            <div id="_reportHistoryContainer" class="csm_padded_windowshade"></div>     	
		</div>
	</div>
<script type="text/javascript">
    try { GetReportListItems(); }
    catch (err) { }

    try { pageTracker._trackPageview("Reports"); }
    catch (err) {}
</script>
</asp:Content>
