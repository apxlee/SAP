//<![CDATA[
function ApproveRequest(requestId) {

	var postData = "{'requestId':'" + requestId.toString() + "'}";

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