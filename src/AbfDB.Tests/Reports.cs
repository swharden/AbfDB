using NUnit.Framework;
using System.IO;

namespace AbfDB.Tests
{
    internal class Reports
    {
        public readonly string PATH_DB_LOCAL = @"L:\abfdb\abfs.db";

        [Test]
        public void Test_AbfCount_ByDay()
        {
            if (File.Exists(PATH_DB_LOCAL))
                AbfDB.Reports.AbfCountByDay(PATH_DB_LOCAL);
        }

        [Test]
        public void Test_AbfHours_ByDay()
        {
            if (File.Exists(PATH_DB_LOCAL))
                AbfDB.Reports.AbfHoursByDay(PATH_DB_LOCAL);
        }

        [Test]
        public void Test_AbfFolders_ByDay()
        {
            if (File.Exists(PATH_DB_LOCAL))
                AbfDB.Reports.AbfFoldersByDay(PATH_DB_LOCAL);
        }
    }
}
