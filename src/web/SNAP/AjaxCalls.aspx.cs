using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Web.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace Apollo.AIM.SNAP.Web
{
    public partial class _AjaxCalls : SnapPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static List<UserManagerInfo> GetNames(string name)
        {
            List<UserManagerInfo> singleUser = new List<UserManagerInfo>();

            if (name == string.Empty)
            {
                return new List<UserManagerInfo>();
            }

            if (name.Length <= 8)
            {
                UserManagerInfo userManagerInfo = new UserManagerInfo();
                ADUserDetail userDetail;
                userDetail = DirectoryServices.GetUserByLoginName(name);

                if (userDetail != null)
                {
                    if (userDetail.LoginName != null && userDetail.ManagerName != null && userDetail.FirstName != null && userDetail.LastName != null)
                    {
                        userManagerInfo.LoginId = userDetail.LoginName;
                        userManagerInfo.ManagerLoginId = DirectoryServices.GetUserByFullName(userDetail.ManagerName).LoginName;
                        userManagerInfo.ManagerName = userDetail.ManagerName;
                        userManagerInfo.Name = userDetail.FirstName + " " + userDetail.LastName;
                        singleUser.Add(userManagerInfo);
                    }
                }
            }

            if (singleUser.Count == 0) { return DirectoryServices.GetSimplifiedUserManagerInfo(name); }
            else { return singleUser; }
        }


        [WebMethod]
        public static UserManagerInfo GetUserManagerInfoByFullName(string fullName)
        {
            return DirectoryServices.GetUserManagerInfoByFullName(fullName);
        }

/*
        public static bool CreateWorkflow(int requestId, string actorIds)
        {
            var accessReq = new AccessRequest(requestId);
            string[] split = null;
            char[] splitter = { '[' };
            split = actorIds.Split(splitter);

            List<int> actorsList = new List<int>();

            foreach (string actor in split)
            {
                if (actor.Length > 1) { actorsList.Add(Convert.ToInt32(actor.Replace("[", "").Replace("]", ""))); }
            }

            return accessReq.CreateWorkflow(actorsList);
        }
*/

        [WebMethod]
        public static bool CreateWorkflow(int requestId, string managerUserId, string actorIds)
        {
            var accessReq = new AccessRequest(requestId);
            List<int> actorsList = GetActorsList(actorIds);

            return accessReq.CreateWorkflow(managerUserId, actorsList);
        }

        [WebMethod]
        public static bool EditWorkflow(int requestId, string managerUserId, string actorIds)
        {
            var accessReq = new AccessRequest(requestId);
            List<int> actorsList = GetActorsList(actorIds);
            return accessReq.EditWorkflow(managerUserId, actorsList);
        }

        [WebMethod]
        public static int GetActorId(string userId, int groupId)
        {
            int actorId = 0;
            actorId = ApprovalWorkflow.GetActorIdByUserIdAndGroupId(userId, groupId);
            return actorId;
        }

        private static List<int> GetActorsList(string actorIds)
        {
            string[] split = null;
            char[] splitter = { '[' };
            split = actorIds.Split(splitter);

            List<int> actorsList = new List<int>();

            foreach (string actor in split)
            {
                if (actor.Length > 1) { actorsList.Add(Convert.ToInt32(actor.Replace("[", "").Replace("]", ""))); }
            }
            return actorsList;
        }
        
        [WebMethod]
        public static bool ApproverActions(int requestId, WorkflowAction action, string comments)
        {
            var currentUsrId = SnapSession.CurrentUser.LoginId;
            int wfId = 0;

            using (var db = new SNAPDatabaseDataContext())
            {
                wfId = db.GetActiveWorkflowId(requestId, currentUsrId);
                if (wfId == 0)
                    return false;
            }
            

            var accessReq = new AccessRequest(requestId);
            return accessReq.WorkflowAck(wfId, action, comments);

            /*
            switch (action)
            {
                case WorkflowState.Approved:
                    return accessReq.WorkflowAck(wfId, WorkflowAction.Approved);
                case WorkflowState.Change_Requested:
                    //accessReq.addRequestComment(comments, CommentsType.Requested_Change);
                    //accessReq.RequestChanged();
                    return true;
                    break;
                case WorkflowState.Closed_Denied:
                    return true;
                    break;
                default:
                    return false;
                    break;
            }
             */
        }
        
        [WebMethod]
        public static bool AccessTeamActions(int requestId, WorkflowAction action, string comments)
        {
            //TODO: get actorId from current user
            var accessReq = new AccessRequest(requestId);

            switch (action)
            {
                case WorkflowAction.Ack:
                    return accessReq.Ack();
                case WorkflowAction.Change:
                    return accessReq.RequestToChange(comments);
                case WorkflowAction.Cancel:
                    return accessReq.NoAccess(action, comments);
                case WorkflowAction.Denied:
                    return accessReq.NoAccess(action, comments);
                default:
                    return false;
            }
        }

        [WebMethod]
        public static bool BuilderActions(int requestId, WorkflowAction action)
        {
            //TODO: get actorId from current user
            var accessReq = new AccessRequest(requestId);

            switch (action)
            {
                case WorkflowAction.Cancel:
                    return accessReq.NoAccess(action, "");
                case WorkflowAction.Complete:
                    return accessReq.FinalizeRequest();
                case WorkflowAction.Ticket:
                    return accessReq.CreateServiceDeskTicket();
                default:
                    return false;
            }
        }

        [WebMethod]
        public static bool AccessComments(int requestId, CommentsType action, string comments)
        {
            var accessReq = new AccessRequest(requestId);
            return accessReq.AddComment(comments, action);
        }
    }

}
