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
    }
}