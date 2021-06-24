using System;

namespace AbfDB.Databases
{
    public abstract class AbfDatabase : IDisposable
    {
        public abstract void Add(AbfRecord record);
        public abstract void Dispose();
    }
}
