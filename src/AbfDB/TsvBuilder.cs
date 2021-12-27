using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    public class TsvBuilder : IDisposable
    {
        public readonly string FilePath;
        private readonly FileStream Stream;
        private readonly StreamWriter Writer;

        public TsvBuilder(string tsvFilePath)
        {
            if (File.Exists(tsvFilePath))
                throw new InvalidOperationException($"File exists: {tsvFilePath}");

            FilePath = Path.GetFullPath(tsvFilePath);
            Stream = File.OpenWrite(tsvFilePath);
            Writer = new StreamWriter(Stream);

            Writer.WriteLine(AbfRecord.TsvColumns());
        }

        public void Add(string abfPath)
        {
            AbfRecord abf = AbfRecord.FromFile(abfPath);
            Writer.WriteLine(abf.ToTSV());
        }

        public void Dispose()
        {
            Writer.Dispose();
            Stream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
