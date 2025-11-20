# Projekty-Csharp




### Zad1.cs

Program monitoruje wszystkie pliki (`*.*`) w folderze **C:\nauka\testowanie** oraz jego podfolderach, a na każdą zmianę reaguje, **wyświetlając w konsoli komunikaty** o utworzeniu, usunięciu lub zmianie nazwy pliku.





### Zad2.cs

Program pilnuje folderu **testowanie** i wszystkiego, co jest w jego podfolderach. Obserwuje każdy dodany, usunięty albo przemianowany plik i informuje o tym w konsoli.  
Kiedy pojawi się nowy plik, automatycznie przenosi go we właściwe miejsce:

- `.png` do **obrazki**,

- `.txt` do **teksty**,

- `.pdf` do **dokumenty**.

Na bieżąco liczy też, ile jest w całym folderze plików `.txt`, `.png` i `.md`.  
Jeśli w konsoli wpisze się `show`, pokaże aktualne liczby tych plików.





### Zad3.cs

Program działa jak **automatyczny sortownik plików**.  
Pilnuje folderu **szukaj** i wszystkiego w jego podfolderach. Najpierw przegląda wszystkie istniejące pliki i od razu je porządkuje, a później zaczyna obserwować folder na żywo.

Gdy pojawi się nowy plik, program wykrywa go i – po krótkim odczekaniu, żeby plik zdążył się zapisać – sprawdza jego rozszerzenie.  
Następnie automatycznie przenosi go do odpowiedniego folderu:

- `.bmp` → **obrazki**

- `.txt` → **teksty**

- `.pdf` → **dokumenty**

Jeśli w miejscu docelowym istnieje już plik o tej samej nazwie, tworzy nową nazwę z numerem, żeby nic nie nadpisać.

Każde przeniesienie zapisuje dodatkowo w pliku **log.txt**, żeby było wiadomo, co zostało przetworzone.