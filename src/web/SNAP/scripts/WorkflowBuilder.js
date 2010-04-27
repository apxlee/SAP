function approverGroupChecked(obj, requestId)
{
    $(document).ready(function() {
        $(obj).closest("table").find("input[type=radio]").each(
          function() {
              if ($(obj).attr("checked")) {
                  $(this).removeAttr("disabled");
                  $(this).change(function() {
                    $(obj).closest("table").find("input[type=radio]").each(
                        function() {
                            $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val().replace("[" + $(this).attr("id") + "]", ""));
                        });
                    $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val() + "[" + $(this).attr("id") + "]");
                    });
                    if ($(this).is(":checked")) {
                        $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val() + "[" + $(this).attr("id") + "]");
                    }
              }
              else {
                  $(this).attr("disabled", "disabled");
                  if ($(this).is(":checked")) {
                    $("#_selectedActors_" + requestId).val($("#_selectedActors_" + requestId).val().replace("[" + $(this).attr("id") + "]", ""));
                  }
              }
          });
    });
}
function editWorkflow(obj,requestId) {
    $(document).ready(function() {
        $(obj).parent().parent().find("input[type=checkbox]").each(
          function() {
              if ($(this).attr("id").indexOf("_requiredCheck") < 0) {
                  $(this).removeAttr("disabled");
                  approverGroupChecked(this, requestId);
              }
          });
        $("#closed_cancelled_" + requestId).removeAttr("disabled");
        $("#create_workflow_" + requestId).removeAttr("disabled");
        $(obj).attr("disabled", "disabled");

        var editLink = $(obj).parent().parent().find(".oospa_edit_icon_disabled");
        editLink.addClass("oospa_edit_icon");
        editLink.removeClass("oospa_edit_icon_disabled");
        editLink.click(function() {
            managerEdit(this);
        });
    });
}
function builderActions(obj, requestId, state)
{
    var postData = "{'requestId':'" + requestId.toString() + "','state':'" + state + "'}";
    alert(postData);
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "AjaxCalls.aspx/BuilderActions",
        data: postData,
        dataType: "json",
        success: function(msg) {
            if (msg.d) {
                switch (state) {
                    case "2":
                        alert("closed cancelled");
                        break;
                    case "3":
                        alert("closed completed");
                        break;
                    case "8":
                        alert("ticket created");
                        break;
                }
            }
            else { alert("action failed"); }
        }
		,
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("GetNames Error: " + XMLHttpRequest);
            alert("GetNames Error: " + textStatus);
            alert("GetNames Error: " + errorThrown);
        }
    });
}
function createWorkflow(obj, requestId) {
    $(document).ready(function() {
        if ($("#_managerUserId_" + requestId).val() > "") {
            var postData = "{'requestId':'" + requestId.toString() + "','managerUserId':'" + $("#_managerUserId_" + requestId).val() + "','actorIds':'" + $("#_selectedActors_" + requestId).val() + "'}";
            alert(postData); //TODO: (added parameter managerUserId)format postdata to match AjaxCalls.aspx\CreateWorkflow parameter"

            $.ajax({
                type: "POST",
                contentType: "application/json; character=utf-8",
                url: "AjaxCalls.aspx/CreateWorkflow",
                data: postData,
                dataType: "json",
                success: function(msg) {
                    alert(msg.d);
                    if (msg.d) {
                        alert("Workflow Created");

                        var editLink = $(obj).parent().parent().find(".oospa_edit_icon");
                        editLink.addClass("oospa_edit_icon_disabled");
                        editLink.removeClass("oospa_edit_icon");
                        editLink.unbind('click');

                        $(obj).parent().parent().find("input[type=checkbox]").each(
                          function() {
                              $(this).attr("disabled", "disabled");
                          });
                        $(obj).parent().parent().find("input[type=radio]").each(
                          function() {
                              $(this).attr("disabled", "disabled");
                          });
                        $("#closed_cancelled_" + requestId).attr("disabled", "disabled");
                        $("#edit_workflow_" + requestId).removeAttr("disabled");
                        $(obj).attr("disabled", "disabled");
                        $("#_selectedActors_" + requestId).val("");
                    }
                    else {
                        alert("Workflow Creation Failed");
                    }
                },

                error: function(XMLHttpRequest, textStatus, errorThrown) {
                    alert("GetNames Error: " + XMLHttpRequest);
                    alert("GetNames Error: " + textStatus);
                    alert("GetNames Error: " + errorThrown);
                }
            });
        }
        else {
            alert("Invalid Manager Name!"); //TODO: Add some style to this validation
        }
    });
}


function editCreatedWorkflow(obj, requestId) {
    $(document).ready(function() {
        if ($("#_managerUserId_" + requestId).val() > "") {
            var postData = "{'requestId':'" + requestId.toString() + "','managerUserId':'" + $("#_managerUserId_" + requestId).val() + "','actorIds':'" + $("#_selectedActors_" + requestId).val() + "'}";
            alert(postData); //TODO: (added parameter managerUserId)format postdata to match AjaxCalls.aspx\CreateWorkflow parameter"

            $.ajax({
                type: "POST",
                contentType: "application/json; character=utf-8",
                url: "AjaxCalls.aspx/EditWorkflow",
                data: postData,
                dataType: "json",
                success: function(msg) {
                    alert(msg.d);
                    if (msg.d) {
                        alert("Workflow Created");

                        var editLink = $(obj).parent().parent().find(".oospa_edit_icon");
                        editLink.addClass("oospa_edit_icon_disabled");
                        editLink.removeClass("oospa_edit_icon");
                        editLink.unbind('click');

                        $(obj).parent().parent().find("input[type=checkbox]").each(
                          function() {
                              $(this).attr("disabled", "disabled");
                          });
                        $(obj).parent().parent().find("input[type=radio]").each(
                          function() {
                              $(this).attr("disabled", "disabled");
                          });
                        $("#closed_cancelled_" + requestId).attr("disabled", "disabled");
                        $("#edit_workflow_" + requestId).removeAttr("disabled");
                        $(obj).attr("disabled", "disabled");
                        $("#_selectedActors_" + requestId).val("");
                    }
                    else {
                        alert("Workflow Creation Failed");
                    }
                },

                error: function(XMLHttpRequest, textStatus, errorThrown) {
                    alert("GetNames Error: " + XMLHttpRequest);
                    alert("GetNames Error: " + textStatus);
                    alert("GetNames Error: " + errorThrown);
                }
            });
        }
        else {
            alert("Invalid Manager Name!"); //TODO: Add some style to this validation
        }
    });
}

function managerEdit(obj) {
    $(document).ready(function() {
        var managerLabelSection = $(obj).prev().prev().prev().prev()
        var managerLabelDispalyName = $(obj).prev().prev().prev().prev().children();
        var managerInputUserId = managerLabelDispalyName.next();
        var managerInputSection = $(obj).prev().prev().prev();
        var managerInputDisplayName = $(obj).prev().prev().prev().children("input[type=text]");
        var managerInputCheckButton = managerInputDisplayName.next();

        managerLabelSection.hide();
        managerInputDisplayName.val(managerLabelDispalyName.html());
        managerInputSection.show();

        managerInputDisplayName.change(function() {
            managerInputUserId.val("");
        });

        managerInputCheckButton.click(function() {
            if (managerInputUserId.val() == "") {
                GetNames(managerInputDisplayName);
            }
            else {
                managerInputSection.hide();
                managerLabelSection.show();
            }
        });
    });
}

function GetNames(name) {
    var indicator = $('.oospa_ajax_indicator');
    var selection = $('select[id$=_managerSelection]');
    var postData = "{'name':'" + name.val().replace("(", "").replace(")", "").replace(/\\/, "").replace("'", "\\'") + "'}";
    selection.hide();
    indicator.show();
    OpenDialog(name);
    
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "AjaxCalls.aspx/GetNames",
        data: postData,
        dataType: "json",
        success: function(msg) {

            var names = msg.d;
            // no match                
            if (names.length == 0) {
                FillErrorFields(name);
            }

            // direct match
            if (names.length == 1) {
                FillAllFields(name, names);
            }

            // match list of names
            if (names.length > 1) {
                FillSelection(name, names);
            }
        },

        error: function(XMLHttpRequest, textStatus, errorThrown) {
            $("#_managerSelectionDiv").dialog("destroy");
            alert("GetNames Error: " + XMLHttpRequest);
            alert("GetNames Error: " + textStatus);
            alert("GetNames Error: " + errorThrown);
        }
    });
}
function FillAllFields(name, names) {
    var managerLabelSection = $(name).parent().prev();
    var managerLabelDispalyName = $(name).parent().prev().children("span");
    var managerInputUserId = managerLabelDispalyName.next();
    var managerInputSection = $(name).parent();
    var managerInputDisplayName = $(name).parent().children("input[type=text]");
    $("#_managerSelectionDiv").dialog("destroy");
    managerInputUserId.val(names[0].LoginId);
    managerLabelDispalyName.html(names[0].Name);
    managerInputSection.hide();
    managerLabelSection.show();
}

function FillErrorFields(name) {
    var managerInputDisplayName = $(name).parent().children("input[type=text]");
    var managerInputUserId = $(name).parent().prev().children("span").next();

    $("#_managerSelectionDiv").dialog("destroy");
    managerInputDisplayName.val("No such name! Try again");
    managerInputUserId.val("");
    managerInputDisplayName.focus();
}
function FillSelection(name, names) {
    var managerLabelSection = $(name).parent().prev();
    var managerLabelDispalyName = $(name).parent().prev().children("span");
    var managerInputUserId = managerLabelDispalyName.next();
    var managerInputSection = $(name).parent();
    var managerInputDisplayName = $(name).parent().children("input[type=text]");
    var selection = $('select[id$=_managerSelection]');
    var indicator = $('.oospa_ajax_indicator');

    selection.change(function() {
        managerLabelDispalyName.html($('#' + selection.attr('id') + ' :selected').text());
        managerInputSection.hide();
        managerLabelSection.show();
        managerInputUserId.val($('#' + selection.attr('id') + ' :selected').val());
        $("#_managerSelectionDiv").dialog("destroy");
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