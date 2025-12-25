using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public static class ComparePopularRoutes
    {
        public static void Run(string tripsDirectory)
        {
            Dictionary<string, long> allRoutes = new();
            Dictionary<string, long> directedRoutes = new();
            Dictionary<string, long> undirectedRoutes = new();

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

                    if (c.Length < 9)
                        continue;

                    string start = c[4].Trim();
                    string end = c[8].Trim();

                    if (string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end))
                        continue;

                    string allKey = $"{start} -> {end}";
                    allRoutes.TryGetValue(allKey, out long allCount);
                    allRoutes[allKey] = allCount + 1;

                    if (start == end)
                        continue;

                    string directedKey = $"{start} -> {end}";
                    directedRoutes.TryGetValue(directedKey, out long dCount);
                    directedRoutes[directedKey] = dCount + 1;

                    string a = string.Compare(start, end, StringComparison.Ordinal) < 0 ? start : end;
                    string b = a == start ? end : start;

                    string undirectedKey = $"{a} <-> {b}";
                    undirectedRoutes.TryGetValue(undirectedKey, out long uCount);
                    undirectedRoutes[undirectedKey] = uCount + 1;
                }
            }

            Console.WriteLine("V. COMPARE POPULAR ROUTES");
            PrintTop5All(allRoutes);
            PrintTop5Directed(directedRoutes);
            PrintTop5Undirected(undirectedRoutes);
        }

        static void PrintTop5All(Dictionary<string, long> routes)
        {
            Console.WriteLine("Top 5 most popular routes (including loops):");

            int i = 1;
            foreach (var r in routes
                .OrderByDescending(x => x.Value)
                .Take(5))
            {
                Console.WriteLine($"{i}. {r.Key} : {r.Value:N0}");
                i++;
            }
        }

        static void PrintTop5Directed(Dictionary<string, long> routes)
        {
            Console.WriteLine("\nTop 5 most popular directed routes (no loops):");

            int i = 1;
            foreach (var r in routes
                .OrderByDescending(x => x.Value)
                .Take(5))
            {
                Console.WriteLine($"{i}. {r.Key} : {r.Value:N0}");
                i++;
            }
        }

        static void PrintTop5Undirected(Dictionary<string, long> routes)
        {
            Console.WriteLine("\nTop 5 most popular undirected routes (no loops):");

            int i = 1;
            foreach (var r in routes
                .OrderByDescending(x => x.Value)
                .Take(5))
            {
                Console.WriteLine($"{i}. {r.Key} : {r.Value:N0}");
                i++;
            }
        }
    }
}
