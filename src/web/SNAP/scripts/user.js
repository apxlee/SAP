/// <reference path="jquery-1.3.2-vsdoc.js" />
//$(document).ready(function() { });

//$.ready(DocReady);

function DocReady() {
    BorderMyh3();
    ToggleNameSelection();
    ToggleManagerSelection();
    HandleGetUserNames();
    HandleGetManagerNames();
    HandleNameSelectionChange();
    HandleManagerSelectionChange();
}

function BorderMyh3() { $("h3").css("border", "3px solid red"); }
function ToggleNameSelection() { $('select[id$=nameSelection]').toggle(); }
function ToggleManagerSelection() { $('select[id$=mgrSelection]').toggle(); }
function HandleGetUserNames() { $("button[id$='userCheck']").click(GetUserNames) }
function HandleGetManagerNames() { $("button[id$='mgrCheck']").click(GetManagerNames) }
function HandleNameSelectionChange() { $('select[id$=nameSelection]').change(UserNameSelected) }
function HandleManagerSelectionChange() { $('select[id$=mgrSelection]').change(ManagerNameSelected) }
function GetUserNames() {

    var postData = "{'name':'" +  $("input[id$='userName']").val() + "'}";

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
                ToggleNameSelection();
                var nameSelection = $('select[id$=nameSelection]');
                $(nameSelection).empty();
                $(nameSelection).append(listItems.join(''));
            }
            var result = $("div#result");
            result.empty();
            result.append("done!");

        }
    });

}

function UserNameSelected() {
    $("input[id$='userName']").val($('select[id$=nameSelection] option:selected').text());
    ToggleNameSelection();
    var userInfo = $('body').data('userInfo');
    for (var key in userInfo) {
        if (userInfo[key].Name == $("input[id$='userName']").val()) {
            $("input[id$='mgrName']").val(userInfo[key].ManagerName);
            break;
        }
    }
}

function GetManagerNames() {

    var postData = "{'name':'" + $("input[id$='mgrName']").val() + "'}";

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
                ToggleManagerSelection();
                var nameSelection = $('select[id$=mgrSelection]');
                $(nameSelection).empty();
                $(nameSelection).append(listItems.join(''));
            }
            var result = $("div#result");
            result.empty();
            result.append("done!");

        }
    });

}

function ManagerNameSelected() {
    $("input[id$='mgrName']").val($('select[id$=mgrSelection] option:selected').text());
    ToggleManagerSelection();
    /*
    var userInfo = $('body').data('userInfo');
    for (var key in userInfo) {
        if (userInfo[key].Name == $("input[id$='userName']").val()) {
            $("input[id$='mgrName']").val(userInfo[key].ManagerName);
            break;
        }
    }
    */
}
