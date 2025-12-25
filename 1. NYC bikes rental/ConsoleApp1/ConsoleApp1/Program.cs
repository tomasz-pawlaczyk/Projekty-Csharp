using System;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var totalStopwatch = Stopwatch.StartNew();
            
            Console.WriteLine("NYC Bike Rentals 2015 – Analysis");
            Console.WriteLine("--------------------------------");

            string projectRoot = GetProjectRoot();
            string tripsPath = Path.Combine(projectRoot, "2015-citibike-tripdata");

            RunWithTimer(
                "Compare gender usage",
                () => CompareGenderUsers.Run(tripsPath)
            );
            
            RunWithTimer(
                "Compare by time",
                () => CompareTripsByMonthAndHour.Run(tripsPath)
            );
            
            string weatherPath = Path.Combine(projectRoot, "nyc-weather-2015.csv");
            
            RunWithTimer(
                "Trips vs weather",
                () => CompareWeatherAndTrips.Run(tripsPath, weatherPath)
            );
            
            RunWithTimer(
                "Station rebalancing",
                () => CompareStationBalance.Run(tripsPath)
            );

            RunWithTimer(
                "Compare popular routes",
                () => ComparePopularRoutes.Run(tripsPath)
            );
            
            RunWithTimer(
                "Compare Customer vs Subscriber",
                () => CompareUserTypes.Run(tripsPath)
            );

            RunWithTimer(
                "Detect anomalies",
                () => DetectAnomalies.Run(tripsPath)
            );
            
            Console.WriteLine("\nAll analyses finished.");
            
            totalStopwatch.Stop();
            TimeSpan total = totalStopwatch.Elapsed;

            int seconds = (int)total.TotalSeconds;
            int milliseconds = (int)((total.TotalMilliseconds % 1000) / 10);

            Console.WriteLine($"TOTAL EXECUTION TIME: {seconds}.{milliseconds:D2} s");


        }


        static void RunWithTimer(string name, Action action)
        {
            var stopwatch = Stopwatch.StartNew();

            action();

            stopwatch.Stop();

            TimeSpan t = stopwatch.Elapsed;

            string formattedTime = $"{t.Seconds}.{t.Milliseconds / 10:D2} s\n";

            Console.WriteLine($"[{name}] Execution time: {formattedTime}");
        }


        static string GetProjectRoot()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);

            while (dir != null &&
                   !Directory.Exists(Path.Combine(dir.FullName, "2015-citibike-tripdata")))
            {
                dir = dir.Parent;
            }

            if (dir == null)
                throw new DirectoryNotFoundException("Nie znaleziono katalogu z danymi.");

            return dir.FullName;
        }
    }
}