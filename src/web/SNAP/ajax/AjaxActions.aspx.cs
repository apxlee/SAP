using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.ajax
{
    public partial class AjaxActions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }





        [WebMethod]
        public static WebMethodResponse AccessTeamActions(int requestId, WorkflowAction action, string comments)
        {
            //TODO: get actorId from current user
            var accessReq = new AccessRequest(requestId);
            comments = comments.FromJSONStringToObj<string>();
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
                    return new WebMethodResponse(false, "AIM", "Unknown operation");
            }
        }


        [WebMethod]
        public static WebMethodResponse CreateWorkflow(int requestId, string managerUserId, string actorIds)
        {
            var accessReq = new AccessRequest(requestId);
            List<int> actorsList = GetActorsList(actorIds);

            return accessReq.CreateWorkflow(managerUserId, actorsList);
        }

        [WebMethod]
        public static WebMethodResponse EditWorkflow(int requestId, string managerUserId, string actorIds)
        {
            var accessReq = new AccessRequest(requestId);
            List<int> actorsList = GetActorsList(actorIds);
            return accessReq.EditWorkflow(managerUserId, actorsList);
        }


        [WebMethod]
        public static WebMethodResponse BuilderActions(int requestId, WorkflowAction action)
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
                    return new WebMethodResponse(false, "BuilderActions", "Unknown action");
            }
        }


        [WebMethod]
        public static WebMethodResponse ApproverActions(int requestId, WorkflowAction action, string comments)
        {
            var currentUsrId = SnapSession.CurrentUser.DistributionGroup != null ? SnapSession.CurrentUser.DistributionGroup : SnapSession.CurrentUser.LoginId;
            int wfId = 0;

            using (var db = new SNAPDatabaseDataContext())
            {
                wfId = db.GetActiveWorkflowId(requestId, currentUsrId);
                if (wfId == 0)
                    return new WebMethodResponse(false, "Approver Action Failed", "Approver has nothing to approve");
            }


            var accessReq = new AccessRequest(requestId);
            comments = comments.FromJSONStringToObj<string>();
            return accessReq.WorkflowAck(wfId, action, comments);

        }


        [WebMethod]
        public static WebMethodResponse AccessComments(int requestId, CommentsType action, string comments)
        {
            var accessReq = new AccessRequest(requestId);
            comments = comments.FromJSONStringToObj<string>();
            return accessReq.AddComment(comments, action);
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

    }
}
