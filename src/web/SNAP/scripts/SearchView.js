$(document).ready(function() {
    $("#startDatepicker").datepicker({
        showOn: "button",
        buttonImage: "images/calendar.gif",
        buttonImageOnly: true,
        maxDate: "0d",
        changeMonth: true,
        changeYear: true,
        onSelect: function(dateText, inst) {
            $("#__startDate").html(dateText);
            $("#endDatepicker").datepicker("enable");
            $("#endDatepicker").datepicker("option", "minDate", new Date(dateText));
            $("#__endDate").html("");
        }
    });
    $("#endDatepicker").datepicker({
        showOn: "button",
        buttonImage: "images/calendar.gif",
        buttonImageOnly: true,
        maxDate: "0d",
        changeMonth: true,
        changeYear: true,
        onSelect: function(dateText, inst) {
            $("#__endDate").html(dateText);
        }
    });
    $("#endDatepicker").datepicker("disable");

});
function ToggleSearch() {
    $("#__advancedSearchContainer").toggle();
    if ($("#__advancedSearchContainer").is(":visible")) { $("#__advacnedSearchToggle").html("Hide Advanced Search"); }
    else 
    {
        $("#__advacnedSearchToggle").html("Show Advanced Search");
        $("#__searchContents").val("");
        $("#__startDate").html("");
        $("#__endDate").html("");
        $("#endDatepicker").datepicker("disable");
    }
}

function Search(primary, contents, rangeStart, rangeEnd) {
    this.primary = primary;
    this.contents = contents;
    this.rangeStart = rangeStart;
    this.rangeEnd = rangeEnd;
    this.toJSON = $.toJSON(this);
}

function Clear() {
    $("#__searchInput").val("");
    $("#__searchContents").val("");
    $("#__startDate").html("");
    $("#__endDate").html("");
    $("#endDatepicker").datepicker("disable");
}

function ValidateInput() {
    var searchInput = $("#__searchInput").val();
    var searchContents = $("#__searchContents").val();
    var searchRangeStart = $("#__startDate").html();
    var searchRangeEnd = $("#__endDate").html();
    var newSearch = new Search(searchInput, searchContents, searchRangeStart, searchRangeEnd);
    if (newSearch.primary > "" || newSearch.contents > "" || newSearch.rangeStart > "") {
        $("#_searchResultsContainer").html("");
        $("#__searchButton").attr("disabled", "disabled");
        ProcessingMessage("Searching Requests", "");
        var t = setTimeout('GetRequests(' + ViewIndexEnum.Search + ',\'' + newSearch.toJSON + '\')', 1000); 
        }
    else { ActionMessage("No input", "Input criteria is required for a successful search."); }
}

function ClickButton(e, button) {
    var evt = e ? e : window.event;
    if (evt.keyCode == 13) {
        ValidateInput();
    }
}
function CreateBlades(requests) {
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

            $("#_searchResultsContainer").append($(newRequestBlade));

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
    }
    else {
        var newNullResults = $("#_nullDataMessage").html().replace("__nullDataMessage_ID", "__nullDataMessage_SearchRequests")
        .replace("__message_TEXT", "There are no Search Results.");
        $("#_searchResultsContainer").append($(newNullResults));
        $("#__searchInput").focus();
    }
    ActionMessage("Done!", "");
    $("#__searchButton").removeAttr("disabled");
}
function CreateRequestDetails(details, requestId) {
    var data = jQuery.parseJSON(details);
    var highLight = $("#__searchContents").val().replace(/(\r\n|[\r\n])/g, " ").split(/,| /);
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
        var newLabel = value.Label + ":";
        var newValue = value.Text;
        $.each(highLight, function(index, word) {
            if (word != "") {
                var reL = new RegExp(word.toLowerCase(), "g");
                var reU = new RegExp(word.toUpperCase(), "g");
                var firstL = word.substring(0, 1).toUpperCase();
                var leftOver = word.substring(1, word.length).toLowerCase();
                var reF = new RegExp(firstL + leftOver, "g");
                newValue = newValue.replace(reL, "<font style=\"background:yellow;\">" + word.toLowerCase() + "</font>");
                newValue = newValue.replace(reU, "<font style=\"background:yellow;\">" + word.toUpperCase() + "</font>");
                newValue = newValue.replace(reF, "<font style=\"background:yellow;\">" + firstL + leftOver + "</font>");
            }
            
        });
        newValue = newValue.replace(/(\r\n|[\r\n])/g, "<br />");
        newField = newField.replace("__fieldLabel", newLabel).replace("__fieldText", newValue);
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

    GetTracking(null, null, requestId);
}
function BindEvents(builderGroups, builderButtons, requestId) {
    $("#__legendToggle_" + requestId).click(function() {
        ToggleLegend(requestId);
    });
    ToggleLoading(requestId);
}