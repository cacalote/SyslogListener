using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using LogViewer.Enumerations;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        readonly Worker w = new Worker();
        public MainWindow()
        {
            InitializeComponent();
            w.MessageReceived += MessageReceived;
            ThreadStart ts = () => w.Run();
            new Thread(ts).Start();
        }

        void MessageReceived(object o, LogMessageEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<SyslogMessage>(AddLogMessage), e.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddLogMessage(SyslogMessage message)
        {
            int item = listView1.Items.Add(message);
            listView1.ScrollIntoView(listView1.Items[item]);
            var lastRow = listView1.ItemContainerGenerator.ContainerFromIndex(item) as ListViewItem;
            SetColorFromMessageSeverity(lastRow, message);
        }

        private static void SetColorFromMessageSeverity(ListViewItem lastRow, SyslogMessage message)
        {
            if (lastRow == null) return;
            switch (message.Severity)
            {
                case SyslogPriority.Emergency: lastRow.Background = Brushes.Red; lastRow.Foreground = Brushes.White; break;
                case SyslogPriority.Alert: lastRow.Background = Brushes.Red; lastRow.Foreground = Brushes.White; break;
                case SyslogPriority.Critical: lastRow.Background = Brushes.Violet; break;
                case SyslogPriority.Error: lastRow.Background = Brushes.SteelBlue;  break;
                case SyslogPriority.Warning: lastRow.Background = Brushes.Turquoise; break;
                case SyslogPriority.Notice: lastRow.Background = Brushes.Yellow;  break;
                case SyslogPriority.Informational: lastRow.Background = Brushes.LightGreen; break;
                case SyslogPriority.Debug: break;
            }
        }
    }
}
