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
    $(obj).closest("div.csm_container_center_700").find("div.csm_content_container").each(
        function() {
            blade = this;
            if (filter == "Show All") {
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
    var comments;
    var textarea = $(obj).parent().prev().find("textarea");

    switch (action) {
        case '4':
            comments = ""
            break;
        case '2':
            if (textarea.val() == "") { ActionMessage("Validation Error", "Please specify the change."); return false; }
            else { comments = textarea.val(); }
            break;
        case '3':
            if (textarea.val() == "") { ActionMessage("Validation Error", "Please specify the reason for cancel."); return false; }
            else { comments = textarea.val(); }
            break;
        case '1':
            if (textarea.val() == "") { ActionMessage("Validation Error", "Please specify the reason for denial."); return false; }
            else { comments = textarea.val(); }
            break;
    }

    var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + comments + "'}";
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
                        ActionMessage("Acknowledged", "You have just acknowledged this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Pending Workflow");
                        $(obj).attr("disabled", "disabled");
                        $(obj).next().attr("disabled", "disabled");
                        editBuilder(obj, requestId);
                        break;
                    case '2':
                        ActionMessage("Change Requested", "You have just requested a change.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Change Requested");
                        disableBladeActions(obj);
                        addComments(obj, "Change Requested", comments);
                        break;
                    case '3':
                        ActionMessage("Closed Cancelled", "You have just closed this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Closed Cancelled");
                        disableBladeActions(obj);
                        addComments(obj, "Closed Cancelled", comments);
                        animateActions(obj, "Closed Requests");
                        hideSections(obj);
                        updateRequestStatus(obj);
                        break;
                    case '1':
                        ActionMessage("Closed Denied", "This request has been closed.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Closed Denied");
                        disableBladeActions(obj);
                        addComments(obj, "Closed Denied", comments);
                        animateActions(obj, "Closed Requests");
                        hideSections(obj);
                        updateRequestStatus(obj);
                        break;
                }
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
function disableBladeActions(obj) {
    $(obj).closest("div.csm_content_container").find("input").each(function() {
        $(this).attr("disabled", "disabled");
    });
}
function addComments(obj, status, comments) {
    var newcomment = "<p class='csm_error_text'><u>" + status + " by (GET ACTOR NAME) on " + curr_date + "</u><br />" + comments + "</p>";
    $(obj).closest("div.csm_hidden_block").children().find("span").each(
 function() {
     if ($(this).attr("id").indexOf("_workflowStatus") > -1) {
         if ($(this).html() == status) {
             $(this).closest("div.csm_data_row").next().html($(this).closest("div.csm_data_row").next().html() + newcomment);
         }
     }
 });
}
function audienceClick(obj) {
    $(obj).closest("table").next().find("input[type=button]").removeAttr("disabled");
}

function AccessComments(obj, requestId) {
    var comments;
    var action = $(obj).parent().prev().find("input[name=_audience]:checked").val();
    var textarea = $(obj).parent().prev().find("textarea");

    if (textarea.val() != "") {
        comments = textarea.val();
        var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + comments + "'}";
        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "AjaxCalls.aspx/AccessComments",
            data: postData,
            dataType: "json",
            success: function(msg) {
                if (msg.d) {
                    ActionMessage("Access Comment", "Your comment has been added to this request.");
                }
                else {
                    ActionMessage("Action Failed","");
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
        ActionMessage("Validation Error","Please insert comment");
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
            builderActions(obj, requestId, state);
            break;
        case "Closed Completed":
            builderActions(obj, requestId, state);
            break;
        case "Create Ticket":
            builderActions(obj, requestId, state);
            break;
        case "Edit Workflow":
            editBuilder(obj, requestId);
            break;
        case "Create Workflow":
            createWorkflow(obj, requestId);
            break;
        case "Continue Workflow":
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
        //$("#closed_cancelled_" + requestId).removeAttr("disabled");
        //$("#create_workflow_" + requestId).removeAttr("disabled");
        //$(obj).attr("disabled", "disabled");

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
        $("#_selectedActors_" + requestId).val("");
        //$("#closed_cancelled_" + requestId).attr("disabled", "disabled");
        //$("#create_workflow_" + requestId).attr("disabled", "disabled");
        //$(obj).attr("disabled", "disabled");

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
                        ActionMessage("Closed Cancelled", "You have just cancelled this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Closed Cancelled");
                        animateActions(obj, "Closed Requests");
                        hideSections(obj);
                        updateRequestStatus(obj);
                        break;
                    case "6":
                        ActionMessage("Closed Completed", "You have just completed this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Closed Completed");
                        animateActions(obj, "Closed Requests");
                        hideSections(obj);
                        updateRequestStatus(obj);
                        break;
                    case "5":
                        ActionMessage("Pending Provisioning", "A ticket has been created to provision the access for this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Pending Provisioning");
                        break;
                }
            }
            else { ActionMessage("Action Failed",""); }
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
            if ($(this).attr("id").indexOf("_workflowActorName") > -1) {
                if ($(this).html() == approverName) {
                    //                    if ($(this).parent().next().next().next().children().html() == "-") {
                    $(this).parent().next().children().html(newStatus);
                    $(this).parent().next().next().next().children().html("<span>" + curr_date + "</span>")
                    //                    }
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
                        ActionMessage("Workflow Created", "The workflow has been created for this request.");
                        updateRequestTracking(obj, "Access &amp; Identity Management", "Workflow Created");
                        disableBuilder(obj, requestId);
                    }
                    else {
                        ActionMessage("Workflow Creation Failed", "The workflow creation failed.");
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
                        ActionMessage("Workflow Updated", "The workflow has been updated for this request.");
                        disableBuilder(obj, requestId);
                    }
                    else {
                        ActionMessage("Workflow Updated Failed", "The workflow update failed. No changes where made.");
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
                GetNames(managerInputDisplayName);
            }
            else {
                managerInputSection.hide();
                managerLabelSection.show();
            }
        });
    });
}
function GetNames(name) {
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
            if (names.length == 0) {
                FillErrorFields(name);
            }

            // direct match
            if (names.length == 1) {
                FillAllFields(name, names);
            }

            // match list of names
            if (names.length > 1) {
                FillSelection(name, names);
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
function FillAllFields(name, names) {
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
}
function FillErrorFields(name) {
    var managerInputDisplayName = $(name).parent().children("input[type=text]");
    var managerInputUserId = $(name).parent().prev().children("span").next();

    $("#_managerSelectionDiv").dialog("destroy");
    managerInputDisplayName.val("No such name! Try again");
    managerInputUserId.val("");
    managerInputDisplayName.focus();
}
function FillSelection(name, names) {
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
function ActionMessage(header, message) {
    $(document).ready(function() {
        $('div.messageBox').children("h2").html(header);
        $('div.messageBox').children("p").html(message);
        $('#_actionMessageDiv').fadeIn().delay(2000).fadeOut();
    });
}
//]]>