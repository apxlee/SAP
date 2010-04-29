function filterClick(obj) {
    $(obj).closest("table").parent().children().find("div.csm_content_container").each(
    function() {
        alert($(this).html());
    });
}