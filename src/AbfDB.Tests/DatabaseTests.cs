using NUnit.Framework;
using System.IO;

namespace AbfDB.Tests
{
    public class Tests
    {
        [Test]
        public void Test_SampleData_HasContent()
        {
            string[] lines = File.ReadAllLines(SampleData.TsvFilePath);
            Assert.IsNotNull(lines);
            Assert.IsNotEmpty(lines);
            Assert.Greater(lines.Length, 1000);
        }

        [Test]
        public void Test_DatabaseBuilder_ReadsWithoutCrashing()
        {
            var builder = new DatabaseBuilder();
            builder.LoadTsv(SampleData.TsvFilePath);
            Assert.Greater(builder.AbfInfos.Count, 1000);
        }
    }
}