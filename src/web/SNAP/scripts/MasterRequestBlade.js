$(document).bind('keydown', 'alt+ctrl+h', HideAll);

// ALL VIEWS - BEGIN
var m_names = new Array("Jan", "Feb", "Mar",
    "Apr", "May", "Jun", "Jul", "Aug", "Sep",
    "Oct", "Nov", "Dec");

var d = new Date();
var curr_day = d.getDate();
var curr_month = d.getMonth();
var curr_year = d.getFullYear();
var curr_date = m_names[curr_month] + " " + curr_day + ", " + curr_year;

var ViewIndexEnum = {
		Login: 0,
		Request_Form: 1,
		My_Requests: 2,
		My_Approvals: 3,
		Access_Team: 4,
		Search: 5,
		Support: 6	
		}

$().ready(function() {
    $.ajaxSetup({
        error: function(x, e) {
            switch (x.status) {
                case 0:
                case 12007:
                    MessageDialog("Network connection Issue.", "Please check you connection.");
                    CloseRedirect();
                    break;
                case 404:
                    MessageDialog("Requested URL not found.", "You will be redirected to the login page.");
                    CloseRedirect();
                    break;
                case 500:
                    MessageDialog("Session Timeout", "You will be redirected to the login page.");
                    CloseRedirect();
                    break;
                default:
                    if (e == 'parsererror') { MessageDialog("Application Error", "You will be redirected to the login page."); }
                    else if (e == 'timeout') { MessageDialog("Application Timeout", "You will be redirected to the login page."); }
                    else { MessageDialog("Uknown Error.", "You will be redirected to the login page."); }
                    CloseRedirect();
                    break;
            }
        }
    });
});
function CloseRedirect() {
    var closeDiv = $("#_closeMessageDiv");
    $(closeDiv).find("input[type=button]").click(function() {
        window.location.href = "default.aspx";
        $(this).unbind("click");
        $(this).click(function() {
            $('#_actionMessageDiv').hide(); 
            $('#_closeMessageDiv').hide();
        });
    });
}
function GetRequests(viewIndex, search) {
    var postData = "{\"view\":\"" + viewIndex + "\",\"search\":" + search + "}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetRequests",
        data: postData,
        dataType: "json",
        success: function(msg) {
            CreateBlades(msg.d);
        }
    });
}
function GetDetails(requestId) {
    var postData = "{\"requestId\":\"" + requestId + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetDetails",
        data: postData,
        dataType: "json",
        success: function(msg) {
        if (msg.d.length > 0) {
                CreateRequestDetails(msg.d, requestId);
            }
        }
    });
}
//function ToggleRequestLoader() {
//    if ($("#_requestLoaderDiv").is(":hidden")) {
//        $("#preloader_load").removeClass("preloader_load_FULL");
//        $("#preloader_load").addClass("preloader_load_ANIM");
//        $("#preloader_build").removeClass("preloader_build_ANIM");
//        $("#preloader_build").addClass("preloader_build_EMPTY");
//        $("#_requestLoaderDiv").fadeIn();
//    }
//    else {
//        $("#_requestLoaderDiv").delay(2000).fadeOut();   
//    }
//}
function ToggleLoading(requestId) {
    var blade = $("#__toggleIconContainer_" + requestId);
    if (blade.hasClass("csm_toggle_loading")) {
        blade.removeClass("csm_toggle_loading");
        $("#__toggledContentContainer_" + requestId).removeClass("csm_hidden_block");
        blade.addClass("csm_toggle_hide");
        blade.hover(function() {
            $(this).addClass("csm_toggle_hide_hover");
        },
	      function() {
	          $(this).removeClass("csm_toggle_hide_hover");
	      }
	    );
    }
    else {
        blade.removeClass("csm_toggle_show");
        blade.removeClass("csm_toggle_show_hover");
        blade.unbind('mouseenter mouseleave')
        blade.addClass("csm_toggle_loading");
    }
}
function ToggleLegend(requestId) {
    $("#__legend_" + requestId).toggle();
    if ($("#__legend_" + requestId).is(":visible")) {$("#__legendToggle_" + requestId).html("Hide Legend");}
    else { $("#__legendToggle_" + requestId).html("Show Legend"); }
}
function ToggleDetails(requestId) {
    var section = $("#__toggledContentContainer_" + requestId);
    var blade = $("#__toggleIconContainer_" + requestId);
    //Search for (Request Details) which is the title of the first section
    if (section.html().indexOf("Request Details") == -1) {
        ToggleLoading(requestId);
        GetDetails(requestId);
    }
    else {
        if (section.hasClass("csm_hidden_block")) {
            section.removeClass("csm_hidden_block");
            blade.addClass("csm_toggle_hide");
            blade.removeClass("csm_toggle_show");
            blade.removeClass("csm_toggle_show_hover");
            blade.unbind('mouseenter mouseleave')
            blade.hover(function() {
                $(this).addClass("csm_toggle_hide_hover");
            },
			      function() {
			          $(this).removeClass("csm_toggle_hide_hover");
			      }
			    );
        }
        else {
            section.addClass("csm_hidden_block");
            blade.addClass("csm_toggle_show");
            blade.removeClass("csm_toggle_hide");
            blade.removeClass("csm_toggle_hide_hover");
            blade.unbind('mouseenter mouseleave')
            blade.hover(function() {
                $(this).addClass("csm_toggle_show_hover");
            },
			      function() {
			          $(this).removeClass("csm_toggle_show_hover");
			      }
			    );
        }   
    }
}
function ProcessingMessage(header, message) {
    $(document).ready(function() {
        $('#_closeMessageDiv').hide();
        $('div.messageBox').children("h2").html(header);
        $('div.messageBox').children("p").html(message);
        $('#_indicatorDiv').show();
        $('#_actionMessageDiv').fadeIn();
    });
}
function ActionMessage(header, message) {
    $(document).ready(function() {
        $('div.messageBox').children("h2").html(header);
        $('div.messageBox').children("p").html(message);
        $('#_actionMessageDiv').fadeIn().delay(2000).fadeOut();
    });
}
function HideAll() {
    $("div[id*='__toggledContentContainer_']").each(function() {
        if (!$(this).hasClass("csm_hidden_block")) {
            $(this).addClass("csm_hidden_block");
            var blade = $(this).closest("div.csm_content_container").find("div.csm_toggle_container");
            blade.addClass("csm_toggle_show");
            blade.removeClass("csm_toggle_hide");
            blade.removeClass("csm_toggle_hide_hover");
            blade.unbind('mouseenter mouseleave')
            blade.hover(function() {
                $(this).addClass("csm_toggle_show_hover");
            },
			      function() {
			          $(this).removeClass("csm_toggle_show_hover");
			      }
			    );
        }
    });
}
function AnimateActions(newSection, requestId) {
    var blade = $("#__requestContainer_" + requestId);
    var emptyDiv = "<div class='csm_clear'></div>";
    var toggle = $("#__toggleIconContainer_" + requestId);
    ToggleDetails(requestId);
    blade.fadeOut(1000, function() {
        switch (newSection) {
            case "Open Requests":
                $("#__nullDataMessage_OpenRequests").remove();
                if ($("#_openRequestsContainer").children().first().html() == null) { $("#_openRequestsContainer").append($(this)); }
                else { $(this).insertBefore($("#_openRequestsContainer").children().first()); }
                break;
            case "Closed Requests":
                $("#__nullDataMessage_ClosedRequests").remove();
                if ($("#_closedRequestsContainer").children().first().html() == null) { $("#_closedRequestsContainer").append($(this)); }
                else { $(this).insertBefore($("#_closedRequestsContainer").children().first()); }
                break;
        }
        $(this).fadeIn(1000);
    });
}
function UpdateCount(countName) {
    $("#csm_ribbon_container_inner").find("span").each(function() {
        if ($(this).attr("snap") == countName) {
            var newCount = parseInt($(this).html()) - 1;
            $(this).html(newCount.toString());
            if (newCount == 0) {
                if (countName == "_approvalCount") {
                    var newNullPending = $("#_nullDataMessage").html().replace("__nullDataMessage_ID", "__nullDataMessage_PendingRequests")
                    .replace("__message_TEXT", "There are no requests Pending Approval at this time.");
                    $("#_pendingApprovalsContainer").append($(newNullPending).hide().show(2000));
                }
                if (countName == "_accessTeamCount") {
                    var newNullOpen = $("#_nullDataMessage").html().replace("__nullDataMessage_ID", "__nullDataMessage_OpenRequests")
                    .replace("__message_TEXT", "There are no Open Requests at this time.");
                    $("#_openRequestsContainer").append($(newNullOpen).hide().show(2000));
                }
            }
        }
    });
}
function UpdateRequestTracking(requestId) {
    $("#__requestTrackingSection_" + requestId).html("");
    GetTracking(null, null, requestId, true);
}
function Comment(requestId, action, comments) {
    this.requestId = requestId;
    this.action = action;
    this.comments = $.quoteString(comments);
    this.toJSON = $.toJSON(this);
}

var ActorGroupTypeEnum =
{
    Team_Approver: 0,
    Technical_Approver: 1,
    Manager: 2,
    Workflow_Admin: 3
};

function GetTracking(builderGroups, builderButtons, requestId, isRebuild) {
    var postData = "{\"requestId\":\"" + requestId + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetAllTrackingData",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d.length > 0) {
                BuildTrackingSection(msg.d, builderGroups, builderButtons, requestId, isRebuild);
            }
        }
    });
}

function BuildTrackingSection(trackingObject, builderGroups, builderButtons, requestId, isRebuild) {
    var trackingSectionHtml = "";
    
    // Build sections in the following order and loop thru them
    //	
    var headingOrder = [
		ActorGroupTypeEnum.Workflow_Admin,
		ActorGroupTypeEnum.Manager,
		ActorGroupTypeEnum.Team_Approver,
		ActorGroupTypeEnum.Technical_Approver
		];

    for (var i = 0; i < headingOrder.length; i++) {
        // If the trackingObject has at least one row that matches the heading, then draw it
        //
        if (IsActorGroupInTrackingData(trackingObject, headingOrder[i])) {
            trackingSectionHtml += CreateTrackingHeader(headingOrder[i]);

            $.each(trackingObject, function(index, value) {
                var data = jQuery.parseJSON(value);
                if (data.ActorGroupType == headingOrder[i]) {
                    trackingSectionHtml += CreateTrackingBlade(data);
                }
            });
        }
    }

    $("#__requestTrackingSection_" + requestId).append(trackingSectionHtml);
    if (!isRebuild) {
        BindEvents(builderGroups, builderButtons, requestId);
    }
    //DisplayRequestChangeLink();
}

//function DisplayRequestChangeLink() {
//	// This  is to hide 'Edit Request Form' link unless currently logged-in user is also AEU
//	var hiddenSpanId = '_' + $("input[id*='_hiddenCurrentUserId']").attr("value") + '_request_link';
//	$("span[id*=" + hiddenSpanId + "]").removeClass("csm_hidden_span");
//}

function IsActorGroupInTrackingData(trackingObject, actorGroupEnum) {
    var isInGroup = false;

    $.each(trackingObject, function(index, value) {
        var data = jQuery.parseJSON(value);
        if (data.ActorGroupType == actorGroupEnum) {
            isInGroup = true;
        }
    });

    return isInGroup;
}

function CreateTrackingBlade(data) {
    var newTrackingBlade = $("#_trackingBladeTemplate").html();

    newTrackingBlade = newTrackingBlade
			.replace("%%ActorName%%", data.ActorName)
			.replace("%%Status%%", data.WorkflowStatus)
			.replace("%%DueDate%%", data.DueDate)
			.replace("%%CompletedDate%%", data.CompletedDate)
			.replace("%%GroupTypeId%%", data.ActorGroupType)
			.replace("%%WorkflowComments%%", data.WorkflowComments)
			.replace("%%WorkflowCommentsStyle%%", data.WorkflowCommentsStyle)
			
	if (data.ActorGroupType == ActorGroupTypeEnum.Workflow_Admin) {
		newTrackingBlade = newTrackingBlade.replace("%%AlternatingCss%%", "csm_alternating_bg");
	}

    return newTrackingBlade;
}

function CreateTrackingHeader(actorGroupType) {
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

function MessageDialog(header, message) {
    $(document).ready(function() {
        $('#_indicatorDiv').hide();
        $('#_actionMessageDiv').fadeIn();
        $('#_closeMessageDiv').show();
        $('div.messageBox').children("h2").html(header);
        $('div.messageBox').children("p").html(message);
    });
}
// ALL VIEWS - END