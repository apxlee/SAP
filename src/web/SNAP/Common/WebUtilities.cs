using System;
using System.Reflection;
using System.Threading;
using System.Configuration;
using System.Net;
using System.Text;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.CA.Logging;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class WebUtilities
    {
		public static string ClientScriptPath
		{
			get { return VirtualPathUtility.ToAbsolute( ConfigurationManager.AppSettings["OOSPAScriptsPath"] ); }
		}
		
		public static string AppVersion
		{
			get
			{
			    return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			    //return ConfigurationManager.AppSettings["OOSPAVersion"];
			}
		}
		
		public static string CurrentServer
		{
			get { return HttpContext.Current.Server.MachineName; }
		}
		
		public static string GetPageName(Page currentPage)
		{
			return currentPage.GetType().Name.StripUnderscoreAndExtension().ToLower();
		}
		
		public static bool IsSuperUser(string networkId)
		{
			string[] superUsers = ConfigurationManager.AppSettings["SNAPSuperUsers"].ToString().Split(',');
			foreach (string user in superUsers)
			{
				if (user == networkId) { return true; }
			}
			return false;
		}

        public static Control FindControlRecursive(Control controlRoot, string controlId)
        {
            if (controlId == string.Empty) { return null; }
            if (controlRoot.ID == controlId) { return controlRoot; }
            foreach (Control childControl in controlRoot.Controls)
            {
                Control target = FindControlRecursive(childControl, controlId);
                if (target != null) { return target; }
            }
            return null;
        }

		public static void SetRibbonContainerClass(string className)
		{
			Page currentPage = HttpContext.Current.Handler as Page;
			Panel ribbonContainer = (Panel)WebUtilities.FindControlRecursive(currentPage, "_ribbonContainerOuter");
			ribbonContainer.CssClass = className;
		}

		public static PlaceHolder WrapControl(WebControl innerControl, string outerElementName, string outerElementID)
		{
			PlaceHolder placeHolder = new PlaceHolder();

			Literal openElement = new Literal();
			openElement.Text = string.Format("<{0} id='{1}'>", outerElementName, outerElementID.ToLower());

			Literal closeElement = new Literal();
			closeElement.Text = "</" + outerElementName + ">";

			placeHolder.Controls.Add(openElement);
			placeHolder.Controls.Add(innerControl);
			placeHolder.Controls.Add(closeElement);

			return placeHolder;
		}		
		
		public static void RoleCheck(string pageName)
		{
			Role currentRole = SnapSession.CurrentUser.CurrentRole;
			//TODO REMOVE Logger.Info(WebUtilities.GetTimestamp() + "WebUtilities > RoleCheck (85):" + currentRole.ToString() + "\r\n"); 
			bool isRedirect = true;

			if (currentRole == Role.SuperUser) {isRedirect = false;}
			else
			{
				if (pageName.ToLower() == PageNames.ACCESS_TEAM.ToLower())
				{
					isRedirect = (currentRole == Role.AccessTeam) ? false : true;
				}
				else if (pageName.ToLower() == PageNames.APPROVING_MANAGER.ToLower())
				{
					isRedirect = (currentRole == Role.ApprovingManager) ? false : true;
				}
			}

			//TODO REMOVE Logger.Info(WebUtilities.GetTimestamp() + "WebUtilities > isRedirect (102):" + isRedirect + "\r\n"); 
			if (isRedirect) { WebUtilities.Redirect("AppError.aspx?errorReason=wrongRole", true); }
		}
         
        public static void Redirect(string redirectUrl, bool endResponse)
        {
			try
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Response.Redirect(redirectUrl, endResponse);
			}
			catch (ThreadAbortException)
			{
				// No need to log this silly ThreadAbortException, can't get around this
			}
			
			//HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        
        public static void Redirect(string pageConstant)
        {
			Redirect(pageConstant + ".aspx", true);
        }

		public static string GetTimestamp()
		{
			return "[" + DateTime.Now.ToString("HH:mm:ss.ffff") + "] ";
		}

        public static string TestAndConvertDate(string date)
        {
            if (!string.IsNullOrEmpty(date.ToString())) { return Convert.ToDateTime(date).ToString("MMM d\\, yyyy"); }
            else { return "-"; }
        }
     }

    public class FilteredTrackingData
    {
        private DataTable _filteredTrackingData;
        private DataTable _unfilteredTrackingData;

        public DataTable GetAllTracking(string requestId)
        {
            _unfilteredTrackingData = GetAllTrackingData(requestId);

            _filteredTrackingData = BuildEmptyTrackingBladeTable();

            // build these backwards to force sort order
            BuildTechnicalApprovers();
            BuildTeamApprovers();
            BuildManagerTracking();
            BuildAIMTracking();

            return _filteredTrackingData;
        }

        #region Tracking Groups

        private void BuildAIMTracking()
        {
            try
            {
                DataRow selectedRow;

                selectedRow = (
                    from bladeRow in _unfilteredTrackingData.AsEnumerable()
                    where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Workflow_Admin
                    select bladeRow).First();

                if ((int)selectedRow["workflow_status"] == (int)WorkflowState.Workflow_Created)
                {
                    selectedRow.SetField("workflow_status", WorkflowState.In_Workflow);
                    selectedRow.SetField("workflow_due_date", string.Empty);
                }
                else if ((int)selectedRow["workflow_status"] == (int)WorkflowState.Approved)
                {
                    selectedRow.SetField("workflow_status", WorkflowState.Pending_Provisioning);
                }

                _filteredTrackingData.ImportRow(selectedRow);
            }
            catch
            {
                // TODO: Linq returns exception if query is null.  Need to refactor?
            }
        }

        private void BuildManagerTracking()
        {
            try
            {
                DataRow selectedRow;

                selectedRow = (
                    from bladeRow in _unfilteredTrackingData.AsEnumerable()
                    where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Manager
                    select bladeRow).First();

                _filteredTrackingData.ImportRow(selectedRow);
            }
            catch
            {
                // TODO: Linq returns exception if query is null.  Need to refactor?
            }
        }

        private void BuildTeamApprovers()
        {
            try
            {
                DataRow selectedRow;

                var distinctTeamApprovers = (
                    from bladeRow in _unfilteredTrackingData.AsEnumerable()
                    where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Team_Approver
                    select bladeRow["workflow_actor_name"]).Distinct();

                foreach (string teamApprover in distinctTeamApprovers)
                {
                    selectedRow = (
                        from bladeRow in _unfilteredTrackingData.AsEnumerable()
                        where (string)bladeRow["workflow_actor_name"] == teamApprover
                        where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Team_Approver
                        select bladeRow).First();

                    _filteredTrackingData.ImportRow(selectedRow);
                }
            }
            catch
            {
                // TODO: Linq returns exception if query is null.  Need to refactor?
            }
        }

        private void BuildTechnicalApprovers()
        {
            try
            {
                DataRow selectedRow;

                var distinctTechnicalApprovers = (
                    from bladeRow in _unfilteredTrackingData.AsEnumerable()
                    where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Technical_Approver
                    select bladeRow["workflow_actor_name"]).Distinct();

                foreach (string technicalApprover in distinctTechnicalApprovers)
                {
                    selectedRow = (
                        from bladeRow in _unfilteredTrackingData.AsEnumerable()
                        where (string)bladeRow["workflow_actor_name"] == technicalApprover
                        where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Technical_Approver
                        select bladeRow).First();

                    _filteredTrackingData.ImportRow(selectedRow);
                }
            }
            catch
            {
                // TODO: Linq returns exception if query is null.  Need to refactor?
            }
        }

        #endregion

        public static string BuildBladeComments(int WorkflowId)
        {
            DataTable workflowCommentsTable = GetWorkflowComments(WorkflowId);

            if (workflowCommentsTable.Rows.Count > 0)
            {
                StringBuilder workflowComments = new StringBuilder();

                foreach (DataRow comment in workflowCommentsTable.Rows)
                {
                    // TODO: move string to config file?
                    workflowComments.AppendFormat("<p{0}><u>{1} by {2} on {3}</u><br />{4}</p>"
                        , (bool)comment["is_new"] ? " class=csm_error_text" : string.Empty
                        , Convert.ToString((CommentsType)Enum.Parse(typeof(CommentsType), comment["action"].ToString())).StripUnderscore()
                        , (comment["action"].ToString() == CommentsType.Email_Reminder.ToString()) ? "AIM" : comment["workflow_actor"].ToString()
                        , Convert.ToDateTime(comment["comment_date"]).ToString("MMM d\\, yyyy")
                        , comment["comment"].ToString().Replace("\n", "<br />"));
                }

                return workflowComments.ToString();
            }

            return string.Empty;
        }

        public static DataTable GetWorkflowComments(int workflowId)
        {
            // NOTE: dataset must be ordered from first to last
            //
            DataTable table = new DataTable();
            table.Columns.Add("action", typeof(string));
            table.Columns.Add("workflow_actor", typeof(string));
            table.Columns.Add("comment_date", typeof(string));
            table.Columns.Add("comment", typeof(string));
            table.Columns.Add("is_new", typeof(bool));

            using (var db = new SNAPDatabaseDataContext())
            {
                var workflowComments = from w in db.SNAP_Workflows
                                       join a in db.SNAP_Actors on w.actorId equals a.pkId
                                       join wc in db.SNAP_Workflow_Comments on w.pkId equals wc.workflowId
                                       where w.pkId == workflowId
                                       orderby wc.pkId ascending
                                       select new
                                       {
                                           commentTypeEnum = wc.commentTypeEnum,
                                           actorName = a.displayName,
                                           createdDate = wc.createdDate,
                                           commentText = wc.commentText,
                                       };

                if (workflowComments != null)
                {
                    foreach (var comment in workflowComments)
                    {
                        table.Rows.Add(
                            comment.commentTypeEnum.ToString()
                            , (comment.actorName == "Access & Identity Management") ? "AIM" : comment.actorName
                            , TestAndConvertDate(comment.createdDate.ToString())
                            , comment.commentText
                            , (comment.commentTypeEnum == (int)CommentsType.Requested_Change || comment.commentTypeEnum == (int)CommentsType.Email_Reminder) ? true : false);
                    }
                }

                return table;
            }
        }

        public DataTable GetAllTrackingData(string requestId)
        {
            DataTable unfilteredTrackingData = BuildEmptyTrackingBladeTable();

            using (var db = new SNAPDatabaseDataContext())
            {
                var tracking = from wf in db.SNAP_Workflows
                               where wf.requestId == Convert.ToInt32(requestId)
                               join ws in db.SNAP_Workflow_States on wf.pkId equals ws.workflowId
                               join a in db.SNAP_Actors on wf.actorId equals a.pkId
                               join ag in db.SNAP_Actor_Groups on a.actor_groupId equals ag.pkId
                               orderby ws.workflowId ascending, ws.pkId descending
                               select new
                               {
                                   display_name = a.displayName,
                                   workflow_status = ws.workflowStatusEnum,
                                   workflow_due_date = ws.dueDate,
                                   workflow_completed_date = ws.completedDate,
                                   workflow_pkid = ws.workflowId,
                                   actor_group_type = ag.actorGroupType
                               };

                foreach (var trackingRow in tracking)
                {
                    unfilteredTrackingData.Rows.Add
                        (
                            trackingRow.display_name
                            , trackingRow.workflow_status
                            , trackingRow.workflow_due_date
                            , trackingRow.workflow_completed_date
                            , trackingRow.workflow_pkid
                            , trackingRow.actor_group_type
                        );
                }
            }

            return unfilteredTrackingData;
            // datatable > rows > Non-Public members > list > Results View
        }

        private DataTable BuildEmptyTrackingBladeTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("workflow_actor_name", typeof(string));
            table.Columns.Add("workflow_status", typeof(int));
            table.Columns.Add("workflow_due_date", typeof(string));
            table.Columns.Add("workflow_completed_date", typeof(DateTime));
            table.Columns.Add("workflow_pkid", typeof(int));
            table.Columns.Add("actor_group_type", typeof(int));

            return table;
        }

        public static string TestAndConvertDate(string date)
        {
            if (!string.IsNullOrEmpty(date.ToString())) { return Convert.ToDateTime(date).ToString("MMM d\\, yyyy"); }
            else { return "-"; }
        }
    }

    public static class ExtensionMethods
    {
		public static string StripTitleFromUserName(this String value)
		{
			try
			{
				return value.Remove( value.ToString().IndexOf("(") );
			}
			catch
			{
				return value;
			}
		}
		
		public static string StripUnderscore(this String value)
		{
			return value.Replace("_", " ");
		}
		
		public static string StripUrlDelimitersAndExtension(this String value)
		{
			try
			{
				string s = value.TrimStart('~', '/');
				return s.Remove(s.ToString().IndexOf("."));
			}
			catch
			{
				return value;
			}
		}
		
		public static string StripUnderscoreAndExtension(this String value)
		{
			try
			{
				return value.Remove( value.ToString().IndexOf("_") );
			}
			catch
			{
				return value;
			}
		}
    }
}
