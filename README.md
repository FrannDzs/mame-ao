# mame-ao

Run MAME easily, just download this and run.

Automatically downloads all MAME binaries (from github.com) and ROMs (from archive.org) on the fly.

Built in web UI just click image and wait.

You can also use the command line just enter machine / software short name.

![MAME-AO UI](https://raw.githubusercontent.com/sam-ludlow/mame-ao/main/mame-ao-ui.png)

## Installation & Usage

- create an empty directory e.g. "C:\MameAO"
- download latest release ZIP from https://github.com/sam-ludlow/mame-ao/releases/latest
- put ZIP in empty directory and extract
- double click "mame-ao.exe"
- wait for the MAME Shell to start
- use the Web UI it will pop up
- or use command line, enter a machine name and press enter e.g. `mrdo`

TIP: Run command `.upany` to self update program, this will stop the microsoft defender notice.

## Important notes

- The first time you run it will take a while and use a lot of CPU & RAM.
- Please be patient, subsequent runs will not.

## UI

- After mame-ao has fully started the web UI will apear in the default browser.
- You can access it at http://localhost:12380/.
- Just click on image to start the machine. Select machine query profiles using the top menu.
- If the machine has software it will be listed, you can change software list using the top menu, click what you want to run.
- Click on the mame-ao console window to see what it's doing, no progress is shown in the UI (TODO).

## System requirements

- Windows with .net framework 4.8
- 32 bit / 64 bit (application is 32 bit keeps RAM usage down)
- 2 Gb RAM free

## Known issues

- Not all CHD SL (Software list) disks are available in the source & may contains incorect files (sha1 don't match). See section below "Sources - Software Disk" for more information.
- UI could do with refinement.

## Reporting issues

https://github.com/sam-ludlow/mame-ao/issues

## Symbolic Links - Save disk space

When MAME-AO downloads assets it keeps them in a "hash store", this makes keeping account of them very simple, you don’t have to keep grooming a bunch of ZIP files.

Obviously for MAME to run the ROMs have to be in the right place with the right name, so they need duplicating from the store. The best way to do this is use symbolic links, these use virtually zero disk space and point to the actual file in the store, otherwise you have to copy the files wasting diskspace.

To be able to create symbolic links you have to grant permission.

- Run "secpol" (Local Security Policy) as Administrator. Do a Windows search for "secpol" click "Run as administrator".
- Go to "Local Policies\User Rights Assignment\Create symbolic links"
- Add the required user or group. Enter your username, click the "Check Names" button to check it is correct.
- You will need to re-start for settings to take effect.

## MAME Quick usage help

Here are some quick MAME usage notes. For more detail see the official docs. https://docs.mamedev.org/usingmame/usingmame.html

### Keyboard controls

NOTE: Machines that emulate keyboards will take over, use `Scroll Lock` to toggle between standard MAME controls and full keyboard.

You should use a joystick but you will need a few keyboard commands. Full keyboard docs here https://docs.mamedev.org/usingmame/defaultkeys.html

- Player 1 Coin Up: `5`
- Player 1 Start: `1`

NOTE: Other players continue along, coin: `5`, `6`, `7`, `8` start: `1`, `2`, `3`, `4`

- Player 1 Joystick: `Cursor keys`
- Player 1 Button 1: `Left Ctrl`
- Player 1 Button 2: `Left Alt`
- Player 1 Button 3: `Spacebar`

- Main Menu - `TAB`
- Pause - `P`

- Service Mode: `F2`

- Load Saved State: `F7`
- Save Saved State: `Left Shift` + `F7`

NOTE: When saving state you have to then press another key or button to name the save, so you can have many.

- Snap Screen: `F12`

- Exit MAME: `ESC`

- Keyboard UI controls OR full keyboard : `Scroll Lock`

### MAME UI

When starting MAME without a machine you will get the MAME UI.

Use the mouse or `Cursor keys` and `Enter` to navigate.

Use `Tab` to move between windows.

The `available` filter (top left) is very handy when running previous MAME versions, use the mouse or `Tab` to get to the filters.

NOTE: Selecting machines that have software will then take you to the software lists, you can use the same `available` filter trick.

## Shell / Console
From the shell you normally just enter machine name and maybe software name.

There are also commands available they all start with a dot `.`
- `.`	- Run current MAME without a machine (start MAME UI) you can also pass arguments to MAME
- `.0255` - Run a previous version of MAME, you can still pass the machine and software but MAME-AO will not place assets in previous versions, you are better off not passing the machine and use the MAME UI with the available filter.
- `.list` - Show all saved state across all MAME versions, previous MAME versions will also be listed even without saved state.
- `.up` - Self update MAME-AO to the latest on GitHub
- `.import` - Run the import function, see below.
- `.export` - Run the export function, see below.

## Saved State and previous MAME versions

- Saved state often does not work between MAME versions. If you have started something with saved state you should continue to run the same MAME version.
- MAME-AO leaves previous MAME versions isolated in their own directory. You can easily list all saved stated across all previous and current MAME versions with the command `.list`.
- You can start a particular version of MAME with the command `.VVVV` where V is the version e.g. `.0252` or pass arguments e.g. `.0252 mrdo -window`.
- NOTE: MAME-AO only allows placing of assets in current MAME version. Any machine you ran before in previous MAME versions will already have all assets in place.
- You can list saved state in the UI. The link will start the chosen MAME version without any machine, select machine & software in the MAME UI, use the available filter (top left).
- Sometimes a regression in MAME will break a machine in the current version, so if a machine doesn't work after updating MAME you can run a previous version.

## Import
MAME-AO is all about downloading files on the fly and not bothering the user with the details.

If you have already downloaded ROM & CHD files you can feed them to MAME-AO into its "hash store" with the import function. MAME-AO will still place them as normal.

This makes sense for large files, or files that are not available on archive.org (in the sources used). You can download them on the side using whatever method you prefer.

Use the following command to perform an import

`.import <directory>`

Files are imported based on their filename extension.
- .ZIP – Archives will be extracted and imported with the same rules, this is recursive (ZIPs in ZIPs... will also be imported).
- .CHD – Disk files, will have chdman.exe run against them to determine the SHA1.
- Everything else – Considered a ROM, a hash of the file to determine the SHA1.

Important notes on import
- Filenames are not important, except the extention, the file will be imported based on its SHA1.
- Files will not be imported if its SHA1 is not in the current MAME version.
- Only .ZIP archives will be extracted, other archive formats (.7z, .rar, ...) will be considered ROMs and not work. If you have these extract them manually to the import directory.

## Export
If you want to use the downloaded assets (ROMs & DISKs) anywhere other than MAME-AO you probably aren't that happy with the way it stores files. Hash stores, no ZIPs, uncompressed ROMs or symbolic links, oh dear what am I supposed to do with those?

Well that's what the export command is for use it like so:

`.export <type> <directory>`

For example `.export mr C:\My MAME ROMs`

Type can be:
- MR : Machine ROM
- MD : Machine DISK
- SR : Software ROM
- SD : Software DISK
- \* : Everything

Everything it can will be exported to the specified directory, based on the current MAME version.
 
Machine ROMs will be in split-set format (separate parent ZIP & child diff ZIPs). If anything is missing the ZIP will not be created.

Machine DISKs that exist in a parent machine will not be exported, as the file would be duplicated.

A HTML report will be created containing details of the export.

## HTML Reports
Some MAME-AO functions will produce HTML reports, so you can see what it's been doing

There are several informational reports that you can run from the UI or with the command `.report <type>`

Use the `.report` command without any arguments to list available reports.

When the report has finished running it will pop up in the browser.

You can list previous created reports in the UI or have a look in the `_REPORTS` directory.

## MAME Data Operations
MAME-AO has the capability to perform various MAME Data operations by passing command line options when starting the program, it will exit immediately when finished.

These may be used for generating data sets in various formats, you could use it for data processing pipelines for example automatically updating a database when MAME is updated.

You have to get MAME first, all the data formats require XML to start with. If the operation has already been performed (file already exists) nothing will hapern.

You can specify a specific version e.g. `VERSION=0250` or use `VERSION=0` to mean the latest avaialable.

### Get MAME
Download and extract the MAME binaries from GitHub, needed to extract the XML.

If a new version is found the processes exit code will be set to 1.

`mame-ao.exe OPERATION=GET_MAME VERSION=0 DIRECTORY="C:\My MAME Data"`

### XML
The native format output from the MAME binary, you need this first.

`mame-ao.exe OPERATION=MAKE_XML VERSION=0 DIRECTORY="C:\My MAME Data"`

### JSON
Convert XML to JSON.

`mame-ao.exe OPERATION=MAKE_JSON VERSION=0 DIRECTORY="C:\My MAME Data"`

### SQLite
Convert XML to SQLite.

`mame-ao.exe OPERATION=MAKE_SQLITE VERSION=0 DIRECTORY="C:\My MAME Data"`

### Microsoft SQL
Convert XML to Microsoft SQL.

`mame-ao.exe OPERATION=MAKE_MSSQL VERSION=0 DIRECTORY="C:\My MAME Data" MSSQL_SERVER="Data Source='MYSERVER';Integrated Security=True;TrustServerCertificate=True;" MSSQL_TARGET_NAMES="MameAoMachine, MameAoSoftware"`

### Microsoft SQL - Make Payload Tables (XML & JSON)
Create payload tables for machine, softwarelist, and software. Create XML & JSON payloads.

`mame-ao.exe OPERATION=MAME_MSSQL_PAYLOADS VERSION=0 DIRECTORY="C:\My MAME Data" MSSQL_SERVER="Data Source='MYSERVER';Integrated Security=True;TrustServerCertificate=True;" MSSQL_TARGET_NAMES="MameAoMachine, MameAoSoftware"`

### Microsoft SQL - Make HTML Payloads
Create HTML payloads for machine, softwarelist, and software.

`mame-ao.exe OPERATION=MAME_MSSQL_PAYLOADS_HTML DIRECTORY="C:\My MAME Data" MSSQL_SERVER="Data Source='MYSERVER';Integrated Security=True;TrustServerCertificate=True;" MSSQL_TARGET_NAMES="MameAoMachine, MameAoSoftware"`

## Internal Workings

### Overall
Assets (ROMs & CHDs) are downloaded to a "Hash Store". Uncompressed individual files are stored with a filename that is the SHA1 of the file. This is completely separate to the MAME rom directories. ZIP files are not used at all.

Each MAME version is kept completely isolated, when a new version of MAME is used a fresh MAME directory is created. Previous versions are left in place, you can go back to them anytime, let’s say you have some saved state (these often don’t work with different MAME versions).

When you select a machine MAME-AO will download the files from archive.org if they are not already in the Hash Store. When in the Hash Store the files are copied (or preferably linked if enabled) to the correct place in the MAME rom directory.

### Data
SQLite databases are generated from the MAME XML output, 2 databases machine & software. This uses quiet a bit of CPU & RAM but once done is quick to load next time. If MAME-AO or MAME have version bumps the database will be re-created.

### Sources
There are 4 types of source these each relate to an item on archive.org.
- Machine ROM (version master)
- Machine DISK
- Software ROM
- Software DISK (not complete or updated as often, uses multiple archive.org items, see below)

Machine ROM is considered the version master, new MAME binaries will only be used that match the version in this archive.org item.

The version is determined from the archive.org item’s title. If a source’s archive.org item has a version mismatch you will see a warning at the Initializing preparing sources stage.

Archive.org metadata for the items are downloaded and cached to know what’s available and the file size.

### Sources - Software Disk
The other 3 sources have very good archive.org items, they are complete, accurate, and get updated within a few days of MAME releases. They just use a single archive.org item each and work like a charm.

Software Disks are a little trickier there is not a single archive.org item that covers them all. So multiple archive.org items are used.

Note that MAME support for machines with software disks is not that great, many of them have preliminary status, meaning that they barely work, you are better off using another emulator. Examples being Sony PlayStation & Sega Saturn, the disks are massive and the machines have preliminary status. You can see here that there isn't actually that many https://mame.spludlow.co.uk/WorkingMachines/SoftwareListDisk.aspx

Of the consoles there are only really these 3: Philips CD-i, Neo-Geo CD, and PC Engine (with CD Super System Card). So currently these 3 have their own archive.org items.

See the sources source code to see what archive.org items MAME-AO is using https://github.com/sam-ludlow/mame-ao/blob/main/source/Sources.cs

## Credits

### MAME
Emulator software
https://www.mamedev.org/

### Archive.org
ROM & DISK download
https://archive.org/

### Progetto Snaps
UI Images
https://www.progettosnaps.net/snapshots/

UI Genres
https://www.progettosnaps.net/catver/

### Newtonsoft
JSON Library
https://www.newtonsoft.com/json

### SQLite
SQL Database
https://www.sqlite.org/

### Spludlow MAME
Image hosting
https://mame.spludlow.co.uk/
