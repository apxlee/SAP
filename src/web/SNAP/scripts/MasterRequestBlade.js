$(document).ready(function() {
    $(".csm_toggle_container").hover(
			  function() {
			      $(this).addClass("csm_toggle_hover");
			  },
			  function() {
			      $(this).removeClass("csm_toggle_hover");
			  }
			);
});
function csmToggle(sender, tree) {
    if (tree == "ppnn") {
        $(document).ready(function() {
            if ($(sender).parent().parent().next().next().is(":hidden")) {
                $(sender).parent().parent().next().nextAll().slideDown("fast");
                var $kid = $(sender).children(".csm_toggle_icon_down").removeClass("csm_toggle_icon_down");
                $kid.addClass("csm_toggle_icon_up");
            }
            else {
                $(sender).parent().parent().next().nextAll().slideUp("fast");
                var $kid = $(sender).children(".csm_toggle_icon_up").removeClass("csm_toggle_icon_up");
                $kid.addClass("csm_toggle_icon_down");
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