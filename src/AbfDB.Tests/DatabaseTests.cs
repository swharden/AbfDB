using NUnit.Framework;

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
        public void Test_DB_Add()
        {
            using TemporaryDatabase db = new();
            Assert.AreEqual(0, db.GetAbfCount());
            db.AddRandomEntry();
            Assert.AreEqual(1, db.GetAbfCount());
        }

        [Test]
        public void Test_DB_AttachAndAppend()
        {
            using TemporaryDatabase db1 = new();
            db1.AddRandomEntry();
            db1.Dispose();

            using TemporaryDatabase db2 = new(db1.FilePath);
            Assert.AreEqual(1, db2.GetAbfCount());
            db2.AddRandomEntry();
            Assert.AreEqual(2, db2.GetAbfCount());
        }
    }
}