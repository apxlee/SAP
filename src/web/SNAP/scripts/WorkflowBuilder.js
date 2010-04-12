function approverGroupChecked(obj, requestId)
{
    $(document).ready(function() {
        $(obj).closest("table").find("input[type=radio][checked]").each(
          function() {
              if ($(obj).attr("checked")) {
                  $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val() + "[" + $(this).attr("id") + "]");
              }
              else {
                  $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val().replace("[" + $(this).attr("id") + "]", ""));
              }
          });
    });
}
function createWorkflow(requestId) {

    alert($("#_managerId_" + requestId).val());
    var postData = "{'requestId':'" + requestId.toString() + "','actorIds':'" + $("#_selectedActors_" + requestId).val() + "[" + $("#_managerId_" + requestId).val() + "]'}";
    
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "AjaxCalls.aspx/CreateWorkflow",
        data: postData,
        dataType: "json",
        success: function(msg) {
            alert(msg.d);
            //            userManager.ajaxIndicatorUser.hide();

            //            var names = msg.d;

            //            // no match                
            //            if (names.length == 0) {
            //                userManager.FillErrorFields(name);
            //            }

            //            // direct match
            //            if (names.length == 1) {
            //                userManager.FillAllFields(name, names);
            //            }

            //            // match list of names
            //            if (names.length > 1) {
            //                userManager.FillSelection(selection, names, dialogDiv);
            //            }
        },

        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetNames Error: " + XMLHttpRequest);
            alert("GetNames Error: " + textStatus);
            alert("GetNames Error: " + errorThrown);
        }
    });
}