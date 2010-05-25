$(document).ready(function() {
    $(".csm_toggle_container").hover(
			  function() {
			      if ($(this).attr("class").indexOf("csm_toggle_show") > -1) {
			          $(this).addClass("csm_toggle_show_hover");
			      }
			      else {
			          $(this).addClass("csm_toggle_hide_hover");
			      }
			  },
			  function() {
			      if ($(this).attr("class").indexOf("csm_toggle_show") > -1) {
			          $(this).removeClass("csm_toggle_show_hover");
			      }
			      else {
			          $(this).removeClass("csm_toggle_hide_hover");
			      }
			  }
			);
});
function csmToggle(sender, tree) {
    if (tree == "ppnn") {
        $(document).ready(function() {
            if ($(sender).parent().parent().next().next().is(":hidden")) {
                $(sender).parent().parent().next().nextAll().slideDown("fast");
                $(sender).addClass("csm_toggle_hide");
                $(sender).removeClass("csm_toggle_show");
                $(sender).removeClass("csm_toggle_show_hover");
                $(sender).unbind('mouseenter mouseleave')
                $(sender).hover(function() {
                    $(this).addClass("csm_toggle_hide_hover");
                },
			      function() {
			        $(this).removeClass("csm_toggle_hide_hover");
			      }
			    );
            }
            else {
                $(sender).parent().parent().next().nextAll().slideUp("fast");
                $(sender).addClass("csm_toggle_show");
                $(sender).removeClass("csm_toggle_hide");
                $(sender).removeClass("csm_toggle_hide_hover");
                $(sender).unbind('mouseenter mouseleave')
                $(sender).hover(function() {
                    $(this).addClass("csm_toggle_show_hover");
                },
			      function() {
                    $(this).removeClass("csm_toggle_show_hover");
			      }
			    );
            }
        });
    }
    else {
        $(document).ready(function() {
            if ($(sender).next().is(":hidden")) { $(sender).next().slideDown("fast"); }
            else { $(sender).next().slideUp("fast"); }
        });
    }
}
function toggleLegend(obj) {
    $(obj).closest("div.csm_text_container").find("div").each(function() {
        if ($(this).attr("snap") == "_legend") {
            $(this).toggle(100);
            if ($(obj).html() == "[Show Legend]") { $(obj).html("[Hide Legend]"); }
            else { $(obj).html("[Show Legend]"); }
        }
    });
}