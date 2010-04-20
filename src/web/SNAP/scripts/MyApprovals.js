//<![CDATA[

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
function ApproverActions(obj,requestId,action) {
    var comments;
       
    switch (action) 
    {
        case '0':
            comments = ""
        break;
        case '1':
            if ($(".csm_textarea_short").text() == "") { alert("please specify the change"); return false; }
            else { comments = $(".csm_textarea_short").text(); }
        break;
        case '4':
            if ($(".csm_textarea_short").text() == "") { alert("please specify the reason for denial"); return false; }
            else { comments = $(".csm_textarea_short").text(); }
        break;
    }
    
    var postData = "{'requestId':'" + requestId.toString() + "','action':'" + action + "','comments':'" + comments + "'}";

	$.ajax({
		type: "POST",
		contentType: "application/json; character=utf-8",
		url: "AjaxCalls.aspx/ApproveRequest",
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