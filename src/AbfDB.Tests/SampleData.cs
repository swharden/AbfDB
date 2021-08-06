using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AbfDB.Tests
{
    public static class SampleData
    {
        public static string TsvFilePath
        {
            get
            {
                string path = "../../../../../dev/data/demo.tsv";
                path = Path.Combine(TestContext.CurrentContext.TestDirectory, path);
                path = Path.GetFullPath(path);
                if (!File.Exists(path))
                    throw new FileNotFoundException(path);
                return path;
            }
        }
    }
}
