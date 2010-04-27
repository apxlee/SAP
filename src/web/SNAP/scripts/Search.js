function validateInput(obj) {
    if($(obj).parent().parent().find("input").val() > ""){return true;}
    else{alert("Search Input Required!");return false;}
}