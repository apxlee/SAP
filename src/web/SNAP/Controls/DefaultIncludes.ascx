<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultIncludes.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.DefaultIncludes" %>
<!-- NOTE: Style sheets must appear in this order: 1) reset, 2) grid, 3) template -->
<link href="styles/Csm_Reset.css" rel="stylesheet" type="text/css" />
<link href="styles/Csm_Grid976.css" rel="stylesheet" type="text/css" />
<link href="styles/SNAP.css" rel="stylesheet" type="text/css" />
<!--[if IE 6]>
	<link href="styles/SNAP_IE6.css" rel="stylesheet" type="text/css" />
<![endif]-->
<link href="styles/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>jquery-1.4.2.min.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>jquery.hotkeys-0.7.9.min.js" type="text/javascript"></script>
<script src="<%=Apollo.AIM.SNAP.Web.Common.WebUtilities.ClientScriptPath%>jquery.json-2.2.min.js" type="text/javascript"></script>
<script type="text/javascript">

	$(document).ready(

		// This function is to hide 'Edit Request Form' link unless currently logged-in user is also AEU
		function() {
			var hiddenSpanId = '_' + $("input[id*='_hiddenCurrentUserId']").attr("value") + '_request_link';
			$("span[id*=" + hiddenSpanId + "]").removeClass("csm_hidden_span");
		}

		);
</script>

<script type='html/template' id='_requestBlade'>
    <div class="csm_content_container" id="__requestContainer_ID"> 
	    <!-- BEGIN FLOATING HEADING --> 
	    <div class="csm_divided_heading"> 
		    <div class="csm_float_left"> 
		        <table border="0" cellpadding="0" cellspacing="0" style="margin-left:3px;"> 
		            <tr class="csm_stacked_heading_title"> 
		                <td style="width:200px;">AFFECTED END USER</td> 
		                <td style="width:170px;">STATUS</td> 
		                <td style="width:110px;">LAST MODIFIED</td> 
		                <td style="width:90px;">REQUEST ID</td> 
		            </tr> 
		            <tr class="csm_stacked_heading_label"> 
		                <td><span id="__affectedEndUserName_ID">__affectedEndUserName_TEXT</span></td> 
		                <td><span id="__overallRequestStatus_ID">__overallRequestStatus_TEXT</span></td> 
		                <td><span id="__lastUpdatedDate_ID">__lastUpdatedDate_TEXT</span></td> 
		                <td><span id="__requestId_ID">__requestId_TEXT</span></td>
		            </tr> 
		        </table> 
		    </div> 
		    <div class="csm_float_right"> 
			    <div id="__toggleIconContainer" snap="__snapRequestId" class="csm_toggle_container csm_toggle_show"> 
				    <span>&nbsp;</span> 
			    </div> 
		    </div> 
	    </div>
	    <input id="__workflowStatus_ID" type="hidden" value="__workflowStatus_TEXT">
	    <input id="__multiplePendingApprovals_ID" type="hidden" value=""> 
	    <div class="csm_clear">&nbsp;</div> 
	    <!-- END FLOATING HEADING --> 
	    <!-- TOGGLED CONTENT AREA --> 
        <div id="__toggledContentContainer" class="csm_hidden_block">
            <!--__requestDetailsSection-->
            <!--__requestAcknowledgement-->
            <!--__requestApproval-->
            <!--__requestWorkflowBuilder-->
            <!--__requestComments-->
            <!--__requestTracking-->
        </div>
    </div>
    <div class="csm_clear">&nbsp;</div>
</script>
<script type='html/template' id='_requestDetails'>
<!-- REQUEST DETAILS SECTION --> 	    
    <div class="csm_text_container csm_top5"> 
    <div class="csm_icon_heading_container border oospa_request_details"> 
	    <h2>Request Details</h2> 
	    <p class="">This section contains the detailed description of the access being requested, including the 
	    justification for the request.  Please review this section for accuracy.</p> 
        </div>            
        <table style="font-size:1em;margin-left:10px;margin-right:10px" cellpadding="0" cellspacing="0" width="100%"> 
	        <tr> 
		        <td style="text-align:right;width:140px;padding-right:4px;"><p>Title&#58;</p></td> 
		        <td style="padding:1px 10px 1px 4px;"> 
			        <p><span id="__affectedEndUserTitle_ID">__affectedEndUserTitle_TEXT</span></p> 
		        </td> 
	        </tr> 
	        <tr> 
		        <td style="text-align:right;width:140px;padding-right:4px;"><p>Manager Name&#58;</p></td> 
		        <td style="padding:1px 10px 1px 4px;"> 
			        <p><span id="__requestorsManager_ID">__requestorsManager_TEXT</span>
			        &nbsp;&nbsp;
			        <span class="csm_error_text" id="__adManagerName_ID">__adManagerName_TEXT</span></p> 
		        </td> 
	        </tr> 
	        <tr> 
		        <td style="text-align:right;width:140px;padding-right:4px;"><p>Requestor&#58;</p></td> 
		        <td style="padding:1px 10px 1px 4px;"> 
			        <p><span id="__requestorName_ID">__requestorName_TEXT</span></p> 
		        </td> 
	        </tr> 
	        <!--__requestFormDetails-->
	        <!--__requestCommentSection-->
        </table> 
    </div>
</script>
<script type='html/template' id='_requestFormField'>
    <tr> 
	    <td style="text-align:right;width:140px;padding-right:4px;"><p>__fieldLabel</p></td> 
	    <td style="padding:1px 10px 1px 4px;"><p>__fieldText</p></td> 
	</tr> 
</script>
<script type='html/template' id='_requestCommentSection'>
    <tr style="line-height:1.2em;color:Red;">
	<td style="text-align:right;width:140px;padding-right:4px;"><p>Access Notes&#58;</p></td>
	<td style="padding:1px 10px 1px 4px;">
	    <!--__requestComments-->
    </td>
</tr>
</script>
<script type='html/template' id='_requestComment'>
    <p><u>__commentLabel</u><br />__commentText</p> 
</script>
<script type='html/template' id='_approverActions'>
    <div class="csm_text_container csm_top5"> 
	    <div class="csm_icon_heading_container snap_my_approvals"> 
		    <h2>Approver Actions</h2> 
		    <p class="">As an Approver, please review the access details above and select an appropriate action below.  Denials
		    and Request for Change require a detailed description.</p> 
	    </div>      
        <fieldset><!-- use fieldset for standards & compliance --> 
	    <table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container"> 
		    <tr> 
			    <td class="csm_input_form_label_column"> 
				    <label>Approval</label> 
			    </td> 
			    <td class="csm_input_form_control_column"> 
				   <input name="_approve" type="button" id="__approve_ID" value="Approve" class="csm_html_button" /> 
	               <input name="_approveAndMoveNext" type="button" id="__approveAndMoveNext_ID" value="Approve and Review Next" class="csm_html_button" /> 
	            </td> 
		    </tr> 
		    <tr> 
			    <td class="csm_input_form_label_column"> 
				    <label>Request Change or Deny</label> 
			    </td> 
			        <td class="csm_input_form_control_column"> 
				    <input type="radio" name="__changeDeny_ID" id="__radioApproverChange_ID" class="csm_input_checkradio" value="Request Change"/><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Request Change</span> 
				    <input type="radio" name="__changeDeny_ID" id="__radioApproverDeny_ID" class="csm_input_checkradio" value="Deny"/><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Deny</span>				
				    <div class="csm_clear">&nbsp;</div> 
				    <textarea rows="10" cols="" id="__changeDenyComment_ID" class="csm_text_input csm_textarea_short"></textarea> 
				    <p><em><strong>Note:&nbsp;</strong>Specific reasons for Request Changes, Denial are required.</em></p> 
			    </td> 
		    </tr>					
	    </table> 
	    <div class="csm_input_buttons_container" style="margin-right:6px;"> 
		    <input name="_requestChange" type="button" id="__approverRequestChange_ID" value="Request Change" class="csm_html_button" disabled="disabled" /> 
	        <input name="_deny" type="button" id="__approverDeny_ID" value="Deny" class="csm_html_button" disabled="disabled" /> 
	    </div>				
        </fieldset>	        
    </div> 
</script>
<script type='html/template' id='_acknowledgement'>
    <div class="csm_text_container csm_top5"> 
	    <div class="csm_icon_heading_container oospa_aim_acknowledgement"> 
		    <h2>Acknowledgement</h2> 
		    <p class="">Please review the access details above and select an appropriate action below.  Denials
		    and Request for Change require a detailed description.</p> 
	    </div> 
        <fieldset><!-- use fieldset for standards & compliance --> 
	    <table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container"> 
		    <tr> 
			    <td class="csm_input_form_label_column"> 
				    <label>Acknowledgement</label> 
			    </td> 
			    <td class="csm_input_form_control_column"> 
				    <input type="button" name="__acknowledge_ID" id="__acknowledge_ID" value="Acknowledge" class="csm_html_button" disabled="disabled" /> 
			    </td> 
		    </tr> 
		    <tr> 
			    <td class="csm_input_form_label_column"> 
				    <label>Request Change, Deny or Cancel</label> 
			    </td> 
			    <td class="csm_input_form_control_column"> 
				    <input value="Request Change" name="__changeDenyCancel_ID" type="radio" id="__radioChange_ID" class="csm_input_checkradio" disabled="disabled" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Request Change</span> 
				    <input value="Deny" name="__changeDenyCancel_ID" type="radio" id="__radioDeny_ID" class="csm_input_checkradio" disabled="disabled" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Deny</span> 
				    <input value="Cancel" name="__changeDenyCancel_ID" type="radio" id="__radioCancel_ID" class="csm_input_checkradio" disabled="disabled" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Cancel</span>				
				    <div class="csm_clear">&nbsp;</div> 
				    <textarea rows="10" cols="" id="__actionComments_ID" class="csm_text_input csm_textarea_short" disabled="disabled"></textarea> 
				    <p><em><strong>Note:&nbsp;</strong>Specific reasons for Request Changes, Denial or Cancellation are required.</em></p> 
			    </td> 
		    </tr>					
	    </table> 
	    <div class="csm_input_buttons_container" style="margin-right:6px;"> 
		    <input type="button" id="__requestChange_ID" value="Request Change" class="csm_html_button" disabled="disabled" /> 
	        <input type="button" id="__requestDeny_ID" value="Deny" class="csm_html_button" disabled="disabled" /> 
	        <input type="button" id="__requestCancel_ID" value="Cancel" class="csm_html_button" disabled="disabled" /> 
	    </div>				
        </fieldset>	        
    </div> 
</script>
<script type='html/template' id='_workflowBuilder'>
    <!-- WORKFLOW BUILDER SECTION --> 
    <div class="csm_text_container csm_top5" id="__workflowBuilder_ID"> 
         <div class="csm_icon_heading_container oospa_aim_builder"> 
            <h2>Workflow Builder</h2> 
            <p class="">Please review the access details above and select an appropriate action below.</p> 
        </div> 
	    <fieldset>	
	    <table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container"> 
		    <tr> 
			    <td class="csm_input_form_label_column"> 
				    <label>Required Approver</label> 
			    </td> 
			    <td class="csm_input_form_control_column"> 
				    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row"> 
					    <tr> 
						    <td width="22"><input type="checkbox" id="__requiredCheck" checked="checked" disabled="disabled" class="csm_input_checkradio" /></td> 
						    <td width="386" id="__managerLabelSection_ID"> 
						        <span id="__managerDisplayName_ID" class="csm_inline">__managerDisplayName_TEXT</span><input id="__managerUserId_ID" type="hidden" value="__managerUserId_TEXT">		        
				            </td> 
				            <td width="386" id="__managerInputSection_ID" style="display:none;" class="csm_input_form_control_column"> 
				                <input type="text" id="__managerName_ID" class="csm_text_input_short" /> 
			                    <button type="button" id="__checkManagerName_ID" class="csm_html_button">Check</button> 
				            </td> 
				            <td>&nbsp</td> 
						    <td width="20" class="">&nbsp;</td> 
						    <td id="__managerEditButton_ID" width="20" class="oospa_edit_icon">&nbsp;</td> 
    		
					    </tr> 
				    </table>							
			    </td> 
		    </tr>
		    <!--__workflowBuilder_normal_approvers-->
		    <!--__workflowBuilder_large_group_approvers-->
	    </table> 
	    <div class="csm_input_buttons_container"> 
	        <input type="hidden" id="__selectedActors_ID" />
	        <!--__workflowBuilderButtons--> 
	    </div>			
	    </fieldset>		        
        
    </div>
</script>
<script type='html/template' id='_workflowBuilderButton'>
    <input type="button" id="__builderButton_ID" disabled="disabled" value="__builderButton_TEXT"  class="csm_html_button"/>
</script>
<script type='html/template' id='_workflowBuilderSpecial'>
    <label>Special Approvers</label><p>These are 'sequential' actors who must approve the request before other downstream actors receive notification of pending action.</p>
</script>
<script type='html/template' id='_workflowBuilderTechnical'>
    <label>Technical Approvers</label><p>These are 'shotgun' approvers. All actors in this section receive the request at the same time.</p>  
</script>
<script type='html/template' id='_actorGroup'>
    <tr> 
		<td class="csm_input_form_label_column"> 
			<!--__actorGroupTitle-->
	    </td> 
		<td class="csm_input_form_control_column"> 
			<table id="__actorGroup_ID_GROUPID" border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row"> 
				<tr> 
					<td width="22"><input type="checkbox" id="__actorGroupCheckbox_ID_GROUPID" checked="checked" class="csm_input_checkradio" /></td> 
					<td width="386"><span id="__actorGroupName_ID_GROUPID" class="csm_inline">__actorGroupName_TEXT</span></td> 
				</tr> 
				<tr> 
					<td colspan="2">
					    <span id="__actorGroupDescription_ID_GROUPID" class="group_description">__actorGroupDescription_TEXT</span>
					</td> 
				</tr>
				<!--__actorSection--> 
			</table>										        
		</td> 
	</tr> 
</script>
<script type='html/template' id='_workflowActor'>
    <tr>
        <td>&nbsp;</td>
        <td><input type="radio" id="__radio_ID_GROUPID_ACTORID" name="__radio_ID_GROUPID" value="_ACTORID" class="csm_input_checkradio" checked="checked" disabled="disabled" />
        <span class="csm_inline">__actorDisplayName_TEXT</span>
        </td>
    </tr>
</script>
<script type='html/template' id='_workflowLargeGroup'>
    <tr> 
	    <td class="csm_input_form_label_column"> 
		    <label>__workflowBuilderLargeGroupTitle_TEXT</label><p>__workflowBuilderLargeGroupDescription_TEXT</p> 
	    </td> 
	    <td class="csm_input_form_control_column"> 
		    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="oospa_workflow_builder_row"> 
			    <tr> 
				    <td class="csm_workflow_builder_control_column"> 
					    <select id="__dropdownActors_ID_GROUPID"> 
			                <option selected="selected" value="0">Please select...</option> 
			                <!--__actorOptions--> 
		                </select> 
					    <br /> 
					    <input type="text" id="__actorDisplayName_ID_GROUPID" class="csm_text_input_short"/> 
					    <input type="hidden" id="__actorUserId_ID_GROUPID" style="display:none;" /> 
					    <input type="hidden" id="__actorActorId_ID_GROUPID" style="display:none;" /> 
					    <input type="hidden" id="__actorGroupId_ID_GROUPID" style="display:none;"/> 
					    <input type="button" id="__checkActor_ID_GROUPID" class="csm_html_button" value="Check" disabled="disabled" /> 
					    <input type="button" id="__addActor_ID_GROUPID" class="csm_html_button" value="Add" disabled="disabled" /> 
				    </td> 
			    </tr> 
			    <tr> 
				    <td class="csm_workflow_builder_control_column"> 
    					<!--__actorsSelected--> 
				    </td> 
			    </tr> 
		    </table>							
	    </td> 
    </tr> 
</script>
<script type='html/template' id='_workflowLargeGroupActorOption'>
    <option value="__actorId">__actorDisplayName</option> 
</script>
<script type='html/template' id='_workflowLargeGroupActorSelected'>
    <table class='listview_table'>
        <tr class='listview_tr'>
            <td class='listview_td'>__actorDisplayName</td>
            <td style='display:none;'>__actorId</td>
            <td class='listview_button'>
                <input type='button' value='Remove' class='csm_html_button'/>
            </td>
        </tr>
    </table>
</script>
<script type='html/template' id='_comments'>
    <div class="csm_text_container csm_top5"> 
	    <div class="csm_icon_heading_container oospa_aim_acknowledgement"> 
		    <h2>Comments</h2> 
		    <p>Enter comments that relate to the current request.  Optionally select the community who will be able to see the comments.</p> 
	    </div> 
        		     
        <fieldset><!-- use fieldset for standards & compliance --> 
	    <table border="0" cellpadding="0" cellspacing="0" class="csm_input_form_container"> 
		    <tr> 
			    <td class="csm_input_form_label_column"> 
				    <label>Comments</label> 
				    <p>Select the community who will be able to view the comments.  Comments will appear in the 'Request Details' section.</p> 
			    </td> 
			    <td class="csm_input_form_control_column"> 
				    <input type="radio" name="_audience_ID" id="__radioAIM_ID" class="csm_input_checkradio" value="7" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">AIM</span> 
				    <input type="radio" name="_audience_ID" id="__radioApprovers_ID" class="csm_input_checkradio" value="6" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Approvers</span> 
				    <input type="radio" name="_audience_ID" id="__radioEveryone_ID" class="csm_input_checkradio" value="5" /><span class="" style="line-height: 1.5em;font-size:.85em;margin:0 8px 0 2px;float:left;">Everyone</span> 
				    <div class="csm_clear">&nbsp;</div> 
				    <textarea rows="10" id="__comments_ID" cols="" class="csm_text_input csm_textarea_short"></textarea> 
			    </td> 
		    </tr>					
     
	    </table> 
	    <div class="csm_input_buttons_container" style="margin-right:6px;"> 
		    <input name="__submitComments_ID" type="button" id="__submitComments_ID" value="Submit Comments" class="csm_html_button" disabled="disabled" /> 
	    </div>				
        </fieldset>	        
    </div> 
</script>

<div id="_actionMessageDiv" style="display:none;">
    <div class="messageBox"> 
        <h2>header</h2>
        <div id="_indicatorDiv" style="display:none;text-align:center;padding-top:15px;">
            <img alt="processing..." src="images/ajax_indicator.gif" width="16" height="16" />
        </div>
        <p style="margin-left:5px;margin-right:5px;">message</p>
        <div id="_closeMessageDiv" style="display:none;">
            <input type="button" value="Close" onclick="$('#_actionMessageDiv').hide();$('#_closeMessageDiv').hide();" />
        </div>
    </div>
</div>
<div id="_managerSelectionDiv" style="display:none"; >
    <p />
    <div style="display:none;text-align:center;padding-top:100px;" class="oospa_ajax_indicator">
        <img alt="loading..." src="images/ajax_indicator.gif" width="16" height="16" />
    </div>
    <select style="display:none;" size="3" class="oospa_select_user" name="managerSelection" id="_managerSelection"></select>
</div>