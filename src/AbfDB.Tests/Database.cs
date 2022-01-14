using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Linq;

namespace AbfDB.Tests
{
    [TestClass]
    public class Database
    {
        static string[] AbfPaths = Array.Empty<string>();

        [ClassInitialize]
        public static void SetupTests(TestContext testContext) => AbfPaths = SampleData.GetTestAbfs(testContext);

        [TestMethod]
        public void Test_SampleDataFolder_HasAbfs()
        {
            Assert.IsNotNull(AbfPaths);
            Assert.AreNotEqual(0, AbfPaths.Length);
        }

        static AbfDB.Database.AbfDatabase MakeTestDatabase()
        {
            string dbFileName = "testdb-" + Path.GetRandomFileName().Replace(".", "") + ".db";
            string dbFilePath = Path.GetFullPath(dbFileName);
            Console.WriteLine(dbFilePath);
            return new AbfDB.Database.AbfDatabase(dbFilePath);
        }

        [TestMethod]
        public void Test_Database_Create()
        {
            var db = MakeTestDatabase();
            Assert.AreEqual(0, db.GetRecordCount());
        }

        [TestMethod]
        public void Test_Database_AddAbfsIndividually()
        {
            var db = MakeTestDatabase();

            foreach (string abfPath in AbfPaths)
                db.Add(abfPath);

            int databaseRecordCount = db.GetRecordCount();
            Assert.AreNotEqual(0, databaseRecordCount);
            Assert.AreEqual(AbfPaths.Length, databaseRecordCount);
        }

        [TestMethod]
        public void Test_Database_AddAbfsBulk()
        {
            var db = MakeTestDatabase();

            db.Add(AbfPaths);

            int databaseRecordCount = db.GetRecordCount();
            Assert.AreNotEqual(0, databaseRecordCount);
            Assert.AreEqual(AbfPaths.Length, databaseRecordCount);
        }
    }
}