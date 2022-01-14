using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AbfDB.Tests;

public static class SampleData
{
    public static string[] GetTestAbfs(TestContext testContext)
    {
        string dataFolder = Path.GetFullPath(Path.Join(testContext.TestRunDirectory, "../../../dev/SampleData"));
        return Directory.GetFiles(dataFolder, "*.abf").ToArray();
    }
}

