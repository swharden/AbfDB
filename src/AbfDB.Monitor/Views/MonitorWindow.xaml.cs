using AbfDB.Monitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AbfDB.Monitor.Views
{
    /// <summary>
    /// Interaction logic for MonitorWindow.xaml
    /// </summary>
    public partial class MonitorWindow : Window
    {
        readonly Services.FileWatcherService FWS;
        readonly DispatcherTimer RefreshTimer = new() { Interval = TimeSpan.FromSeconds(1) };

        public MonitorWindow()
        {
            InitializeComponent();
            FWS = new(App.WatchFolderPath, App.DatabaseFilePath);
            DataContext = FWS;
            RefreshTimer.Tick += RefreshTimer_Tick;
            RefreshTimer.Start();
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            FWS.ProcessABFs();
            DataGrid1.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FWS.AddRandomFile();
            DataGrid1.Items.Refresh();
        }
    }
}
