using System;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public static class CompareGenderUsers
    {
        public static void Run(string tripsDirectory)
        {
            int maleCount = 0;
            int femaleCount = 0;

            long maleTripDurationSum = 0;
            long femaleTripDurationSum = 0;

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
                    var columns = line.Split(',');

                    if (columns.Length < 15)
                        continue;
                    
                    if (!int.TryParse(columns[0], out int tripDuration) || tripDuration <= 0)
                        continue;
                    
                    if (!int.TryParse(columns[^1], out int gender))
                        continue;

                    if (gender == 1)
                    {
                        maleCount++;
                        maleTripDurationSum += tripDuration;
                    }
                    else if (gender == 2)
                    {
                        femaleCount++;
                        femaleTripDurationSum += tripDuration;
                    }
                }
            }

            int total = maleCount + femaleCount;

            if (total == 0)
            {
                Console.WriteLine("Brak danych o płci.");
                return;
            }

            double malePercent = maleCount * 100.0 / total;
            double femalePercent = femaleCount * 100.0 / total;

            double maleAvgMinutes = maleTripDurationSum / 60.0 / maleCount;

            double femaleAvgMinutes = femaleTripDurationSum / 60.0 / femaleCount;

            Console.WriteLine("\nI. GENDER USAGE ANALYSIS");
            Console.WriteLine($"Men:    {maleCount:N0} rides ({malePercent:F2}%)");
            Console.WriteLine($"Women:  {femaleCount:N0} rides ({femalePercent:F2}%)");

            Console.WriteLine("\nAverage trip duration:");
            Console.WriteLine($"Men:    {maleAvgMinutes:F2} minutes");
            Console.WriteLine($"Women:  {femaleAvgMinutes:F2} minutes");
        }
    }
}
