using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AbfDB.Tests;

public static class SampleData
{
    private static string GetSampleDataPath(TestContext testContext, int maxDepth = 10)
    {
        string possibleRepoRootFolder = Path.GetFullPath(testContext.TestRunDirectory);

        for (int i = 0; i < maxDepth; i++)
        {
            string dataPath = Path.Combine(possibleRepoRootFolder ?? string.Empty, "dev/SampleData");
            if (Directory.Exists(dataPath))
            {
                return dataPath;
            }
            else
            {
                possibleRepoRootFolder = Path.GetDirectoryName(possibleRepoRootFolder) ?? string.Empty;
            }
        }

        throw new DirectoryNotFoundException();
    }

    public static string[] GetTestAbfs(TestContext testContext)
    {
        string dataFolder = GetSampleDataPath(testContext);
        return Directory.GetFiles(dataFolder, "*.abf").ToArray();
    }
}

