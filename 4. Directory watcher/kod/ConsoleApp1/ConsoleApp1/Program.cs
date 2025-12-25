using System;
using System.IO;
using System.Linq;

class Program
{
    static string folderRoot = @"C:\Users\tomas\Studia\III semestr\Csharp\warsztaty\nauka_brak_war\testowanie";
    static string folderSearch => Path.Combine(folderRoot, "szukaj");

    static string folderObrazki => Path.Combine(folderRoot, "obrazki");
    static string folderTeksty => Path.Combine(folderRoot, "teksty");
    static string folderDokumenty => Path.Combine(folderRoot, "dokumenty");

    static void Main()
    {
        if (!Directory.Exists(folderSearch))
        {
            Console.WriteLine($"Folder '{folderSearch}' nie istnieje!");
            return;
        }

        Directory.CreateDirectory(folderObrazki);
        Directory.CreateDirectory(folderTeksty);
        Directory.CreateDirectory(folderDokumenty);

        Console.WriteLine("Start sortowania początkowego…\n");
        SortFilesRecursively(); 

        Console.WriteLine("\nStart obserwacji folderu 'szukaj'…");
        StartWatcher();

        Console.WriteLine("Naciśnij Enter, aby zakończyć.");
        Console.ReadLine();
    }

    static void StartWatcher()
    {
        FileSystemWatcher watcher = new FileSystemWatcher(folderSearch);
        watcher.IncludeSubdirectories = true;
        watcher.Filter = "*.*";

        watcher.Created += OnCreated;

        watcher.EnableRaisingEvents = true;
    }

    static void OnCreated(object sender, FileSystemEventArgs e)
    {
        // Czasem Watcher zadziała zanim plik się zamknie — dajemy mini opóźnienie
        Thread.Sleep(100);

        string file = e.FullPath;

        if (!File.Exists(file))
            return;

        Console.WriteLine($"\n[WYKRYTO NOWY PLIK] {file}");

        MoveFileToCategory(file);
    }

    static void SortFilesRecursively()
    {
        var files = Directory.GetFiles(folderSearch, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
            MoveFileToCategory(file);
    }

    static void MoveFileToCategory(string file)
    {
        string ext = Path.GetExtension(file).ToLower();
        string dest = null;

        if (ext == ".bmp")
            dest = Path.Combine(folderObrazki, Path.GetFileName(file));
        else if (ext == ".txt")
            dest = Path.Combine(folderTeksty, Path.GetFileName(file));
        else if (ext == ".pdf")
            dest = Path.Combine(folderDokumenty, Path.GetFileName(file));
        else
            return;

        try
        {
            dest = GetUniquePath(dest);

            File.Move(file, dest);
            Console.WriteLine($" → przeniesiono do: {dest}");
            Log($"Przeniesiono plik: {file} → {dest}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd przenoszenia: {ex.Message}");
            Log($"Błąd przenoszenia pliku {file}: {ex.Message}");
        }
    }

    static string GetUniquePath(string path)
    {
        string folder = Path.GetDirectoryName(path);
        string name = Path.GetFileNameWithoutExtension(path);
        string ext = Path.GetExtension(path);

        int counter = 1;
        string newPath = path;

        while (File.Exists(newPath))
        {
            newPath = Path.Combine(folder, $"{name} ({counter}){ext}");
            counter++;
        }

        return newPath;
    }
    
    static void Log(string message)
    {
        string folder = @"C:\Users\tomas\Studia\III semestr\Csharp\warsztaty\nauka_brak_war\testowanie";


        string logPath = Path.Combine(folder, "log.txt");
        string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}";

        File.AppendAllText(logPath, entry + Environment.NewLine);
    }
    
}
