using System;

namespace AbfDB.Databases
{
    public abstract class AbfDatabase : IDisposable
    {
        public abstract void AddAbf(string path, int episodes, uint date, uint time, int stopwatch, string md5);
        public abstract void Dispose();
    }
}
