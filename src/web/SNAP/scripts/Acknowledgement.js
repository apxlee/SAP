var m_names = new Array("Jan", "Feb", "Mar",
    "Apr", "May", "Jun", "Jul", "Aug", "Sep",
    "Oct", "Nov", "Dec");

var d = new Date();
var curr_day = d.getDate();
var curr_month = d.getMonth();
var curr_year = d.getFullYear();
var curr_date = m_names[curr_month] + " " + curr_day + ", " + curr_year;

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
    
    switch (action) 
    {
        case '4':
            comments = ""
        break;
        case '2':
            if (textarea.val() == "") { alert("please specify the change"); return false; }
            else { comments = textarea.val(); }
        break;
        case '3':
            if (textarea.val() == "") { alert("please specify the reason for cancel"); return false; }
            else { comments = textarea.val(); }
            break;
        case '1':
            if (textarea.val() == "") { alert("please specify the reason for denial"); return false; }
            else { comments = textarea.val(); }
        break;
    }

    var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + comments + "'}";
    alert(postData);
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
                    case '9':
                        updateRequestTracking(obj, "Pending Acknowlegement", "Pending Workflow");
                        $(obj).attr("disabled", "disabled");
                        $(obj).next().attr("disabled", "disabled");
                        break;
                    case '1':
                        updateRequestTracking(obj, "Pending Acknowlegement", "Change Requested");
                        disableBladeActions(obj);
                        addComments(obj, "Change Requested", comments);
                        break;
                    case '2':
                        updateRequestTracking(obj, "Pending Acknowlegement", "Closed Cancelled");
                        disableBladeActions(obj);
                        addComments(obj, "Closed Cancelled", comments);
                        break;
                    case '4':
                        updateRequestTracking(obj, "Pending Acknowlegement", "Closed Denied");
                        disableBladeActions(obj);
                        addComments(obj, "Closed Denied", comments);
                        break;
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
function updateRequestTracking(obj, oldstring, newstring) {
    $(obj).closest("div.csm_hidden_block").children().find("span").each(
     function() {
         if ($(this).attr("id").indexOf("_workflowStatus") > -1) {
             if ($(this).html() == oldstring) {
                 $(this).parent().next().next().children().html("<span>" + curr_date + "</span>");
                 $(this).closest("div.csm_padded_windowshade").append($(this).closest("div.csm_padded_windowshade").html().replace(oldstring, newstring));
             }
         }
     });
     
     //find the new tracking blade and clear out the dates, might be a better way to do this.
     $(obj).closest("div.csm_hidden_block").children().find("span").each(
     function() {
         if ($(this).attr("id").indexOf("_workflowStatus") > -1) {
             if ($(this).html() == newstring) {
                 $(this).parent().next().children().html("<span>-</span>");
                 $(this).parent().next().next().children().html("<span>-</span>");
             }
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