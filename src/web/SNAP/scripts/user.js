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

    GetNames($("input[id$='userName']").val(), $('select[id$=nameSelection]'), ToggleNameSelection);
    
    /*
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
    */
}

function UserNameSelected() {
/*
    $("input[id$='userName']").val($('select[id$=nameSelection] option:selected').text());
    ToggleNameSelection();
*/
    AssignSelectedName( $("input[id$='userName']"),
                        $('select[id$=nameSelection] option:selected').text(),
                        ToggleNameSelection);
    FillManagerName();
}

function GetManagerNames() {

    GetNames($("input[id$='mgrName']").val(), $('select[id$=mgrSelection]'), ToggleManagerSelection);

 /*
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
*/
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