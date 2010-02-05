using System;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.SessionState;
using System.Web;
using Apollo.Ultimus;
using Apollo.Ultimus.UltimusIncidents;
using Apollo.Ultimus.Web;
using TaskStatuses=Apollo.Ultimus.TaskStatuses;

namespace Apollo.AIM.SNAP.Web
{
	/// <summary>
	/// Summary description for UltimusTools.
	/// </summary>
	public static class UltimusTools

	{
        
		private static bool IsOfStatus( int TaskStatus, int TestStatus )
		{
			return (TaskStatus | TestStatus) == TaskStatus;
			
		}
		

        public static Hashtable GetVariableList(HttpSessionState session, HttpRequest request)
		{
            CapSession cs = new CapSession(session);
            return cs.GetTaskVariables(request);
		}
	    
		public static object GetVariableValue(string VariableName, HttpSessionState Session, HttpRequest Request)
		{
			if(GetVariableList(Session, Request).ContainsKey(VariableName))
			{
                return GetVariableList(Session, Request)[VariableName];
			}

			return null;
		}
		
		private static string appendStatus(string result, int statusNum, int testStatus, string append)
		{
			if (IsOfStatus(statusNum, testStatus)) {
				if (result.Length > 0) result += ", ";
				result += append;
			}
			
			return result;
		}
		
		public static string ConvertIntToStatusString(int statusNum)
		{
			string result = "";
			
			appendStatus(result, statusNum, TaskStatuses.TASK_STATUS_ACTIVE, "Active");
			appendStatus(result, statusNum, TaskStatuses.TASK_STATUS_ABORTED, "Aborted");
			appendStatus(result, statusNum, TaskStatuses.TASK_STATUS_COMPLETED, "Completed");
			appendStatus(result, statusNum, TaskStatuses.TASK_STATUS_DELAYED, "Delayed");
			appendStatus(result, statusNum, TaskStatuses.TASK_STATUS_FAILED, "Failed");
			appendStatus(result, statusNum, TaskStatuses.TASK_STATUS_INQUEUE, "InQueue");
			appendStatus(result, statusNum, TaskStatuses.TASK_STATUS_REJECTED, "Rejected");
			appendStatus(result, statusNum, TaskStatuses.TASK_STATUS_SKIPPED, "Skipped");

			return result;	
		}
		public static DateTime DoubleToDate(double number)
		{
			DateTime date = DateTime.Parse("1/1/1900");

			date = date.AddDays(number);

			return date;
		}
		public static UltTaskList GetAuditTaskList(string userId, HttpSessionState session)
		{
			UltTaskList list = new UltTaskList();
			Hashtable filter = new Hashtable();

			filter.Add("STEPLABEL", "Audit User");
            filter.Add("ASSIGNEDTOUSER", ConfigurationManager.AppSettings["personDomainName"] + "/" + userId);
			filter.Add("STATUS", TaskStatuses.TASK_STATUS_ACTIVE);

			list.GetFiltered(filter);

			return list;
		}

		public static TaskList GetTaskList(string StepLabel, string UserId , HttpSessionState Session, int FilterMask )
		{
			UltTaskList list = new UltTaskList();
			Hashtable filter = new Hashtable();

			filter.Add("STEPLABEL", "Audit User");
            filter.Add("ASSIGNEDTOUSER", ConfigurationManager.AppSettings["personDomainName"] + "/" + UserId);
			filter.Add("STATUS", TaskStatuses.TASK_STATUS_ACTIVE);

			list.GetFiltered(filter);
			TaskList tasks = new TaskList(list, Session);
			return tasks;
		}
		
		public static UltTaskList GetApprovalTaskList(string UserId, HttpSessionState Session)
		{
			TaskList tasks = GetTaskList("Manager Approval", UserId, Session, Filters.nFilter_Complete);

			tasks.Add( GetTaskList("Manager Approval", UserId, Session, Filters.nFilter_Overdue));
			tasks.Add( GetTaskList("Manager Approval", UserId, Session, Filters.nFilter_Urgent));
			
			tasks.Add( GetTaskList("Validate not Duplicate", UserId, Session, Filters.nFilter_Current));
			tasks.Add( GetTaskList("Validate not Duplicate", UserId, Session, Filters.nFilter_Overdue));
			tasks.Add( GetTaskList("Validate not Duplicate", UserId, Session, Filters.nFilter_Urgent));

			tasks.Add( GetTaskList("Manager Manager Approval", UserId, Session, Filters.nFilter_Current));
			tasks.Add( GetTaskList("Manager Manager Approval", UserId, Session, Filters.nFilter_Overdue));
			tasks.Add( GetTaskList("Manager Manager Approval", UserId, Session, Filters.nFilter_Urgent));

			tasks.Add( GetTaskList("Get Approval", UserId, Session, Filters.nFilter_Current));
			tasks.Add( GetTaskList("Get Approval", UserId, Session, Filters.nFilter_Overdue));
			tasks.Add( GetTaskList("Get Approval", UserId, Session, Filters.nFilter_Urgent));
			UltTaskList retval = new UltTaskList();
			for(int i = 0; i < tasks.Count; i++)
			{
			retval.Add(new UltTask(((UltTask)tasks[i]).TaskId));				
			}

			return retval;
		}
		/// <summary>
		/// Pass in a task id and this will search for a sub incident
		/// </summary>
		/// <param name="parentIncidentTaskID"></param>
		/// <returns></returns>
		internal static UltIncident GetSubIncidentForTaskID(string parentIncidentTaskID)
		{
			UltIncident inc;

			string			childProcessName=string.Empty;
			int				childProcessIncidentID=0;
			StringBuilder selectStr = new StringBuilder();

			selectStr.Append("Select * from subprocs where PTASKID='");
			selectStr.Append(parentIncidentTaskID);
			selectStr.Append("'");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["UltimusDb"]))
			{
				//get list of incidents from databound  table
                using (SqlCommand cmd = new SqlCommand(selectStr.ToString(), conn))
                {
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            while (dr.Read())
                            {
                                childProcessName = dr.GetString(dr.GetOrdinal("CNAME")).Trim();
                                childProcessIncidentID = dr.GetInt32(dr.GetOrdinal("CINCIDENT"));
                            }
                        }
                    }//dispose SqlDataReader
                }//dispose SqlCommand
            }//dispose SqlConnection
			if(childProcessName!=string.Empty && childProcessIncidentID >0)
			{
				inc = new UltIncident(childProcessName,childProcessIncidentID);
				return inc;
			}
			return null;
		}
	    
		internal static UltIncidentList GetListOfUserIncidents(int userID)
		{
			UltIncidentList ul = UltIncidentList.NewAccessRequestlist(userID);
			IList list = UltIncidentList.GetListOfUserIncidents(userID);
			int daysOld=30;
			try
			{
                daysOld = int.Parse(ConfigurationManager.AppSettings["MyStatusIncidentDaysOld"]);
			}
			catch{}
			
			if(list != null && list.Count > 0)
			{
				foreach(UltIncident i in list)
				{
					if(DateTime.Today.Subtract(i.InitiateDate).Days <= daysOld)
					{
						ul.Add(i);
					}
				}
			}
			return ul;
		}
	    
		internal static string GetCurrentIncidentCount(int userID)
		{
			return GetInicidentList(userID).Count.ToString();
			
		}

	    internal static UltIncident GetInicident(int incidentID, int requestedUserOID)
	    {
	        CapSession cs = new CapSession(HttpContext.Current.Session);
	        UltIncidentList list = cs.UserIncidentsList;
	        //Check to see if the sessions incident list is set if not, get it
	        if (list == null)
	        {
	            list = (UltIncidentList) GetInicidentList(requestedUserOID);
	        }

	        return list.GetIncident(incidentID);
	    }

	    internal static IList GetInicidentList(int userOID)
	    {
	        CapSession cs = new CapSession(HttpContext.Current.Session);

	        if ((cs.UserIncidentsList == null) || cs.RefreshIncidentList || cs.UserIncidentsList.UserOID != userOID)
	        {
	            cs.UserIncidentsList = GetListOfUserIncidents(userOID);
	        }

	        cs.RefreshIncidentList = false;
	        return cs.UserIncidentsList;
	    }
		
		static class Filters
		{
			public const int nFilter_Initiate = 1;
            public const int nFilter_Urgent = 2;
            public const int nFilter_Overdue = 4;
            public const int nFilter_Current = 8;
            public const int nFilter_Complete = 0x10;
            public const int nFilter_Archive = 0x20;
		}
	}
}
