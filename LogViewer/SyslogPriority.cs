using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogViewer.Enumerations;

namespace LogViewer
{
    public struct Pri
    {
        public SyslogFacility Facility;
        public SyslogPriority Severity;
        public Pri(string strPri)
        {
            int intPri = Convert.ToInt32(strPri);
            int intFacility = intPri >> 3;
            int intSeverity = intPri & 0x7;
            this.Facility = (SyslogFacility)Enum.Parse(typeof(SyslogFacility),
               intFacility.ToString());
            this.Severity = (SyslogPriority)Enum.Parse(typeof(SyslogPriority),
               intSeverity.ToString());
        }
        public override string ToString()
        {
            return string.Format("{0}.{1}", this.Facility, this.Severity);
        }
    }
}
