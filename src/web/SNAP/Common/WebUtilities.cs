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

    public class TrackingBlades
    {
		private static DataTable _filteredTrackingData;
		private static DataTable _unfilteredTrackingData;

        public static DataTable GetAllTracking(string requestId)
        {
			// Get all the raw tracking data, then build an empty DataTable and use the helper 
			// methods below to parse the raw data into a filtered set...
			//
			_unfilteredTrackingData = Database.GetAllTrackingData(requestId);
			_filteredTrackingData = Database.BuildEmptyTrackingBladeTable();

            // Note: Build these backwards to force sort order...
			//
            BuildTechnicalApprovers();
            BuildTeamApprovers();
            BuildManagerTracking();
            BuildAIMTracking();

            return _filteredTrackingData;
        }

        #region Tracking Groups

        private static void BuildAIMTracking()
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

        private static void BuildManagerTracking()
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

        private static void BuildTeamApprovers()
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

        private static void BuildTechnicalApprovers()
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
            DataTable workflowCommentsTable = Database.GetWorkflowComments(WorkflowId);

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
