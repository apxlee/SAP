using System;
using System.Data;
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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using Apollo.CA.Logging;
using Apollo.AIM.SNAP.Web.JONTEST;

namespace Apollo.AIM.SNAP.Web.Common
{
	public class TrackingUI
	{
		public static List<string> GetTrackingBlades(string requestId)
		{
			List<string> trackingBlades = new List<string>();
			DataTable trackingData = new DataTable();
			FilteredTrackingData trackingClass = new FilteredTrackingData();

			trackingData = trackingClass.GetAllTracking(requestId);
			// datatable > rows > Non-Public members > list > Results View

			foreach (DataRow trackingRow in trackingData.Rows)
			{
				TrackingBlade newBlade = new TrackingBlade()
				{
					WorkflowId = Convert.ToInt32(trackingRow["workflow_pkid"].ToString()),
					ActorName = trackingRow["workflow_actor_name"].ToString(),
					//WorkflowStatus = trackingRow["workflow_status"].ToString(),
					WorkflowStatus = Convert.ToString((WorkflowState)Enum.Parse(typeof(WorkflowState), trackingRow["workflow_status"].ToString())).StripUnderscore(),
					DueDate = trackingRow["workflow_due_date"].ToString(),
					CompletedDate = trackingRow["workflow_completed_date"].ToString(),
					ActorGroupType = Convert.ToInt32(trackingRow["actor_group_type"].ToString()),
					WorkflowComments = FilteredTrackingData.BuildBladeComments(Convert.ToInt32(trackingRow["workflow_pkid"]))
				};

				// TODO: move to utility
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

		[DataContract]
		public class TrackingBlade
		{
			public TrackingBlade() { }
			
			public TrackingBlade(int workflowId, string actorName, string workflowStatus, string dueDate, string completedDate, int actorGroupType, string workflowComments)
			{
				this.WorkflowId = workflowId;
				this.ActorName = actorName;
				this.WorkflowStatus = workflowStatus;
				this.DueDate = dueDate;
				this.CompletedDate = completedDate;
				this.ActorGroupType = actorGroupType;
				this.WorkflowComments = workflowComments;
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
			//public List<WorkflowComments> WorkflowComments { get; set; }
			public string WorkflowComments { get; set; }
		}

		[DataContract]
		public class WorkflowComments
		{
			public WorkflowComments() { }
			public WorkflowComments(string action, string actorName, string commentDate, string comment, bool isNew)
			{
				this.Action = action;
				this.ActorName = actorName;
				this.CommentDate = commentDate;
				this.Comment = comment;
				this.IsNew = isNew;
			}

			[DataMember]
			public string Action { get; set; }

			[DataMember]
			public string ActorName { get; set; }

			[DataMember]
			public string CommentDate { get; set; }

			[DataMember]
			public string Comment { get; set; }

			[DataMember]
			public bool IsNew { get; set; }
		}
	}
}
