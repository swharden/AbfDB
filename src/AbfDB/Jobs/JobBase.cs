using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB.Jobs
{
    public abstract class JobBase
    {
        public readonly List<string> AbfPaths = new();
        public int AbfCount => AbfPaths.Count;
        private readonly Stopwatch Watch = Stopwatch.StartNew();
        public void Completed(string prefix)
        {
            Watch.Stop();
            Console.WriteLine(
                $"{prefix} - {AbfCount} ABFs in {Watch.Elapsed} " +
                $"({AbfCount / Watch.Elapsed.TotalSeconds:0.00} ABFs/sec)");
        }
    }
}
