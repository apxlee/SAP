        function throwModal() {
            $("#dialog").dialog('open');
        }

        $(function() {
            $("#datepickerFrom").datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: 'button',
                buttonImage: 'App_Styles/calendar.gif',
                buttonImageOnly: true
            });
            $("#datepickerTo").datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: 'button',
                buttonImage: 'App_Styles/calendar.gif',
                buttonImageOnly: true
            });
        });
        
        
        $(function() {
        $("#dialog").dialog({
                title: 'Disclaimer',
                bgiframe: true,
                resizable: false,
                draggable: false,
                height: 500,
                width: 500,
                modal: true,
                overlay: {
                    backgroundColor: '#ff0000', opacity: 0.5
                    
                },
                buttons: {
                    'Acknowledge': function() {
                    alert('This would submit the form and take you to the View Page');
                    $(this).dialog('close');
                    },
                    Cancel: function() {
                        $(this).dialog('close');
                    }
                }
            });
            $("#dialog").dialog('close');
        });
		
		function csmToggle(sender, tree)
		{
			if (tree == "ppnn"){
				$(document).ready(function() {
					if ( $(sender).parent().parent().next().next().is(":hidden") )
					{
						$(sender).parent().parent().next().nextAll().slideDown("fast");
						var $kid = $(sender).children(".csm_toggle_icon_down").removeClass("csm_toggle_icon_down");
						$kid.addClass("csm_toggle_icon_up");
					}
					else 
					{
						$(sender).parent().parent().next().nextAll().slideUp("fast");
						var $kid = $(sender).children(".csm_toggle_icon_up").removeClass("csm_toggle_icon_up");
						$kid.addClass("csm_toggle_icon_down");
					}
				});
			}
			else
			{
				$(document).ready(function() {
					if ( $(sender).next().is(":hidden") ) {$(sender).next().slideDown("fast");}
					else {$(sender).next().slideUp("fast");}
				});
			}
		}

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
		
		$(document).ready(function() {
			$("input, textarea, select").focus(
			  function() {
				$(this).addClass("csm_input_focus");
			  }
			);
		});
		
		$(document).ready(function() {
			$("input, textarea, select").blur(
			  function() {
				$(this).removeClass("csm_input_focus");
			  }
			);
			 });

			 $(function() {
			 	$("#userCheck").dialog({
			 		title: 'Select User from Results',
			 		bgiframe: true,
			 		resizable: false,
			 		draggable: true,
			 		height: 200,
			 		width: 400,
			 		modal: true,
			 		overlay: {
			 			backgroundColor: '#ff0000', opacity: 0.5

			 		},
			 		buttons: {
			 			'Done': function() {
			 				//alert('This would insert the selected user');
			 				$(this).dialog('close');
			 			},
			 			Cancel: function() {
			 				$(this).dialog('close');
			 			}
			 		}
			 	});
			 	$("#userCheck").dialog('close');

			 	$("#modifyGroup").dialog({
			 		title: 'Modify The Selected Group',
			 		bgiframe: true,
			 		resizable: false,
			 		draggable: true,
			 		height: 450,
			 		width: 500,
			 		modal: true,
			 		overlay: {
			 			backgroundColor: '#ff0000', opacity: 0.5

			 		},
			 		buttons: {
			 			'Confirm': function() {
			 				//alert('This would modify the selected group');
			 				$(this).dialog('close');
			 			},
			 			'Add Approver': function() {
			 				//alert('This would add a new approver');
			 			},
			 			Cancel: function() {
			 				$(this).dialog('close');
			 			}
			 		}
			 	});
			 	$("#modifyGroup").dialog('close');
			 });

			 function throwUserCheckModal() {
			 	$("#userCheck").dialog('open');
			 }

			 function throwModifyGroupModal() {
			 	$("#modifyGroup").dialog('open');
			 }

			 function selectItem(obj) {
			 	$(document).ready(function() {
			 		var color = $(obj).css("background-color");
			 		var id = $(obj).attr("id");
			 		var name = $(obj).children().html();

			 		if ($("#selectedUser").val() > "") {
			 			var clearId = $("#selectedUser").val();
			 			$("#" + clearId).attr("style", "background-color: #fff; cursor: pointer;");
			 			$("#selectedUser").val("");
			 		}

			 		if (color == "#fff") {
			 			$(obj).attr("style", "background-color: #ccc; cursor: pointer;");
			 			$("#selectedUser").val($(obj).attr("id"));
			 		}
			 	});
			 }