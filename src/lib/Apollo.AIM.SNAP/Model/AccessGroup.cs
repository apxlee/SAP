using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    [Serializable]
    public class AccessGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool IsLargeGroup { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDisabled { get; set; }
        public List<AccessApprover> AvailableApprovers { get; set; }
        public ActorGroupType ActorGroupType { get; set; }
        public AccessGroup CreateDeepCopy(AccessGroup copyGroup)
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, copyGroup);
            m.Position = 0;
            return (AccessGroup)b.Deserialize(m);
        }
    }
}
