using System;
using System.IO;

class Program
{
    static void Main()
    {
        string sciezka = @"C:\nauka\testowanie";

        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = sciezka;
        watcher.IncludeSubdirectories = true;
        watcher.Filter = "*.*";

        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;

        watcher.EnableRaisingEvents = true;

        Console.WriteLine($"Obserwuję folder: {sciezka}");
        Console.WriteLine("Dodaj, usuń lub zmień nazwę pliku, aby zobaczyć reakcję.");
        Console.WriteLine("Naciśnij Enter, aby zakończyć.\n");

        Console.ReadLine();
    }

    static void OnCreated(object sender, FileSystemEventArgs e)
        => Console.WriteLine($"[UTWORZONO] {e.FullPath}");

    static void OnDeleted(object sender, FileSystemEventArgs e)
        => Console.WriteLine($"[USUNIĘTO] {e.FullPath}");

    static void OnRenamed(object sender, RenamedEventArgs e)
        => Console.WriteLine($"[ZMIENIONO NAZWĘ] {e.OldFullPath} → {e.FullPath}");
}