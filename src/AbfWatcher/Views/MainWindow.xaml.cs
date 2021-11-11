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
    }
}
