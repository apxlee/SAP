//<![CDATA[
function CreateBlades(requests) {
    var pendingCount = 0;
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
        .replace("__workflowStatus_ID", "__workflowStatus_" + data.RequestId)
        .replace("__workflowStatus_TEXT", data.WorkflowStatus)
        .replace("__legendToggle_ID", "__legendToggle_" + data.RequestId)
        .replace("__legend_ID", "__legend_" + data.RequestId)
        .replace("__requestTrackingSection_ID", "__requestTrackingSection_" + data.RequestId);

            if (data.RequestStatus != "Closed") {
                if (data.WorkflowStatus == 7) {
                    pendingCount = pendingCount + 1
                    $("#_pendingApprovalsContainer").append($(newRequestBlade));
                }
                else { $("#_openRequestsContainer").append($(newRequestBlade)); openCount++; }
            }
            else { $("#_closedRequestsContainer").append($(newRequestBlade)); closedCount++; }

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
        if (pendingCount > 1) { $("#__multiplePendingApprovals").val("yes"); }

        var selectedRequestId = $("input[id*='_hiddenSelectedRequestId']");
        if (selectedRequestId.val() != "") { ToggleDetails(selectedRequestId.val()); selectedRequestId.val(""); }
    }

    if (pendingCount == 0) {
        var newNullPending = $("#_nullDataMessage").html().replace("__nullDataMessage_ID", "__nullDataMessage_PendingRequests")
        .replace("__message_TEXT", "There are no requests Pending Approval at this time.");
        $("#_pendingApprovalsContainer").append($(newNullPending));
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
    if (data.ADManager != "") { ADManagaer = "[Active Directory: " + data.ADManager + "]"; }
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
    $.each(data.Details, function(index, value){
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
    
    if ($("#__workflowStatus_" + requestId).val() == 7) { CreateApproverActions(requestId); }
    else { GetTracking(null,null,requestId); }

}
function CreateApproverActions(requestId) {
    var newApproval = $("#_approverActions").html();
    newApproval = newApproval.replace("__approverActions_ID", "__approverActions_" + requestId)
    .replace(/__changeDeny_ID/g, "__changeDeny_" + requestId)
    .replace("__approve_ID", "__approve_" + requestId)
    .replace("__approveAndMoveNext_ID", "__approveAndMoveNext_" + requestId)
    .replace("__radioApproverChange_ID", "__radioApproverChange_" + requestId)
    .replace("__radioApproverDeny_ID", "__radioApproverDeny_" + requestId)
    .replace("__changeDenyComment_ID", "__changeDenyComment_" + requestId)
    .replace("__approverRequestChange_ID", "__approverRequestChange_" + requestId)
    .replace("__approverDeny_ID", "__approverDeny_" + requestId);

    $("#__toggledContentContainer_" + requestId).html($("#__toggledContentContainer_" + requestId).html()
    .replace("<!--__requestApproval-->", newApproval));
    
    if ($("#__multiplePendingApprovals").val() == "no") { $("#__approveAndMoveNext_" + requestId).hide(); }

    GetTracking(null, null, requestId);

}
function BindEvents(builderGroups, builderButtons, requestId) {

    $("#__approve_" + requestId).click(function() {
        ApproverActions(this, requestId, '0');
    });

    $("#__approveAndMoveNext_" + requestId).click(function() {
        ApproverActions(this, requestId, '0');
    });

    $("#__approverRequestChange_" + requestId).click(function() {
        ApproverActions(this, requestId, '2');
    });

    $("#__approverDeny_" + requestId).click(function() {
        ApproverActions(this, requestId, '1');
    });

    $("#__radioApproverDeny_" + requestId).click(function() {
        ChangeDenyClick(this);
    });

    $("#__radioApproverChange_" + requestId).click(function() {
        ChangeDenyClick(this);
    });

    $("#__legendToggle_" + requestId).click(function() {
        ToggleLegend(requestId);
    });
    ToggleLoading(requestId);

}
function ChangeDenyClick(obj) {
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
    var textarea = $("#__changeDenyComment_" + requestId);
    var approverName = $("input[id*='_hiddenCurrentUserFullName']").val();
    ProcessingMessage("Updating Request", "");
    switch (action) {
        case '2':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please detail the specific change required."); return false; }
            break;
        case '1':
            if (textarea.val() == "") { ActionMessage("Required Input", "Please specify the reason for denial."); return false; }
            break;
    }
    comments = "<br />" + textarea.val();
    var newAction = new Comment(requestId, action, textarea.val());
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "AjaxCalls.aspx/ApproverActions",
        data: newAction.toJSON,
        dataType: "json",
        success: function(msg) {
            if (msg.d.Success) {
                switch (action) {
                    case '0':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Approved", "You have successfully approved this request.");
                        UpdateRequestTracking(requestId, approverName, "Approved");
                        AnimateActions("Open Requests", requestId);
                        $("#__approverActions_" + requestId).hide();
                        if ($(obj).attr("id").indexOf("_approveAndMoveNext") > -1) { OpenNext(requestId); }
                        UpdateCount("_approvalCount");
                        break;
                    case '2':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Change Requested", "You have just requested a change.");
                        UpdateRequestTracking(requestId, approverName, "Change Requested");
                        AddComments(requestId, approverName, "Change Requested", comments, false);
                        AnimateActions("Open Requests", requestId);
                        $("#__approverActions_" + requestId).hide();
                        $("#__overallRequestStatus_" + requestId).html("Change Requested");
                        UpdateCount("_approvalCount");
                        break;
                    case '1':
                        $('#_indicatorDiv').hide();
                        ActionMessage("Closed Denied", "You have just denied this request.");
                        UpdateRequestTracking(requestId, approverName, "Closed Denied");
                        AddComments(requestId, approverName, "Closed Denied", comments, false);
                        AnimateActions("Closed Requests", requestId);
                        $("#__approverActions_" + requestId).hide();
                        $("#__overallRequestStatus_" + requestId).html("Closed");
                        UpdateCount("_approvalCount");
                        break;
                }
            }
            else {
                MessageDialog(msg.d.Title, msg.d.Message);
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
function OpenNext(requestId) {
    var requestIds = [];
    var openRequest = 0;
    $("#_pendingApprovalsContainer").find("div.csm_toggle_container").each(function() {
        if($(this).attr("snap") != ""){requestIds.push($(this).attr("snap"));}
    });

    $.each(requestIds, function(index, value) {
        if (requestIds.length <= 2) {
            $("#__multiplePendingApprovals").val("no");
            $("#__approveAndMoveNext_" + requestIds[index]).hide();
        }
        else {
            $("#__multiplePendingApprovals").val("yes");
        }
        if (value == requestId) {
            if (index == requestIds.length - 1) { openRequest = requestIds[0]; }
            else {
                openRequest = requestIds[index + 1];
            }
        }
    });
    var blade = $("#__toggleIconContainer_" + openRequest);
    blade.click();
}
//]]>