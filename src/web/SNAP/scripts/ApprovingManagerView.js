//<![CDATA[
var m_names = new Array("Jan", "Feb", "Mar",
    "Apr", "May", "Jun", "Jul", "Aug", "Sep",
    "Oct", "Nov", "Dec");

var d = new Date();
var curr_day = d.getDate();
var curr_month = d.getMonth();
var curr_year = d.getFullYear();
var curr_date = m_names[curr_month] + " " + curr_day + ", " + curr_year;

function changeDenyClick(obj) {
    $(obj).closest("table").next().find("input[type=button]").each(
     function() {
         if ($(this).val() == $(obj).val()) {
             $(this).removeAttr("disabled");
         }
         else {
             $(this).attr("disabled", "disabled");
         }
     });
}
function ApproverActions(obj, requestId, action) {
    var comments = "";
    var postComments = "";
    var textarea = $(obj).parent().prev().find("textarea");
    if (textarea.val() > "") { textarea.val(textarea.val().replace(/(<([^>]+)>)/ig, "").replace(/\\/g, "/").replace(/\'/g, "\"")}
    
    var approverName = $("input[id$='_currentUserDisplayName']").val()
    switch (action) {
        case '0':
            comments = ""
            ProcessingMessage("Updating Request", "");
            break;
        case '2':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please detail the specific change required."); return false; }
            else {
                postComments = textarea.val().replace("'", "\\'");
                comments = "<br />" + textarea.val();
                ProcessingMessage("Updating Request", "");
            }
            break;
        case '1':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please specify the reason for denial."); return false; }
            else {
                postComments = textarea.val().replace("'", "\\'");
                comments = "<br />" + textarea.val();
                ProcessingMessage("Updating Request", "");
            }
            break;
    }

    var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + comments.replace("'", "\\'") + "'}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "AjaxCalls.aspx/ApproverActions",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d) {

                switch (action) {
                    case '0':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Approved", "You have successfully approved this request.");
                        updateRequestTracking(obj, approverName, "Approved");
                        animateActions(obj, "Open Requests");
                        if ($(obj).attr("id").indexOf("_approveAndMoveNext") > -1) { openNext(obj); }
                        updateCount();
                        break;
                    case '2':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Change Requested", "You have just requested a change.");
                        updateRequestTracking(obj, approverName, "Change Requested");
                        addComments(obj, approverName, "Change Requested", comments, false);
                        animateActions(obj, "Open Requests");
                        $(obj).closest("div.csm_content_container").find("tr.csm_stacked_heading_label").children().each(function() {
                            if ($(this).next().children().html() == "Pending") {
                                $(this).next().children().html("Change Requested");
                            }
                        });
                        updateCount();
                        break;
                    case '1':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Denied", "You have just denied this request.");
                        updateRequestTracking(obj, approverName, "Closed Denied");
                        addComments(obj, approverName, "Closed Denied", comments, false);
                        animateActions(obj, "Closed Requests");
                        $(obj).closest("div.csm_content_container").find("tr.csm_stacked_heading_label").children().each(function() {
                            if ($(this).next().children().html() == "Pending") {
                                $(this).next().children().html("Closed");
                            }
                        });
                        updateCount();
                        break;
                }
            }
            else {
                $('#_indicatorDiv').hide();
                $('#_closeMessageDiv').show();
                $('div.messageBox').children("h2").html("Action Failed");
                $('div.messageBox').children("p").html("Please try again.");
            }
        }
		,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("ApproverAction Error: " + XMLHttpRequest);
            alert("ApproverAction Error: " + textStatus);
            alert("ApproverAction Error: " + errorThrown);
        }
    });

}
function updateCount() {
    $("span").each(function() {
        if ($(this).attr("snap") == "_approvalCount") {
            $(this).html((parseInt($(this).html()) - 1).toString());
        }
    });
}
function updateRequestTracking(obj, approverName, newStatus) {
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
function animateActions(obj, newSection) {
    var blade = $(obj).closest("div.csm_content_container");
    $(obj).closest("div.csm_text_container").fadeOut("slow", function() {
        $(obj).closest("div.csm_content_container").children().next().slideUp("fast", function() {
            $(obj).closest("div.csm_container_center_700").find("h1").each(
            function() {
                var section = $(this);
                if ($(this).html() == newSection) {
                    blade.fadeOut(1000, function() {
                        if ($(section).next().attr("snap") == "_nullDataMessage") { $(section).next().remove(); }
                        $(this).insertAfter(section);

                        var toggle = $(obj).closest("div.csm_content_container").children().find("div.csm_toggle_container");
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
                        
                        $(this).fadeIn(1000);
                    });
                }
            });
        });
    });
}
function openNext(obj) {
    var blade = $(obj).closest("div.csm_content_container");
    $(obj).closest("div.csm_text_container").fadeOut("slow", function() {
    $(obj).closest("div.csm_content_container").children().next().slideUp("fast", function() {
            var toggle = $(obj).closest("div.csm_content_container").children().find("div.csm_toggle_container");
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
            $(obj).closest("div.csm_content_container").next().next().children().next().slideDown("fast");
        });
    });
}
function addComments(obj, approverName, action, comments, includeDate) {
    var newcomment = "";
    comments = comments.replace(/(\r\n|[\r\n])/g, "<br />");
    $(obj).closest("div.csm_hidden_block").children().find("span").each(
        function() {
            if ($(this).attr("snap") == "_actorDisplayName") {
                if ($(this).html() == approverName) {
                    var commentsContainer = $(this).closest("div.csm_data_row").parent().find("div.csm_text_container_nodrop");
                    if (commentsContainer.html() == null) {
                        newcomment = "<div class='csm_text_container_nodrop'><p class='csm_error_text'><u>"
                        + action + " by " + approverName + " on " + curr_date + "</u>" + comments;
                        if (includeDate) { newcomment += "<br />Due Date: " + $(this).parent().next().next().children().html(); }
                        newcomment += "</p></div>";
                        $(newcomment).appendTo($(this).closest("div.csm_data_row").parent());
                    }
                    else {
                        newcomment = "<p class='csm_error_text'><u>"
                        + action + " by " + approverName + " on " + curr_date + "</u>" + comments;
                        if (includeDate) { newcomment += "<br />Due Date: " + $(this).parent().next().next().children().html(); }
                        newcomment += "</p>";
                        $(newcomment).appendTo(commentsContainer);
                    }

                }
            }
        });

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
//]]>