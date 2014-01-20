using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewer
{
    public class LogMessageEventArgs : EventArgs
    {
        public readonly SyslogMessage Message;

        public LogMessageEventArgs(SyslogMessage message)
        {
            Message = message;
        }
    }
}
