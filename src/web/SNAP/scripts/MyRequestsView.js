// VIEW SPECIFIC
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
        .replace("__workflowStatus_ID", "__workflowStatus_" + data.RequestId)
        .replace("__workflowStatus_TEXT", data.WorkflowStatus)
        .replace("__legendToggle_ID", "__legendToggle_" + data.RequestId)
        .replace("__legend_ID", "__legend_" + data.RequestId)
        .replace("__requestTrackingSection_ID", "__requestTrackingSection_" + data.RequestId);

            if (data.RequestStatus != "Closed") { $("#_openRequestsContainer").append($(newRequestBlade)); openCount++; }
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

        var selectedRequestId = $("input[id*='_hiddenSelectedRequestId']");
        if (selectedRequestId.val() != "") { ToggleDetails(selectedRequestId.val()); selectedRequestId.val(""); }
    }
    
    if (openCount==0) {
        var newNullOpen = $("#_nullDataMessage").html().replace("__nullDataMessage_ID", "__nullDataMessage_OpenRequests")
        .replace("__message_TEXT", "There are no Open Requests at this time.");
        $("#_openRequestsContainer").append($(newNullOpen));
    }
    if (closedCount==0) {
        var newNullClosed = $("#_nullDataMessage").html().replace("__nullDataMessage_ID", "__nullDataMessage_ClosedRequests")
        .replace("__message_TEXT", "There are no Closed Requests at this time.");
        $("#_closedRequestsContainer").append($(newNullClosed));
    }

    //ToggleRequestLoader();
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
        newField = newField.replace("__fieldLabel",value.Label + ":")
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

    GetTracking(null,null,requestId, false);
}
function BindEvents(builderGroups, builderButtons, requestId) {
    $("#__legendToggle_" + requestId).click(function() {
        ToggleLegend(requestId);
    });
    ToggleLoading(requestId);
}
// VIEW SPECIFIC - END