using NUnit.Framework;
using System.IO;

namespace AbfDB.Tests
{
    public class DatabaseTests
    {
        [Test]
        public void Test_DB_Create()
        {
            using TemporaryDatabase db = new();
            Assert.AreEqual(0, db.GetAbfCount());
        }

        [Test]
        public void Test_DB_AddRandom()
        {
            using TemporaryDatabase db = new();
            Assert.AreEqual(0, db.GetAbfCount());
            db.AddRandom();
            Assert.AreEqual(1, db.GetAbfCount());
        }

        [Test]
        public void Test_DB_AddReal()
        {
            using TemporaryDatabase db = new();
            Assert.AreEqual(0, db.GetAbfCount());

            foreach (string abfPath in SampleData.ABF_PATHS)
                db.Add(abfPath);

            Assert.AreEqual(SampleData.ABF_PATHS.Length, db.GetAbfCount());
        }

        [Test]
        public void Test_DB_AttachAndAppend()
        {
            using TemporaryDatabase db1 = new();
            db1.AddRandom();
            db1.Dispose();

            using TemporaryDatabase db2 = new(db1.FilePath);
            Assert.AreEqual(1, db2.GetAbfCount());
            db2.AddRandom();
            Assert.AreEqual(2, db2.GetAbfCount());
        }
    }
}