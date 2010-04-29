function filterClick(obj) {
    var blade = "";
    var filter = $(obj).val();
    $(obj).closest("div.csm_container_center_700").find("div.csm_content_container").each(
        function() {
            blade = this;
            if (filter == "Show All") {
                $(blade).show();
            }
            else {
                $(this).find("div.csm_hidden_block").children().find("span").each(
                     function() {
                         if ($(this).attr("id").indexOf("_workflowActorName") > -1) {
                             if ($(this).html() == "Access &amp; Identity Management") {
                                 if ($(this).parent().next().next().next().children().html() == "-") {
                                     if ($(this).parent().next().children().html() != filter) {
                                         $(blade).hide();
                                     }
                                     else {
                                         $(blade).show();
                                     }
                                 }
                             }
                         }
                 });
            }
    });
}