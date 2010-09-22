<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrackingBladeTest.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.JONTEST.TrackingBladeTest" %>
<%@ Register src="~/Controls/DefaultIncludes.ascx" tagname="DefaultIncludes" tagprefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Tracking Test</title>
	<link href="styles/Csm_Reset.css" rel="stylesheet" type="text/css" />
	<link href="styles/Csm_Grid976.css" rel="stylesheet" type="text/css" />
	<link href="styles/SNAP.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

// Apollo.AIM.SNAP.Model.Enumeration
//
var ActorGroupTypeEnum =
{
	Team_Approver: 0,
	Technical_Approver: 1,
	Manager: 2,
	Workflow_Admin: 3	
};


function LoadTracking(requestId) 
{
    var postData = "{\"requestId\":\"" + requestId + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "TrackingAjax.aspx/GetAllTrackingData",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d.length > 0) {
                BuildTrackingSection(msg.d);
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetRequests Error: " + XMLHttpRequest);
            alert("GetRequests Error: " + textStatus);
            alert("GetRequests Error: " + errorThrown);
        }
    });
}

function BuildTrackingSection(trackingObject) 
{
	var trackingSectionHtml;

	// Build sections in the following order and loop thru them
	//	
	var headingOrder = [
		ActorGroupTypeEnum.Workflow_Admin, 
		ActorGroupTypeEnum.Manager, 
		ActorGroupTypeEnum.Team_Approver,
		ActorGroupTypeEnum.Technical_Approver
		];
	
	for (var i = 0; i < headingOrder.length; i++)
	{
		// If the trackingObject has at least one row that matches the heading, then draw it
		//
		if (IsActorGroupInTrackingData(trackingObject, headingOrder[i]))
		{
			trackingSectionHtml += CreateTrackingHeader(headingOrder[i]);
		    
			$.each(trackingObject, function(index, value) {
				var data = jQuery.parseJSON(value);
				if (data.ActorGroupType == headingOrder[i])
				{
					trackingSectionHtml += CreateTrackingBlade(data);
				}
			});
		}
    }
     
    $(trackingSectionHtml).insertAfter($("#_trackingBladeContainer"));
}

function IsActorGroupInTrackingData(trackingObject, actorGroupEnum)
{
	var isInGroup = false;
	
	$.each(trackingObject, function(index, value) {
		var data = jQuery.parseJSON(value);
		if (data.ActorGroupType == actorGroupEnum)
		{
			isInGroup = true;
		}
	});
	
	return isInGroup;
}

function CreateTrackingBlade(data) 
{
        var newTrackingBlade = $("#_trackingBladeTemplate").html();
        
        newTrackingBlade = newTrackingBlade
			.replace("%%ActorName%%", data.ActorName)
			.replace("%%Status%%", data.WorkflowStatus)
			.replace("%%DueDate%%", data.DueDate)
			.replace("%%CompletedDate%%", data.CompletedDate)
			.replace("%%GroupTypeId%%", data.ActorGroupType)
			.replace("%%WorkflowComments%%", data.WorkflowComments)
        
		return newTrackingBlade;
}

function CreateTrackingHeader(actorGroupType) 
{
	var heading;
	
	switch (actorGroupType) {
		case ActorGroupTypeEnum.Team_Approver:
			heading = "Team Approvers";
			break;
			
		case ActorGroupTypeEnum.Technical_Approver:
			heading = "Technical Approvers";
			break;
			
		case ActorGroupTypeEnum.Manager:
			heading = "Affected End User's Manager";
			break;
			
		case ActorGroupTypeEnum.Workflow_Admin:
			heading = "Access & Identity Management";
			break;
		
		default:
			heading = "Unknown Group";
			break;
	}
	
	var trackingHeader = $("#_trackingBladeSectionHeadingTemplate").html();
	trackingHeader = trackingHeader.replace("%%HeadingLabel%%", heading);
	return trackingHeader;
}   

    </script>

   
    
</head>
<body>
<form id="form1" runat="server">
    <uc:DefaultIncludes id="_includes" runat="server" />

	<div class="csm_text_container csm_top5">
		<div class="csm_icon_heading_container border oospa_request_status csm_bottom5">
			<h2>Request Tracking</h2>
			<p class="">This section tracks the status of your request and includes all the people and groups 
			that are involved in the approval process.  Status "Not Active" indicates the person or team will be notified after prerequisite approvals
			have been made.  Due Dates do not include weekends or holidays.&nbsp;&nbsp;<span class="csm_legend_toggle" onclick="toggleLegend(this);">Show Legend</span></p>
		</div>

		<div id="_trackingBladeContainer"></div>

	</div>
    
</form>
</body>

<script type="html/template" id="_trackingBladeSectionHeadingTemplate">
<div class="csm_padded_windowshade">
   <table class="csm_top5" border="0" cellpadding="0" cellspacing="0" style="margin-left:12px;"><!-- width 654 -->
        <tr class="csm_stacked_heading_title">
            <td style="width:228px;">%%HeadingLabel%%</td>
            <td style="width:168px;">STATUS</td>
            <td style="width:120px;">DUE DATE</td>
            <td style="width:120px;">COMPLETED DATE</td>
        </tr>
    </table>    
</div>
<div class="csm_clear">&nbsp;</div>
</script>

<script type='html/template' id='_trackingBladeTemplate'>
<div class="csm_padded_windowshade">
	<div ID="_workflowBladeData" class="csm_data_row %%AlternatingCss%%">
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
				<td style="width:230px;"><span snap="_actorDisplayName">%%ActorName%% | %%GroupTypeId%%</span></td>
				<td style="width:170px;"><span snap="_workflowStatus">%%Status%%</span></td>
				<td style="width:120px;"><span snap="_workflowDueDate">%%DueDate%%</span></td>
				<td style="width:120px;"><span snap="_workflowCompletedDate">%%CompletedDate%%</span></td>
            </tr>
        </table>
     </div>
     <div ID="_workflowBladeCommentsContainer" class="csm_text_container_nodrop" style="display:inline;">%%WorkflowComments%%</div>
</div>
<div class="csm_clear">&nbsp;</div>
</script>

<script type="text/javascript">
    try {LoadTracking('1773');}
    catch (err) {alert(err.toString());}
</script> 



</html>
