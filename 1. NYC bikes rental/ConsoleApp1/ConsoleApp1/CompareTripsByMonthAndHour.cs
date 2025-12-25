using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public static class CompareTripsByMonthAndHour
    {
        public static void Run(string tripsDirectory)
        {
            Dictionary<int, int> tripsPerMonth = new();
            Dictionary<int, long> hourBlockPoints = new();

            for (int h = 0; h < 24; h += 2)
                hourBlockPoints[h] = 0;

            var csvFiles = Directory.GetFiles(
                tripsDirectory,
                "*.csv",
                SearchOption.AllDirectories
            );

            foreach (var file in csvFiles)
            {
                var lines = File.ReadLines(file).Skip(1);

                foreach (var line in lines)
                {
                    var c = line.Split(',');

                    if (c.Length < 15)
                        continue;

                    if (!DateTime.TryParse(c[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start))
                        continue;

                    if (!DateTime.TryParse(c[2], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime stop))
                        continue;

                    int month = start.Month;
                    tripsPerMonth.TryGetValue(month, out int current);
                    tripsPerMonth[month] = current + 1;

                    DateTime currentTime = start;

                    while (currentTime < stop)
                    {
                        int block = (currentTime.Hour / 2) * 2;

                        if (hourBlockPoints.ContainsKey(block))
                            hourBlockPoints[block]++;

                        currentTime = currentTime.AddHours(2);
                    }
                }
            }

            Console.WriteLine("\nII. COMPARE BY TIME");
            PrintHourlyBlocks(hourBlockPoints);
            PrintMonthlyTable(tripsPerMonth);
        }

        static void PrintMonthlyTable(Dictionary<int, int> data)
        {
            Console.WriteLine("\nTrips per month (in thousands):");
            Console.WriteLine("Month | Trips (k)");

            foreach (var m in data.OrderBy(x => x.Key))
            {
                double thousands = m.Value / 1000.0;
                Console.WriteLine($"{m.Key,5} | {thousands,8:F1}");
            }
        }

        static void PrintHourlyBlocks(Dictionary<int, long> blocks)
        {
            Console.WriteLine("Average activity per 2-hour block:");

            long totalDays = 365;

            foreach (var b in blocks.OrderBy(x => x.Key))
            {
                string label = $"{b.Key:00}-{b.Key + 2:00}";
                double avg = b.Value / (double)totalDays;

                Console.WriteLine($"{label}: {avg:F2}");
            }
        }
    }
}
