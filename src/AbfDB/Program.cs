using System;
using System.Diagnostics;

namespace AbfDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("This program will search a folder and build a new database from scratch.");

            if (args.Length == 2)
            {
                DatabaseBuilder.BuildFromScratch(args[0], args[1]);
            }
            else
            {
                Console.WriteLine("ERROR: Invalid arguments. Use like this:");
                Console.WriteLine("  AbfDB.exe X:/database/input/ C:/database/output/");
            }
        }
    }
}