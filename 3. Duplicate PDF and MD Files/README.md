# Duplicate PDF and MD Files

A simple C# console application that searches for files with the same name
that differ only by the **.pdf** and **.md** extensions.

The program displays a list of detected file pairs in the terminal and then —
after user confirmation — deletes the **.pdf** files, leaving only the
**.md** versions.

---

## How It Works

- scanning the `pliki` directory
- detecting `.pdf` + `.md` file pairs
- presenting the results in a clear table
- deleting `.pdf` files after user approval
