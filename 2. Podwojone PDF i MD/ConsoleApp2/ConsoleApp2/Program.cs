using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static void Main()
    {
        string folderPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\pliki")
        );
        

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("Folder nie istnieje: " + folderPath);
            return;
        }
        
        var allFiles = Directory.GetFiles(folderPath)
            .Select(p => new FileInfo(p))
            .Where(fi => fi.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                      || fi.Extension.Equals(".md", StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        var pairs = allFiles
            .GroupBy(fi => Path.GetFileNameWithoutExtension(fi.Name), StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var pdf = g.FirstOrDefault(x => x.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase));
                var md  = g.FirstOrDefault(x => x.Extension.Equals(".md",  StringComparison.OrdinalIgnoreCase));
                return new { BaseName = g.Key, Pdf = pdf, Md = md };
            })
            .Where(x => x.Pdf != null && x.Md != null) // interesują nas pary które mają i .pdf i .md
            .OrderBy(x => x.BaseName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (pairs.Count == 0)
        {
            Console.WriteLine("Nie znaleziono par .pdf + .md w folderze: " + folderPath);
            return;
        }
        
        Console.WriteLine("Znalezione pary (.pdf + .md):\n");
        Console.WriteLine("{0,3}  {1,-40}  {2,10}   {3,19}   {4,10}   {5,19}",
            "nr", "nazwa (bez rozs.)", "pdf [KB]", "pdf - zmodyfikowano", "md [KB]", "md - zmodyfikowano");
        Console.WriteLine(new string('-', 110));

        var ci = CultureInfo.CurrentCulture;
        for (int i = 0; i < pairs.Count; i++)
        {
            var p = pairs[i];
            string pdfSize = (p.Pdf.Length / 1024.0).ToString("N1", ci);
            string mdSize  = (p.Md.Length  / 1024.0).ToString("N1", ci);
            string pdfTime = p.Pdf.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
            string mdTime  = p.Md.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

            Console.WriteLine("{0,3}. {1,-36}  {2,8}KB   {3,19}   {4,8}KB   {5,19}",
                i + 1, Truncate(p.BaseName, 36), pdfSize, pdfTime, mdSize, mdTime);
        }

        Console.WriteLine("\nUWAGA: Po potwierdzeniu program usunie pliki .pdf z wybranych par (pozostaną tylko pliki .md).");
        Console.WriteLine("Opcje:");
        Console.WriteLine("  - wpisz 'tak' aby usunąć PDFy dla wszystkich wymienionych par");
        Console.WriteLine("  - wpisz listę numerów (np. 1,3-5) aby usunąć PDFy tylko dla wybranych pozycji");
        Console.WriteLine("  - pozostaw puste i naciśnij Enter lub wpisz 'nie' by anulować\n");

        Console.Write("Twoj wybor: ");
        string input = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(input) || input.Equals("nie", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Anulowano. Żadne pliki nie zostały usunięte.");
            return;
        }
        
        bool deleteAll = input.Equals("tak", StringComparison.OrdinalIgnoreCase);
        
        HashSet<int> toDeleteIndexes = new HashSet<int>();

        if (deleteAll)
        {
            for (int i = 0; i < pairs.Count; i++) toDeleteIndexes.Add(i);
        }
        else
        {
            var tokens = input.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (token.Contains("-"))
                {
                    var range = token.Split('-');
                    if (range.Length == 2 &&
                        int.TryParse(range[0], out int a) &&
                        int.TryParse(range[1], out int b))
                    {
                        int start = Math.Max(1, Math.Min(a, b));
                        int end   = Math.Min(pairs.Count, Math.Max(a, b));
                        for (int k = start; k <= end; k++) toDeleteIndexes.Add(k - 1);
                    }
                }
                else
                {
                    if (int.TryParse(token, out int idx))
                    {
                        if (idx >= 1 && idx <= pairs.Count) toDeleteIndexes.Add(idx - 1);
                    }
                }
            }

            if (toDeleteIndexes.Count == 0)
            {
                Console.WriteLine("Nie rozpoznano żadnych pozycji do usunięcia. Anulowano.");
                return;
            }
        }
        
        Console.WriteLine("\nZamierzasz usunąć PDFy dla następujących pozycji:");
        foreach (int idx in toDeleteIndexes.OrderBy(x => x))
        {
            var p = pairs[idx];
            Console.WriteLine($"  {idx+1}. {p.Pdf.Name}  (-> pozostanie: {p.Md.Name})");
        }
        Console.Write("\nPotwierdź usunięcie wpisując 'TAK': ");
        string finalConfirm = Console.ReadLine()?.Trim();
        if (!finalConfirm.Equals("tak", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Potwierdzenie nie otrzymane. Anulowano.");
            return;
        }
        
        Console.WriteLine();
        int deleted = 0;
        int errors = 0;
        foreach (int idx in toDeleteIndexes.OrderBy(x => x))
        {
            var p = pairs[idx];
            string pdfPath = p.Pdf.FullName;
            try
            {
                if (File.Exists(pdfPath))
                {
                    File.Delete(pdfPath);
                    Console.WriteLine($"Usunięto: {p.Pdf.Name}");
                    deleted++;
                }
                else
                {
                    Console.WriteLine($"Nie znaleziono (pominięto): {p.Pdf.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy usuwaniu {p.Pdf.Name}: {ex.Message}");
                errors++;
            }
        }

        Console.WriteLine($"\nZakończono. Usunięto: {deleted}. Błędów: {errors}.");
        Console.WriteLine("Pozostałe pliki .md są nadal w katalogu.");
    }
    
    static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }
}
