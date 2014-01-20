using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogViewer;
using LogViewer.Enumerations;

namespace TestLogParser
{
    [TestClass]
    public class LogParserTest
    {
        [TestMethod]
        public void Test_RFC_Input()
        {
            string rawMsg = "<133>Jul 19 19:05:32 GRAFFEN-PC NLog: This is a sample trace message";
            SyslogMessage m = new SyslogMessage().ParseSyslogMessage("127.0.0.1", rawMsg);

            Assert.AreEqual(m.Facility,SyslogFacility.Local0);
            Assert.AreEqual(m.Severity, SyslogPriority.Notice);
            Assert.AreEqual(m.Message, "This is a sample trace message");
            Assert.AreEqual(m.SourceIP, "127.0.0.1");
            Assert.AreEqual(m.Tag, "NLog");
        }

        [TestMethod]
        public void Test_GrandStream_Input()
        {
            string rawMsg = "<14>GS_LOG: [00:0B:82:06:46:BF][000][FFFF][01000821] Send SIP message: 8 To 87.54.25.114:5060";
            SyslogMessage m = new SyslogMessage().ParseSyslogMessage("127.0.0.1", rawMsg);

            Assert.AreEqual(m.Facility, SyslogFacility.User);
            Assert.AreEqual(m.Severity, SyslogPriority.Informational);
            Assert.AreEqual(m.Message, "[00:0B:82:06:46:BF][000][FFFF][01000821] Send SIP message: 8 To 87.54.25.114:5060");
            Assert.AreEqual(m.SourceIP, "127.0.0.1");
            Assert.AreEqual(m.Tag, "GS_LOG");
        }

        [TestMethod]
        public void Test_Netgear_Input()
        {
            string rawMsg = "<8>[System boot up]";
            SyslogMessage m = new SyslogMessage().ParseSyslogMessage("127.0.0.1", rawMsg);

            Assert.AreEqual(m.Facility, SyslogFacility.User);
            Assert.AreEqual(m.Severity, SyslogPriority.Emergency);
            Assert.AreEqual(m.Message, "[System boot up]");
            Assert.AreEqual(m.SourceIP, "127.0.0.1");
            Assert.AreEqual(m.Tag, "");
        }
    }
}
