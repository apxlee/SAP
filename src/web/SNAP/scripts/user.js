/// <reference path="jquery-1.3.2-vsdoc.js" />
//$(document).ready(function() { });

//$.ready(DocReady);

function DocReady() {
    BorderMyh3();
    userManager.Ready();
}

var userManager = {
    Setup: function() {

        this.userName = $("input[id$='_requestorId']");
        this.userSelection = $('select[id$=_nameSelection]');
        this.mgrName = $("input[id$='_managerName']");
        this.mgrSelection = $('select[id$=mgrSelection]');
        this.userNameCheck = $("button[id$='_checkRequestorId']");
        this.mgrNameCheck = $("button[id$='_checkManagerName']");
        this.userLoginId = $("input[id$='_requestorLoginId']");
        this.mgrLoginId = $("input[id$='_managerLoginId']");

        this.ajaxIndicator = $("div[id$='_notification']");
    },

    ToggleSelecction: function() {
        this.userSelection.toggle();
        this.mgrSelection.toggle();
    },
    GetNames: function(name, selection) {
        var postData = "{'name':'" + name.val() + "'}";
        userManager.ajaxIndicator.show();

        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "/Default.aspx/GetNames",
            data: postData,
            dataType: "json",
            success: function(msg) {
                userManager.ajaxIndicator.hide();

                var names = msg.d;
                $('body').data('userInfo', names);

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
                    userManager.FillSelection(selection, names);
                }
            },

            error: function(XMLHttpRequest, textStatus, errorThrown) {
                userManager.ajaxIndicator.hide();
                alert("GetNames Error: " + XMLHttpRequest);
                alert("GetNames Error: " + textStatus);
                alert("GetNames Error: " + errorThrown);
            }
        });
    },

    GetUserManagerInfo: function(flag, fullName) {
        var postData = "{'fullName':'" + fullName.val() + "'}";
        userManager.ajaxIndicator.show();

        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "/Default.aspx/GetUserManagerInfoByFullName",
            data: postData,
            dataType: "json",
            success: function(msg) {
                userManager.ajaxIndicator.hide();
                var userInfo = msg.d;
                userManager.FillUserManagerInfo(flag, userInfo);
            },

            error: function(XMLHttpRequest, textStatus, errorThrown) {
                userManager.ajaxIndicator.hide();
                alert("GetNames Error: " + XMLHttpRequest);
                alert("GetNames Error: " + textStatus);
                alert("GetNames Error: " + errorThrown);
            }
        });
    },

    HandleGetUserNames: function() {
        userManager.userNameCheck.click(function() {
            // this is ref to the button not usermanager, hence to call usermanager function I need to ref to qualify user manager
            userManager.GetNames(userManager.userName, userManager.userSelection);
        })
    },
    HandleGetManagerNames: function() {
        userManager.mgrNameCheck.click(function() {
            // this is ref to the button not usermanager, hence to call usermanager function I need to ref to qualify user manager
            userManager.GetNames(userManager.mgrName, userManager.mgrSelection);
        })
    },

    HandleNameSelectionChange: function() { userManager.userSelection.change(userManager.UserNameSelected) },
    HandleManagerSelectionChange: function() { userManager.mgrSelection.change(userManager.ManagerNameSelected) },

    UserNameSelected: function() {
        userManager.AssignSelectedName(userManager.userName,
        //$('select[id$=nameSelection] option:selected').text(),
                            $('#' + userManager.userSelection.attr('id') + ' option:selected').text(),
                            userManager.userSelection);
        userManager.GetUserManagerInfo("user", userManager.userName);

    },

    ManagerNameSelected: function() {
        userManager.AssignSelectedName(userManager.mgrName,
        //$('select[id$=mgrSelection] option:selected').text(),
                            $('#' + userManager.mgrSelection.attr('id') + ' :selected').text(),
                            userManager.mgrSelection);
        userManager.GetUserManagerInfo("manager", userManager.mgrName);

    },

    AssignSelectedName: function(nameElement, selectedName, selection) {
        nameElement.val(selectedName);
        selection.toggle();
    },

    FillAllFields: function(name, names) {

        if (name.attr('id').indexOf("manager") > -1) {
            userManager.mgrLoginId.val(names[0].LoginId);
            userManager.mgrName.val(names[0].Name);
        } else {

            userManager.userName.val(names[0].Name);
            userManager.userLoginId.val(names[0].LoginId);
            userManager.mgrName.val(names[0].ManagerName);
            userManager.mgrLoginId.val(names[0].ManagerLoginId);
        }

    },

    FillErrorFields: function(name) {
        if (name.attr('id').indexOf("manager") > -1) {
            userManager.mgrName.val("No such name! Try again");
            userManager.mgrLoginId.val("unknown");
        }
        else {
            userManager.userName.val("No such name! Try again");
            userManager.userLoginId.val("unknown");
        }
    },

    FillSelection: function(selection, names) {
        var listItems = [];
        for (var key in names) {
            listItems.push('<option value="' + key + '">' + names[key].Name + '</option>');
        }
        selection.toggle();
        selection.empty();
        selection.append(listItems.join(''));
        selection.attr('size', names.length);
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

        userManager.mgrName.val(userManagerInfo.ManagerName);
        userManager.mgrLoginId.val(userManagerInfo.ManagerLoginId)
    },

    FillManagerInfoFromManagerSelection: function(userManagerInfo) {
        userManager.mgrName.val(userManagerInfo.Name);
        userManager.mgrLoginId.val(userManagerInfo.LoginId)
    },

    Ready: function() {
        // this: userManager object
        this.Setup();
        this.ToggleSelecction();
        this.HandleGetUserNames();
        this.HandleGetManagerNames();
        this.HandleNameSelectionChange();
        this.HandleManagerSelectionChange();
    }
}

function BorderMyh3() { $("h3").css("border", "3px solid red"); }
