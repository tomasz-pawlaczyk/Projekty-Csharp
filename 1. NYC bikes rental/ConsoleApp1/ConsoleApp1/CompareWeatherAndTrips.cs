using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public static class CompareWeatherAndTrips
    {
        public static void Run(string tripsDirectory, string weatherFilePath)
        {
            var tripsPerDay = LoadTripsPerDay(tripsDirectory);
            var weatherPerDay = LoadWeather(weatherFilePath);

            Console.WriteLine("\nIII. TRIPS VS WEATHER");
            CompareTripsByTemperature(tripsPerDay, weatherPerDay);
            CompareTripsByWeatherEvent(tripsPerDay, weatherPerDay);
        }

        static Dictionary<DateTime, int> LoadTripsPerDay(string tripsDirectory)
        {
            Dictionary<DateTime, int> tripsPerDay = new();

            var csvFiles = Directory.GetFiles(tripsDirectory, "*.csv", SearchOption.AllDirectories);

            foreach (var file in csvFiles)
            {
                var lines = File.ReadLines(file).Skip(1);

                foreach (var line in lines)
                {
                    var c = line.Split(',');

                    if (c.Length < 3)
                        continue;

                    if (!DateTime.TryParse(c[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start))
                        continue;

                    DateTime day = start.Date;

                    tripsPerDay.TryGetValue(day, out int count);
                    tripsPerDay[day] = count + 1;
                }
            }

            return tripsPerDay;
        }

        static Dictionary<DateTime, (double meanTemp, string events)> LoadWeather(string weatherFilePath)
        {
            Dictionary<DateTime, (double, string)> weather = new();

            var lines = File.ReadLines(weatherFilePath).Skip(1);

            foreach (var line in lines)
            {
                var c = line.Split(',');

                if (c.Length < 22)
                    continue;

                if (!DateTime.TryParse(c[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    continue;

                if (!double.TryParse(c[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double meanTemp))
                    continue;

                string events = c[21].Trim();

                weather[date.Date] = (meanTemp, events);
            }

            return weather;
        }

        static void CompareTripsByTemperature(
            Dictionary<DateTime, int> trips,
            Dictionary<DateTime, (double meanTemp, string events)> weather)
        {
            var joined = trips
                .Where(t => weather.ContainsKey(t.Key))
                .Select(t => new
                {
                    Temp = weather[t.Key].meanTemp,
                    Trips = t.Value
                })
                .ToList();

            double minTemp = joined.Min(x => x.Temp);
            double maxTemp = joined.Max(x => x.Temp);

            Dictionary<int, (int trips, int days)> buckets = new();

            foreach (var d in joined)
            {
                int bucket = (int)Math.Floor((d.Temp - minTemp) / 5);

                if (!buckets.ContainsKey(bucket))
                    buckets[bucket] = (0, 0);

                var current = buckets[bucket];
                buckets[bucket] = (current.trips + d.Trips, current.days + 1);
            }

            Console.WriteLine("Average trips by temperature range (2°C buckets):");

            foreach (var b in buckets.OrderBy(x => x.Key))
            {
                double from = minTemp + b.Key * 5;
                double to = from + 5;
                double avg = b.Value.trips / (double)b.Value.days;

                Console.WriteLine($"{from,5:F1} <–> {to,5:F1} °C : {avg:F0} trips/day");
            }
        }

        static void CompareTripsByWeatherEvent(
            Dictionary<DateTime, int> trips,
            Dictionary<DateTime, (double meanTemp, string events)> weather)
        {
            Dictionary<string, (int trips, int days)> eventStats = new();

            foreach (var t in trips)
            {
                if (!weather.ContainsKey(t.Key))
                    continue;

                string ev = weather[t.Key].events;

                if (string.IsNullOrWhiteSpace(ev))
                    ev = "None";

                if (!eventStats.ContainsKey(ev))
                    eventStats[ev] = (0, 0);

                var current = eventStats[ev];
                eventStats[ev] = (current.trips + t.Value, current.days + 1);
            }

            Console.WriteLine("\nAverage trips by weather event:");

            foreach (var e in eventStats.OrderByDescending(x => x.Value.trips))
            {
                double avg = e.Value.trips / (double)e.Value.days;
                Console.WriteLine($"{e.Key,-12}: {avg:F0} trips/day");
            }
        }
    }
}
