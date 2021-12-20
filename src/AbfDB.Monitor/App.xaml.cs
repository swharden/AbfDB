using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AbfDB.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string WatchFolderPath { get; private set; } = string.Empty;
        public static string DatabaseFilePath { get; private set; } = string.Empty;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 2)
            {
                WatchFolderPath = e.Args[0];
                DatabaseFilePath = e.Args[1];
            }
            else
            {
                WatchFolderPath = @"C:\Users\swharden\Documents\temp";
                DatabaseFilePath = @"C:\Users\swharden\Documents\important\abfdb\abfs2.db";
            }
        }
    }
}
