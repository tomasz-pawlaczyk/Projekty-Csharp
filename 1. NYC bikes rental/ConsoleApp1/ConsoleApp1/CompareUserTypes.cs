using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public static class CompareUserTypes
    {
        public static void Run(string tripsDirectory)
        {
            var trips = LoadTrips(tripsDirectory);

            Console.WriteLine("VI. COMPARE BY USER TYPE");
            PrintMostActiveTimeBlock(trips);
            PrintAverageTimeAndDistance(trips);
            PrintRouteTypeStats(trips);
        }

        // ===================== DATA =====================

        class Trip
        {
            public DateTime Start;
            public DateTime Stop;
            public string StartStation;
            public string EndStation;
            public string UserType;
            public double DistanceKm;
        }

        static List<Trip> LoadTrips(string tripsDirectory)
        {
            List<Trip> trips = new();

            var files = Directory.GetFiles(tripsDirectory, "*.csv", SearchOption.AllDirectories);

            foreach (var file in files)
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

                    string userType = c[12].Trim();

                    if (userType != "Subscriber" && userType != "Customer")
                        continue;

                    double lat1 = double.Parse(c[5], CultureInfo.InvariantCulture);
                    double lon1 = double.Parse(c[6], CultureInfo.InvariantCulture);
                    double lat2 = double.Parse(c[9], CultureInfo.InvariantCulture);
                    double lon2 = double.Parse(c[10], CultureInfo.InvariantCulture);

                    trips.Add(new Trip
                    {
                        Start = start,
                        Stop = stop,
                        StartStation = c[4],
                        EndStation = c[8],
                        UserType = userType,
                        DistanceKm = Haversine(lat1, lon1, lat2, lon2)
                    });
                }
            }

            return trips;
        }

        static void PrintMostActiveTimeBlock(List<Trip> trips)
        {
            Console.WriteLine("Time blocks");

            foreach (var group in trips.GroupBy(t => t.UserType))
            {
                var blocks = group
                    .GroupBy(t => (t.Start.Hour / 2) * 2)
                    .Select(g => new { Block = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .First();

                Console.WriteLine($"{group.Key}: {blocks.Block:00}-{blocks.Block + 2:00}");
            }
        }

        static void PrintAverageTimeAndDistance(List<Trip> trips)
        {
            Console.WriteLine("\nAverage trip duration and distance:");

            foreach (var group in trips.GroupBy(t => t.UserType))
            {
                double avgMinutes = group.Average(t => (t.Stop - t.Start).TotalMinutes);
                double avgDistance = group.Average(t => t.DistanceKm);

                Console.WriteLine($"{group.Key,-10}: {avgMinutes:F2} min | {avgDistance:F2} km");
            }
        }


        static void PrintRouteTypeStats(List<Trip> trips)
        {
            Console.WriteLine("\nMost common route type:");

            foreach (var group in trips.GroupBy(t => t.UserType))
            {
                int loops = group.Count(t => t.StartStation == t.EndStation);
                int total = group.Count();

                double loopPercent = loops * 100.0 / total;

                string type = loopPercent > 50 ? "Loop (A→A)" : "Transfer (A→B)";

                Console.WriteLine($"{group.Key,-10}: {type} ({loopPercent:F1}% loops)");
            }
        }

        static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        static double ToRad(double v) => v * Math.PI / 180;
    }
}
