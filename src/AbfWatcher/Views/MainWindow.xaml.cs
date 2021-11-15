using System;
using System.IO;
using System.Windows;

namespace AbfWatcher
{
    public partial class MainWindow : Window
    {
        readonly ViewModels.WatcherViewModel WatcherViewModel;

        public MainWindow()
        {
            InitializeComponent();

            WatcherViewModel = new(
                watchFolder: @"C:\Users\swharden\Documents\GitHub\AbfDB\src\AbfFileTester\bin\Debug\net6.0\TestAbfs",
                databaseFolder: @"C:\Users\swharden\Documents\temp\database");

            DataContext = WatcherViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new();
            string[] abfs = Directory.GetFiles(@"X:\Data\SD\practice\Scott\2019-09-26", "*.abf");
            string abf = abfs[rand.Next(abfs.Length)];
            WatcherViewModel.ManualCreate(abf);
        }
    }
}
