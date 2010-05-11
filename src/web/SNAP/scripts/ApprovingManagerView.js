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
    var comments;
    var textarea = $(obj).parent().prev().find("textarea");
    var approverName = $("input[id$='_currentUserDisplayName']").val()
    switch (action) {
        case '0':
            comments = ""
            break;
        case '2':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please detail the specific change required."); return false; }
            else { comments = "<br />" + textarea.val(); }
            break;
        case '1':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please specify the reason for denial."); return false; }
            else { comments = "<br />" + textarea.val(); }
            break;
    }

    var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + comments + "'}";
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
                        ActionMessage("Approved", "You have successfully approved this request.");
                        updateRequestTracking(obj, approverName, "Approved");
                        animateActions(obj, "Open Requests");

                        if ($(obj).attr("id").indexOf("_approveAndMoveNext") > -1) { openNext(obj); }

                        break;
                    case '2':
                        ActionMessage("Change Requested", "You have just requested a change.");
                        updateRequestTracking(obj, approverName, "Change Requested");
                        addComments(obj, approverName, "Change Requested", comments);
                        animateActions(obj, "Open Requests");
                        $(obj).closest("div.csm_content_container").find("tr.csm_stacked_heading_label").children().each(function() {
                            if ($(this).next().children().html() == "Pending") {
                                $(this).next().children().html("Change Requested");
                            }
                        });
                        break;
                    case '1':
                        ActionMessage("Closed Denied", "You have just denied this request.");
                        updateRequestTracking(obj, approverName, "Closed Denied");
                        addComments(obj, approverName, "Closed Denied", comments);
                        animateActions(obj, "Closed Requests");
                        $(obj).closest("div.csm_content_container").find("tr.csm_stacked_heading_label").children().each(function() {
                            if ($(this).next().children().html() == "Pending") {
                                $(this).next().children().html("Closed");
                            }
                        });
                        break;
                }
            }
            else {
                ActionMessage("Action Failed", "");
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
function updateRequestTracking(obj, approverName, newStatus) {
    $(obj).closest("div.csm_hidden_block").children().find("span").each(
        function() {
            if ($(this).attr("id").indexOf("_workflowActorName") > -1) {
                if ($(this).html() == approverName) {
                    if ($(this).parent().next().next().next().children().html() == "-") {
                        $(this).parent().next().children().html(newStatus);
                        $(this).parent().next().next().next().children().html("<span>" + curr_date + "</span>")
                    }
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
                        $(section).next().hide();
                        $(this).insertAfter(section);
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
            $(obj).closest("div.csm_content_container").next().next().children().next().slideDown("fast");
        });
    });
}
function addComments(obj, approverName, action, comments) {
    var newcomment = "";
    $(obj).closest("div.csm_hidden_block").children().find("span").each(function() {
        if ($(this).attr("id").indexOf("_workflowActorName") > -1) {
            if ($(this).html() == approverName) {
                var commentsContainer = $(this).closest("div.csm_data_row").parent().find("div.csm_text_container_nodrop");
                if (commentsContainer.html() == null) {
                    newcomment = "<div class='csm_text_container_nodrop'><p><u>"
                    + action + " by AIM on " + curr_date + "</u>" + comments + "<br />" +
                    "Due Date: " + $(this).parent().next().next().children().html() + "</p></div>";
                    $(newcomment).appendTo($(this).closest("div.csm_data_row").parent());
                }
                else {
                    newcomment = "<p><u>"
                    + action + " by AIM on " + curr_date + "</u>" + comments + "<br />" +
                    "Due Date: " + $(this).parent().next().next().children().html() + "</p>"
                    $(newcomment).appendTo(commentsContainer);
                }

            }
        }
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