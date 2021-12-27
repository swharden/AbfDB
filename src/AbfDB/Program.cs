using System;

namespace AbfDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("This program will search a folder and build a new database from scratch.");

            if (args.Length == 2)
            {
                DatabaseBuilder.CreateTSV(searchPath: args[0], tsvPath: args[1] + ".tsv");
                DatabaseBuilder.CreateSQL(tsvPath: args[1] + ".tsv", dbPath: args[1]);
            }
            else
            {
                Console.WriteLine("ERROR: Invalid arguments. Use like this:");
                Console.WriteLine("  AbfDB.exe X:/data C:/abfs.db");
            }
        }
    }
}