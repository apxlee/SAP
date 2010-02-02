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
        var postData = "{'name':'" + name + "'}";

        $.ajax({
            type: "POST",
            contentType: "application/json; character=utf-8",
            url: "/User.aspx/GetNames",
            data: postData,
            dataType: "json",
            success: function(msg) {
                var names = msg.d;
                $('body').data('userInfo', names);

                if (names.length > 0) {
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
            userManager.GetNames(userManager.userName.val(), userManager.userSelection);
        })
    },
    HandleGetManagerNames: function() {
        userManager.mgrNameCheck.click(function() {
            // this is ref to the button not usermanager, hence to call usermanager function I need to ref to qualify user manager
            userManager.GetNames(userManager.mgrName.val(), userManager.mgrSelection);
        })
    },

    HandleNameSelectionChange: function() { userManager.userSelection.change(userManager.UserNameSelected) },
    HandleManagerSelectionChange: function() { userManager.mgrSelection.change(userManager.ManagerNameSelected) },


    UserNameSelected: function() {
        userManager.AssignSelectedName(userManager.userName,
                            $('select[id$=nameSelection] option:selected').text(),
                            userManager.userSelection);
        userManager.FillUserLoginId();
        userManager.FillManagerName();
    },

    ManagerNameSelected: function() {
        userManager.AssignSelectedName(userManager.mgrName,
                            $('select[id$=mgrSelection] option:selected').text(),
                            userManager.mgrSelection);
        userManager.FillManagerLoginId();

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

    FillUserLoginId: function() {
        var userInfo = $('body').data('userInfo');
        for (var key in userInfo) {
            if (userInfo[key].Name == userManager.userName.val()) {
                userManager.userLoginId.empty();
                userManager.userLoginId.val(userInfo[key].LoginId);
                break;
            }
        }
    },

    FillManagerLoginId: function() {
        var userInfo = $('body').data('userInfo');
        for (var key in userInfo) {
            if (userInfo[key].Name == userManager.mgrName.val()) {
                userManager.mgrLoginId.empty();
                userManager.mgrLoginId.val(userInfo[key].LoginId);
                break;
            }
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

/*
function ToggleNameSelection() { $('select[id$=nameSelection]').toggle(); }
function ToggleManagerSelection() { $('select[id$=mgrSelection]').toggle(); }
function HandleGetUserNames() { $("button[id$='userCheck']").click(GetUserNames) }
function HandleGetManagerNames() { $("button[id$='mgrCheck']").click(GetManagerNames) }
function HandleNameSelectionChange() { $('select[id$=nameSelection]').change(UserNameSelected) }
function HandleManagerSelectionChange() { $('select[id$=mgrSelection]').change(ManagerNameSelected) }

function GetUserNames() {
    GetNames($("input[id$='userName']").val(), $('select[id$=nameSelection]'), ToggleNameSelection);
}

function UserNameSelected() {
    AssignSelectedName( $("input[id$='userName']"),
                        $('select[id$=nameSelection] option:selected').text(),
                        ToggleNameSelection);
    FillManagerName();
}

function GetManagerNames() {
    GetNames($("input[id$='mgrName']").val(), $('select[id$=mgrSelection]'), ToggleManagerSelection);
}


function GetNames(name, selection, toggleSelection) {
    var postData = "{'name':'" + name + "'}";

    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "/User.aspx/GetNames",
        data: postData,
        dataType: "json",
        success: function(msg) {
            var names = msg.d;
            $('body').data('userInfo', names);

            if (names.length > 0) {
                var listItems = [];
                for (var key in names) {
                    listItems.push('<option value="' + key + '">' + names[key].Name + '</option>');
                }
                toggleSelection();
                selection.empty();
                selection.append(listItems.join(''));
            }
        }
    });
}

function ManagerNameSelected() {
    AssignSelectedName( $("input[id$='mgrName']"),
                        $('select[id$=mgrSelection] option:selected').text(), 
                        ToggleManagerSelection);
}

function AssignSelectedName(nameElement, selectedName, toggleSelection) {
    nameElement.val(selectedName);
    toggleSelection();
}

function FillManagerName() {
    var userInfo = $('body').data('userInfo');
    for (var key in userInfo) {
        if (userInfo[key].Name == $("input[id$='userName']").val()) {
            $("input[id$='mgrName']").val(userInfo[key].ManagerName);
            break;
        }
    }
}

*/