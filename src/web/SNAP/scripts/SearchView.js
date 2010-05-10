function validateInput(obj) {
    if($(obj).parent().parent().find("input").val() > ""){return true;}
    else{alert("Search Input Required!");return false;}
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