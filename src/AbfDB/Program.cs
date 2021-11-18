namespace AbfDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TsvDatabase.SqliteFromTsv(@"L:\abfdb\abfs.tsv", @"C:\Users\swharden\Documents\important\abfdb\abfs.db");
            Console.WriteLine("DONE");
        }
    }
}