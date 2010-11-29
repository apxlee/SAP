$(document).ready(function() {
    var link = $("a.enlarge");
    $("#_imageEnlargeDiv").dialog({
        autoOpen: false,
        bgiframe: true,
        resizable: false,
        draggable: false,
        height: 1132,
        width: 979,
        modal: true,
        dialogClass: "ui-widget-contentC",
        overlay: {
            backgroundColor: '#ff0000', opacity: 0.2
        }
    });

    $("a.enlarge").click(function(e) {
        $("#_imageEnlargeDiv").dialog('open');
        $("#_imageEnlargeDiv").click(function() {
            $(this).dialog('close');
        });
    });

    $(".ui-widget-overlay").live("click", function() { $("#_imageEnlargeDiv").dialog("close"); });
});

function EmailReport(obj) {
    $(document).ready(function() {
    $('#_emailerDiv').dialog({
        autoOpen: true,
        bgiframe: true,
        resizable: false,
        draggable: false,
        height: 300,
        width: 350,
        modal: true,
        overlay: {
            backgroundColor: '#ff0000', opacity: 0.2
        },
        buttons: {
            Send: function() {
                alert("Validate!");
            },
            Cancel: function() {
                $(this).dialog("destroy");
            }
        }
    });
        //$('div.messageBox').children("h2").html("Email Report");
        //var position = $(obj).position();
        //alert("left: " + position.left + ", top: " + position.top);
        //$('#_emailerDiv').dialog("option", "position", [position.left, position.top]);
        //$('#_emailerDiv').fadeIn().delay(2000).fadeOut();
    });
}

function GetReportListItems() {
    var postData = "";
    ProcessingMessage("Retrieving Reports", "");
    $.ajax({
        type: "POST",
        contentType: "application/json; character=utf-8",
        url: "ajax/AjaxUtilities.aspx/GetReportListItems",
        data: postData,
        dataType: "json",
        success: function(msg) {
            CreateReportHistory(msg.d);
        }
    });
}
function CreateReportHistory(reportData) {
    if (reportData.length > 0) {
        var reportHistoryHtml = "";
        var count = 0;
        $.each(reportData, function(index, value) {
            if (count == 0) {
                $("#current_report").css("background-image", 'url(' + value.SmallImage + ')');
                $("#_bottleneck_large").attr("src", value.LargeImage);
            }
            reportHistoryHtml += CreateReportBlade(value);
            count++;
        });
        $("#_reportHistoryContainer").append(reportHistoryHtml);
        ActionMessage("Done!", "");
        $("#_emailReport").show();
        $("#_zoomReport").show();
    }
}
function CreateReportBlade(data) {
    var newReportBlade = $("#_reportHistoryDiv").html();
    newReportBlade = newReportBlade
			.replace("%%ReportLink%%", data.Link)
			.replace("%%ReportTitle%%", data.Title)
    return newReportBlade;
}