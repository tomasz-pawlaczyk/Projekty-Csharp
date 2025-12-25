using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public static class CompareStationBalance
    {
        public static void Run(string tripsDirectory)
        {
            Dictionary<int, StationStats> stations = new();

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

                    if (!int.TryParse(c[3], out int startId))
                        continue;

                    if (!int.TryParse(c[7], out int endId))
                        continue;

                    string startName = c[4];
                    string endName = c[8];

                    if (!stations.ContainsKey(startId))
                        stations[startId] = new StationStats(startId, startName);

                    if (!stations.ContainsKey(endId))
                        stations[endId] = new StationStats(endId, endName);

                    stations[startId].Starts++;
                    stations[endId].Ends++;
                }
            }
            
            Console.WriteLine("\nIV. REBALANCING BIKE FLEET");

            const double DAYS = 365.0;

            var balances = stations.Values
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    AvgStarts = s.Starts / DAYS,
                    AvgEnds = s.Ends / DAYS,
                    AvgBalance = (s.Ends - s.Starts) / DAYS
                })
                .ToList();

            Console.WriteLine("Top 3 stations losing bikes:");

            foreach (var s in balances
                .OrderBy(x => x.AvgBalance)
                .Take(3))
            {
                Console.WriteLine(
                    $"{s.Name} | {s.AvgBalance:F2} bikes/day"
                );
            }

            Console.WriteLine("\nTop 3 stations gaining bikes:");

            foreach (var s in balances
                .OrderByDescending(x => x.AvgBalance)
                .Take(3))
            {
                Console.WriteLine(
                    $"{s.Name} | +{s.AvgBalance:F2} bikes/day"
                );
            }
        }

        class StationStats
        {
            public int Id { get; }
            public string Name { get; }
            public long Starts { get; set; }
            public long Ends { get; set; }

            public StationStats(int id, string name)
            {
                Id = id;
                Name = name;
                Starts = 0;
                Ends = 0;
            }
        }
    }
}
