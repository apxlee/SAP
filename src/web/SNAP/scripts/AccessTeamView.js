//<![CDATA[
var m_names = new Array("Jan", "Feb", "Mar",
    "Apr", "May", "Jun", "Jul", "Aug", "Sep",
    "Oct", "Nov", "Dec");

var d = new Date();
var curr_day = d.getDate();
var curr_month = d.getMonth();
var curr_year = d.getFullYear();
var curr_date = m_names[curr_month] + " " + curr_day + ", " + curr_year;

function filterClick(obj) {
    var blade = "";
    var filter = $(obj).val();
    $(obj).closest("table").next().html("Open Requests" + " - " + filter);
    $(obj).closest("div.csm_container_center_700").find("div.csm_content_container").each(
        function() {
            blade = this;
            if (filter == "All") {
                $(blade).show();
            }
            else {
                $(this).find("div.csm_hidden_block").children().find("span").each(
                     function() {
                         if ($(this).attr("id").indexOf("_workflowActorName") > -1) {
                             if ($(this).html() == "Access &amp; Identity Management") {
                                 if ($(this).parent().next().next().next().children().html() == "-") {
                                     if ($(this).parent().next().children().html() != filter) {
                                         $(blade).hide();
                                     }
                                     else {
                                         $(blade).show();
                                     }
                                 }
                             }
                         }
                     });
            }
        });
}
function changeDenyCancelClick(obj) {
    $(obj).parent().children("textarea").removeAttr("disabled");
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
function AccessTeamActions(obj, requestId, action) {
    var comments = "";
    var postComments = ""
    var textarea = $(obj).parent().prev().find("textarea");
    if (textarea.val() > "") { textarea.val(textarea.val().replace(/(<([^>]+)>)/ig, '')); }
    
    switch (action) {
        case '4':
            comments = ""
            ProcessingMessage("Updating Request", "");
            break;
        case '2':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please specify the change required."); return false; }
            else 
            {
                postComments = textarea.val().replace("'", "\\'");
                comments = "<br />" + textarea.val();
                ProcessingMessage("Updating Request", ""); 
            }
            break;
        case '3':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please specify the reason for cancel."); return false; }
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

    var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + postComments + "'}";
    textarea.val("");
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "AjaxCalls.aspx/AccessTeamActions",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d) {
                switch (action) {
                    case '4':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Acknowledged", "You have just acknowledged this request, you may now create its workflow.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Pending Workflow");
                        $(obj).attr("disabled", "disabled");
                        $(obj).closest("tr").next().children("td.csm_input_form_control_column").find("input").each(function() {
                            $(this).removeAttr("disabled");
                        });
                        $("#create_workflow_" + requestId).removeAttr("disabled");
                        addComments(obj, "Access &amp; Identity Management", "Acknowledged", "", true);
                        editBuilder($("#closed_cancelled_" + requestId), requestId);
                        $(obj).closest("div.csm_content_container").find("tr.csm_stacked_heading_label").children().each(function() {
                            if ($(this).next().children().html() == "Open") {
                                $(this).next().children().html("Pending");
                            }
                        });
                        break;
                    case '2':
                        $('#_indicatorDiv').hide();
                        $(obj).closest("div.csm_content_container").find("tr.csm_stacked_heading_label").children().each(function() {
                            if ($(this).next().children().html() == "Pending") {
                                $(this).next().children().html("Change Requested");
                            }
                        });
                        ActionMessage("Change Requested", "You have just requested a change.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Change Requested");
                        disableBladeActions(obj);
                        addComments(obj, "Access &amp; Identity Management", "Change Requested", comments, false);
                        updateCount();
                        break;
                    case '3':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Cancelled", "You have just closed this request with this cancellation.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Closed Cancelled");
                        disableBladeActions(obj);
                        addComments(obj, "Access &amp; Identity Management", "Closed Cancelled", comments, false);
                        animateActions(obj, "Closed Requests");
                        hideSections(obj);
                        updateRequestStatus(obj);
                        updateCount();
                        break;
                    case '1':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Denied", "You have just closed this request with this denial.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Closed Denied");
                        disableBladeActions(obj);
                        addComments(obj, "Access &amp; Identity Management", "Closed Denied", comments, false);
                        animateActions(obj, "Closed Requests");
                        hideSections(obj);
                        updateRequestStatus(obj);
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
            alert("AccessTeamAction Error: " + XMLHttpRequest);
            alert("AccessTeamAction Error: " + textStatus);
            alert("AccessTeamAction Error: " + errorThrown);
        }
    });
}
function updateCount() {
    $("span").each(function() {
        if ($(this).attr("snap") == "_accessTeamCount") {
            $(this).html((parseInt($(this).html()) - 1).toString());
        }
    });
}
function disableBladeActions(obj) {
    $(obj).closest("div.csm_content_container").find("input").each(function() {
        $(this).attr("disabled", "disabled");
    });
}
function addComments(obj, approverName, action, comments, includeDate) {
    var newcomment = "";
    $(obj).closest("div.csm_hidden_block").children().find("span").each(
        function() {
            if ($(this).attr("snap") == "_actorDisplayName") {
                if ($(this).html() == approverName) {
                    var commentsContainer = $(this).closest("div.csm_data_row").parent().find("div.csm_text_container_nodrop");
                    if (commentsContainer.html() == null) {
                        newcomment = "<div class='csm_text_container_nodrop'><p><u>"
                        + action + " by AIM on " + curr_date + "</u>" + comments;
                            if (includeDate) { newcomment += "<br />Due Date: " + $(this).parent().next().next().children().html(); }
                        newcomment += "</p></div>";
                        $(newcomment).appendTo($(this).closest("div.csm_data_row").parent());
                    }
                    else {
                        newcomment = "<p><u>"
                        + action + " by AIM on " + curr_date + "</u>" + comments;
                        if (includeDate) { newcomment += "<br />Due Date: " + $(this).parent().next().next().children().html(); }
                        newcomment += "</p>";
                        $(newcomment).appendTo(commentsContainer);
                    }

                }
            }
        });

}
function audienceClick(obj) {
    $(obj).closest("table").next().find("input[type=button]").removeAttr("disabled");
}

function AccessComments(obj, requestId) {
    var comments = "";
    var postComments = ""
    var newNotes = true;
    var action = $(obj).parent().prev().find("input[name=_audience]:checked").val();
    var notesFor = $(obj).parent().prev().find("input[name=_audience]:checked").next().html();
    var textarea = $(obj).parent().prev().find("textarea");
    textarea.val(textarea.val().replace(/(<([^>]+)>)/ig, ''));
    
    ProcessingMessage("Adding Comments", "");
    
    if (textarea.val() != "") {
        comments = textarea.val();
        postComments = comments.replace("'", "\\'");
        textarea.val("");

        var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + postComments + "'}";
        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "AjaxCalls.aspx/AccessComments",
            data: postData,
            dataType: "json",
            success: function(msg) {
                if (msg.d) {
                    $('#_indicatorDiv').hide();
                    ActionMessage("Access Comment", "Your comment has been added to this request.");
                    $(obj).closest("div.csm_content_container").find("div.oospa_request_details").next().find("tr").each(function() {
                        if ($(this).children().children().html() == "Access Notes:") {
                            var accessNotes = "<p><u>" + curr_date + "&nbsp;for&nbsp;" +
				            notesFor + "</u><br />" + comments + "</p>";
                            $(accessNotes).insertAfter($(this).children().next().children().last());
                            newNotes = false;
                        }
                    });

                    //if not found
                    if (newNotes) {
                        var accessNotes = "<tr style='line-height:1.2em;color:Red;'>" +
				        "<td style='text-align:right;width:140px;padding-right:4px;'><p>Access Notes&#58;</p></td>" +
				        "<td style='padding:1px 10px 1px 4px;'><p><u>" + curr_date + "&nbsp;for&nbsp;" +
				        notesFor + "</u><br />" + comments + "</p></td></tr>";
                        $(accessNotes).insertAfter($(obj).closest("div.csm_content_container").find("div.oospa_request_details").next().children());
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
                alert("Add Comment Error: " + XMLHttpRequest);
                alert("Add Comment Error: " + textStatus);
                alert("Add Comment Error: " + errorThrown);
            }
        });
    }
    else {
        ActionMessage("Required Input", "Please insert comment");
    }
}
function approverGroupChecked(obj, requestId) {
    $(document).ready(function() {
        $(obj).closest("table").find("input[type=radio]").each(
          function() {
              if ($(obj).attr("checked")) {
                  $(this).removeAttr("disabled");
                  $(this).change(function() {
                      $(obj).closest("table").find("input[type=radio]").each(
                        function() {
                            $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val().replace("[" + $(this).attr("id") + "]", ""));
                        });
                      $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val() + "[" + $(this).attr("id") + "]");
                  });
                  if ($(this).is(":checked")) {
                      $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val() + "[" + $(this).attr("id") + "]");
                  }
              }
              else {
                  $(this).attr("disabled", "disabled");
                  if ($(this).is(":checked")) {
                      $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val().replace("[" + $(this).attr("id") + "]", ""));
                  }
              }
          });
    });
}
function actionClicked(obj, requestId, state) {
    switch ($(obj).val()) {
        case "Closed Cancelled":
            ProcessingMessage("Updating Request", "");
            builderActions(obj, requestId, state);
            break;
        case "Closed Completed":
            ProcessingMessage("Updating Request", "");
            builderActions(obj, requestId, state);
            break;
        case "Create Ticket":
            ProcessingMessage("Creating Ticket", "")
            builderActions(obj, requestId, state);
            break;
        case "Edit Workflow":
            editBuilder(obj, requestId);
            $(obj).attr("disabled", "disabled");
            break;
        case "Create Workflow":
            ProcessingMessage("Creating Workflow", "");
            createWorkflow(obj, requestId);
            break;
        case "Continue Workflow":
            ProcessingMessage("Updating Workflow", "");
            editCreatedWorkflow(obj, requestId);
            break;
    }
}
function hideSections(obj) {
    $(document).ready(function() {
        $(obj).closest("div.csm_content_container").find("div.csm_text_container").each(function() {
            if ($(this).children().children().html() == "Acknowledgement" ||
                $(this).children().children().html() == "Workflow Builder" ||
                $(this).children().children().html() == "Comments") {
                $(this).hide();
            }
        });
    });
}
function updateRequestStatus(obj) {
    $(obj).closest("div.csm_content_container").find("tr.csm_stacked_heading_label").children().each(function() {
        if ($(this).next().children().html() == "Open" || $(this).next().children().html() == "Pending" || $(this).next().children().html() == "Change Requested") {
            $(this).next().children().html("Closed");
        }
    });
}
function editBuilder(obj, requestId) {
    $(document).ready(function() {
        $(obj).parent().parent().find("input[type=checkbox]").each(
          function() {
              if ($(this).attr("id").indexOf("_requiredCheck") < 0) {
                  $(this).removeAttr("disabled");
                  approverGroupChecked(this, requestId);
              }
          });
        $(obj).parent().parent().find("input[type=text]").removeAttr("disabled");
        $(obj).parent().parent().find("input[type=button]").each(function() {
            $(this).removeAttr("disabled");
            if ($(this).val() == "Remove") {
                $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val() + "[" + $(this).parent().prev().html() + "]");
            }
        });
        $(obj).parent().parent().find("select").removeAttr("disabled");

        $("#closed_cancelled_" + requestId).removeAttr("disabled");
        $("#continue_workflow_" + requestId).removeAttr("disabled");

        var editLink = $(obj).parent().parent().find(".oospa_edit_icon_disabled");
        editLink.addClass("oospa_edit_icon");
        editLink.removeClass("oospa_edit_icon_disabled");
        editLink.click(function() {
            managerEdit(this);
        });
    });
}
function disableBuilder(obj, requestId) {
    $(document).ready(function() {
        $(obj).parent().parent().find("input[type=checkbox]").each(
          function() {
              if ($(this).attr("id").indexOf("_requiredCheck") < 0) {
                  $(this).attr("disabled", "disabled");
              }
          });
        $(obj).parent().parent().find("input[type=text]").attr("disabled", "disabled");
        $(obj).parent().parent().find("input[type=button]").attr("disabled", "disabled");
        $(obj).parent().parent().find("select").attr("disabled", "disabled");
        $("#_selectedActors_" + requestId).val("");
        $("#closed_cancelled_" + requestId).attr("disabled", "disabled");
        $("#create_workflow_" + requestId).attr("disabled", "disabled");
        $(obj).attr("disabled", "disabled");

        var editLink = $(obj).parent().parent().find(".oospa_edit_icon");
        editLink.addClass("oospa_edit_icon_disabled");
        editLink.removeClass("oospa_edit_icon");
        editLink.unbind("click");
    });
}
function builderActions(obj, requestId, state) {

    var postData = "{'requestId':'" + requestId.toString() + "','action':'" + state + "'}";

    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "AjaxCalls.aspx/BuilderActions",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d) {
                switch (state) {
                    case "3":
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Cancelled", "You have just cancelled this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Closed Cancelled");
                        animateActions(obj, "Closed Requests");
                        hideSections(obj);
                        updateRequestStatus(obj);
                        updateCount();
                        break;
                    case "6":
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Completed", "You have just completed this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Closed Completed");
                        animateActions(obj, "Closed Requests");
                        hideSections(obj);
                        updateRequestStatus(obj);
                        updateCount();
                        break;
                    case "5":
                        $('#_indicatorDiv').hide();
                        ActionMessage("Pending Provisioning", "A ticket has been created to provision the access for this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Pending Provisioning");
                        $("#closed_completed_" + requestId).removeAttr("disabled");
                        $("#create_ticket_" + requestId).attr("disabled", "disabled");
                        break;
                }
            }
            else {
                if (state == 5) {
                    $('#_indicatorDiv').hide();
                    $('#_closeMessageDiv').show();
                    $('div.messageBox').children("h2").html("Ticket Creation Failed");
                    $('div.messageBox').children("p").html("Please try again or create the ticket manually.(add the ticket number within the comments section)");
                    $("#closed_completed_" + requestId).removeAttr("disabled");
                }
                else {
                    $('#_indicatorDiv').hide();
                    $('#_closeMessageDiv').show();
                    $('div.messageBox').children("h2").html("Action Failed");
                    $('div.messageBox').children("p").html("Please try again.");
                }
            }
        }
		,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetNames Error: " + XMLHttpRequest);
            alert("GetNames Error: " + textStatus);
            alert("GetNames Error: " + errorThrown);
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
    var emptyDiv = "<div class='csm_clear'></div>";
    $(obj).closest("div.csm_text_container").fadeOut("slow", function() {
        $(obj).closest("div.csm_content_container").children().next().slideUp("fast", function() {
            $(obj).closest("div.csm_container_center_700").find("h1").each(
            function() {
                var section = $(this);
                if ($(this).html() == newSection) {
                    blade.fadeOut(1000, function() {
                        if ($(section).next().attr("snap") == "_nullDataMessage") { $(section).next().remove(); }
                        $(emptyDiv).insertAfter(section);
                        $(this).insertAfter(section);
                        $(this).fadeIn(1000);
                    });
                }
            });
        });
    });
}
function createWorkflow(obj, requestId) {
    $(document).ready(function() {

        if ($("#_managerUserId_" + requestId).val() > "") {
            var postData = "{'requestId':'" + requestId.toString() + "','managerUserId':'" + $("#_managerUserId_" + requestId).val() + "','actorIds':'" + $("#_selectedActors_" + requestId).val() + "'}";
            //TODO: (added parameter managerUserId)format postdata to match AjaxCalls.aspx\CreateWorkflow parameter"
            $.ajax({
                type: "POST",
                contentType: "application/json; character=utf-8",
                url: "AjaxCalls.aspx/CreateWorkflow",
                data: postData,
                dataType: "json",
                success: function(msg) {
                    if (msg.d) {
                        $('#_indicatorDiv').hide();
                        ActionMessage("Workflow Created", "The workflow has been created for this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Workflow Created");
                        disableBuilder(obj, requestId);
                    }
                    else {
                        $('#_indicatorDiv').hide();
                        $('#_closeMessageDiv').show();
                        $('div.messageBox').children("h2").html("Workflow Creation Failed");
                        $('div.messageBox').children("p").html("The workflow creation failed.");
                    }
                },

                error: function(XMLHttpRequest, textStatus, errorThrown) {
                    alert("GetNames Error: " + XMLHttpRequest);
                    alert("GetNames Error: " + textStatus);
                    alert("GetNames Error: " + errorThrown);
                }
            });
        }
        else {
            ActionMessage("Validation Error", "The manager you have selected has not been checked.");
        }
    });
}
function editCreatedWorkflow(obj, requestId) {
    $(document).ready(function() {
        if ($("#_managerUserId_" + requestId).val() > "") {
            var postData = "{'requestId':'" + requestId.toString() + "','managerUserId':'" + $("#_managerUserId_" + requestId).val() + "','actorIds':'" + $("#_selectedActors_" + requestId).val() + "'}";
            //TODO: (added parameter managerUserId)format postdata to match AjaxCalls.aspx\CreateWorkflow parameter"

            $.ajax({
                type: "POST",
                contentType: "application/json; character=utf-8",
                url: "AjaxCalls.aspx/EditWorkflow",
                data: postData,
                dataType: "json",
                success: function(msg) {
                    if (msg.d) {
                        $('#_indicatorDiv').hide();
                        ActionMessage("Workflow Updated", "The workflow has been updated for this request.");
                        disableBuilder(obj, requestId);
                    }
                    else {
                        $('#_indicatorDiv').hide();
                        $('#_closeMessageDiv').show();
                        $('div.messageBox').children("h2").html("Workflow Updated Failed");
                        $('div.messageBox').children("p").html("TThe workflow update failed. No changes where made.");
                    }
                },

                error: function(XMLHttpRequest, textStatus, errorThrown) {
                    alert("GetNames Error: " + XMLHttpRequest);
                    alert("GetNames Error: " + textStatus);
                    alert("GetNames Error: " + errorThrown);
                }
            });
        }
        else {
            ActionMessage("Validation Error", "The manager you have selected has not been checked.");
        }
    });
}
function managerEdit(obj) {
    $(document).ready(function() {
        var managerLabelSection = $(obj).prev().prev().prev().prev()
        var managerLabelDispalyName = $(obj).prev().prev().prev().prev().children();
        var managerInputUserId = managerLabelDispalyName.next();
        var managerInputSection = $(obj).prev().prev().prev();
        var managerInputDisplayName = $(obj).prev().prev().prev().children("input[type=text]");
        var managerInputCheckButton = managerInputDisplayName.next();

        managerLabelSection.hide();
        managerInputDisplayName.val(managerLabelDispalyName.html());
        managerInputSection.show();

        managerInputDisplayName.change(function() {
            managerInputUserId.val("");
        });

        managerInputCheckButton.click(function() {
            if (managerInputUserId.val() == "") {
                GetNames(obj,managerInputDisplayName,"manager");
            }
            else {
                managerInputSection.hide();
                managerLabelSection.show();
            }
        });
    });
}
function GetNames(obj,name,section) {
    var indicator = $('.oospa_ajax_indicator');
    var selection = $('select[id$=_managerSelection]');
    var postData = "{'name':'" + name.val().replace("(", "").replace(")", "").replace(/\\/, "").replace("'", "\\'") + "'}";
    selection.hide();
    indicator.show();
    OpenDialog(name);

    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "AjaxCalls.aspx/GetNames",
        data: postData,
        dataType: "json",
        success: function(msg) {

            var names = msg.d;
            // no match
            if (section == "manager") {
                if (names.length == 0) {
                    FillManagerErrorFields(name);
                }

                // direct match
                if (names.length == 1) {
                    FillManagerAllFields(obj, name, names);
                }

                // match list of names
                if (names.length > 1) {
                    FillManagerSelection(obj, name, names);
                }
            }
            else {
                if (names.length == 0) {
                    FillActorErrorFields(name);
                }

                // direct match
                if (names.length == 1) {
                    FillActorAllFields(obj, name, names);
                }

                // match list of names
                if (names.length > 1) {
                    FillActorSelection(obj, name, names);
                }
            }
        },

        error: function(XMLHttpRequest, textStatus, errorThrown) {
            $("#_managerSelectionDiv").dialog("destroy");
            alert("GetNames Error: " + XMLHttpRequest);
            alert("GetNames Error: " + textStatus);
            alert("GetNames Error: " + errorThrown);
        }
    });
}
function FillManagerAllFields(obj, name, names) {
    var managerLabelSection = $(name).parent().prev();
    var managerLabelDispalyName = $(name).parent().prev().children("span");
    var managerInputUserId = managerLabelDispalyName.next();
    var managerInputSection = $(name).parent();
    var managerInputDisplayName = $(name).parent().children("input[type=text]");
    $("#_managerSelectionDiv").dialog("destroy");
    managerInputUserId.val(names[0].LoginId);
    managerLabelDispalyName.html(names[0].Name);
    managerInputSection.hide();
    managerLabelSection.show();
    updateManagerName(obj, names[0].Name);
}
function FillManagerErrorFields(name) {
    var managerInputDisplayName = $(name).parent().children("input[type=text]");
    var managerInputUserId = $(name).parent().prev().children("span").next();
    $("#_managerSelectionDiv").dialog("destroy");
    managerInputDisplayName.val("No such name! Try again");
    managerInputUserId.val("");
    managerInputDisplayName.focus();
}
function FillManagerSelection(obj, name, names) {
    var managerLabelSection = $(name).parent().prev();
    var managerLabelDispalyName = $(name).parent().prev().children("span");
    var managerInputUserId = managerLabelDispalyName.next();
    var managerInputSection = $(name).parent();
    var managerInputDisplayName = $(name).parent().children("input[type=text]");
    var selection = $('select[id$=_managerSelection]');
    var indicator = $('.oospa_ajax_indicator');

    selection.change(function() {
        managerLabelDispalyName.html($('#' + selection.attr('id') + ' :selected').text());
        managerInputSection.hide();
        managerLabelSection.show();
        managerInputUserId.val($('#' + selection.attr('id') + ' :selected').val());
        $("#_managerSelectionDiv").dialog("destroy");
        updateManagerName(obj, $('#' + selection.attr('id') + ' :selected').text());
    });

    var listItems = [];
    for (var key in names) {
        listItems.push('<option value="' + names[key].LoginId + '">' + names[key].Name + '</option>');
    }
    selection.empty();
    selection.append(listItems.join(''));

    // don't over expand the dialog box
    if (names.length >= 10)
        selection.attr('size', 10);
    else
        selection.attr('size', names.length);

    indicator.hide();
    selection.show();

}

function FillActorAllFields(obj, name, names) {
    var actorDisplayName = $(obj).parent().children("input[id$='_actorDisplayName']");
    var actorUserId = $(obj).parent().children("input[id$='_actorUserId']");
    $("#_managerSelectionDiv").dialog("destroy");
    actorUserId.val(names[0].LoginId);
    actorDisplayName.val(names[0].Name);
}
function FillActorErrorFields(name) {
    var actorDisplayName = $(name).parent().children("input[id$='_actorDisplayName']");
    var actorUserId = $(name).parent().children("input[id$='_actorUserId']");
    $("#_managerSelectionDiv").dialog("destroy");
    actorDisplayName.val("No such name! Try again");
    actorUserId.val("");
    actorDisplayName.focus();
}
function FillActorSelection(obj, name, names) {
    var actorDisplayName = $(obj).parent().children("input[id$='_actorDisplayName']");
    var actorUserId = $(obj).parent().children("input[id$='_actorUserId']");
    var selection = $('select[id$=_managerSelection]');
    var indicator = $('.oospa_ajax_indicator');

    selection.change(function() {
        actorDisplayName.val($('#' + selection.attr('id') + ' :selected').text());
        actorUserId.val($('#' + selection.attr('id') + ' :selected').val());
        $("#_managerSelectionDiv").dialog("destroy");
    });

    var listItems = [];
    for (var key in names) {
        listItems.push('<option value="' + names[key].LoginId + '">' + names[key].Name + '</option>');
    }
    selection.empty();
    selection.append(listItems.join(''));

    // don't over expand the dialog box
    if (names.length >= 10)
        selection.attr('size', 10);
    else
        selection.attr('size', names.length);

    indicator.hide();
    selection.show();
}

function OpenDialog(name) {
    $("#_managerSelectionDiv").dialog({
        title: 'Select User',
        autoOpen: true,
        bgiframe: true,
        resizable: false,
        draggable: false,
        height: 300,
        width: 350,
        modal: true,
        overlay: {
            backgroundColor: '#ff0000', opacity: 0.5

        },
        buttons: {
            Cancel: function() {
                $(this).dialog("destroy");
                $(name).parent().children("input[type=text]").focus();
            }
        }
    });

}
function updateManagerName(obj, newManager) {
    $(obj).closest("div.csm_content_container").find("div.oospa_request_details").next().find("tr").each(function() {
        if ($(this).children().children().html() == "Manager Name:") {
            $(this).children().next().children().html(newManager);
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

function ActorSelected(obj) {
    var actorDisplayName = $(obj).parent().children("input[id$='_actorDisplayName']");
    var actorUserId = $(obj).parent().children("input[id$='_actorUserId']");
    var actorActorId = $(obj).parent().children("input[id$='_actorActorId']");
    var actorCheckButton = $(obj).parent().children("input[id$='_checkActor']");

    actorCheckButton.attr("disabled", "disabled");
    
    if ($(obj).val() != "0") {
        actorDisplayName.val($('#' + $(obj).attr('id') + ' option:selected').text());
        actorActorId.val($(obj).val());
        actorUserId.val("");
    }
    else {
        actorDisplayName.val("");
        actorUserId.val("");
        actorActorId.val("");
    }
}

function ActorChanged(obj) {
    var actorDisplayName = $(obj).parent().children("input[id$='_actorDisplayName']");
    var actorUserId = $(obj).parent().children("input[id$='_actorUserId']");
    var actorActorId = $(obj).parent().children("input[id$='_actorActorId']");
    var actorCheckButton = $(obj).parent().children("input[id$='_checkActor']");

    actorUserId.val("");
    actorActorId.val("");

    if (actorDisplayName.val().length == 0) { actorCheckButton.attr("disabled", "disabled"); }
    else {
        actorCheckButton.removeAttr("disabled");
    }
}

function ActorCheck(obj) {
    var actorDisplayName = $(obj).parent().children("input[id$='_actorDisplayName']");
    var actorUserId = $(obj).parent().children("input[id$='_actorUserId']");

    if (actorUserId.val() == "") {
        GetNames(obj, actorDisplayName, "actor");
    }
}

function ActorAdd(obj,requestId) {
    $(document).ready(function() {
        var actorUserId = $(obj).parent().children("input[id$='_actorUserId']");
        var actorGroupId = $(obj).parent().children("input[id$='_actorGroupId']");
        var actorActorId = $(obj).parent().children("input[id$='_actorActorId']");
        if (actorActorId.val() == "") {
            var postData = "{'userId':'" + actorUserId.val() + "','groupId':'" + actorGroupId.val() + "'}";
            $.ajax({
                type: "POST",
                contentType: "application/json; character=utf-8",
                url: "AjaxCalls.aspx/GetActorId",
                data: postData,
                dataType: "json",
                success: function(msg) {
                    if (msg.d > "0") {
                        $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val() + "[" + msg.d + "]");
                        UpdateActorList(obj, msg.d);
                    }
                },

                error: function(XMLHttpRequest, textStatus, errorThrown) {
                    alert("GetNames Error: " + XMLHttpRequest);
                    alert("GetNames Error: " + textStatus);
                    alert("GetNames Error: " + errorThrown);
                }
            });
        }
        else {
            $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val() + "[" + actorActorId.val() + "]");
            UpdateActorList(obj, actorActorId.val());
        }
    });
}

function UpdateActorList(obj, actorId) {
    var actorDisplayName = $(obj).parent().children("input[id$='_actorDisplayName']");
    var actorUserId = $(obj).parent().children("input[id$='_actorUserId']");
    var actorActorId = $(obj).parent().children("input[id$='_actorActorId']");
    var listItem;
    var table = $('<table />').attr('class','listview_table');
    var tbody = $('<tbody />')
    var tableTr = $('<tr />').attr('class', 'listview_tr');
    var tableButton = $('<td />').attr('class', 'listview_button');
    var removeButton = $("<input type='button'>")
    removeButton.bind('click', function() { RemoveActor(this); });
    removeButton.val("Remove");
    
    if ($(obj).closest("tr").next().find("tr").html() == null) {
        $('<td />').attr('class', 'listview_td').html(actorDisplayName.val()).appendTo(tableTr);
        $('<td />').css('display', 'none').html(actorActorId.val()).appendTo(tableTr);
        removeButton.appendTo(tableButton);
        tableButton.appendTo(tableTr);
        tableTr.appendTo(tbody)
        tbody.appendTo(table);
        table.appendTo($(obj).closest("tr").next().children());
    }
    else {
        $('<td />').attr('class', 'listview_td').html(actorDisplayName.val()).appendTo(tableTr);
        $('<td />').css('display', 'none').html(actorActorId.val()).appendTo(tableTr);
        removeButton.appendTo(tableButton);
        tableButton.appendTo(tableTr);
        $(tableTr).insertAfter($(obj).closest("tr").next().find("tr").last());
    }
    
    actorDisplayName.val('');
    actorUserId.val('');
    actorActorId.val('');
    $(obj).parent().find("option").each(function() {
        if ($(this).val() == actorId) { $(this).remove(); }
    });

}

function RemoveActor(obj) {
    var actorDisplayName = $(obj).closest("tr.listview_tr").children();
    var actorActorId = $(obj).closest("tr.listview_tr").children().next();
    var actorOption = $("<option value='" + actorActorId.html() + "'>");
    actorOption.html(actorDisplayName.html());
    actorOption.appendTo($(obj).closest("table.oospa_workflow_builder_row").find("select"));

    var selectedActors = $(obj).closest("table.csm_input_form_container").next("div.csm_input_buttons_container").children().first();
    selectedActors.val(selectedActors.val().replace("[" + actorActorId.html() + "]", ""));
    
    //finally remove row
    $(obj).closest("tr.listview_tr").remove();
}

//]]>