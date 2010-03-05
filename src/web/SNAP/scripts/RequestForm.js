﻿//$(function() {
//    $("#dialog").dialog({
//        title: 'Disclaimer',
//        bgiframe: true,
//        resizable: false,
//        draggable: false,
//        height: 500,
//        width: 500,
//        modal: true,
//        overlay: {
//            backgroundColor: '#ff0000', opacity: 0.5

//        },
//        buttons: {
//            'Acknowledge': function() {
//                alert('This would submit the form and take you to the View Page');
//                $(this).dialog('close');
//            },
//            Cancel: function() {
//                $(this).dialog('close');
//            }
//        }
//    });
//    $("#dialog").dialog('close');
//});
//function throwModal() {
//    $("#dialog").dialog('open');
//}
//$(document).ready(function() {
//    $("input, textarea, select").focus(
//			  function() {
//			      $(this).addClass("csm_input_focus");
//			  }
//			);
//});

//$(document).ready(function() {
//    $("input, textarea, select").blur(
//			  function() {
//			      $(this).removeClass("csm_input_focus");
//			  }
//			);
//});
/// <reference path="jquery-1.3.2-vsdoc.js" />
//$(document).ready(function() { });

//$.ready(DocReady);

function DocReady() {
    userManager.Ready();
}

var userManager = {
    Setup: function() {

        this.userName = $("input[id$='_requestorId']");
        this.userSelection = $('select[id$=_nameSelection]');
        this.mgrName = $("input[id$='_managerName']");
        this.mgrSelection = $('select[id$=_managerSelection]');
        this.userNameCheck = $("button[id$='_checkRequestorId']");
        this.mgrNameCheck = $("button[id$='_checkManagerName']");
        this.userLoginId = $("input[id$='_requestorLoginId']");
        this.mgrLoginId = $("input[id$='_managerLoginId']");
        this.userSelectionDiv = $("#_nameSelectionDiv");
        this.mgrSelectionDiv = $("#_managerSelectionDiv");

        this.mgrEdit = $("button[id$='_editManagerName']");
        this.ajaxIndicatorUser = $("div[id$='_notificationUser']");
        this.ajaxIndicatorManager = $("div[id$='_notificationManager']");

        this.submitButton = $("input[id$='_submitForm']");
        this.submitButtonLower = $("input[id$='_submitForm_lower']");

        this.clearButton = $("input[id$='_clearForm']");
        this.clearButtonLower = $("input[id$='_clearForm_lower']");

        this.inputFields = $("input[type='text'][id*='requestFormControl']");
        this.textAreas = $("textarea[id*='requestFormControl']");
        this.labels = $("label[id*='requestFormControl']");
    },

    ToggleSelecction: function() {
        this.userSelection.toggle();
        this.mgrSelection.toggle();
        this.mgrName.attr("disabled", true)
    },

    GetNames: function(name, selection, dialogDiv) {
        var postData = "{'name':'" + name.val() + "'}";
        userManager.ajaxIndicatorUser.show();

        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "UserCheck.aspx/GetNames",
            data: postData,
            dataType: "json",
            success: function(msg) {
                userManager.ajaxIndicatorUser.hide();

                var names = msg.d;

                // no match
                if (names.length == 0) {
                    userManager.FillErrorFields(name);
                }

                // direct match
                if (names.length == 1) {
                    userManager.FillAllFields(name, names);
                }

                // match list of names
                if (names.length > 1) {
                    userManager.FillSelection(selection, names, dialogDiv);
                }
            },

            error: function(XMLHttpRequest, textStatus, errorThrown) {
                userManager.ajaxIndicatorUser.hide();
                alert("GetNames Error: " + XMLHttpRequest);
                alert("GetNames Error: " + textStatus);
                alert("GetNames Error: " + errorThrown);
            }
        });
    },

    GetUserManagerInfo: function(flag, fullName) {
        var postData = "{'fullName':'" + fullName.val() + "'}";
        userManager.ajaxIndicatorUser.show();

        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "UserCheck.aspx/GetUserManagerInfoByFullName",
            data: postData,
            dataType: "json",
            success: function(msg) {
                userManager.ajaxIndicatorUser.hide();
                var userInfo = msg.d;
                userManager.FillUserManagerInfo(flag, userInfo);
            },

            error: function(XMLHttpRequest, textStatus, errorThrown) {
                userManager.ajaxIndicatorUser.hide();
                alert("GetUserManagerInfo Error: " + XMLHttpRequest);
                alert("GetUserManagerInfo Error: " + textStatus);
                alert("GetUserManagerInfo Error: " + errorThrown);
            }
        });
    },


    // !!! since we disable the mgrName text box when we fill the manager name there, we will not be able to read the value when post back to the server due to security
    // !!! Due to server does not process read-only text box, we need to remove the disabled attr when we click the submit button so that the mgrName field is available to the server

    SubmitHack: function() {
        if (userManager.InputChange()) {
            userManager.mgrName.removeAttr("disabled");
            return true;
        }
        return false;
    },

    HandleSubmitButtonClick: function() {
        userManager.submitButton.click(function() {
            if (userManager.SubmitHack())
                return true;
            else
                return false;
        })
    },

    HandleSubmitButtonLowerClick: function() {
        userManager.submitButtonLower.click(function() {
            if (userManager.SubmitHack())
                return true;
            else
                return false;
        })
    },


    HandleSubmitClick: function() {
        userManager.HandleSubmitButtonClick();
        userManager.HandleSubmitButtonLowerClick();
    },


    Clear: function() {
        userManager.textAreas.val('');
        userManager.inputFields.val('');
        userManager.mgrNameCheck.attr("disabled", true);
        userManager.mgrEdit.removeAttr('disabled');
        userManager.mgrName.attr("disabled", true);
    },

    InputChange: function() {

        var status = true;
        var check = false;
        var strSections = '';
        var strElement = '';
        var strName = '';

        //check dynamic fields
        userManager.labels.each(function() {
            if ($(this).hasClass("csm_input_required_field")) {
                strName = $(this).html();
                strElement = strElement + "[" + strName;
                check = false;
                $(this).parent().next().children('textarea').each(function() {
                    if ($(this).val() > '') { check = true; }
                    else { strElement = strElement + "," + $(this).attr("id"); }
                });
                strElement = strElement + "]";
            }
            if (!check) {
                strSections = strSections + strElement;
                strElement = '';
                status = check;
            }
            else { strElement = ''; }
        });

        //check static fields
        if (userManager.userLoginId.val() == '') { strSections = strSections + "[User Name,_requestFormControl$_requestorId]"; status = false; }
        if (userManager.mgrLoginId.val() == '') { strSections = strSections + "[Manager Name,_requestFormControl$_managerName]"; status = false; }

        //test validation bit
        if (!status) {
            //create elements and add to DOM
            var arySections = strSections.split("][");

            var span = jQuery(document.createElement("span"));
            span.html("Required Fields Are Missing");

            var ul = jQuery(document.createElement('ul'));

            $("#_formValidationTop").html('');
            $("#_formValidationTop").append(span);
            $("#_formValidationTop").append(ul);

            jQuery.each(arySections, function(i, val) {
                var aryElements = val.split(",");

                jQuery.each(aryElements, function(i, val) {
                    if (i == 0) { strSection = val.replace("[", ""); }
                    else {
                        var li = jQuery(document.createElement('li'));
                        var label = jQuery(document.createElement('label'));
                        label.html(strSection + " empty field");
                        label.attr("for", val.replace("]", ""));
                        li.append(label);
                        li.appendTo($("#_formValidationTop").children("ul"));
                    }
                });
            });
            $("#_formValidationBottom").html($("#_formValidationTop").html());
            $("#_formValidationTop").addClass("csm_input_validation_summary");
            $("#_formValidationBottom").addClass("csm_input_validation_summary");
        }

        return status;
    },

    HandleClearButtonClick: function() {
        userManager.clearButton.click(function() { userManager.Clear(); })
    },

    HandleClearButtonLowerClick: function() {
        userManager.clearButtonLower.click(function() { userManager.Clear(); })
    },

    HandleClearClick: function() {
        userManager.HandleClearButtonClick();
        userManager.HandleClearButtonLowerClick();
    },

    HandleGetUserNames: function() {
        userManager.userNameCheck.click(function() {
            // this is ref to the button not usermanager, hence to call usermanager function I need to ref to qualify user manager
            userManager.GetNames(userManager.userName, userManager.userSelection, userManager.userSelectionDiv);
        })
    },
    HandleGetManagerNames: function() {
        userManager.mgrNameCheck.click(function() {
            // this is ref to the button not usermanager, hence to call usermanager function I need to ref to qualify user manager
            userManager.GetNames(userManager.mgrName, userManager.mgrSelection, userManager.mgrSelectionDiv);
        })
    },

    HandleEditManagerName: function() {
        userManager.mgrEdit.click(function() {
            userManager.mgrName.removeAttr('disabled');
            userManager.mgrNameCheck.removeAttr('disabled');
            userManager.mgrEdit.attr("disabled", true);
            userManager.mgrName.focus();
        })
    },

    HandleNameSelectionChange: function() { userManager.userSelection.change(userManager.UserNameSelected) },
    HandleManagerSelectionChange: function() { userManager.mgrSelection.change(userManager.ManagerNameSelected) },

    UserNameFocusOut: function() {
        userManager.userName.focusout(function() {
            if (userManager.userLoginId.val() == "") {
                userManager.GetNames(userManager.userName, userManager.userSelection, userManager.userSelectionDiv);
            }
        })
    },

    UserNameChange: function() {
        userManager.userName.change(function() {
            userManager.GetNames(userManager.userName, userManager.userSelection, userManager.userSelectionDiv);
        })
    },

    UserNameSelected: function() {
        userManager.AssignSelectedName(userManager.userName,
                            $('#' + userManager.userSelection.attr('id') + ' option:selected').text(),
                            userManager.userSelection);
        userManager.userLoginId.val('loading');
        userManager.GetUserManagerInfo("user", userManager.userName);
        userManager.userSelectionDiv.dialog('close');
    },

    MgrNameFocusOut: function() {
        userManager.mgrName.focusout(function() {
            if (userManager.mgrLoginId.val() == "") {
                userManager.GetNames(userManager.mgrName, userManager.mgrSelection, userManager.mgrSelectionDiv);
            }
        })
    },

    MgrNameChange: function() {
        userManager.mgrName.change(function() {
            userManager.GetNames(userManager.mgrName, userManager.mgrSelection, userManager.mgrSelectionDiv);
        })
    },

    ManagerNameSelected: function() {
        userManager.AssignSelectedName(userManager.mgrName,
                            $('#' + userManager.mgrSelection.attr('id') + ' :selected').text(),
                            userManager.mgrSelection);
        userManager.GetUserManagerInfo("manager", userManager.mgrName);
        userManager.mgrSelectionDiv.dialog('close');
    },

    AssignSelectedName: function(nameElement, selectedName, selection) {
        nameElement.val(selectedName);
    },

    AssignManagerName: function(name) {
        userManager.mgrName.val(name);
        userManager.mgrName.attr("disabled", true);
        userManager.mgrNameCheck.attr("disabled", true);
        userManager.mgrEdit.removeAttr('disabled');
    },

    FillAllFields: function(name, names) {

        if (name.attr('id').indexOf("manager") > -1) {
            userManager.mgrLoginId.val(names[0].LoginId);
            userManager.AssignManagerName(names[0].Name);
        } else {

            userManager.userName.val(names[0].Name);
            userManager.userLoginId.val(names[0].LoginId);
            userManager.AssignManagerName(names[0].ManagerName);
            userManager.mgrLoginId.val(names[0].ManagerLoginId);
        }

    },

    FillErrorFields: function(name) {
        if (name.attr('id').indexOf("manager") > -1) {
            userManager.mgrName.val("No such name! Try again");
            userManager.mgrLoginId.val("");
            userManager.mgrName.focus();
        }
        else {
            userManager.userName.val("No such name! Try again");
            userManager.userLoginId.val("");
            userManager.mgrLoginId.val("");
            userManager.userName.focus();
        }
    },

    FillSelection: function(selection, names, dialogDiv) {
        var listItems = [];
        for (var key in names) {
            listItems.push('<option value="' + key + '">' + names[key].Name + '</option>');
        }
        selection.empty();
        selection.append(listItems.join(''));

        // don't over expand the dialog box
        if (names.length >= 10)
            selection.attr('size', 10);
        else
            selection.attr('size', names.length);

        dialogDiv.dialog('open');
    },


    FillUserManagerInfo: function(flag, userManagerInfo) {
        if (flag == 'user') {
            userManager.FillUserInfo(userManagerInfo);
            userManager.FillManagerInfoFromUserSelection(userManagerInfo);
        }
        else {
            userManager.FillManagerInfoFromManagerSelection(userManagerInfo);
        }
    },

    FillUserInfo: function(userManagerInfo) {
        userManager.userName.val(userManagerInfo.Name);
        userManager.userLoginId.val(userManagerInfo.LoginId);
    },

    FillManagerInfoFromUserSelection: function(userManagerInfo) {

        userManager.AssignManagerName(userManagerInfo.ManagerName);
        userManager.mgrLoginId.val(userManagerInfo.ManagerLoginId)
    },

    FillManagerInfoFromManagerSelection: function(userManagerInfo) {
        userManager.AssignManagerName(userManagerInfo.Name);
        userManager.mgrLoginId.val(userManagerInfo.LoginId)
    },

    DialogClose: function(obj) {
        if ($(obj).attr('id').indexOf("name") > -1) {
            if (userManager.userLoginId.val() == "") {
                userManager.userName.focus();
            }
        }
        else {
            if (userManager.mgrName.attr("disabled") == false) {
                if (userManager.mgrLoginId.val() == "") {
                    userManager.mgrName.focus();
                }
            }
        }
    },

    ConvertToDialog: function(obj) {
        obj.dialog({
            title: 'Select User',
            bgiframe: true,
            resizable: false,
            draggable: false,
            height: 300,
            width: 350,
            modal: true,
            close: function(event, ui) {
                userManager.DialogClose(obj);
            },
            overlay: {
                backgroundColor: '#ff0000', opacity: 0.5

            },
            buttons: {
                Cancel: function() {
                    $(this).dialog('close');
                }
            }
        });
        obj.dialog('close');
    },


    BuildDialog: function() {
        userManager.ConvertToDialog(userManager.mgrSelectionDiv);
        userManager.ConvertToDialog(userManager.userSelectionDiv);
    },

    Ready: function() {
        // this: userManager object
        this.Setup();
        this.BuildDialog();
        this.HandleGetUserNames();
        this.HandleGetManagerNames();
        this.UserNameFocusOut();
        this.UserNameChange();
        this.HandleNameSelectionChange();
        this.MgrNameFocusOut();
        this.MgrNameChange();
        this.HandleManagerSelectionChange();
        this.HandleEditManagerName();
        this.HandleClearClick();
        this.HandleSubmitClick();
        this.Clear();
    }
}

/*

http://stackoverflow.com/questions/1342676/to-disable-a-send-button-if-fields-empty-by-jquery

# // When DOM loads, init the page.
# $( InitPage );
#  
# // Init the page.
# function InitPage(){
# var jInput = $( ":input" );
#  
# // Bind the onchange event of the inputs to flag
# // the inputs as being "dirty".
# jInput.change(
# function( objEvent ){
# // Add dirtry flag to the input in
# // question (whose value has changed).
# $( this ).addClass( "dirty" );
# }
# );
# }
*/