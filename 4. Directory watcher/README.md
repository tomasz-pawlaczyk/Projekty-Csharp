# C# Projects

### Task1.cs

The program monitors all files (`*.*`) in the **C:\nauka\testowanie** directory
and all of its subdirectories.  
For every change, it **prints messages to the console** informing about file
creation, deletion, or renaming.

---

### Task2.cs

The program watches the **testowanie** directory and everything inside its
subdirectories.  
It observes every added, deleted, or renamed file and reports these events in
the console.

When a new file appears, it is automatically moved to the appropriate folder:

- `.png` → **obrazki**
- `.txt` → **teksty**
- `.pdf` → **dokumenty**

The program also keeps a live count of all `.txt`, `.png`, and `.md` files in
the entire directory tree.  
When the user types `show` in the console, the current counts are displayed.

---

### Task3.cs

The program works as an **automatic file sorter**.  
It monitors the **szukaj** directory and all of its subdirectories.

First, it scans and organizes all existing files.  
Then it starts watching the directory in real time.

When a new file appears, the program detects it and — after a short delay to
ensure the file is fully written — checks its extension.  
The file is then automatically moved to the correct folder:

- `.bmp` → **obrazki**
- `.txt` → **teksty**
- `.pdf` → **dokumenty**

If a file with the same name already exists in the destination directory, a new
name with a number is generated to prevent overwriting.

Each file move is additionally logged in **log.txt**, so it is always possible
to see what has been processed.
