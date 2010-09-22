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
function GetDetails(sender, requestId) {
    var postData = "{\"requestId\":\"" + requestId + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetDetails",
        data: postData,
        dataType: "json",
        success: function(msg) {
        if (msg.d.length > 0) {
                CreateRequestDetails(msg.d, sender, requestId);
            }
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetDetails Error: " + XMLHttpRequest);
            alert("GetDetails Error: " + textStatus);
            alert("GetDetails Error: " + errorThrown);
        }
    });
}
function ToggleLoading(sender, requestId) {
    if ($(sender).hasClass("csm_toggle_loading")) {
        $(sender).removeClass("csm_toggle_loading");
        $("#__toggledContentContainer_" + requestId).removeClass("csm_hidden_block");
        $(sender).addClass("csm_toggle_hide");
        $(sender).hover(function() {
            $(this).addClass("csm_toggle_hide_hover");
        },
	      function() {
	          $(this).removeClass("csm_toggle_hide_hover");
	      }
	    );
    }
    else {
        $(sender).removeClass("csm_toggle_show");
        $(sender).removeClass("csm_toggle_show_hover");
        $(sender).unbind('mouseenter mouseleave')
        $(sender).addClass("csm_toggle_loading");
    }
}
function ToggleDetails(requestId) {
    var section = $("#__toggledContentContainer_" + requestId);
    var toggle = $("#__toggleIconContainer_" + requestId);

    //Search for (Request Details) which is the title of the first section
    if ($(section).html().indexOf("Request Details") == -1) {
        ToggleLoading(toggle, requestId);
        GetDetails(toggle, requestId);
    }
    else {
        if (section.hasClass("csm_hidden_block")) {
            section.removeClass("csm_hidden_block");
            toggle.addClass("csm_toggle_hide");
            toggle.removeClass("csm_toggle_show");
            toggle.removeClass("csm_toggle_show_hover");
            toggle.unbind('mouseenter mouseleave')
            toggle.hover(function() {
                $(this).addClass("csm_toggle_hide_hover");
            },
			      function() {
			          $(this).removeClass("csm_toggle_hide_hover");
			      }
			    );
        }
        else {
            section.addClass("csm_hidden_block");
            toggle.addClass("csm_toggle_show");
            toggle.removeClass("csm_toggle_hide");
            toggle.removeClass("csm_toggle_hide_hover");
            toggle.unbind('mouseenter mouseleave')
            toggle.hover(function() {
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
function UpdateCount() {
//    $("span").each(function() {
//        if ($(this).attr("snap") == "_approvalCount") {
//            $(this).html((parseInt($(this).html()) - 1).toString());
//        }
//    });
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

// ALL VIEWS - END