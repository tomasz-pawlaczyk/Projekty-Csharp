using System;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    static string folderPath = @"C:\Users\tomas\Studia\III semestr\Csharp\warsztaty\nauka_brak_war\testowanie";
    static int countTxt = 0;
    static int countPng = 0;
    static int countMd = 0;
    
    
    
    static void Main()
    {
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"Folder {folderPath} does not exist");
            return;
        }

        UpdateCounts();
        
        string folderObrazki = Path.Combine(folderPath, "obrazki");
        string folderTeksty   = Path.Combine(folderPath, "teksty");
        string folderDokumenty = Path.Combine(folderPath, "dokumenty");

        Directory.CreateDirectory(folderObrazki);
        Directory.CreateDirectory(folderTeksty);
        Directory.CreateDirectory(folderDokumenty);

        
        FileSystemWatcher watcher = new FileSystemWatcher(folderPath);
        watcher.IncludeSubdirectories = true;
        watcher.Filter = "*.*";
        
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;

        
        watcher.EnableRaisingEvents = true;
        
        Console.WriteLine($"Obserwuję folder: {folderPath}");
        Console.WriteLine("Dodaj, usuń lub zmień nazwę pliku, aby zobaczyć reakcję.");
        Console.WriteLine("Naciśnij Enter, aby zakończyć.\n");

        string input;
        do
        {
            input = Console.ReadLine();
            if (input == "show")
                ShowCounts();
        } 
        while (!string.IsNullOrEmpty(input));
    }

    static void OnCreated(object sender, FileSystemEventArgs e)
    {
        string nazwaPliku = Path.GetFileName(e.FullPath);
        Console.WriteLine($"[UTWORZONO] Dodano plik o nazwie: {nazwaPliku}");
        UpdateCounts();
        
        
        string extension = Path.GetExtension(e.FullPath).ToLower();
        string destination = null;

        if (extension == ".png")
            destination = Path.Combine(folderPath, "obrazki", Path.GetFileName(e.FullPath));
        else if (extension == ".txt")
            destination = Path.Combine(folderPath, "teksty", Path.GetFileName(e.FullPath));
        else if (extension == ".pdf")
            destination = Path.Combine(folderPath, "dokumenty", Path.GetFileName(e.FullPath));

        if (destination != null)
        {
            try
            {
                File.Move(e.FullPath, destination);
                Console.WriteLine($"Przeniesiono do: {destination}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy przenoszeniu: {ex.Message}");
            }
        }
        
    }
        

    static void OnDeleted(object sender, FileSystemEventArgs e)
    {
        string nazwaPliku = Path.GetFileName(e.FullPath);
        Console.WriteLine($"[USUNIĘTO] Usunięto plik o nazwie: {nazwaPliku}");
        UpdateCounts();
    }
    
    static void OnRenamed(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"[EDYTOWANO] Zmieniono nazwe na: {Path.GetFileName(e.FullPath)}");
    }

    static void UpdateCounts()
    {
        try
        {
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            countTxt = files.Count(f => f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase));
            countPng = files.Count(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase));
            countMd = files.Count(f => f.EndsWith(".md", StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas aktualizacji: {ex.Message}");
        }
    }

    static void ShowCounts()
    {
        Console.WriteLine($"Pliki .txt: {countTxt}");
        Console.WriteLine($"Pliki .png: {countPng}");
        Console.WriteLine($"Pliki .md: {countMd}");
    }

}










