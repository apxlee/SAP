function validateInput(obj) {
    if($(obj).parent().parent().find("input").val() > ""){return true;}
    else { ActionMessage("No input", "Input criteria is required for a successful search."); return false; }
}
function clickButton(e, buttonid) {
    var evt = e ? e : window.event;
    var bt = document.getElementById(buttonid);
    if (bt) {
        if (evt.keyCode == 13) {
            bt.click();
            return false;
        }
    }
}
function ActionMessage(header, message) {
    $(document).ready(function() {
        $('div.messageBox').children("h2").html(header);
        $('div.messageBox').children("p").html(message);
        $('#_actionMessageDiv').fadeIn().delay(2000).fadeOut();
    });
}