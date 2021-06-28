using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using AbfDB.Databases;

namespace AbfDB
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new AbfDatabase("dev.sqlite.db");
            if (Debugger.IsAttached)
            {
                AddFolder(@"C:\Users\scott\Documents\GitHub\pyABF\data\abfs", db);
                AddFolder(@"C:\Users\scott\Documents\GitHub\AbfSharp\dev\abfs", db);
            }
            Console.WriteLine("DONE");
        }

        static void AddFolder(string folder, AbfDatabase db)
        {
            foreach (string abfPath in Directory.GetFiles(folder))
            {
                if (!abfPath.EndsWith(".abf"))
                    continue;

                var abf = new AbfSharp.ABFFIO.ABF(abfPath);
                Console.WriteLine(abfPath);
                db.Add(abf);
            }
        }
    }
}
