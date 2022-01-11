using CommandLine;
using System;
using System.IO;
using System.Collections.Generic;

namespace AbfDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var err in errs)
                Console.WriteLine(err);
        }

        static void RunOptions(CommandLineOptions opts)
        {
            if (!string.IsNullOrWhiteSpace(opts.RebuildPath))
            {
                if (File.Exists(opts.DatabaseFilePath))
                    throw new InvalidOperationException($"ERROR: database file already exists: {opts.RebuildPath}");

                using AbfDatabase db = new(opts.DatabaseFilePath);
                db.AddFolder(opts.RebuildPath);
            }
            else if (!string.IsNullOrWhiteSpace(opts.AddPath))
            {
                using AbfDatabase db = new(opts.DatabaseFilePath);
                db.AddFolder(opts.AddPath);
            }
            else if (!string.IsNullOrWhiteSpace(opts.DeletePath))
            {
                using AbfDatabase db = new(opts.DatabaseFilePath);
                db.RemoveFolder(opts.DeletePath);
            }
        }
    }
}