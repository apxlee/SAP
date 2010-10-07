//<![CDATA[
$(document).bind('keyup', 'alt+ctrl+s', ToggleQuickSearch);

var m_names = new Array("Jan", "Feb", "Mar",
    "Apr", "May", "Jun", "Jul", "Aug", "Sep",
    "Oct", "Nov", "Dec");

var d = new Date();
var curr_day = d.getDate();
var curr_month = d.getMonth();
var curr_year = d.getFullYear();
var curr_date = m_names[curr_month] + " " + curr_day + ", " + curr_year;
var currentFilterClass = "filter_view_all";

$(document).ready(function() {
    GetAccessTeamFilter();
    var quicksearchInput = $("#__quicksearchInput");
    quicksearchInput.keyup(function() {
        QuickFilter(quicksearchInput.val());
    });
});

function ToggleQuickSearch() {
    var quicksearch = $("#__quicksearch");
    var quicksearchInput = $("#__quicksearchInput");
    if (quicksearch.hasClass("hide")) {
        quicksearch.removeClass("hide");
        quicksearchInput.focus();
    }
    else {
        quicksearch.addClass("hide");
        QuickFilter("");
    }
}
function QuickFilter(quicksearch) {
    $("#_openRequestsContainer").find("tr.csm_stacked_heading_label").each(function() {
        var found = false;
        var blade = $(this).closest("div.csm_content_container");
        $(this).find("span").each(function() {
            if ($(this).html().indexOf(quicksearch) > -1) {
                found = true;
            }
        });
        if (found) { $(this).closest("div.csm_content_container").show(); $(this).closest("div.csm_content_container").next().show(); }
        else { $(this).closest("div.csm_content_container").hide(); $(this).closest("div.csm_content_container").next().hide(); }
    });
}
function GetAccessTeamFilter() {
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetAccessTeamFilter",
        dataType: "json",
        success: function(msg) {
            if (msg.d.length > 0) {
                UpdateFilterCounts(msg.d);
            }
        }
    });
}
function UpdateFilterCounts(filter) {
    var ackCount = 0;
    var ackIds = "";
    var wrkCount = 0;
    var wrkIds = "";
    var prvCount = 0;
    var prvIds = "";
    var inCount = 0;
    var inIds = "";
    var data = jQuery.parseJSON(filter);
    $.each(data.Filters, function(index, value) {
        switch (value.FilterName) {
            case "Pending Acknowledgement":
                ackCount = value.RequestIds.length;
                $.each(value.RequestIds, function(index, requestId) {
                    ackIds = ackIds + "[" + requestId + "]";
                });
                break;
            case "Pending Workflow":
                wrkCount = value.RequestIds.length;
                $.each(value.RequestIds, function(index, requestId) {
                    wrkIds = wrkIds + "[" + requestId + "]";
                });
                break;
            case "Pending Provisioning":
                prvCount = value.RequestIds.length;
                $.each(value.RequestIds, function(index, requestId) {
                    prvIds = prvIds + "[" + requestId + "]";
                });
                break;
            case "In Workflow":
                inCount = value.RequestIds.length;
                $.each(value.RequestIds, function(index, requestId) {
                    inIds = inIds + "[" + requestId + "]";
                });
                break;
        }
    });
    $("#filter_pending_acknowledgement_count").html(ackCount);
    $("#filter_pending_acknowledgement").attr("requestIds", ackIds);
    if (ackCount > 0) { BindFilter($("#filter_pending_acknowledgement")); }
    else { UnbindFilter($("#filter_pending_acknowledgement")); }

    $("#filter_pending_workflow_count").html(wrkCount);
    $("#filter_pending_workflow").attr("requestIds", wrkIds);
    if (wrkCount > 0) { BindFilter($("#filter_pending_workflow")); }
    else { UnbindFilter($("#filter_pending_workflow")); }

    $("#filter_pending_provisioning_count").html(prvCount);
    $("#filter_pending_provisioning").attr("requestIds", prvIds);
    if (prvCount > 0) { BindFilter($("#filter_pending_provisioning")); }
    else { UnbindFilter($("#filter_pending_provisioning")); }

    $("#filter_in_workflow_count").html(inCount);
    $("#filter_in_workflow").attr("requestIds", inIds);
    if (inCount > 0) { BindFilter($("#filter_in_workflow")); }
    else { UnbindFilter($("#filter_in_workflow")); }
}
function BindFilter(obj) {
    obj.removeClass("disabled_text");
    obj.bind('click', function() { FilterClick(this); });
    obj.bind('mouseenter', function() { FilterHover(this); });
    obj.bind('mouseleave', function() { FilterHover(this); });
}
function UnbindFilter(obj) {
    obj.addClass("disabled_text");
    obj.unbind("click");
    obj.unbind("mouseenter");
    obj.unbind("mouseleave");
}
function FilterHover(obj) {
    if ($(obj).attr('id') != currentFilterClass) {
        if ($(obj).hasClass("active_carrot")) {
            $(obj).removeClass("active_carrot");
            $(obj).addClass("hover_carrot");
            $("#access_filter_container").addClass($(obj).attr("id"));
        }
        else {
            $(obj).addClass("active_carrot");
            $(obj).removeClass("hover_carrot");
            $("#access_filter_container").removeClass($(obj).attr("id"));
        }
    }
}
function FilterClick(obj) {
    var filter = $(obj).attr("snap");
    var requestIds = $(obj).attr("requestIds");

    $("#_openRequestsContainer").find("div.csm_content_container").each(function() {
        var blade = this;
        if (filter == "All") { $(blade).show(); $(blade).next().show(); }
        else { $(blade).hide(); $(blade).next().hide(); }
    });
    if (requestIds > "") {
        var requestList = requestIds.split("[");
        $.each(requestList, function(index, value) {
            var requestId = value.replace("]", "");
            if (requestId > "") {
                $("#__requestContainer_" + requestId).show();
                $("#__requestContainer_" + requestId).next().show();
            }
        });
    }
    currentFilterClass = $(obj).attr('id');
    $(obj).addClass("active_carrot");
    $(obj).removeClass("hover_carrot");
    $("div[id='access_filter_container']").attr("class", $(obj).attr('id'));
}
function CreateBlades(requests) {
    var openCount = 0;
    var closedCount = 0;
    if (requests.length > 0) {
        $.each(requests, function(index, value) {
            var data = value;
            var newRequestBlade = $("#_requestBlade").html();
            newRequestBlade = newRequestBlade.replace("__requestContainer_ID", "__requestContainer_" + data.RequestId)
        .replace("__affectedEndUserName_TEXT", data.DisplayName)
        .replace("__affectedEndUserName_ID", "__affectedEndUserName_" + data.RequestId)
        .replace("__overallRequestStatus_TEXT", data.RequestStatus)
        .replace("__overallRequestStatus_ID", "__overallRequestStatus_" + data.RequestId)
        .replace("__lastUpdatedDate_TEXT", data.LastModified)
        .replace("__lastUpdatedDate_ID", "__lastUpdatedDate_" + data.RequestId)
        .replace("__requestId_TEXT", data.RequestId)
        .replace("__requestId_ID", "__requestId_" + data.RequestId)
        .replace("__toggleIconContainer_ID", "__toggleIconContainer_" + data.RequestId)
        .replace("__toggledContentContainer_ID", "__toggledContentContainer_" + data.RequestId)
        .replace("__snapRequestId", data.RequestId)
        .replace("__legendToggle_ID", "__legendToggle_" + data.RequestId)
        .replace("__legend_ID", "__legend_" + data.RequestId)
        .replace("__requestTrackingSection_ID", "__requestTrackingSection_" + data.RequestId);

            if (data.RequestStatus != "Closed") { $("#_openRequestsContainer").append($(newRequestBlade)); openCount++; }
            else { $("#_closedRequestsContainer").append($(newRequestBlade)); closedCount++ }
            $("#__toggleIconContainer_" + data.RequestId).hover(function() {
                $(this).addClass("csm_toggle_show_hover");
            },
		    function() {
		        $(this).removeClass("csm_toggle_show_hover");
		    }
	    );
            $("#__toggleIconContainer_" + data.RequestId).bind('click', function() {
                ToggleDetails(data.RequestId);
            });
        });

        var selectedRequestId = $("input[id*='_hiddenSelectedRequestId']");
        if (selectedRequestId.val() != "") { ToggleDetails(selectedRequestId.val()); selectedRequestId.val(""); }

        //ToggleRequestLoader();
    }

    if (openCount == 0) {
        var newNullOpen = $("#_nullDataMessage").html().replace("__nullDataMessage_ID", "__nullDataMessage_OpenRequests")
        .replace("__message_TEXT", "There are no Open Requests at this time.");
        $("#_openRequestsContainer").append($(newNullOpen));
    }
    if (closedCount == 0) {
        var newNullClosed = $("#_nullDataMessage").html().replace("__nullDataMessage_ID", "__nullDataMessage_ClosedRequests")
        .replace("__message_TEXT", "There are no Closed Requests at this time.");
        $("#_closedRequestsContainer").append($(newNullClosed));
    }
}
function CreateRequestDetails(details, requestId) {
    var data = jQuery.parseJSON(details);
    var ADManagaer = "";
    if (data.ADManager != null) { ADManagaer = "[Active Directory: " + data.ADManager + "]"; }
    var newRequestDetails = $("#_requestDetails").html();
    newRequestDetails = newRequestDetails.replace("__affectedEndUserTitle_TEXT", data.Title)
    .replace("__affectedEndUserTitle_ID", "__affectedEndUserTitle_" + requestId)
    .replace("__requestorsManager_TEXT", data.Manager)
    .replace("__requestorsManager_ID", "__requestorsManager_" + requestId)
    .replace("__adManagerName_TEXT", ADManagaer)
    .replace("__adManagerName_ID", "__adManagerName_" + requestId)
    .replace("__requestorName_TEXT", data.Requestor)
    .replace("__requestorName_ID", "__requestorName_" + requestId);

    var newForm = "";
    $.each(data.Details, function(index, value) {
        var newField = $("#_requestFormField").html();
        newField = newField.replace("__fieldLabel", value.Label + ":")
        .replace("__fieldText",value.Text.replace(/(\r\n|[\r\n])/g, "<br />"));
        newForm = newForm + newField
    });

    var newCommentSection = "";

    if (data.Comments.length > 0) {
        var newComments = "";
        $.each(data.Comments, function(index, value) {
            var newComment = $("#_requestComment").html();
            newComment = newComment.replace("__commentLabel", value.CreatedDate + " for " + value.Audience).replace("__commentText", value.Text.replace(/(\r\n|[\r\n])/g, "<br />"));
            newComments = newComments + newComment
        });

        newCommentSection = $("#_requestCommentSection").html();
        newCommentSection = newCommentSection.replace("<!--__requestComments-->", newComments);
    }

    newRequestDetails = newRequestDetails.replace("<!--__requestFormDetails-->", newForm);
    newRequestDetails = newRequestDetails.replace("<!--__requestCommentSection-->", newCommentSection);

    $("#__toggledContentContainer_" + requestId).html($("#__toggledContentContainer_" + requestId).html()
    .replace("<!--__requestDetailsSection-->", newRequestDetails));

    if ($("#__overallRequestStatus_" + requestId).html() != "Closed") { GetBuilder(requestId); }
    else { GetTracking(null, null, requestId, false); }
    
}
function GetBuilder(requestId) {
    var postData = "{\"requestId\":\"" + requestId + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUI.aspx/GetBuilder",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d.length > 0) {
                CreateBuilder(msg.d, requestId);
            }
        }
    });
}
function CreateBuilder(builder, requestId) {
    var data = jQuery.parseJSON(builder);
    var newAck = $("#_acknowledgement").html();
    newAck = newAck.replace(/__acknowledge_ID/g, "__acknowledge_" + requestId)
    .replace(/__changeDenyCancel_ID/g, "__changeDenyCancel_" + requestId)
    .replace("__radioChange_ID", "__radioChange_" + requestId)
    .replace("__radioDeny_ID", "__radioDeny_" + requestId)
    .replace("__radioCancel_ID", "__radioCancel_" + requestId)
    .replace("__actionComments_ID", "__actionComments_" + requestId)
    .replace("__requestChange_ID", "__requestChange_" + requestId)
    .replace("__requestDeny_ID", "__requestDeny_" + requestId)
    .replace("__requestCancel_ID", "__requestCancel_" + requestId);

    $("#__toggledContentContainer_" + requestId).html($("#__toggledContentContainer_" + requestId).html()
    .replace("<!--__requestAcknowledgement-->", newAck));

    switch (data.AccessTeamState) {
        case "Pending_Acknowledgement":
            $("#__acknowledge_" + requestId).removeAttr("disabled");
            break;
        case "Pending_Workflow": 
            $("#__radioCancel_" + requestId).removeAttr("disabled");
            $("#__radioChange_" + requestId).removeAttr("disabled");
            $("#__radioDeny_" + requestId).removeAttr("disabled");
            break;
        default:
            $("#__radioCancel_" + requestId).removeAttr("disabled");
            break;
    }

    var newBuilder = $("#_workflowBuilder").html();
    newBuilder = newBuilder.replace("__workflowBuilder_ID", "__workflowBuilder_" + requestId)
    .replace("__managerLabelSection_ID", "__managerLabelSection_" + requestId)
    .replace("__managerDisplayName_TEXT", data.ManagerDisplayName)
    .replace("__managerDisplayName_ID", "__managerDisplayName_" + requestId)
    .replace("__managerInputSection_ID", "__managerInputSection_" + requestId)
    .replace("__managerUserId_TEXT", data.ManagerUserId)
    .replace("__managerUserId_ID", "__managerUserId_" + requestId)
    .replace("__managerName_ID", "__managerName_" + requestId)
    .replace("__checkManagerName_ID", "__checkManagerName_" + requestId)
    .replace("__managerEditButton_ID", "__managerEditButton_" + requestId);
    var addSpecial = true;
    var addTechnical = true;
    var groupSection = "";
    var largeGroupSection = "";
    $.each(data.AvailableGroups, function(index, group) {
        if (group.IsLargeGroup) {
            var largeGroup = $("#_workflowLargeGroup").html();
            largeGroup = largeGroup.replace("__workflowBuilderLargeGroupTitle_TEXT", group.GroupName)
            .replace("__workflowBuilderLargeGroupDescription_TEXT", group.Description)
            .replace("__dropdownActors_ID_GROUPID", "__dropdownActors_" + requestId + "_" + group.GroupId)
            .replace("__actorDisplayName_ID_GROUPID", "__actorDisplayName_" + requestId + "_" + group.GroupId)
            .replace("__actorUserId_ID_GROUPID", "__actorUserId_" + requestId + "_" + group.GroupId)
            .replace("__actorActorId_ID_GROUPID", "__actorActorId_" + requestId + "_" + group.GroupId)
            .replace("__actorGroupId_ID_GROUPID", "__actorGroupId_" + requestId + "_" + group.GroupId)
            .replace("__checkActor_ID_GROUPID", "__checkActor_" + requestId + "_" + group.GroupId)
            .replace("__addActor_ID_GROUPID", "__addActor_" + requestId + "_" + group.GroupId);
            var selected = "";
            var options = "";
            $.each(group.AvailableActors, function(index, actor) {
                if (actor.IsSelected) {
                    var newSelected = $("#_workflowLargeGroupActorSelected").html();
                    newSelected = newSelected.replace("__actorId", actor.ActorId)
                    .replace("__actorDisplayName", actor.DisplayName);
                    selected = selected + newSelected;
                }
                else {
                    var newOption = $("#_workflowLargeGroupActorOption").html();
                    newOption = newOption.replace("__actorId", actor.ActorId)
                    .replace("__actorDisplayName", actor.DisplayName);
                    options = options + newOption;
                }
            });
            largeGroup = largeGroup.replace("<!--__actorsSelected-->", selected)
            .replace("<!--__actorOptions-->", options);
            largeGroupSection = largeGroupSection + largeGroup;
        }
        else {
            var newGroup = $("#_actorGroup").html();
            if (addSpecial || addTechnical) {
                switch (group.ActorGroupType) {
                    case 0:
                        newGroup = newGroup.replace("<!--__actorGroupTitle-->", $("#_workflowBuilderSpecial").html());
                        addSpecial = false;
                        break;
                    case 1:
                        newGroup = newGroup.replace("<!--__actorGroupTitle-->", $("#_workflowBuilderTechnical").html());
                        addTechnical = false;
                        break;
                }
            }
            newGroup = newGroup.replace("__actorGroup_ID_GROUPID", "__actorGroup_" + requestId + "_" + group.GroupId)
            .replace("__actorGroupCheckbox_ID_GROUPID", "__actorGroupCheckbox_" + requestId + "_" + group.GroupId)
            .replace("__requestId", requestId)
            .replace("__actorGroupName_ID_GROUPID", "__actorGroupName_" + requestId + "_" + group.GroupId)
            .replace("__actorGroupName_TEXT", group.GroupName)
            .replace("__actorGroupDescription_ID_GROUPID", "__actorGroupDescription_" + requestId + "_" + group.GroupId);
            if (group.Description != null) { newGroup = newGroup.replace("__actorGroupDescription_TEXT", group.Description); }
            else { newGroup = newGroup.replace("__actorGroupDescription_TEXT", ""); }
            var actorSection = "";
            $.each(group.AvailableActors, function(index, actor) {
                var newActor = $("#_workflowActor").html();
                newActor = newActor.replace("__radio_ID_GROUPID_ACTORID", "__radio_" + requestId + "_" + group.GroupId + "_" + actor.ActorId)
                .replace("__radio_ID_GROUPID", "__radio_" + requestId + "_" + group.GroupId)
                .replace("_ACTORID", actor.ActorId);
                if (actor.IsDefault) { newActor = newActor.replace("__actorDisplayName_TEXT", actor.DisplayName + " (Default)"); }
                else { newActor = newActor.replace("__actorDisplayName_TEXT", actor.DisplayName); }

                if (group.IsSelected) {
                    if (!actor.IsSelected) { newActor = newActor.replace("checked=\"checked\"", ""); }
                }
                else {
                    newGroup = newGroup.replace("checked=\"checked\"", "");
                    if (!actor.IsDefault) { newActor = newActor.replace("checked=\"checked\"", ""); }
                }

                actorSection = actorSection + newActor;
            });
            newGroup = newGroup.replace("<!--__actorSection-->", actorSection);
            groupSection = groupSection + newGroup;

        }
    });
    var buttonSection = "";
    $.each(data.AvailableButtons, function(index, button) {
        var newButton = $("#_workflowBuilderButton").html();
        newButton = newButton.replace("__builderButton_ID", button.ButtonId)
        .replace("__builderButton_TEXT", button.ButtonName);
        buttonSection = buttonSection + newButton;
    });

    newBuilder = newBuilder.replace("<!--__workflowBuilder_normal_approvers-->", groupSection)
    .replace("<!--__workflowBuilder_large_group_approvers-->", largeGroupSection)
    .replace("<!--__workflowBuilderButtons-->", buttonSection)
    .replace("__selectedActors_ID", "__selectedActors_" + requestId);
    $("#__toggledContentContainer_" + requestId).html($("#__toggledContentContainer_" + requestId).html()
    .replace("<!--__requestWorkflowBuilder-->", newBuilder));
    if (data.IsDisabled) { DisableBuilder(requestId); }

    var newComments = $("#_comments").html();
    newComments = newComments.replace(/_audience_ID/g, "_audience_" + requestId)
    .replace("__radioAIM_ID", "__radioAIM_" + requestId)
    .replace("__radioApprovers_ID", "__radioApprovers_" + requestId)
    .replace("__radioEveryone_ID", "__radioEveryone_" + requestId)
    .replace("__comments_ID", "__comments_" + requestId)
    .replace(/__submitComments_ID/g, "__submitComments_" + requestId);

    $("#__toggledContentContainer_" + requestId).html($("#__toggledContentContainer_" + requestId).html()
    .replace("<!--__requestComments-->", newComments));
    
    GetTracking(data.AvailableGroups, data.AvailableButtons, requestId, false);
}

function BindEvents(builderGroups, builderButtons, requestId) {
    
    $("#__managerEditButton_" + requestId).click(function() {
        ManagerEdit(requestId);
    });

    var builder = $("#__workflowBuilder_" + requestId);

    BindApproverGroupCheckboxes(requestId);
    
    if (builderGroups != null) {
        $.each(builderGroups, function(index, group) {
            if (group.IsLargeGroup) {
                $("#__dropdownActors_" + requestId + "_" + group.GroupId).change(function() {
                    ActorSelected(this, requestId, group.GroupId);
                });
                $("#__actorDisplayName_" + requestId + "_" + group.GroupId).keyup(function() {
                    ActorChanged(requestId, group.GroupId);
                });
                $("#__checkActor_" + requestId + "_" + group.GroupId).click(function() {
                    ActorCheck(this, requestId, group.GroupId);
                });
                $("#__addActor_" + requestId + "_" + group.GroupId).click(function() {
                    ActorAdd(this, requestId, group.GroupId);
                });
            }
        });
    }
    
    builder.find("td.listview_button").each(function() {
    $(this).children("input[type=button]").click(function() {
            RemoveActor(this, requestId);
        });
    });
    
    if (builderButtons != null) {
        $.each(builderButtons, function(index, button) {
            var builderButton = $("#" + button.ButtonId);
            if (!button.IsDisabled) { builderButton.removeAttr("disabled"); }
            builderButton.click(function() {
                ActionClicked(builderButton, requestId, button.ActionId);
            });
        });
    }
    
    $("#__radioAIM_" + requestId).click(function() {
        AudienceClick(this);
    });

    $("#__radioApprovers_" + requestId).click(function() {
        AudienceClick(this);
    });

    $("#__radioEveryone_" + requestId).click(function() {
        AudienceClick(this);
    });

    $("#__submitComments_" + requestId).click(function() {
        AccessComments(this, requestId);
    });

    $("#__acknowledge_" + requestId).click(function() {
        AccessTeamActions(this, requestId, '4');
    });

    $("#__radioChange_" + requestId).click(function() {
        ChangeDenyCancelClick(this, requestId);
    });

    $("#__radioDeny_" + requestId).click(function() {
        ChangeDenyCancelClick(this, requestId);
    });

    $("#__radioCancel_" + requestId).click(function() {
        ChangeDenyCancelClick(this, requestId);
    });

    $("#__requestChange_" + requestId).click(function() {
        AccessTeamActions(this, requestId, '2');
    });

    $("#__requestDeny_" + requestId).click(function() {
        AccessTeamActions(this, requestId, '1');
    });

    $("#__requestCancel_" + requestId).click(function() {
        AccessTeamActions(this, requestId, '3');
    });

    $("#__legendToggle_" + requestId).click(function() {
        ToggleLegend(requestId);
    });
    
    ToggleLoading(requestId);
}
function ChangeDenyCancelClick(obj, requestId) {
    $("#__actionComments_" + requestId).removeAttr("disabled");
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
    var textarea = $("#__actionComments_" + requestId);
    ProcessingMessage("Updating Request", "");
    switch (action) {
        case '2':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please specify the change required."); return false; }
            break;
        case '3':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please specify the reason for cancel."); return false; }
            break;
        case '1':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please specify the reason for denial."); return false; }
            break;
    }
    var newAction = new Comment(requestId, action, textarea.val());
    comments = "<br />" + textarea.val().replace(/(\r\n|[\r\n])/g, "<br />");
    textarea.val("");
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxActions.aspx/AccessTeamActions",
        data: newAction.toJSON,
        dataType: "json",
        success: function(msg) {
            if (msg.d.Success) {
                switch (action) {
                    case '4':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Acknowledged", "You have just acknowledged this request, you may now create its workflow.");
                        UpdateRequestTracking(requestId);
                        $(obj).attr("disabled", "disabled");
                        $(obj).closest("tr").next().children("td.csm_input_form_control_column").find("input").each(function() {
                            $(this).removeAttr("disabled");
                        });
                        $("#create_workflow_" + requestId).removeAttr("disabled");
                        //AddComments(requestId, "Access &amp; Identity Management", "Acknowledged", "", true);
                        EditBuilder($("#closed_cancelled_" + requestId), requestId);
                        $("#__overallRequestStatus_" + requestId).html("Pending");
                        GetAccessTeamFilter();
                        break;
                    case '2':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Change Requested", "You have just requested a change.");
                        UpdateRequestTracking(requestId);
                        DisableBladeActions(obj);
                        DisableBuilder(requestId);
                        //AddComments(requestId, "Access &amp; Identity Management", "Change Requested", comments, false);
                        $("#__overallRequestStatus_" + requestId).html("Change Requested");
                        GetAccessTeamFilter();
                        UpdateCount("_accessTeamCount");
                        break;
                    case '3':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Cancelled", "You have just closed this request with this cancellation.");
                        UpdateRequestTracking(requestId);
                        DisableBladeActions(obj);
                        DisableBuilder(requestId);
                        //AddComments(requestId, "Access &amp; Identity Management", "Closed Cancelled", comments, false);
                        AnimateActions("Closed Requests", requestId);
                        HideSections(obj);
                        $("#__overallRequestStatus_" + requestId).html("Closed");
                        GetAccessTeamFilter();
                        UpdateCount("_accessTeamCount");
                        break;
                    case '1':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Denied", "You have just closed this request with this denial.");
                        UpdateRequestTracking(requestId);
                        DisableBladeActions(obj);
                        DisableBuilder(requestId);
                        //AddComments(requestId, "Access &amp; Identity Management", "Closed Denied", comments, false);
                        AnimateActions("Closed Requests", requestId);
                        HideSections(obj);
                        $("#__overallRequestStatus_" + requestId).html("Closed");
                        GetAccessTeamFilter();
                        UpdateCount("_accessTeamCount");
                        break;
                }                
            }
            else {
                MessageDialog(msg.d.Title, msg.d.Message);
            }
        }
    });
}
function AudienceClick(obj) {
    $(obj).closest("table").next().find("input[type=button]").removeAttr("disabled");
}
function AccessComments(obj, requestId) {
    var comments = "";
    var postComments = ""
    var newNotes = true;
    var action = $(obj).parent().prev().find("input[name=_audience_" + requestId + "]:checked");
    var notesFor = action.next().html();
    var textarea = $("#__comments_" + requestId);

    ProcessingMessage("Adding Comments", "");
    if (textarea.val() != "") {
        var newAction = new Comment(requestId, action.val(), textarea.val());
        comments = textarea.val().replace(/(\r\n|[\r\n])/g, "<br />"); ;
        textarea.val("");
        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "ajax/AjaxActions.aspx/AccessComments",
            data: newAction.toJSON,
            dataType: "json",
            success: function(msg) {
                if (msg.d.Success) {
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
                    MessageDialog(msg.d.Title, msg.d.Message);

                }
            }
        });
    }
    else {
        ActionMessage("Required Input", "Please insert comment");
    }
}
function DisableBuilder(requestId) {
    var editLink = $("#__managerEditButton_" + requestId);
    var builder = $("#__workflowBuilder_" + requestId);
    builder.find("input[type=checkbox]").attr("disabled", "disabled");
    builder.find("input[type=radio]").attr("disabled", "disabled");
    builder.find("input[type=text]").attr("disabled", "disabled");
    builder.find("input[type=button]").attr("disabled", "disabled");
    builder.find("select").attr("disabled", "disabled");
    $("#closed_cancelled_" + requestId).removeAttr("disabled");
    $("#__selectedActors_" + requestId).val("");
    editLink.removeClass("oospa_edit_icon");
    editLink.addClass("oospa_edit_icon_disabled");
    editLink.unbind("click");
}
function ManagerEdit(requestId) {
    var managerLabelSection = $("#__managerLabelSection_" + requestId);
    var managerLabelDispalyName = $("#__managerDisplayName_" + requestId);
    var managerInputSection = $("#__managerInputSection_" + requestId);
    var managerInputUserId = $("#__managerUserId_" + requestId);
    var managerInputDisplayName = $("#__managerName_" + requestId);
    var managerInputCheckButton = $("#__checkManagerName_" + requestId);
    managerLabelSection.hide();
    managerInputDisplayName.val(managerLabelDispalyName.html());
    managerInputSection.show();

    managerInputDisplayName.bind('change', function() {
        managerInputUserId.val("");
    });
    managerInputCheckButton.click(function() {
        if (managerInputUserId.val() == "") {
            GetNames(requestId, 0, managerInputDisplayName, "manager");
        }
        else {
            managerInputSection.hide();
            managerLabelSection.show();
        }
    });
}
function GetNames(requestId, groupId, name, section) {
    var indicator = $('.oospa_ajax_indicator');
    var selection = $('select[id$=_managerSelection]');
    var postData = "{\"name\":\"" + name.val().replace("(", "").replace(")", "").replace(/\\/, "").replace("'", "\\'") + "\"}";
    selection.hide();
    indicator.show();
    OpenDialog(name);
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxPersons.aspx/GetNames",
        data: postData,
        dataType: "json",
        success: function(msg) {
            var names = msg.d;
            // no match
            if (section == "manager") {
                if (names.length == 0) {
                    FillManagerErrorFields(requestId, name);
                }
                // direct match
                if (names.length == 1) {
                    FillManagerAllFields(requestId, name, names);
                }
                // match list of names
                if (names.length > 1) {
                    FillManagerSelection(requestId, name, names);
                }
            }
            else {
                if (names.length == 0) {
                    FillActorErrorFields(requestId, groupId, name);
                }
                // direct match
                if (names.length == 1) {
                    FillActorAllFields(requestId, groupId, name, names);
                }
                // match list of names
                if (names.length > 1) {
                    FillActorSelection(requestId, groupId, name, names);
                }
            }
        }
    });
}
function FillManagerAllFields(requestId, name, names) {
    var managerLabelSection = $("#__managerLabelSection_" + requestId);
    var managerLabelDispalyName = $("#__managerDisplayName_" + requestId);
    var managerInputSection = $("#__managerInputSection_" + requestId);
    var managerInputUserId = $("#__managerUserId_" + requestId);
    var managerInputDisplayName = $("#__managerName_" + requestId);

    var nameArray = new Array();
    $("#_managerSelectionDiv").dialog("destroy");
    managerInputUserId.val(names[0].LoginId);
    managerLabelDispalyName.html(names[0].Name);
    nameArray = names[0].Name.split("(");
    if (CheckForDuplicateApprovers($.trim(nameArray[0]), requestId, "manager")) {
        ActionMessage("Duplicate Removed", "Duplicate approver has been removed from the workflow");
    }
    managerInputSection.hide();
    managerLabelSection.show();
    UpdateManagerName(requestId, names[0].Name);
}
function FillManagerErrorFields(requestId, name) {
    var managerInputDisplayName = $("#__managerName_" + requestId);
    var managerInputUserId = $("#__managerUserId_" + requestId);
    $("#_managerSelectionDiv").dialog("destroy");
    managerInputDisplayName.val("No such name! Try again");
    managerInputUserId.val("");
    managerInputDisplayName.focus();
}
function FillManagerSelection(requestId, name, names) {
    var managerLabelSection = $("#__managerLabelSection_" + requestId);
    var managerLabelDispalyName = $("#__managerDisplayName_" + requestId);
    var managerInputSection = $("#__managerInputSection_" + requestId);
    var managerInputUserId = $("#__managerUserId_" + requestId);
    var managerInputDisplayName = $("#__managerName_" + requestId);

    var selection = $('select[id$=_managerSelection]');
    var indicator = $('.oospa_ajax_indicator');
    var nameArray = new Array();

    selection.change(function() {
        managerLabelDispalyName.html($('#' + selection.attr('id') + ' :selected').text());
        nameArray = $('#' + selection.attr('id') + ' :selected').text().split("(");
        if (CheckForDuplicateApprovers($.trim(nameArray[0]), requestId, "manager")) {
            ActionMessage("Duplicate Removed", "Duplicate approver has been removed from the workflow");
        }
        managerInputSection.hide();
        managerLabelSection.show();
        managerInputUserId.val($('#' + selection.attr('id') + ' :selected').val());
        $("#_managerSelectionDiv").dialog("destroy");
        UpdateManagerName(requestId, $('#' + selection.attr('id') + ' :selected').text());
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
function CheckForDuplicateApprovers(approver, requestId, source) {
    var managerName = "";
    var approverArray = new Array();
    var nameArray = new Array();
    var i = 0;
    var isApprover = false;
    var duplicate = false;
    var builder = $("#__workflowBuilder_" + requestId);
    var managerName = $("#__managerDisplayName_" + requestId).html();

    if (source != "manager") {
        nameArray = managerName.split("(");
        approverArray[i] = $.trim(nameArray[0]);
        i++;
    }

    builder.find("input[type=radio]").each(function() {
        var checkbox = $(this).closest("table").find("input[type=checkbox]");
        if (checkbox.is(":checked")) {
            if (!checkbox.is(':disabled')) {
                if ($(this).is(":checked")) {
                    nameArray = $(this).next().html().split("(");
                    if (source == "manager") {
                        if ($.trim(nameArray[0]) == approver) {
                            duplicate = true;
                            checkbox.removeAttr("checked");
                            $(this).attr("disabled", "disabled");
                            $(this).closest("table").find("input[type=radio]").each(function() {
                                $(this).attr("disabled", "disabled");
                            });
                            $("#__selectedActors_" + requestId).val($("#__selectedActors_" + requestId).val().replace("[" + $(this).val() + "]", ""));
                        }
                    }
                    else {
                        approverArray[i] = $.trim(nameArray[0]);
                        i++;
                    }
                }
            }
        }
    });

    builder.find("td.listview_td").each(function() {
        nameArray = $(this).html().split("(");
        if (source == "manager") {
            if ($.trim(nameArray[0]) == approver) {
                duplicate = true;
                RemoveActor(this, requestId);
            }
        }
        else {
            approverArray[i] = $.trim(nameArray[0]);
            i++;
        }
    });

    if (!duplicate) {
        $.each(approverArray, function() {
            if (source == "large") {
                if (approver == this) {
                    duplicate = true;
                }
            }
            if (approver == this) {
                if (isApprover) { duplicate = true; }
                else { isApprover = true; }
            }
        });
    }

    return duplicate;
}
function FillActorAllFields(requestId, groupId, name, names) {
    var actorDisplayName = $("#__actorDisplayName_" + requestId + "_" + groupId);
    var actorUserId = $("#__actorUserId_" + requestId + "_" + groupId);
    var addActor = $("#__addActor_" + requestId + "_" + groupId);
    $("#_managerSelectionDiv").dialog("destroy");
    actorUserId.val(names[0].LoginId);
    actorDisplayName.val(names[0].Name);
    addActor.removeAttr("disabled");
}
function FillActorErrorFields(requestId, groupId, name) {
    var actorDisplayName = $("#__actorDisplayName_" + requestId + "_" + groupId);
    var actorUserId = $("#__actorUserId_" + requestId + "_" + groupId);
    $("#_managerSelectionDiv").dialog("destroy");
    actorDisplayName.val("No such name! Try again");
    actorUserId.val("");
    actorDisplayName.focus();
}
function FillActorSelection(requestId, groupId, name, names) {
    var actorDisplayName = $("#__actorDisplayName_" + requestId + "_" + groupId);
    var actorUserId = $("#__actorUserId_" + requestId + "_" + groupId);
    var addActor = $("#__addActor_" + requestId + "_" + groupId);
    var selection = $('select[id$=_managerSelection]');
    var indicator = $('.oospa_ajax_indicator');

    selection.change(function() {
        actorDisplayName.val($('#' + selection.attr('id') + ' :selected').text());
        actorUserId.val($('#' + selection.attr('id') + ' :selected').val());
        $("#_managerSelectionDiv").dialog("destroy");
        addActor.removeAttr("disabled");
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
function BindApproverGroupCheckboxes(requestId) {
    var selectedActors = $("#__selectedActors_" + requestId);
    var builder = $("#__workflowBuilder_" + requestId);

    builder.find("input[type=checkbox]").each(function(index, checkbox) {
        if ($(checkbox).is(":checked")) {
            $(checkbox).closest("table").find("input[type=radio]").each(function(index, radio) {
                if ($(radio).is(":checked")) {
                    selectedActors.val(selectedActors.val() + "[" + $(this).val() + "]");
                }
            });
        }

        $(checkbox).click(function() {
            ApproverGroupChecked(this,requestId);
        });
    });     
}
function ApproverGroupChecked(checkbox, requestId) {
    var selectedActors = $("#__selectedActors_" + requestId);
    var builder = $("#__workflowBuilder_" + requestId);
    var isDuplicate = false;
    var nameArray = new Array();
    var approver;
    
    if ($(checkbox).is(":checked")) {
        $(checkbox).closest("table").find("input[type=radio]").each(function(index, radio) {
            $(radio).removeAttr("disabled");
            $(radio).change(function() {
                ApproverGroupRadioChanged(radio, requestId);
            });
            if ($(radio).is(":checked")) {
                nameArray = $(radio).next().html().split("(");
                approver = $.trim(nameArray[0]);
                isDuplicate = CheckForDuplicateApprovers(approver, requestId, "approver")
                if (isDuplicate) {
                    ActionMessage("Duplicate Selection", approver + " has already be added to this workflow.");
                    $(checkbox).removeAttr("checked");
                }
                else {
                    selectedActors.val(selectedActors.val() + "[" + $(radio).val() + "]");
                }
            }
        });
    }
    else {
        $(checkbox).closest("table").find("input[type=radio]").each(function(index, radio) {
            $(radio).attr("disabled","disabled");
            $(radio).unbind("change");
            if ($(radio).is(":checked")) {
                selectedActors.val(selectedActors.val().replace("[" + $(radio).val() + "]", ""));
            }
        });
    }
    if (isDuplicate) {
        $(checkbox).closest("table").find("input[type=radio]").each(function(index, radio) {
            $(radio).attr("disabled", "disabled");
        });
    }
}

function ApproverGroupRadioChanged(radioClicked, requestId) {
    var selectedActors = $("#__selectedActors_" + requestId);
    var isDuplicate = false;
    var nameArray = new Array();
    var approver;

    nameArray = $(radioClicked).next().html().split("(");
    approver = $.trim(nameArray[0]);
    isDuplicate = CheckForDuplicateApprovers(approver, requestId, "approver");
    
    if (isDuplicate) {
        ActionMessage("Duplicate Selection", approver + " has already be added to this workflow.");
        $(radioClicked).closest("table").find("input[type=radio]").each(function(index, radio) {
            if (selectedActors.val().indexOf("[" + $(radio).val() + "]") > 0) {
                $(radio).attr("checked", "checked");
            }
        });
        $(radioClicked).attr("disabled", "disabled");
    }
    else {
        $(radioClicked).closest("table").find("input[type=radio]").each(function(index, radio) {
            selectedActors.val(selectedActors.val().replace("[" + $(radio).val() + "]", ""));
        });
        selectedActors.val(selectedActors.val() + "[" + $(radioClicked).val() + "]");
    }
}

function ActionClicked(obj, requestId, state) {
    switch ($(obj).val()) {
        case "Closed Cancelled":
            ProcessingMessage("Updating Request", "");
            BuilderActions(obj, requestId, state);
            break;
        case "Closed Completed":
            ProcessingMessage("Updating Request", "");
            BuilderActions(obj, requestId, state);
            break;
        case "Create Ticket":
            ProcessingMessage("Creating Ticket", "")
            BuilderActions(obj, requestId, state);
            break;
        case "Edit Workflow":
            EditBuilder(obj, requestId);
            $(obj).attr("disabled", "disabled");
            break;
        case "Create Workflow":
            ProcessingMessage("Creating Workflow", "");
            CreateWorkflow(obj, requestId);
            break;
        case "Continue Workflow":
            ProcessingMessage("Updating Workflow", "");
            EditCreatedWorkflow(obj, requestId);
            break;
    }
}
function EditBuilder(obj, requestId) {
    var builder = $("#__workflowBuilder_" + requestId);
    builder.find("input[type=checkbox]").each(function(index, checkbox) {
        if ($(checkbox).attr("id").indexOf("_requiredCheck") < 0) {
            $(checkbox).removeAttr("disabled");
            if ($(checkbox).is(":checked")) {
                $(checkbox).closest("table").find("input[type=radio]").each(function(index, radio) {
                    $(radio).removeAttr("disabled");
                    $(radio).change(function() {
                        ApproverGroupRadioChanged(radio, requestId);
                    });
                });
            }
        }
    });
    
    builder.find("input[type=text]").removeAttr("disabled");
    builder.find("input[type=button]").each(function() {
        $(this).removeAttr("disabled");
        if ($(this).val() == "Remove") {
            $("#__selectedActors_" + requestId).val($("#__selectedActors_" + requestId).val() + "[" + $(this).parent().prev().html() + "]");
        }
    });
    builder.find("select").removeAttr("disabled");
    $("#closed_cancelled_" + requestId).removeAttr("disabled");
    $("#continue_workflow_" + requestId).removeAttr("disabled");
    var editLink = $(obj).parent().parent().find(".oospa_edit_icon_disabled");
    editLink.addClass("oospa_edit_icon");
    editLink.removeClass("oospa_edit_icon_disabled");
    editLink.click(function() {
        ManagerEdit(this);
    });
}
function BuilderActions(obj, requestId, state) {
    var postData = "{\"requestId\":\"" + requestId.toString() + "\",\"action\":\"" + state + "\"}";
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxActions.aspx/BuilderActions",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d.Success) {
                switch (state) {
                    case "3":
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Cancelled", "You have just cancelled this request.");
                        UpdateRequestTracking(requestId);
                        AnimateActions("Closed Requests", requestId);
                        HideSections(obj);
                        $("#__overallRequestStatus_" + requestId).html("Closed");
                        GetAccessTeamFilter();
                        UpdateCount("_accessTeamCount");
                        break;
                    case "6":
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Completed", "You have just completed this request.");
                        UpdateRequestTracking(requestId);
                        AnimateActions("Closed Requests", requestId);
                        HideSections(obj);
                        $("#__overallRequestStatus_" + requestId).html("Closed");
                        GetAccessTeamFilter();
                        UpdateCount("_accessTeamCount");
                        break;
                    case "5":
                        $('#_indicatorDiv').hide();
                        ActionMessage("Pending Provisioning", "A ticket has been created to provision the access for this request.");
                        UpdateRequestTracking(requestId);
                        $("#closed_completed_" + requestId).removeAttr("disabled");
                        $("#create_ticket_" + requestId).attr("disabled", "disabled");
                        break;
                }
            }
            else {
                if (state == 5) {
                    MessageDialog(msg.d.Title, msg.d.Message);
                    $("#closed_completed_" + requestId).removeAttr("disabled");
                }
                else {
                    MessageDialog(msg.d.Title, msg.d.Message);
                }
            }
        }
    });
}
function CreateWorkflow(obj, requestId) {
    if ($("#__managerUserId_" + requestId).val() > "") {
        var postData = "{\"requestId\":\"" + requestId.toString() + "\",\"managerUserId\":\"" + $("#__managerUserId_" + requestId).val() + "\",\"actorIds\":\"" + $("#__selectedActors_" + requestId).val() + "\"}";
        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "ajax/AjaxActions.aspx/CreateWorkflow",
            data: postData,
            dataType: "json",
            success: function(msg) {
                if (msg.d.Success) {
                    $('#_indicatorDiv').hide();
                    ActionMessage("Workflow Created", "The workflow has been created for this request.");
                    DisableBuilder(requestId);
                    $("#create_workflow_" + requestId).hide();
                    $("#__radioChange_" + requestId).attr("disabled", "disabled");
                    $("#__radioDeny_" + requestId).attr("disabled", "disabled");
                    
                    var continueButton = $("#closed_cancelled_" + requestId).clone().val("Continue Workflow").attr("id", "continue_workflow_" + requestId).attr("disabled", "disabled");
                    continueButton.insertAfter($("#closed_cancelled_" + requestId));
                    $(continueButton).click(function() {
                        EditCreatedWorkflow($(this), requestId);
                    });

                    var editButton = $("#closed_cancelled_" + requestId).clone().val("Edit Workflow").attr("id", "edit_workflow_" + requestId);
                    editButton.insertAfter($("#closed_cancelled_" + requestId));
                    $(editButton).click(function() {
                        EditBuilder($(this), requestId);
                    });

                    var space = "<b>&nbsp;</b>";
                    $(space).insertAfter($("#closed_cancelled_" + requestId));
                    $(space).insertAfter($("#edit_workflow_" + requestId));

                    if (!$("#access_filter_container").hasClass("filter_view_all")) {
                        ToggleDetails(requestId);
                        $("#__requestContainer_" + requestId).fadeOut(1000);
                    }
                    UpdateRequestTracking(requestId);
                    GetAccessTeamFilter();
                }
                else {
                    MessageDialog(msg.d.Title, msg.d.Message);
                }
            }
        });
    }
    else {
        ActionMessage("Validation Error", "The manager you have selected has not been checked.");
    }
}
function EditCreatedWorkflow(obj, requestId) {
    if ($("#__managerUserId_" + requestId).val() > "") {
        var postData = "{\"requestId\":\"" + requestId.toString() + "\",\"managerUserId\":\"" + $("#__managerUserId_" + requestId).val() + "\",\"actorIds\":\"" + $("#__selectedActors_" + requestId).val() + "\"}";

        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "ajax/AjaxActions.aspx/EditWorkflow",
            data: postData,
            dataType: "json",
            success: function(msg) {
                if (msg.d.Success) {
                    $('#_indicatorDiv').hide();
                    ActionMessage("Workflow Updated", "The workflow has been updated for this request.");
                    DisableBuilder(requestId);
                    $("#edit_workflow_" + requestId).removeAttr("disabled");
            
                    $("#__requestTrackingSection_" + requestId).html("");
                    GetTracking(null, null, requestId, true);
                }
                else {
                    MessageDialog(msg.d.Title, msg.d.Message);
                }
            }
        });
    }
    else {
        ActionMessage("Validation Error", "The manager you have selected has not been checked.");
    }
}
function HideSections(obj) {
    $(obj).closest("div.csm_content_container").find("div.csm_text_container").each(function() {
        if ($(this).children().children().html() == "Acknowledgement" ||
            $(this).children().children().html() == "Workflow Builder" ||
            $(this).children().children().html() == "Comments") {
            $(this).hide();
        }
    });
}
function DisableBladeActions(obj) {
    $(obj).closest("div.csm_content_container").find("input").each(function() {
        $(this).attr("disabled", "disabled");
    });
}

function UpdateManagerName(requestId, newManager) {
    $("#__requestorsManager_" + requestId).html()
}
function ActorSelected(obj, requestId, groupId) {
    var actorDisplayName = $("#__actorDisplayName_" + requestId + "_" + groupId);
    var actorUserId = $("#__actorUserId_" + requestId + "_" + groupId);
    var actorActorId = $("#__actorActorId_" + requestId + "_" + groupId);
    var actorCheckButton = $("#__checkActor_" + requestId + "_" + groupId);
    var addActor = $("#__addActor_" + requestId + "_" + groupId);

    actorCheckButton.attr("disabled", "disabled");
    if ($(obj).val() != "0") {
        actorDisplayName.val($('#' + $(obj).attr('id') + ' option:selected').text());
        actorActorId.val($(obj).val());
        actorUserId.val("");
        addActor.removeAttr("disabled");
    }
    else {
        actorDisplayName.val("");
        actorUserId.val("");
        actorActorId.val("");
    }
}

function ActorChanged(requestId, groupId) {
    var actorDisplayName = $("#__actorDisplayName_" + requestId + "_" + groupId);
    var actorUserId = $("#__actorUserId_" + requestId + "_" + groupId);
    var actorActorId = $("#__actorActorId_" + requestId + "_" + groupId);
    var actorCheckButton = $("#__checkActor_" + requestId + "_" + groupId);
    var addActor = $("#__addActor_" + requestId + "_" + groupId);

    actorUserId.val("");
    actorActorId.val("");
    addActor.attr("disabled", "disabled");

    if (actorDisplayName.val().length == 0) { actorCheckButton.attr("disabled", "disabled"); }
    else {
        actorCheckButton.removeAttr("disabled");
    }
}

function ActorCheck(obj, requestId, groupId) {
    var actorDisplayName = $("#__actorDisplayName_" + requestId + "_" + groupId);
    var actorUserId = $("#__actorUserId_" + requestId + "_" + groupId);
    $(obj).attr("disabled", "disabled");
    if (actorUserId.val() == "") {
        GetNames(requestId, groupId, actorDisplayName, "actor");
    }
}

function ActorAdd(obj, requestId, groupId) {
    var actorDisplayName = $("#__actorDisplayName_" + requestId + "_" + groupId);
    var actorUserId = $("#__actorUserId_" + requestId + "_" + groupId);
    var actorActorId = $("#__actorActorId_" + requestId + "_" + groupId);
    var addActor = $("#__addActor_" + requestId + "_" + groupId);

    var nameArray = new Array();
    if (actorActorId.val() == "") {
        var postData = "{\"userId\":\"" + actorUserId.val() + "\",\"groupId\":\"" + groupId + "\"}";
        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "ajax/AjaxPersons.aspx/GetActorId",
            data: postData,
            dataType: "json",
            success: function(msg) {
                if (msg.d > "0") {
                    nameArray = actorDisplayName.val().split("(");
                    if (CheckForDuplicateApprovers($.trim(nameArray[0]), requestId, "large")) {
                        ActionMessage("Duplicate Selection", $.trim(nameArray[0]) + " has already be added to this workflow.");
                        actorDisplayName.val("");
                        actorUserId.val("");
                        actorActorId.val("");
                    }
                    else {
                        $("#__selectedActors_" + requestId).val($("#__selectedActors_" + requestId).val() + "[" + msg.d + "]");
                        UpdateActorList(requestId, groupId, msg.d);
                    }
                }
            }
        });
    }
    else {
        nameArray = actorDisplayName.val().split("(");
        if (CheckForDuplicateApprovers($.trim(nameArray[0]), requestId, "large")) {
            ActionMessage("Duplicate Selection", $.trim(nameArray[0]) + " has already be added to this workflow.");
            actorDisplayName.val("");
            actorUserId.val("");
            actorActorId.val("");
        }
        else {
            $("#__selectedActors_" + requestId).val($("#__selectedActors_" + requestId).val() + "[" + actorActorId.val() + "]");
            UpdateActorList(requestId, groupId, actorActorId.val());
        }
    }
    addActor.attr("disabled", "disabled");
}

function UpdateActorList(requestId, groupId, actorId) {
    var actorDisplayName = $("#__actorDisplayName_" + requestId + "_" + groupId);
    var actorUserId = $("#__actorUserId_" + requestId + "_" + groupId);
    var actorActorId = $("#__actorActorId_" + requestId + "_" + groupId);
    var listItem;
    var table = $('<table />').attr('class', 'listview_table');
    var tbody = $('<tbody />')
    var tableTr = $('<tr />').attr('class', 'listview_tr');
    var tableButton = $('<td />').attr('class', 'listview_button');
    var removeButton = $("<input type='button'>")
    removeButton.bind('click', function() { RemoveActor(this, requestId); });
    removeButton.val("Remove");

    if (actorDisplayName.closest("tr").next().find("tr").html() == null) {
        $('<td />').attr('class', 'listview_td').html(actorDisplayName.val()).appendTo(tableTr);
        $('<td />').css('display', 'none').html(actorActorId.val()).appendTo(tableTr);
        removeButton.appendTo(tableButton);
        tableButton.appendTo(tableTr);
        tableTr.appendTo(tbody)
        tbody.appendTo(table);
        table.appendTo(actorDisplayName.closest("tr").next().children());
    }
    else {
        $('<td />').attr('class', 'listview_td').html(actorDisplayName.val()).appendTo(tableTr);
        $('<td />').css('display', 'none').html(actorId).appendTo(tableTr);
        removeButton.appendTo(tableButton);
        tableButton.appendTo(tableTr);
        $(tableTr).insertAfter(actorDisplayName.closest("tr").next().find("tr").last());
    }

    actorDisplayName.val('');
    actorUserId.val('');
    actorActorId.val('');
    actorDisplayName.parent().find("option").each(function() {
        if ($(this).val() == actorId) { $(this).remove(); }
    });

}
function RemoveActor(obj, requestId) {
    var actorDisplayName = $(obj).closest("tr.listview_tr").children();
    var actorActorId = $(obj).closest("tr.listview_tr").children().next();
    var actorOption = $("<option value='" + actorActorId.html() + "'>");
    actorOption.html(actorDisplayName.html());
    actorOption.appendTo($(obj).closest("table.oospa_workflow_builder_row").find("select"));

    var selectedActors = $("#__selectedActors_" + requestId);
    selectedActors.val(selectedActors.val().replace("[" + actorActorId.html() + "]", ""));
    
    //finally remove row
    $(obj).closest("tr.listview_tr").remove();
}
// ACCESS TEAM VIEW - END
//]]>