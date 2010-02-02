/// <reference path="jquery-1.3.2-vsdoc.js" />
//$(document).ready(function() { });

//$.ready(DocReady);

function DocReady() {
    BorderMyh3();
    userManager.Ready();
}

var userManager = {
    /*
    userName: $("input[id$='userName']"),
    userSelection: $('select[id$=nameSelection]'),
    mgrName: $("input[id$='mgrName']"),
    mgrSelection: $('select[id$=mgrSelection]'),
    userNameCheck: $("button[id$='userCheck']"),
    mgrNameCheck: $("button[id$='mgrCheck']"),
    */

    ToggleSelecction: function() {
        $('select[id$=nameSelection]').toggle();
        $('select[id$=mgrSelection]').toggle();
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
        $("button[id$='userCheck']").click(function() {
            // this is ref to the button not usermanager, hence to call usermanager function I need to ref to qualify user manager
            userManager.GetNames($("input[id$='userName']").val(), $('select[id$=nameSelection]'));
        })
    },
    HandleGetManagerNames: function() {
        $("button[id$='mgrCheck']").click(function() {
            // this is ref to the button not usermanager, hence to call usermanager function I need to ref to qualify user manager
            userManager.GetNames($("input[id$='mgrName']").val(), $('select[id$=mgrSelection]'));
        })
    },

    HandleNameSelectionChange: function() { $('select[id$=nameSelection]').change(userManager.UserNameSelected) },
    HandleManagerSelectionChange: function() { $('select[id$=mgrSelection]').change(userManager.ManagerNameSelected) },


    UserNameSelected: function() {
        userManager.AssignSelectedName($("input[id$='userName']"),
                            $('select[id$=nameSelection] option:selected').text(),
                            $('select[id$=nameSelection]'));
        userManager.FillUserLoginId();
        userManager.FillManagerName();
    },

    ManagerNameSelected: function() {
        userManager.AssignSelectedName($("input[id$='mgrName']"),
                            $('select[id$=mgrSelection] option:selected').text(),
                            $('select[id$=mgrSelection]'));
        userManager.FillManagerLoginId();

    },

    AssignSelectedName: function(nameElement, selectedName, selection) {
        nameElement.val(selectedName);
        selection.toggle();
    },

    FillManagerName: function() {
        var userInfo = $('body').data('userInfo');
        for (var key in userInfo) {
            if (userInfo[key].Name == $("input[id$='userName']").val()) {
                $("input[id$='mgrName']").val(userInfo[key].ManagerName);
                $("input[id$='mgrLoginId']").empty();
                $("input[id$='mgrLoginId']").val(userInfo[key].ManagerLoginId);
                break;
            }
        }
    },

    FillUserLoginId: function() {
        var userInfo = $('body').data('userInfo');
        for (var key in userInfo) {
            if (userInfo[key].Name == $("input[id$='userName']").val()) {
                $("input[id$='userLoginId']").empty();
                $("input[id$='userLoginId']").val(userInfo[key].LoginId);
                break;
            }
        }
    },

    FillManagerLoginId: function() {
        var userInfo = $('body').data('userInfo');
        for (var key in userInfo) {
            if (userInfo[key].Name == $("input[id$='mgrName']").val()) {
                $("input[id$='mgrLoginId']").empty();
                $("input[id$='mgrLoginId']").val(userInfo[key].LoginId);
                break;
            }
        }
    },

    Ready: function() {
        // this: userManager object
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