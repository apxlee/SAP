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

function GetRequests(viewIndex) {
    var postData = "{\"view\":\"" + viewIndex + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetRequests",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d.length > 0) {
                CreateBlades(msg.d);
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetRequests Error: " + XMLHttpRequest);
            alert("GetRequests Error: " + textStatus);
            alert("GetRequests Error: " + errorThrown);
        }
    });
}
function GetSearchRequests(searchString) {
    var postData = "{\"searchString\":\"" + searchString + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetSearchRequests",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d.length > 0) {
                CreateBlades(msg.d);
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetRequests Error: " + XMLHttpRequest);
            alert("GetRequests Error: " + textStatus);
            alert("GetRequests Error: " + errorThrown);
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
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetDetails Error: " + XMLHttpRequest);
            alert("GetDetails Error: " + textStatus);
            alert("GetDetails Error: " + errorThrown);
        }
    });
}
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
    $("div.csm_toggle_container").each(function() {
        $(this).parent().parent().next().nextAll().slideUp("fast");
        $(this).addClass("csm_toggle_show");
        $(this).removeClass("csm_toggle_hide");
        $(this).removeClass("csm_toggle_hide_hover");
        $(this).unbind('mouseenter mouseleave')
        $(this).hover(function() {
            $(this).addClass("csm_toggle_show_hover");
        },
			          function() {
			              $(this).removeClass("csm_toggle_show_hover");
			          }
			        );
    });
}
function AddComments(obj, approverName, action, comments, includeDate) {
    var newcomment = "";
    comments = comments.replace(/(\r\n|[\r\n])/g, "<br />");
    $(obj).closest("div.csm_hidden_block").children().find("span").each(
        function() {
            if ($(this).attr("snap") == "_actorDisplayName") {
                if ($(this).html() == approverName) {
                    var commentsContainer = $(this).closest("div.csm_data_row").parent().find("div.csm_text_container_nodrop");
                    if (commentsContainer.html() == null) {
                        newcomment = "<div class='csm_text_container_nodrop'><p class='csm_error_text'><u>"
                        + action + " by AIM on " + curr_date + "</u>" + comments;
                        if (includeDate) { newcomment += "<br />Due Date: " + $(this).parent().next().next().children().html(); }
                        newcomment += "</p></div>";
                        $(newcomment).appendTo($(this).closest("div.csm_data_row").parent());
                    }
                    else {
                        newcomment = "<p class='csm_error_text'><u>"
                        + action + " by AIM on " + curr_date + "</u>" + comments;
                        if (includeDate) { newcomment += "<br />Due Date: " + $(this).parent().next().next().children().html(); }
                        newcomment += "</p>";
                        $(newcomment).appendTo(commentsContainer);
                    }

                }
            }
        });
}
function AnimateActions(newSection, requestId) {
    var blade = $("#__requestContainer_" + requestId);
    var emptyDiv = "<div class='csm_clear'></div>";
    var toggle = $("#__toggleIconContainer_" + requestId);

    $(blade).closest("div.csm_container_center_700").find("h1").each(
    function() {
        var section = $(this);
        if ($(this).html() == newSection) {
            blade.fadeOut(1000, function() {
                if ($(section).next().attr("snap") == "_nullDataMessage") { $(section).next().remove(); }
                $(this).insertAfter(section);
                ToggleDetails(requestId);
                $(this).fadeIn(1000);
            });
        }
    });
}
function UpdateCount(countName) {
    $("span").each(function() {
        if ($(this).attr("snap") == countName) {
            $(this).html((parseInt($(this).html()) - 1).toString());
        }
    });
}
function UpdateRequestTracking(obj, approverName, newStatus) {
    $(obj).closest("div.csm_hidden_block").children().find("span").each(
        function() {
            if ($(this).attr("snap") == "_actorDisplayName") {
                if ($(this).html() == approverName) {
                    $(this).parent().next().children().html(newStatus);
                    $(this).parent().next().next().next().children().html("<span>" + curr_date + "</span>");
                }
            }
        });
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

function GetTracking(builderGroups, builderButtons, requestId) {
    
    var postData = "{\"requestId\":\"" + requestId + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetAllTrackingData",
        data: postData,
        dataType: "json",
        success: function(msg) {
        if (msg.d.length > 0) {
            BuildTrackingSection(msg.d, builderGroups, builderButtons, requestId);
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetTracking Error: " + XMLHttpRequest);
            alert("GetTracking Error: " + textStatus);
            alert("GetTracking Error: " + errorThrown);
        }
    });
}

function BuildTrackingSection(trackingObject, builderGroups, builderButtons, requestId) {
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

    $("#__toggledContentContainer_" + requestId).html($("#__toggledContentContainer_" + requestId).html()
    .replace("<!--__requestTracking-->", trackingSectionHtml));

    BindEvents(builderGroups, builderButtons, requestId);
}

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

// ALL VIEWS - END