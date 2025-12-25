using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public static class DetectAnomalies
    {
        public static void Run(string tripsDirectory)
        {
            var trips = LoadTrips(tripsDirectory);

            Console.WriteLine("VII. ANOMALY DETECTOR");
            TripDurationConsistencyCheck(trips);
            DetectLongZeroDistanceTrips(trips);
            DetectUnrealisticSpeedTrips(trips);
            DetectBikeUsageAnomalies(trips);
        }

        class Trip
        {
            public int BikeId;
            public DateTime Start;
            public DateTime Stop;
            public string StartStation;
            public string EndStation;
            public double DistanceKm;
            public double DurationMinutes;
            public int TripDurationSeconds;
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

                    if (!int.TryParse(c[0], out int tripDurationSeconds))
                        continue;

                    if (!DateTime.TryParse(c[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start))
                        continue;

                    if (!DateTime.TryParse(c[2], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime stop))
                        continue;

                    if (!int.TryParse(c[11], out int bikeId))
                        continue;

                    double lat1 = double.Parse(c[5], CultureInfo.InvariantCulture);
                    double lon1 = double.Parse(c[6], CultureInfo.InvariantCulture);
                    double lat2 = double.Parse(c[9], CultureInfo.InvariantCulture);
                    double lon2 = double.Parse(c[10], CultureInfo.InvariantCulture);

                    double distance = Haversine(lat1, lon1, lat2, lon2);
                    double durationMinutes = tripDurationSeconds / 60.0;

                    trips.Add(new Trip
                    {
                        BikeId = bikeId,
                        Start = start,
                        Stop = stop,
                        StartStation = c[4],
                        EndStation = c[8],
                        DistanceKm = distance,
                        DurationMinutes = durationMinutes,
                        TripDurationSeconds = tripDurationSeconds
                    });
                }
            }

            return trips;
        }


        static void DetectLongZeroDistanceTrips(List<Trip> trips)
        {
            Console.WriteLine("\nLong trips with zero distance");

            var anomalies = trips
                .Where(t =>
                    t.StartStation == t.EndStation &&
                    t.DistanceKm < 0.1 &&
                    t.DurationMinutes > 60)
                .OrderByDescending(t => t.DurationMinutes)
                .Take(10);

            foreach (var t in anomalies)
            {
                Console.WriteLine(
                    $"{t.StartStation} | {t.DurationMinutes:F1} min | {t.DistanceKm:F2} km"
                );
            }
        }

        static void DetectUnrealisticSpeedTrips(List<Trip> trips)
        {
            Console.WriteLine("\nUnrealistic speed");

            var anomalies = trips
                .Where(t =>
                    t.DurationMinutes >= 1 &&
                    t.DurationMinutes <= 240 &&
                    t.DistanceKm <= 20
                )
                .Select(t => new
                {
                    t.StartStation,
                    t.EndStation,
                    t.DurationMinutes,
                    t.DistanceKm,
                    Speed = t.DistanceKm / (t.DurationMinutes / 60.0)
                })
                .Where(x => x.Speed > 50)
                .OrderByDescending(x => x.Speed)
                .Take(5);

            foreach (var t in anomalies)
            {
                Console.WriteLine(
                    $"{t.StartStation} -> {t.EndStation}\n" +
                    $"  Duration: {t.DurationMinutes:F2} min\n" +
                    $"  Distance: {t.DistanceKm:F2} km\n" +
                    $"  Speed:    {t.Speed:F1} km/h"
                );
            }
        }


        static double HaversineDistanceStations(Trip t)
        {
            return t.DistanceKm;
        }

        static void DetectBikeUsageAnomalies(List<Trip> trips)
        {
            Console.WriteLine("\nBike usage extremes");

            var usage = trips
                .GroupBy(t => t.BikeId)
                .Select(g => new
                {
                    BikeId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            Console.WriteLine("Most used bikes:");
            foreach (var b in usage.Take(5))
            {
                Console.WriteLine($"Bike {b.BikeId} : {b.Count} trips");
            }

            Console.WriteLine("\nLeast used bikes:");
            foreach (var b in usage.TakeLast(5))
            {
                Console.WriteLine($"Bike {b.BikeId} : {b.Count} trips");
            }
        }
        
        static void TripDurationConsistencyCheck(List<Trip> trips)
        {
            Console.WriteLine("\nTrip duration consistency check (> 1s)");

            long total = 0;
            long inconsistent = 0;
            double sumDelta = 0;
            double maxDelta = 0;

            foreach (var t in trips)
            {
                double timestampSeconds = (t.Stop - t.Start).TotalSeconds;
                double delta = Math.Abs(t.TripDurationSeconds - timestampSeconds);

                total++;

                if (delta > 1)
                {
                    inconsistent++;
                    sumDelta += delta;
                    if (delta > maxDelta)
                        maxDelta = delta;
                }
            }

            Console.WriteLine($"Total records checked: {total:N0}");
            Console.WriteLine($"Inconsistent (>1s):     {inconsistent:N0}");
            Console.WriteLine($"Percentage:             {100.0 * inconsistent / total:F2}%");

            if (inconsistent > 0)
            {
                Console.WriteLine($"Average delta:           {sumDelta / inconsistent:F1} s");
                Console.WriteLine($"Max delta:               {maxDelta:F0} s");
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
