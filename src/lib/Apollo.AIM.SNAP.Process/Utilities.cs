using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Process
{
    public class Utilities
    {
        public static void OutputMessage(string msg, EventLogEntryType type)
        {
            /*
            EventLog evt = new EventLog();
            evt.Source = this.ServiceName;
            evt.WriteEntry(msg, type);
             */

            switch (type)
            {
                case EventLogEntryType.Error:
                    Logger.Error(msg);
                    break;
                case EventLogEntryType.Information:
                    Logger.Info(msg);
                    break;
                case EventLogEntryType.Warning:
                    Logger.Warn(msg);
                    break;
                default:
                    Logger.Info(msg);
                    break;
            }
        }


    }
}
