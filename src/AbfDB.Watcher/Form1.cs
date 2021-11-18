namespace AbfDB.Watcher
{
    public partial class Form1 : Form
    {
        private readonly AbfDatabase Database;
        private readonly AbfWatcher Watcher;

        public Form1()
        {
            InitializeComponent();
            Database = new AbfDatabase("test.db");
            lblDatabase.Text = Database.FilePath;

            string watchFolder = @"C:\Users\swharden\Documents\GitHub\AbfDB\dev\AbfFileTester\TestAbfs";
            Watcher = new AbfWatcher(watchFolder, Database);
            lblWatching.Text = Watcher.WatchFolder;
        }

        private void logTimer_Tick(object sender, EventArgs e)
        {
            lblABFs.Text = Database.Count.ToString("N0");
            
            while(Watcher.Messages.Count > 0)
            {
                LogMessage message = Watcher.Messages.Dequeue();
                if (message is null)
                    continue;

                rtbLog.SuspendLayout();
                rtbLog.SelectionStart = rtbLog.TextLength;
                rtbLog.SelectionLength = 0;

                rtbLog.SelectionColor = Color.Black;
                rtbLog.AppendText($"{message.Timestamp} ");

                rtbLog.SelectionColor = Color.Blue;
                rtbLog.AppendText($"{message.Verb} ");

                rtbLog.SelectionColor = Color.Black;
                rtbLog.AppendText($"{message.Noun} ");

                rtbLog.AppendText(Environment.NewLine);
                rtbLog.ScrollToCaret();
                rtbLog.ResumeLayout();
            }
        }
    }
}