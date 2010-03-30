using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public class SLAConfiguration
    {

		private  NameValueCollection _SLAConfig;
		
		#region "constructors"
		public SLAConfiguration()
		{
			_SLAConfig = new NameValueCollection();
		}

        public SLAConfiguration(NameValueCollection configs)
		{
			_SLAConfig = configs;
		}
		#endregion
		
		#region "properties"

        public string AccessTeamAckInMinute
		{

            get { return Initialized ? _SLAConfig[ConfigLabels.AccessTeamAckInMinute] : string.Empty; }
            set { _SLAConfig[ConfigLabels.AccessTeamAckInMinute] = value; }
		}


        public string AccessTeamCreateWorkflowInDays
        {
            get { return Initialized ? _SLAConfig[ConfigLabels.AccessTeamCreateWorkflowInDays] : string.Empty; }
            set { _SLAConfig[ConfigLabels.AccessTeamCreateWorkflowInDays] = value; }
        }


        public string ManagerApprovalInDays
        {

            get { return Initialized ? _SLAConfig[ConfigLabels.ManagerApprovalInDays] : string.Empty; }
            set { _SLAConfig[ConfigLabels.ManagerApprovalInDays] = value; }
        }

        public string TeamApprovalInDays
        {

            get { return Initialized ? _SLAConfig[ConfigLabels.TeamApprovalInDays] : string.Empty; }
            set { _SLAConfig[ConfigLabels.TeamApprovalInDays] = value; }
        }

        public string TechnicalApprovalInDays
        {

            get { return Initialized ? _SLAConfig[ConfigLabels.TechnicalApprovalInDays] : string.Empty; }
            set { _SLAConfig[ConfigLabels.TechnicalApprovalInDays] = value; }
        }

        #endregion


        private bool Initialized
        {
            get { return _SLAConfig != null && _SLAConfig.Count >= 5 ? true : false; }
        }

    }




   
    public static class ConfigLabels
    {
        #region "strings"
        public const string AccessTeamAckInMinute = "AccessTeamAckInMinute";
        public const string AccessTeamCreateWorkflowInDays = "AccessTeamCreateWorkflowInDays";
        public const string ManagerApprovalInDays = "ManagerApprovalInDays";
        public const string TeamApprovalInDays = "TeamApprovalInDays";
        public const string TechnicalApprovalInDays = "TechnicalApprovalInDays";
        #endregion
    }

}
