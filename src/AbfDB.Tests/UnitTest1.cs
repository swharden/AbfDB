using NUnit.Framework;

namespace AbfDB.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            AbfDB.AbfDatabase db = new("test.db");
            Assert.AreEqual(0, db.Count);

            string folder = System.IO.Path.GetFullPath("./");
            string filename = "test" + System.Guid.NewGuid().ToString().Replace("-", "") + ".abf";
            db.Add(folder, filename);
            Assert.AreEqual(1, db.Count);
        }
    }
}