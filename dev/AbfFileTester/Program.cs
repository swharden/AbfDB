namespace AbfFileTester;

public static class Program
{
    static string TestFolder = string.Empty;
    static readonly Random Rand = new();

    /// <summary>
    /// This program randomly creates and deletes ABF files in a test folder
    /// </summary>
    public static void Main()
    {
        TestFolder = Path.GetFullPath("TestAbfs");
        if (!Directory.Exists(TestFolder))
            Directory.CreateDirectory(TestFolder);

        while (GetAbfFiles().Length < 123)
            CreateAbf();

        while (true)
        {
            Thread.Sleep(1000);
            switch (Rand.Next(3))
            {
                case 0:
                    CreateAbf();
                    break;
                case 1:
                    DeleteAbf();
                    break;
                default:
                    break;
            }
        }
    }

    public static string[] GetAbfFiles() => Directory.GetFiles(TestFolder, "*.abf");

    public static void CreateAbf()
    {
        string abfID = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        string filePath = Path.Combine(TestFolder, $"{abfID}.abf");
        Console.WriteLine($"[{DateTime.Now}] Creating {filePath}");
        File.WriteAllText(filePath, abfID);
    }

    public static void DeleteAbf()
    {
        string[] filePaths = GetAbfFiles();
        if (filePaths.Length == 0)
            return;

        int deleteIndex = Rand.Next(filePaths.Length);
        string deleteFilePath = filePaths[deleteIndex];
        Console.WriteLine($"[{DateTime.Now}] Deleting {deleteFilePath}");
        File.Delete(deleteFilePath);
    }
}