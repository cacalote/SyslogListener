using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LogViewer
{
    public class SyslogMessage
    {
        public DateTime TimeStamp { get; set; }
        public string SourceIP { get; set; }
        public string SourceSystem { get; set; }
        public Enumerations.SyslogFacility Facility { get; set; }
        public Enumerations.SyslogPriority Severity { get; set; }
        public string Tag { get; set; }
        public string Message { get; set; }
        private int processingCount = 0;
        private static object _lockObj = new object();

        /// <summary>
        /// Parses the syslog rawMessage.
        /// </summary>
        /// <param name="rawMessage">The raw rawMessage.</param>
        /// <returns>A SyslogMessage object containing the rawMessage fields</returns>
        public SyslogMessage ParseSyslogMessage(string senderIP, string rawMessage)
        {
            // RFC format: <133>Jul 19 19:05:32 GRAFFEN-PC NLog: This is a sample trace message
            // GS Format:  <14>GS_LOG: [00:0B:82:06:46:BF][000][FFFF][01000821] Send SIP message: 8 To 87.54.25.114:5060

            lock (_lockObj)
            {
                Regex r = new Regex(@"<(?<Priority>[0-9]{1,3})>(?<Date>[A-z]{3}\s[\d]{2}\s[\d]{2}:[\d]{2}:[\d]{2})\s(?<SourceSystem>[A-z0-9\-\.]*)\s((?<Tag>[A-z0-9]*):\s)?(?<Message>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
                Match m;
                Pri p;
                SyslogMessage msg = new SyslogMessage();

                m = r.Match(rawMessage);
                // Check for valid DATE stamp
                if ((m.Groups["Date"].Value == String.Empty) && (processingCount == 0))
                {
                    var msgParts = rawMessage.Split('>');
                    StringBuilder newMsg = new StringBuilder();
                    newMsg.Append(msgParts[0]);
                    newMsg.Append(">");
                    newMsg.Append(DateTime.Now.ToString("MMM dd HH:mm:ss"));
                    newMsg.Append(" ");
                    newMsg.Append(senderIP);
                    newMsg.Append(" ");
                    newMsg.Append(msgParts[1]);
                    // Re-parse this new message
                    m = r.Match(newMsg.ToString());
                }
                p = new Pri(m.Groups["Priority"].Value);
                msg = new SyslogMessage();
                msg.TimeStamp = DateTime.Now;
                msg.SourceIP = senderIP;
                msg.SourceSystem = m.Groups["SourceSystem"].Value;
                msg.Severity = p.Severity;
                msg.Facility = p.Facility;
                msg.Tag = m.Groups["Tag"].Value;
                msg.Message = m.Groups["Message"].Value;
                processingCount = 0;
                return msg;
            }
        }
    }


}
