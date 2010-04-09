using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public class AccessApprover
    {
        public int ActorId { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public bool IsDefault { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public ActorApprovalType ActorApprovalType { get; set; }
        public WorkflowState WorkflowState { get; set; }
    }
}
