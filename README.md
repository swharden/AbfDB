# AbfDB

[![](https://github.com/swharden/AbfDB/actions/workflows/ci.yaml/badge.svg)](https://github.com/swharden/AbfDB/actions/workflows/ci.yaml)

**AbfDB searches the filesystem for Axon Binary Format (ABF) files and collects their header information in a database.** ABF header information is read using the official ABFFIO.DLL (a 32-bit Windows binary) so this project is windows-only and must target x86. See [AbfSharp](https://github.com/swharden/ABFsharp#dll-requirements) for details.

> **⚠️ WARNING:** This software is not supported. Code in this repository is intended only to be used by the original authors and is provided here for backup and educational purposes only.

## Usage

### **AbfDB.exe** 

This console application scans a directory folder tree for ABFs and builds a SQLite database containing their information. 

This program only needs to be run once.

```
abfdb.exe [scan folder] [output folder]
```

### AbfDB.Monitor.exe

This GUI application monitors the filesystem of a given folder and updates the database whenever ABF files are added, modified, or deleted.

```
AbfDB.Monitor.exe [watch folder] [database file]
```