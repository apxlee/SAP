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

        this.submitButton = $("input[id$='_submitForm']");
        this.submitButtonLower = $("input[id$='_submitForm_lower']");

        this.clearButton = $("input[id$='_clearForm']");
        this.clearButtonLower = $("input[id$='_clearForm_lower']");

        this.inputFields = $("input[type='text'][id*='requestFormControl']");
        this.textAreas = $("textarea[id*='requestFormControl']");
        this.labels = $("label[id*='requestFormControl']");

        this.acknowledgmentDiv = $("#_acknowledgmentDiv");
    },

    ToggleSelecction: function() {
        this.userSelection.toggle();
        this.mgrSelection.toggle();
        this.mgrName.attr("disabled", true)
    },

    ToolTip: function() {

        xOffset = 10;
        yOffset = 30;

        // these 2 variable determine popup's distance from the cursor
        // you might want to adjust to get the right result

        /* END CONFIG */
        $("a.tooltip").hover(function(e) {
            this.t = this.title;
            this.title = "";
            var c = (this.t != "") ? "<br/>" + this.t : "";
            $("body").append("<p id='tooltip'><img src='" + this.rel + "' alt='tool tip' />" + c + "</p>");
            $("#tooltip")
			    .css("top", (e.pageY - xOffset) + "px")
			    .css("left", (e.pageX + yOffset) + "px")
			    .show();
        },
	    function() {
	        this.title = this.t;
	        $("#tooltip").remove();
	    });
        $("a.tooltip").mousemove(function(e) {
            $("#tooltip")
			    .css("top", (e.pageY - xOffset) + "px")
			    .css("left", (e.pageX + yOffset) + "px");
        });
    },

    GetNames: function(name, selection, dialogDiv) {
        var postData = "{'name':'" + name.val().replace("(", "").replace(")", "").replace(/\\/, "").replace("'", "\\'") + "'}";
        selection.hide()
        dialogDiv.dialog("open");

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
                    userManager.FillErrorFields(name);
                    dialogDiv.dialog("close");
                }

                // direct match
                if (names.length == 1) {
                    userManager.FillAllFields(name, names);
                    dialogDiv.dialog("close");
                }

                // match list of names
                if (names.length > 1) {
                    userManager.FillSelection(selection, names, dialogDiv);
                }
            },

            error: function(XMLHttpRequest, textStatus, errorThrown) {
                dialogDiv.dialog("close");
                alert("GetNames Error: " + XMLHttpRequest);
                alert("GetNames Error: " + textStatus);
                alert("GetNames Error: " + errorThrown);
            }
        });
    },

    GetUserManagerInfo: function(flag, fullName) {
        var postData = "{'fullName':'" + fullName.val().replace("'", "\\'") + "'}";

        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "AjaxCalls.aspx/GetUserManagerInfoByFullName",
            data: postData,
            dataType: "json",
            success: function(msg) {
                var userInfo = msg.d;
                userManager.FillUserManagerInfo(flag, userInfo);
            },

            error: function(XMLHttpRequest, textStatus, errorThrown) {
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
            userManager.acknowledgmentDiv.dialog('open');
            return false;
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
        // NOTE: when requestor info is pre-populated (server-side), this was clearing it out.
        //userManager.inputFields.val('');
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

        //remove special characters
        userManager.textAreas.each(function() {
            $(this).val($(this).val().replace(/(<([^>]+)>)/ig, "").replace(/\\/g, "/").replace(/\'/g, "\""));
        });
        
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
        if (userManager.userLoginId.val() == '') { strSections = strSections + "[Affected End User,ctl00__contentPlaceHolder__requestFormControl__requestorId]"; status = false; }
        if (userManager.mgrLoginId.val() == '') { strSections = strSections + "[Manager Name,ctl00__contentPlaceHolder__requestFormControl__managerName]"; userManager.mgrEdit.click(); status = false; }

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
                        label.html(strSection + " empty or invalid entry");
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
            if (userManager.userLoginId.val() == "") {
                userManager.GetNames(userManager.userName, userManager.userSelection, userManager.userSelectionDiv);
            }
        })
    },
    HandleGetManagerNames: function() {
        userManager.mgrNameCheck.click(function() {
            // this is ref to the button not usermanager, hence to call usermanager function I need to ref to qualify user manager
            if (userManager.mgrLoginId.val() == "") {
                userManager.GetNames(userManager.mgrName, userManager.mgrSelection, userManager.mgrSelectionDiv);
            }
        })
    },

    HandleEditManagerName: function() {
        userManager.mgrEdit.click(function() {
            userManager.mgrName.removeAttr('disabled');
            userManager.mgrNameCheck.removeAttr('disabled');
            userManager.mgrEdit.attr("disabled", true);
        })
    },

    HandleNameSelectionChange: function() { userManager.userSelection.change(userManager.UserNameSelected) },
    HandleManagerSelectionChange: function() { userManager.mgrSelection.change(userManager.ManagerNameSelected) },

    UserNameChange: function() {
        userManager.userName.change(function() {
            userManager.userLoginId.val("");
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

    MgrNameChange: function() {
        userManager.mgrName.change(function() {
            userManager.mgrLoginId.val("");
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

    FillSelection: function(selection, names) {
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

        selection.parent().find('.oospa_ajax_indicator').hide();
        selection.show();
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

        if (userManagerInfo.ManagerName != 'unknown') {
            userManager.AssignManagerName(userManagerInfo.ManagerName);
            userManager.mgrLoginId.val(userManagerInfo.ManagerLoginId);
        }
        else {
            userManager.mgrName.removeAttr('disabled');
            userManager.mgrNameCheck.removeAttr('disabled');
            userManager.mgrEdit.attr("disabled", true);
            userManager.mgrName.val('');
            userManager.mgrLoginId.val('');
            userManager.mgrName.focus();
        }
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

    DialogOpen: function(obj) {
        $(obj).parent().find('.oospa_ajax_indicator').show();
    },

    ConvertToDialog: function(obj, type) {
        if (type == "user") {
            obj.dialog({
                title: 'Select User',
                autoOpen: false,
                bgiframe: true,
                resizable: false,
                draggable: false,
                height: 300,
                width: 350,
                modal: true,
                open: function() {
                    userManager.DialogOpen(obj);
                },
                close: function(event, ui) {
                    userManager.DialogClose(obj);
                },
                overlay: {
                    backgroundColor: '#ff0000', opacity: 0.5

                },
                buttons: {
                    Cancel: function() {
                        $(this).dialog('close');
                        if ($(this).attr('id').indexOf("name") > -1) {
                            userManager.userName.focus();
                        }
                        if ($(this).attr('id').indexOf("manager") > -1) {
                            userManager.mgrName.focus();
                        }
                    }
                }
            });
        }
        if (type == "ack") {
            obj.dialog({
                title: 'Acknowledgement',
                autoOpen: false,
                bgiframe: true,
                resizable: false,
                draggable: false,
                height: 523,
                width: 645,
                modal: true,
                dialogClass: 'ui-dialogB ui-widget ui-widget-contentB ui-corner-all',
                overlay: {
                    backgroundColor: '#ff0000', opacity: 0.5
                },
                buttons: {
                    Acknowledge: function() {
                        $("input[id$='_submit_form']").trigger('click');
                        $(this).dialog('close');
                    },
                    Cancel: function() {
                        $(this).dialog('close');
                    }
                }
            });
            obj.parent().appendTo(jQuery("form:first"));
        }

    },

    BuildDialog: function() {
        userManager.ConvertToDialog(userManager.mgrSelectionDiv, 'user');
        userManager.ConvertToDialog(userManager.userSelectionDiv, 'user');
        userManager.ConvertToDialog(userManager.acknowledgmentDiv, 'ack');
    },

    Ready: function() {
        // this: userManager object
        this.Setup();
        this.ToolTip();
        this.BuildDialog();
        this.HandleGetUserNames();
        this.HandleGetManagerNames();
        this.UserNameChange();
        this.HandleNameSelectionChange();
        this.MgrNameChange();
        this.HandleManagerSelectionChange();
        this.HandleEditManagerName();
        this.HandleClearClick();
        this.HandleSubmitClick();
        //this.Clear();

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