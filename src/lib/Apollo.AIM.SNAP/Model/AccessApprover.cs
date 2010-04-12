using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    [Serializable]
    public class AccessApprover
    {
        public int ActorId { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelected { get; set; }
        public ActorGroupType ActorGroupType { get; set; }
        public WorkflowState WorkflowState { get; set; }
    }
}
