using CommandLine;

namespace AbfDB
{
    internal class CommandLineOptions
    {
        [Option(longName: "db", Required = true, HelpText = "SQLite database file path")]
        public string DatabaseFilePath { get; set; } = string.Empty;

        [Option(longName: "remove", Required = false, HelpText = "Delete this file or folder from the database")]
        public string DeletePath { get; set; } = string.Empty;

        [Option(longName: "add", Required = false, HelpText = "Add this file or folder from the database")]
        public string AddPath { get; set; } = string.Empty;

        [Option(longName: "rebuild", Required = false, HelpText = "Create a database from scratch using this folder")]
        public string RebuildPath { get; set; } = string.Empty;
    }
}
