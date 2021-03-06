﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using Apollo.AIM.SNAP;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class UI
    {
        #region Request Classes

        [DataContract]
        public class RequestBlade
        {
            public RequestBlade() { }
            public RequestBlade(string displayname, string requestStatus, string workflowStatus, string lastmodified, string requestId)
            {
                this.DisplayName = displayname;
                this.RequestStatus = requestStatus;
                this.WorkflowStatus = workflowStatus;
                this.LastModified = lastmodified;
                this.RequestId = requestId;
            }

            [DataMember]
            public string DisplayName { get; set; }
            [DataMember]
            public string RequestStatus { get; set; }
            [DataMember]
            public string WorkflowStatus { get; set; }
            [DataMember]
            public string LastModified { get; set; }
            [DataMember]
            public string RequestId { get; set; }
        }

        #region Filter

        public class RequestFilter
        {
            public RequestFilter() { }
            public RequestFilter(List<Filter> filters)
            {
                this.Filters = filters;
            }
            [DataMember]
            public List<Filter> Filters { get; set; }
        }

        public class Filter
        {
            public Filter() { }
            public Filter(string filterName, List<int> requestIds)
            {
                this.FilterName = filterName;
                this.RequestIds = requestIds;
            }
            [DataMember]
            public string FilterName { get; set; }
            [DataMember]
            public List<int> RequestIds { get; set; }
        }

        #endregion

        #region Details

		[DataContract]
		public class RequestDetails
		{
			public RequestDetails() { }
			//public RequestDetails(string title, string manager, string admanager, string requestor, List<RequestFormField> details, List<RequestComments> comments, string searchLinkUrl)
			//{
			//    this.Title = title;
			//    this.Manager = manager;
			//    this.ADManager = admanager;
			//    this.Requestor = requestor;
			//    this.Details = details;
			//    this.Comments = comments;
			//    this.SearchLinkUrl = searchLinkUrl;
			//}
			[DataMember]
			public string Title { get; set; }
			[DataMember]
			public string Manager { get; set; }
			[DataMember]
			public string ADManager { get; set; }
			[DataMember]
			public string Requestor { get; set; }
			[DataMember]
			public List<RequestFormField> Details { get; set; }
			[DataMember]
			public List<RequestComments> Comments { get; set; }
			[DataMember]
			public string SearchLinkUrl { get; set; }
		}

        [DataContract]
        public class RequestFormField
        {
            public RequestFormField() { }
            public RequestFormField(string label, string text)
            {
                this.Label = label;
                this.Text = text;
            }

            [DataMember]
            public string Label { get; set; }
            [DataMember]
            public string Text { get; set; }
        }
       
        [DataContract]
        public class RequestComments
        {
            public RequestComments() { }
            public RequestComments(string audience, string createdDate, string text)
            {
                this.Audience = audience;
                this.CreatedDate = createdDate;
                this.Text = text;
            }
            [DataMember]
            public string Audience { get; set; }
            [DataMember]
            public string CreatedDate { get; set; }
            [DataMember]
            public string Text { get; set; }
        }

        #endregion

        #region Builder

        [DataContract]
        public class Builder
        {
            public Builder() { }
            public Builder(bool isDisabled, string managerDisplayName, string managerUserId, string accessTeamState, List<BuilderGroup> availableGroups, List<BuilderButton> availableButtons)
            {
                this.IsDisabled = isDisabled;
                this.ManagerDisplayName = managerDisplayName;
                this.ManagerUserId = managerUserId;
                this.AccessTeamState = AccessTeamState;
                this.AvailableGroups = availableGroups;
                this.AvailableButtons = availableButtons;
            }
            [DataMember]
            public bool IsDisabled { get; set; }
            [DataMember]
            public string ManagerDisplayName { get; set; }
            [DataMember]
            public string ManagerUserId { get; set; }
            [DataMember]
            public string AccessTeamState { get; set; }
            [DataMember]
            public List<BuilderGroup> AvailableGroups { get; set; }
            [DataMember]
            public List<BuilderButton> AvailableButtons { get; set; }
        }

        [DataContract]
        [Serializable]
        public class BuilderGroup
        {
            public BuilderGroup() { }
            public BuilderGroup(int groupId, string groupName, string description, bool isLargeGroup,
                bool isSelected, bool isDisabled, List<BuilderActor> availableActors, ActorGroupType actorGroupType)
            {
                this.GroupId = groupId;
                this.GroupName = groupName;
                this.Description = description;
                this.IsLargeGroup = isLargeGroup;
                this.IsSelected = isSelected;
                this.IsDisabled = isDisabled;
                this.AvailableActors = availableActors;
                this.ActorGroupType = actorGroupType;
            }
            public BuilderGroup CreateDeepCopy(BuilderGroup copyGroup)
            {
                MemoryStream m = new MemoryStream();
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(m, copyGroup);
                m.Position = 0;
                return (BuilderGroup)b.Deserialize(m);
            }
            [DataMember]
            public int GroupId { get; set; }
            [DataMember]
            public string GroupName { get; set; }
            [DataMember]
            public string Description { get; set; }
            [DataMember]
            public bool IsLargeGroup { get; set; }
            [DataMember]
            public bool IsSelected { get; set; }
            [DataMember]
            public bool IsDisabled { get; set; }
            [DataMember]
            public List<BuilderActor> AvailableActors { get; set; }
            [DataMember]
            public ActorGroupType ActorGroupType { get; set; }
        }

        [DataContract]
        [Serializable]
        public class BuilderActor
        {
            public BuilderActor() { }
            public BuilderActor(int actorId, string userId, string displayName,
                bool isDefault, bool isSelected, ActorGroupType actorGroupType, WorkflowState workflowState)
            {
                this.ActorId = actorId;
                this.UserId = userId;
                this.DisplayName = displayName;
                this.IsDefault = isDefault;
                this.IsSelected = isSelected;
                this.ActorGroupType = actorGroupType;
                this.WorkflowState = workflowState;
            }
            [DataMember]
            public int ActorId { get; set; }
            [DataMember]
            public string UserId { get; set; }
            [DataMember]
            public string DisplayName { get; set; }
            [DataMember]
            public bool IsDefault { get; set; }
            [DataMember]
            public bool IsSelected { get; set; }
            [DataMember]
            public ActorGroupType ActorGroupType { get; set; }
            [DataMember]
            public WorkflowState WorkflowState { get; set; }
        }

        [DataContract]
        public class BuilderButton
        {
            public BuilderButton() { }
            public BuilderButton(string buttonId, string buttonName, string actionId, bool isDisabled)
            {
                this.ButtonId = buttonId;
                this.ButtonName = buttonName;
                this.ActionId = actionId;
                this.IsDisabled = isDisabled;
            }
            [DataMember]
            public string ButtonId { get; set; }
            [DataMember]
            public string ButtonName { get; set; }
            [DataMember]
            public string ActionId { get; set; }
            [DataMember]
            public bool IsDisabled { get; set; }
        }

        #endregion

        #region Tracking

        [DataContract]
        public class TrackingBlade
        {
            public TrackingBlade() { }

            public TrackingBlade(int workflowId, string actorName, string workflowStatus, string dueDate, string completedDate, int actorGroupType, string workflowComments, string workflowCommentsStyle)
            {
                this.WorkflowId = workflowId;
                this.ActorName = actorName;
                this.WorkflowStatus = workflowStatus;
                this.DueDate = dueDate;
                this.CompletedDate = completedDate;
                this.ActorGroupType = actorGroupType;
                this.WorkflowComments = workflowComments;
				this.WorkflowCommentsStyle = WorkflowCommentsStyle;
            }

            [DataMember]
            public int WorkflowId { get; set; }

            [DataMember]
            public string ActorName { get; set; }

            [DataMember]
            public string WorkflowStatus { get; set; }

            [DataMember]
            public string DueDate { get; set; }

            [DataMember]
            public string CompletedDate { get; set; }

            [DataMember]
            public int ActorGroupType { get; set; }

            [DataMember]
            public string WorkflowComments { get; set; }

			[DataMember]
			public string WorkflowCommentsStyle { get; set; }

        }

        #endregion

        #region Search
        [DataContract]
        public class Search
        {
            public Search() { }
            public Search(string primary, string contents, string rangeStart, string rangeEnd)
            {
                this.Primary = primary;
                this.Contents = contents;
                this.RangeStart = rangeStart;
                this.RangeEnd = rangeEnd;
            }

            [DataMember]
            public string Primary { get; set; }
            [DataMember]
            public string Contents { get; set; }
            [DataMember]
            public string RangeStart { get; set; }
            [DataMember]
            public string RangeEnd { get; set; }
        }
        #endregion

        #endregion

        private static int AccessTeamActorId = 1;

        #region WebMethod Calls

        public static List<UI.RequestBlade> GetRequestBlades(ViewIndex view, Search search)
        {
            //string userId = SnapSession.CurrentUser.LoginId;
            //string condition = "";
            //switch (view)
            //{
            //    case ViewIndex.my_requests:
            //        condition = string.Format("(userId==\"{0}\"||submittedBy==\"{0}\")",userId);
            //        break;
            //    case ViewIndex.my_approvals:
            //condition = string.Format("(workflowStatusEnum=={0}&&completedDate==null)||(workflowStatusEnum=={1})||(workflowStatusEnum=={2})||(workflowStatusEnum=={3}&&completedDate==null)||(workflowStatusEnum=={4}&&completedDate!=null)",
            //    (int)WorkflowState.Pending_Approval, (int)WorkflowState.Change_Requested, (int)WorkflowState.Approved, (int)WorkflowState.Not_Active, (int)WorkflowState.Closed_Denied);
            //        break;
            //    case ViewIndex.access_team:
            //        condition = "pkId>0";
            //        break;
            //    case ViewIndex.search:
            //        string primaryCondition = "";
            //        string contentsCondition = "";
            //        string rangeCondition = "";
            //        primaryCondition = string.Format("(userId==\"{0}\"||displayName==\"{0}\"||submittedBy==\"{0}\")", search.Primary);
                    
            //        if (search.Contents != string.Empty)
            //        {

            //        }                    

            //        condition = primaryCondition + contentsCondition + rangeCondition;

            //        break;
            //}
            //return Database.GetRequests(condition, userId, view);
            return Database.GetRequests(view, search);
        }

        public static string GetAccessTeamFilter()
        {
            string filter = "";
            List<int> openEnums = new List<int>();
            openEnums.Add((int)RequestState.Open);
            openEnums.Add((int)RequestState.Pending);
            openEnums.Add((int)RequestState.Change_Requested);
            RequestFilter newFilter = new RequestFilter();
            
            List<Filter> newFilters = new List<Filter>();

            Filter PendingAcknowledgementFilter = new Filter();
            Filter PendingWorkflowFilter = new Filter();
            Filter PendingTicketFilter = new Filter();
            Filter PendingProvisioningFilter = new Filter();
            Filter ChangeRequestedFilter = new Filter();
            Filter InWorkflowFilter = new Filter();

            List<int> PendingAcknowledgementList = new List<int>();
            List<int> PendingWorkflowList = new List<int>();
            List<int> PendingTicketList = new List<int>();
            List<int> PendingProvisioningList = new List<int>();
            List<int> ChangeRequestedList = new List<int>();
            List<int> InWorkflowList = new List<int>();

            using (var db = new SNAPDatabaseDataContext())
            {
                var filterCounts = from r in db.SNAP_Requests
                                 join w in db.SNAP_Workflows on r.pkId equals w.requestId
                                 join ws in db.SNAP_Workflow_States on w.pkId equals ws.workflowId
                                 join a in db.SNAP_Actors on w.actorId equals a.pkId
                                 where a.pkId == AccessTeamActorId
                                 && openEnums.Contains(r.statusEnum)
                                 && ws.completedDate == null
                                 group r by new { r.pkId, r.ticketNumber, ws.workflowStatusEnum } into grp
                                 select new
                                  {
                                      requestId = grp.Key.pkId,
                                      ticketNumber = grp.Key.ticketNumber,
                                      workflowStatusEnum = grp.Key.workflowStatusEnum
                                  };
                if(filterCounts != null)
                {
                    foreach(var row in filterCounts)
                    {
                        switch (row.workflowStatusEnum)
                        {
                            case (int)WorkflowState.Pending_Acknowledgement:
                                PendingAcknowledgementList.Add(row.requestId);
                                break;
                            case (int)WorkflowState.Pending_Workflow:
                                PendingWorkflowList.Add(row.requestId);
                                break;
                            case (int)WorkflowState.Approved:
                            case (int)WorkflowState.Pending_Provisioning:
                                if (row.ticketNumber == null) { PendingTicketList.Add(row.requestId); }
                                else { PendingProvisioningList.Add(row.requestId); }
                                break;
                            case (int)WorkflowState.Workflow_Created:
                                InWorkflowList.Add(row.requestId);
                                break;
                            case (int)WorkflowState.Change_Requested:
                                ChangeRequestedList.Add(row.requestId);
                                break;
                        }
                    }

                    PendingAcknowledgementFilter.FilterName = "Ack";
                    PendingAcknowledgementFilter.RequestIds = PendingAcknowledgementList;
                    PendingWorkflowFilter.FilterName = "WrkFl";
                    PendingWorkflowFilter.RequestIds = PendingWorkflowList;
                    PendingTicketFilter.FilterName = "Tkt";
                    PendingTicketFilter.RequestIds = PendingTicketList;
                    PendingProvisioningFilter.FilterName = "Prov";
                    PendingProvisioningFilter.RequestIds = PendingProvisioningList;
                    ChangeRequestedFilter.FilterName = "Chng Req";
                    ChangeRequestedFilter.RequestIds = ChangeRequestedList;
                    InWorkflowFilter.FilterName = "In WrkFl";
                    InWorkflowFilter.RequestIds = InWorkflowList;
                    newFilters.Add(PendingAcknowledgementFilter);
                    newFilters.Add(PendingWorkflowFilter);
                    newFilters.Add(PendingTicketFilter);
                    newFilters.Add(PendingProvisioningFilter);
                    newFilters.Add(ChangeRequestedFilter);
                    newFilters.Add(InWorkflowFilter);
                    newFilter.Filters = newFilters;

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(newFilter.GetType());
                    using (MemoryStream ms = new MemoryStream())
                    {
                        serializer.WriteObject(ms, newFilter);
                        string retVal = Encoding.Default.GetString(ms.ToArray());
                        filter = retVal;
                    }
                }
            }

            return filter;
        }

        public static string GetDetails(int requestId)
        {
            Role userRole = SnapSession.CurrentUser.CurrentRole;
            bool includeADManager = false;
            int[] commentFilter;
            switch (userRole)
            {
                case Role.AccessTeam:
                case Role.SuperUser:
                    commentFilter = new int[]{(int)CommentsType.Access_Notes_Requestor,
                        (int)CommentsType.Access_Notes_ApprovingManager,
                        (int)CommentsType.Access_Notes_AccessTeam};
                    includeADManager = true;
                    break;

                case Role.ApprovingManager:
                    commentFilter = new int[]{(int)CommentsType.Access_Notes_Requestor,
                        (int)CommentsType.Access_Notes_ApprovingManager};
                    break;

                case Role.Requestor:
                default:
                    commentFilter = new int[] { (int)CommentsType.Access_Notes_Requestor };
                    break;
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var textIds = from a in db.SNAP_Access_User_Texts
                              where a.requestId == requestId
                              group a by new { a.access_details_formId } into grp
                              select new { ID = grp.Max(s => s.pkId) };
                              
                List<int> currentTextIds = new List<int>();

                foreach (var id in textIds)
                {
                    currentTextIds.Add(id.ID);
                }

                var details = from r in db.SNAP_Requests
                              join ut in db.SNAP_Access_User_Texts on r.pkId equals ut.requestId
                              join adf in db.SNAP_Access_Details_Forms on ut.access_details_formId equals adf.pkId
                              where r.pkId == requestId && adf.isActive == true && currentTextIds.Contains(ut.pkId)
                              orderby adf.pkId ascending
                              select new
                              {
                                  Title = r.userTitle,
                                  Manager = r.managerDisplayName,
                                  ADManager = r.managerUserId,
                                  Requestor = r.submittedBy,
                                  UserId = r.userId,
                                  Label = adf.label,
                                  Text = ut.userText
                              };
                var comments = from r in db.SNAP_Requests
                               join rc in db.SNAP_Request_Comments on r.pkId equals rc.requestId
                               where r.pkId == requestId && commentFilter.Contains(rc.commentTypeEnum)
                               orderby rc.createdDate descending
                               select new
                               {
                                   Audience = MakeFriendlyCommentAudience((CommentsType)rc.commentTypeEnum),
                                   CreatedDate = rc.createdDate,
                                   Text = rc.commentText
                               };

                if (details != null)
                {
                    RequestDetails newDetails = new RequestDetails();
                    List<RequestFormField> newForm = new List<RequestFormField>();
                    string ADManager = String.Empty;
                    foreach (var detail in details)
                    {
                        if (ADManager == String.Empty) { ADManager = (includeADManager ? CompareManagerName(detail.UserId, detail.ADManager) : null); }
                        newDetails.Title = detail.Title;
                        newDetails.Manager = detail.Manager;
                        newDetails.ADManager = ADManager;
                        newDetails.Requestor = detail.Requestor;
						newDetails.SearchLinkUrl= Model.Utilities.WebRootUrl + Model.PageNames.SEARCH + ".aspx?requestId=" +  requestId.ToString();

                        RequestFormField newField = new RequestFormField();
                        newField.Label = detail.Label;
                        newField.Text = detail.Text;
                        newForm.Add(newField);
                    }
                    newDetails.Details = newForm;

                    if (comments != null)
                    {

                        List<RequestComments> newComments = new List<RequestComments>();

                        foreach (var comment in comments)
                        {
                            RequestComments newComment = new RequestComments();
                            newComment.Audience = comment.Audience;
                            newComment.CreatedDate = comment.CreatedDate.ToString("MMM d, yyyy") + " at " + comment.CreatedDate.ToString("h:mm tt");
                            newComment.Text = comment.Text;
                            newComments.Add(newComment);
                        }
                        newDetails.Comments = newComments;
                    }

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(newDetails.GetType());
                    using (MemoryStream ms = new MemoryStream())
                    {
                        serializer.WriteObject(ms, newDetails);
                        string retVal = Encoding.Default.GetString(ms.ToArray());
                        return retVal;
                    }
                }
                return string.Empty;
            }

        }

        public static string GetBuilder(int requestId)
        {
            List<string> groupList = new List<string>();
            Builder currentBuilder = new Builder();
            List<BuilderButton> availableButtons = new List<BuilderButton>();
            List<BuilderGroup> availableGroups = GetBuilderGroups();
            List<BuilderActor> requestActors = GetRequestActors(requestId);;
            bool isOneSelected = false;
            bool actorSelected;
            List<BuilderGroup> UpdatedGroups = new List<BuilderGroup>();

            //make a copy of available groups
            foreach (BuilderGroup group in availableGroups)
            {
                UpdatedGroups.Add(group.CreateDeepCopy(group));
            }

            //update groups with selected actors  
            foreach (BuilderGroup group in UpdatedGroups)
            {
                isOneSelected = false;
                foreach (BuilderActor actor in group.AvailableActors)
                {
                    actorSelected = requestActors.Exists(a => a.ActorId == actor.ActorId);
                    if (actorSelected)
                    {
                        isOneSelected = true;
                        actor.IsSelected = true;
                        group.IsSelected = true;
                        switch (actor.WorkflowState)
                        {
                            case WorkflowState.Approved:
                            case WorkflowState.Closed_Cancelled:
                            case WorkflowState.Closed_Completed:
                            case WorkflowState.Closed_Denied:
                                group.IsDisabled = true;
                                break;
                            default:
                                group.IsDisabled = false;
                                break;
                        }
                    }
                }
                if (!isOneSelected)
                {
                    foreach (BuilderActor actor in group.AvailableActors)
                    {
                        if (actor.IsDefault) { actor.IsSelected = true; }
                    }
                }
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var result = (from r in db.SNAP_Requests
                             where r.pkId == requestId
                             select new
                             {
                                 ManagerDisplayName = r.managerDisplayName.StripTitleFromUserName(),
                                 ManagerUserId = r.managerUserId
                             }).First();
                if (result != null)
                {
                    currentBuilder.ManagerDisplayName = result.ManagerDisplayName;
                    currentBuilder.ManagerUserId = result.ManagerUserId;
                }
            }
            WorkflowState accessTeamWorkflowState = (WorkflowState)ApprovalWorkflow.GetWorkflowState(ApprovalWorkflow.GetWorkflowId(AccessTeamActorId, requestId));
            currentBuilder.AccessTeamState = accessTeamWorkflowState.ToString();
            switch (accessTeamWorkflowState)
            {
                case WorkflowState.Approved:
                    availableButtons.Add(new BuilderButton("closed_cancelled_" + requestId, "Closed Cancelled", "3", false));
                    availableButtons.Add(new BuilderButton("closed_completed_" + requestId, "Closed Completed", "6", false));
                    availableButtons.Add(new BuilderButton("create_ticket_" + requestId, "Create Ticket", "5", false));
                    currentBuilder.IsDisabled = true;
                    break;
                case WorkflowState.In_Workflow:
                case WorkflowState.Pending_Approval:
                case WorkflowState.Change_Requested:
                    availableButtons.Add(new BuilderButton("closed_cancelled_" + requestId, "Closed Cancelled", "3", false));
                    currentBuilder.IsDisabled = true;
                    break;
                case WorkflowState.Pending_Acknowledgement:
                    availableButtons.Add(new BuilderButton("closed_cancelled_" + requestId, "Closed Cancelled", "3", true));
                    availableButtons.Add(new BuilderButton("create_workflow_" + requestId, "Create Workflow", "", true));
                    currentBuilder.IsDisabled = true;
                    break;
                case WorkflowState.Pending_Provisioning:
                    availableButtons.Add(new BuilderButton("closed_cancelled_" + requestId, "Closed Cancelled", "3", false));
                    availableButtons.Add(new BuilderButton("closed_completed_" + requestId, "Closed Completed", "6", false));
                    currentBuilder.IsDisabled = true;
                    break;
                case WorkflowState.Workflow_Created:
                    availableButtons.Add(new BuilderButton("closed_cancelled_" + requestId, "Closed Cancelled", "3", false));
                    availableButtons.Add(new BuilderButton("edit_workflow_" + requestId, "Edit Workflow", "", false));
                    availableButtons.Add(new BuilderButton("continue_workflow_" + requestId, "Continue Workflow", "", true));
                    currentBuilder.IsDisabled = true;
                    break;
                case WorkflowState.Pending_Workflow:
                    availableButtons.Add(new BuilderButton("closed_cancelled_" + requestId, "Closed Cancelled", "3", false));
                    availableButtons.Add(new BuilderButton("create_workflow_" + requestId, "Create Workflow", "", false));
                    currentBuilder.IsDisabled = false;
                    break;
            }
            currentBuilder.AvailableButtons = availableButtons;
            currentBuilder.AvailableGroups = UpdatedGroups;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(currentBuilder.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, currentBuilder);
                string retVal = Encoding.Default.GetString(ms.ToArray());
                return retVal;
            }
        }

        public static List<string> GetTrackingBlades(string requestId)
        {
            List<string> trackingBlades = new List<string>();
            DataTable trackingData = new DataTable();

            trackingData = TrackingBlades.GetAllTracking(requestId);

            foreach (DataRow trackingRow in trackingData.Rows)
            {
				string workflowComments = TrackingBlades.BuildBladeComments(Convert.ToInt32(trackingRow["workflow_pkid"]));
                TrackingBlade newBlade = new TrackingBlade()
                {
                    WorkflowId = Convert.ToInt32(trackingRow["workflow_pkid"].ToString()),
                    ActorName = trackingRow["workflow_actor_name"].ToString(),
                    WorkflowStatus = Convert.ToString((WorkflowState)Enum.Parse(typeof(WorkflowState), trackingRow["workflow_status"].ToString())).StripUnderscore(),
                    DueDate = WebUtilities.TestAndConvertDate(trackingRow["workflow_due_date"].ToString()),
                    CompletedDate = WebUtilities.TestAndConvertDate(trackingRow["workflow_completed_date"].ToString()),
                    ActorGroupType = Convert.ToInt32(trackingRow["actor_group_type"].ToString()),
					WorkflowComments = workflowComments,
					WorkflowCommentsStyle = (string.IsNullOrEmpty(workflowComments)) ? "style=\"display:none;\"" : ""
                };

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(newBlade.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, newBlade);
                    string retVal = Encoding.Default.GetString(ms.ToArray());
                    trackingBlades.Add(retVal);
                }
            }

            return trackingBlades;
        }

        #endregion

        #region Helper Methods
        private static string CompareManagerName(string userId, string mgrUserId)
        {
            try
            {
                ADUserDetail userDetail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(userId);
                if (userDetail.Manager != null)
                {
                    if (mgrUserId != userDetail.Manager.LoginName)
                    {
                        return userDetail.ManagerName;
                    }
                    else { return null; }
                }
                else
                {
                    return "Not Found";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ajax Tests > CompareManagerName [USERID:" + userId + ", MGRUSERID:" + mgrUserId + "]:\r\nMessage" + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }

            return string.Empty;
        }

        private static string MakeFriendlyCommentAudience(CommentsType commentType)
        {
            string audienceName = string.Empty;

            switch (commentType)
            {
                case CommentsType.Access_Notes_AccessTeam:
                    audienceName = "Access & Identity Management";
                    break;

                case CommentsType.Access_Notes_ApprovingManager:
                    audienceName = "Approving Managers";
                    break;

                case CommentsType.Access_Notes_SuperUser:
                case CommentsType.Access_Notes_Requestor:
                default:
                    audienceName = "Requestor";
                    break;
            }

            return audienceName;
        }

        public static List<BuilderGroup> GetBuilderGroups()
        {
            List<BuilderGroup> groupList = new List<BuilderGroup>();

            using (var db = new SNAPDatabaseDataContext())
            {
                var query = from sa in db.SNAP_Actors
                            join sag in db.SNAP_Actor_Groups on sa.actor_groupId equals sag.pkId
                            where sa.isActive == true && sag.isActive == true && sag.actorGroupType < 2
                            orderby sag.actorGroupType, sag.groupName ascending
                            select new
                            {
                                ActorId = sa.pkId,
                                UserId = sa.userId,
                                DisplayName = sa.displayName,
                                IsDefault = sa.isDefault,
                                IsLargeGroup = sag.isLargeGroup,
                                GroupId = sag.pkId,
                                GroupName = sag.groupName,
                                Description = sag.description,
                                ActorGroupType = sag.actorGroupType
                            };
                foreach (var actor in query)
                {
                    List<BuilderActor> actorList = new List<BuilderActor>();
                    BuilderActor newActor = new BuilderActor();
                    newActor.ActorId = actor.ActorId;
                    newActor.UserId = actor.UserId;
                    newActor.DisplayName = actor.DisplayName;
                    newActor.IsDefault = actor.IsDefault;
                    newActor.ActorGroupType = (ActorGroupType)actor.ActorGroupType;

                    BuilderGroup accessGroup = groupList.Find(delegate(BuilderGroup _grp)
                    {
                        if (_grp.GroupId == actor.GroupId)
                        {
                            // group exists
                            return true;
                        }
                        // group doesn't exist
                        return false;
                    });

                    if (accessGroup != null)
                    {
                        actorList = accessGroup.AvailableActors;
                        actorList.Add(newActor);
                        accessGroup.AvailableActors = actorList;
                    }
                    else
                    {

                        BuilderGroup newGroup = new BuilderGroup();
                        actorList.Add(newActor);
                        newGroup.GroupId = actor.GroupId;
                        newGroup.GroupName = actor.GroupName;
                        newGroup.Description = actor.Description;
                        newGroup.ActorGroupType = (ActorGroupType)actor.ActorGroupType;
                        newGroup.IsLargeGroup = (bool)actor.IsLargeGroup;
                        newGroup.AvailableActors = actorList;
                        newGroup.ActorGroupType = (ActorGroupType)actor.ActorGroupType;
                        groupList.Add(newGroup);
                    }
                }
            }

            return groupList;
        }

        public static List<BuilderActor> GetRequestActors(int requestId)
        {
            List<BuilderActor> actorList = new List<BuilderActor>();
            using (var db = new SNAPDatabaseDataContext())
            {
                var query = from a in db.SNAP_Actors
                            join sw in db.SNAP_Workflows on a.pkId equals sw.actorId
                            join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
                            where sw.requestId == requestId
                            group sw by new { a.userId, sw.requestId, sw.actorId } into wfGroup
                            select new
                            {
                                ActorId = wfGroup.Key.actorId,
                                UserId = wfGroup.Key.userId,
                                WorkflowId = wfGroup.Max(sw => sw.pkId) 
                            };

                foreach (var actor in query)
                {
                    BuilderActor newActor = new BuilderActor();
                    newActor.ActorId = actor.ActorId;
                    newActor.UserId = actor.UserId;
                    newActor.WorkflowState = (WorkflowState)ApprovalWorkflow.GetWorkflowState(actor.WorkflowId);
                    actorList.Add(newActor);
                }
            }
            return actorList;
        }
        #endregion
    }
}
