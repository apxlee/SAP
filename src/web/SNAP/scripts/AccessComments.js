//<![CDATA[

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
        alert(postData);
        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "AjaxCalls.aspx/AccessComments",
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
    else {
        alert("please insert comments");
    }  
}

//]]>