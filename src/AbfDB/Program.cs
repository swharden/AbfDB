using System;
using System.Collections.Generic;

namespace AbfDB;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            ShowInvalidArgumentMessage();
            return;
        }

        string command = args[0];
        string searchFolder = args[1];
        string dbFilePath = args[2];

        if (command == "update")
        {
            Dictionary<string, AbfRecord> fsABFs = WindowsSearch.FindIndexedAbfs(searchFolder);
            Database.Update.FromIndexedAbfs(fsABFs, dbFilePath);
        }
        else if (command == "build")
        {
            Database.Build.FromScratch(searchFolder, dbFilePath);
        }
        else
        {
            ShowInvalidArgumentMessage();
        }
    }

    private static void ShowInvalidArgumentMessage()
    {
        Console.WriteLine("ERROR: invalid arguments.");
        Console.WriteLine("");
        Console.WriteLine("To update an existing database:");
        Console.WriteLine("  AbfDB.exe update [searchPath] [dbFilePath]");
        Console.WriteLine("");
        Console.WriteLine("To build a database from scratch:");
        Console.WriteLine("  AbfDB.exe build [searchPath] [dbFilePath]");
        Console.WriteLine("");
    }
}