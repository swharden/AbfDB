using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB.Tests
{
    internal class SampleData
    {
        public static string ABF_FOLDER = GetDataFolder();
        public static string[] ABF_PATHS = GetABFs();
        public static string TSV_PATH = GetTsvPath();

        private static string GetTsvPath()
        {
            return Path.Combine(ABF_FOLDER, "db-2021-12-27-b.tsv");
        }

        private static string GetDataFolder()
        {
            string testFolder = TestContext.CurrentContext.TestDirectory;
            string dataFolder = Path.Combine(testFolder, "../../../../../dev/SampleData/");
            return Path.GetFullPath(dataFolder);
        }

        private static string[] GetABFs()
        {
            return Directory.GetFiles(ABF_FOLDER, "*.abf");
        }

        [Test]
        public void Test_SampleData_FolderExists()
        {
            Assert.That(Directory.Exists(ABF_FOLDER));
        }

        [Test]
        public void Test_SampleData_HasABFs()
        {
            Assert.IsNotEmpty(ABF_PATHS);
        }

        [Test]
        public void Test_SampleData_CanBeRead()
        {
            foreach (string abfPath in ABF_PATHS)
            {
                AbfSharp.ABFFIO.ABF abf = new(abfPath, preloadSweepData: false);
                Console.WriteLine(abf);
            }
        }

        [Test]
        [Ignore("only run when the local network is available")]
        public void Test_InvalidAbf_ProducesValidTSV()
        {
            string abfPath = @"X:\Data\DIC1\2006\02-2006\02-03-2006-BN\ABFMergeC3R12-R15.ABF";
            AbfRecord abf = AbfRecord.FromFile(abfPath);
            Assert.That(!abf.ToTSV().Contains("\n"));
            Assert.That(!abf.ToTSV().Contains("\r"));
            Assert.That(!abf.ToTSV().Contains("\r"));
        }

        [Test]
        public void Test_Tsv_Exists()
        {
            Assert.That(File.Exists(GetTsvPath()));
        }
    }
}