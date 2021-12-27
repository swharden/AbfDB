using System;

namespace AbfDB.Monitor.Models
{
    public class AbfFile
    {
        public string Path { get; private set; }
        public string Reason { get; private set; }

        private DateTime TimeNoted;
        public double Age => (DateTime.Now - TimeNoted).Seconds;

        public AbfFile(string path, string reason)
        {
            TimeNoted = DateTime.Now;
            Path = path;
            Reason = reason;
        }

        public void ResetTime()
        {
            TimeNoted = DateTime.Now;
        }
    }
}
