using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    public static class Reports
    {
        public static void AbfCountByDay(string dbFilePath)
        {
            Dictionary<DateTime, int> countByDay = new();

            SqliteConnectionStringBuilder csBuilder = new() { DataSource = dbFilePath };
            SqliteConnection connection = new(csBuilder.ConnectionString);
            connection.Open();

            using var sqlCmd = connection.CreateCommand();
            sqlCmd.CommandText = "SELECT RecordedDay, COUNT(*) FROM Abfs WHERE RecordedDay > 20010101 GROUP BY RecordedDay;";
            SqliteDataReader reader = sqlCmd.ExecuteReader();

            while (reader.Read())
            {
                int count = int.Parse(reader[1].ToString() ?? string.Empty);
                string dayCode = reader[0].ToString() ?? "00010101";
                int year = int.Parse(dayCode.Substring(0, 4));
                int month = int.Parse(dayCode.Substring(4, 2));
                int day = int.Parse(dayCode.Substring(6, 2));
                DateTime dt = new(year, month, day);
                countByDay[dt] = count;
            }

            connection.Close();

            // add days with zero ABFs
            List<DateTime> allDates = new();
            allDates.Add(countByDay.Keys.Min());
            while (allDates.Last() < DateTime.Now)
            {
                DateTime newDay = allDates.Last() + TimeSpan.FromDays(1);
                allDates.Add(newDay);
                if (!countByDay.ContainsKey(newDay))
                    countByDay[newDay] = 0;
            }

            DateTime[] dates = countByDay.Keys.OrderBy(x => x).ToArray();
            double[] xs = dates.Select(x => x.ToOADate()).ToArray();
            double[] ys = dates.Select(x => (double)countByDay[x]).ToArray();

            var plt = new ScottPlot.Plot(800, 400);
            plt.XAxis.DateTimeFormat(true);
            var sp1 = plt.AddScatterPoints(xs, ys);
            sp1.Color = System.Drawing.Color.FromArgb(50, plt.Palette.GetColor(0));
            sp1.Label = $"daily number";

            int smoothDays = 7 * 4;
            double[] smoothYs = ScottPlot.Statistics.Finance.SMA(ys, smoothDays, true);
            double[] smoothXs = xs.Skip(smoothDays / 2).Take(smoothYs.Length).ToArray();
            var sp2 = plt.AddScatterLines(smoothXs, smoothYs);
            sp2.LineWidth = 2;
            sp2.Color = System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black);
            sp2.Label = $"{smoothDays}-day average";

            plt.YLabel("ABF Count / Day");
            plt.Margins(x: 0);
            plt.SetAxisLimits(yMin: 0, yMax: 300);

            string saveAs = System.IO.Path.GetFullPath("abfCountByDay.png");
            plt.SaveFig(saveAs);
            Console.WriteLine(saveAs);
        }

        public static void AbfHoursByDay(string dbFilePath)
        {
            Dictionary<DateTime, double> valueByDay = new();

            SqliteConnectionStringBuilder csBuilder = new() { DataSource = dbFilePath };
            SqliteConnection connection = new(csBuilder.ConnectionString);
            connection.Open();

            using var sqlCmd = connection.CreateCommand();
            sqlCmd.CommandText = "SELECT RecordedDay, SUM(LengthSec)/60/60 FROM Abfs WHERE RecordedDay > 20010101 GROUP BY RecordedDay;";
            SqliteDataReader reader = sqlCmd.ExecuteReader();

            while (reader.Read())
            {
                double count = double.Parse(reader[1].ToString() ?? string.Empty);
                string dayCode = reader[0].ToString() ?? "00010101";
                int year = int.Parse(dayCode.Substring(0, 4));
                int month = int.Parse(dayCode.Substring(4, 2));
                int day = int.Parse(dayCode.Substring(6, 2));
                DateTime dt = new(year, month, day);
                valueByDay[dt] = count;
            }

            connection.Close();

            // add days with zero ABFs
            List<DateTime> allDates = new();
            allDates.Add(valueByDay.Keys.Min());
            while (allDates.Last() < DateTime.Now)
            {
                DateTime newDay = allDates.Last() + TimeSpan.FromDays(1);
                allDates.Add(newDay);
                if (!valueByDay.ContainsKey(newDay))
                    valueByDay[newDay] = 0;
            }

            DateTime[] dates = valueByDay.Keys.OrderBy(x => x).ToArray();
            double[] xs = dates.Select(x => x.ToOADate()).ToArray();
            double[] ys = dates.Select(x => (double)valueByDay[x]).ToArray();

            var plt = new ScottPlot.Plot(800, 400);
            plt.XAxis.DateTimeFormat(true);
            var sp1 = plt.AddScatterPoints(xs, ys);
            sp1.Color = System.Drawing.Color.FromArgb(50, plt.Palette.GetColor(0));
            sp1.Label = $"daily number";

            int smoothDays = 7 * 4;
            double[] smoothYs = ScottPlot.Statistics.Finance.SMA(ys, smoothDays, true);
            double[] smoothXs = xs.Skip(smoothDays / 2).Take(smoothYs.Length).ToArray();
            var sp2 = plt.AddScatterLines(smoothXs, smoothYs);
            sp2.LineWidth = 2;
            sp2.Color = System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black);
            sp2.Label = $"{smoothDays}-day average";

            plt.YLabel("ABF Hours / Day");
            plt.Margins(x: 0);
            plt.SetAxisLimits(yMin: 0, yMax: 24);

            string saveAs = System.IO.Path.GetFullPath("abfHoursByDay.png");
            plt.SaveFig(saveAs);
            Console.WriteLine(saveAs);
        }

        public static void AbfFoldersByDay(string dbFilePath)
        {
            Dictionary<DateTime, double> valueByDay = new();

            SqliteConnectionStringBuilder csBuilder = new() { DataSource = dbFilePath };
            SqliteConnection connection = new(csBuilder.ConnectionString);
            connection.Open();

            using var sqlCmd = connection.CreateCommand();
            sqlCmd.CommandText = "SELECT RecordedDay, COUNT(DISTINCT(Folder)) FROM Abfs WHERE RecordedDay > 20010101 GROUP BY RecordedDay;";
            SqliteDataReader reader = sqlCmd.ExecuteReader();

            while (reader.Read())
            {
                double count = double.Parse(reader[1].ToString() ?? string.Empty);
                string dayCode = reader[0].ToString() ?? "00010101";
                int year = int.Parse(dayCode.Substring(0, 4));
                int month = int.Parse(dayCode.Substring(4, 2));
                int day = int.Parse(dayCode.Substring(6, 2));
                DateTime dt = new(year, month, day);
                valueByDay[dt] = count;
            }

            connection.Close();

            // add days with zero ABFs
            List<DateTime> allDates = new();
            allDates.Add(valueByDay.Keys.Min());
            while (allDates.Last() < DateTime.Now)
            {
                DateTime newDay = allDates.Last() + TimeSpan.FromDays(1);
                allDates.Add(newDay);
                if (!valueByDay.ContainsKey(newDay))
                    valueByDay[newDay] = 0;
            }

            DateTime[] dates = valueByDay.Keys.OrderBy(x => x).ToArray();
            double[] xs = dates.Select(x => x.ToOADate()).ToArray();
            double[] ys = dates.Select(x => (double)valueByDay[x]).ToArray();

            var plt = new ScottPlot.Plot(800, 400);
            plt.XAxis.DateTimeFormat(true);
            var sp1 = plt.AddScatterPoints(xs, ys);
            sp1.Color = System.Drawing.Color.FromArgb(50, plt.Palette.GetColor(0));
            sp1.Label = $"daily number";

            int smoothDays = 7 * 4;
            double[] smoothYs = ScottPlot.Statistics.Finance.SMA(ys, smoothDays, true);
            double[] smoothXs = xs.Skip(smoothDays / 2).Take(smoothYs.Length).ToArray();
            var sp2 = plt.AddScatterLines(smoothXs, smoothYs);
            sp2.LineWidth = 2;
            sp2.Color = System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black);
            sp2.Label = $"{smoothDays}-day average";

            plt.YLabel("ABF Folders / Day");
            plt.Margins(x: 0);
            plt.SetAxisLimits(yMin: 0, yMax: 6);

            string saveAs = System.IO.Path.GetFullPath("abfFoldersByDay.png");
            plt.SaveFig(saveAs);
            Console.WriteLine(saveAs);
        }
    }
}
