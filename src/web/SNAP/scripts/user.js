/// <reference path="jquery-1.3.2-vsdoc.js" />
//$(document).ready(function() { });

//$.ready(DocReady);

function DocReady() {
    BorderMyh3();
    userManager.Ready();
}

var userManager = {
    Setup: function() {
        this.userName = $("input[id$='userName']");
        this.userSelection = $('select[id$=nameSelection]');
        this.mgrName = $("input[id$='mgrName']");
        this.mgrSelection = $('select[id$=mgrSelection]');
        this.userNameCheck = $("button[id$='userCheck']");
        this.mgrNameCheck = $("button[id$='mgrCheck']");
        this.userLoginId = $("input[id$='userLoginId']");
        this.mgrLoginId = $("input[id$='mgrLoginId']");
    },

    ToggleSelecction: function() {
        this.userSelection.toggle();
        this.mgrSelection.toggle();
    },
    GetNames: function(name, selection) {
        var postData = "{'name':'" + name.val() + "'}";

        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "/Default.aspx/GetNames",
            data: postData,
            dataType: "json",
            success: function(msg) {
                var names = msg.d;
                $('body').data('userInfo', names);

                // direct match
                if (names.length == 1) {
                    userManager.FillAllFields(name, names);
                }

                // match list of names
                if (names.length > 1) {
                    var listItems = [];
                    for (var key in names) {
                        listItems.push('<option value="' + key + '">' + names[key].Name + '</option>');
                    }
                    selection.toggle();
                    selection.empty();
                    selection.append(listItems.join(''));
                }
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
                            $('select[id$=nameSelection] option:selected').text(),
                            userManager.userSelection);
        userManager.FillLoginId(userManager.userName.val(), userManager.userLoginId);
        userManager.FillManagerName();
    },

    ManagerNameSelected: function() {
        userManager.AssignSelectedName(userManager.mgrName,
                            $('select[id$=mgrSelection] option:selected').text(),
                            userManager.mgrSelection);
        userManager.FillLoginId(userManager.mgrName.val(), userManager.mgrLoginId);

    },

    AssignSelectedName: function(nameElement, selectedName, selection) {
        nameElement.val(selectedName);
        selection.toggle();
    },

    FillManagerName: function() {
        var userInfo = $('body').data('userInfo');
        for (var key in userInfo) {
            if (userInfo[key].Name == userManager.userName.val()) {
                userManager.mgrName.val(userInfo[key].ManagerName);
                userManager.mgrLoginId.empty();
                userManager.mgrLoginId.val(userInfo[key].ManagerLoginId);
                break;
            }
        }
    },

    FillLoginId: function(name, loginId) {
        var userInfo = $('body').data('userInfo');
        for (var key in userInfo) {
            if (userInfo[key].Name == name) {
                loginId.empty();
                loginId.val(userInfo[key].LoginId);
                break;
            }
        }
    },

    FillAllFields: function(name, names) {

        if (name.attr('id').indexOf("mgr") > -1) {
            userManager.mgrLoginId.val(names[0].LoginId);
            userManager.mgrName.val(names[0].Name);
        } else {

            userManager.userName.val(names[0].Name);
            userManager.userLoginId.val(names[0].LoginId);
            userManager.mgrName.val(names[0].ManagerName);
            userManager.mgrLoginId.val(names[0].ManagerLoginId);
        }
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
