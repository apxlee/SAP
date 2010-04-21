//<![CDATA[

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
        case '9':
            comments = ""
        break;
        case '1':
            if (textarea.val() == "") { alert("please specify the change"); return false; }
            else { comments = textarea.val(); }
        break;
        case '2':
            if (textarea.val() == "") { alert("please specify the reason for cancel"); return false; }
            else { comments = textarea.val(); }
            break;
        case '4':
            if (textarea.val() == "") { alert("please specify the reason for denial"); return false; }
            else { comments = textarea.val(); }
        break;
    }

    var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + comments + "'}";
    alert(postData);
	$.ajax({
		type: "POST",
		contentType: "application/json; character=utf-8",
		url: "AjaxCalls.aspx/AccessTeamActions",
		data: postData,
		dataType: "json",
		success: function(msg) { alert(msg.d); }
		,
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			alert("GetNames Error: " + XMLHttpRequest);
			alert("GetNames Error: " + textStatus);
			alert("GetNames Error: " + errorThrown);
		}
	});
}

//]]>