using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows;

namespace LogViewer
{
    public class Worker
    {
        readonly object stopLock = new object();
        bool stopping = false;
        bool stopped = false;

        public bool Stopping
        {
            get
            {
                lock (stopLock)
                {
                    return stopping;
                }
            }
        }

        public bool Stopped
        {
            get
            {
                lock (stopLock)
                {
                    return stopped;
                }
            }
        }

        public void Stop()
        {
            lock (stopLock)
            {
                stopping = true;
            }
        }

        void SetStopped()
        {
            lock (stopLock)
            {
                stopped = true;
            }
        }

        //Event stuff
        public delegate void MessageReceivedHandler(object o, LogMessageEventArgs e);
        public event MessageReceivedHandler MessageReceived;

        public void OnMessageReceived(LogMessageEventArgs e)
        {
            if (MessageReceived != null)
            { 
                MessageReceived(new object(), e); 
            }
        }

        private UdpClient listener;
        private IPEndPoint groupEP;

        public void Run()
        {

            try
            {
                listener = new UdpClient(514);
                groupEP = new IPEndPoint(IPAddress.Any, 514);

                //byte[] bytes = listener.Receive(ref groupEP);
                listener.BeginReceive(OnClientConnect, null);
            }
            finally
            {

            }
        }

        private void OnClientConnect(IAsyncResult ar)
        {
            byte[] buffer;

            buffer = listener.EndReceive(ar, ref groupEP);
            string rawMessage = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            
            // Throw rawMessage to parser
            var args = new LogMessageEventArgs(new SyslogMessage().ParseSyslogMessage(groupEP.Address.ToString(), rawMessage));
            
            // Fire event
            OnMessageReceived(args);
            //Listen for new connection
            if (!Stopping)
            {
                listener.BeginReceive(OnClientConnect, null);
            }
            else
            {
                SetStopped();
            }
        }

        
    }
}
