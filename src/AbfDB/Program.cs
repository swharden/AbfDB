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
            string scanFolder = Debugger.IsAttached ? @"X:\Data\AT1-Cre-AT2-eGFP" : args[0];
            Console.WriteLine($"Scanning: {scanFolder}");
        }
    }
}
