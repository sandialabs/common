![COMMON image](src/COMMONWeb/Content/common_header.png)

The COMMON project is split into several different subprojects. These are described below.

### COMMON
- The Windows service
- Collects data using the schedule as specified in the configuration
- Writes daily files each day at 12:01 AM
  - Will write files for any day that has unwritten data
- Also hosts the main web site
- Is a command-line application when built Debug, becomes a service when built Release
### COMMONCLI
- The command-line app
- Collects data the same way the service does, but instead of storing it in a database it is displayed on the terminal
### COMMONConfig
- The configuration tool
- A WPF application that requires the user be an administrator
### COMMONWeb
- The web-based GUI
- All of the browser-side code is written in TypeScript
- The server-side code is C#
### COMMONWebHost
- A small command-line application that fires up an instance of the web site
- Is used if you want to have a GUI but don't want to collect data, which the service does
- The port to listen on and the database to use can be specified so multiple sites can be hosted at the same time on the same machine.
### COMMONWebTest
- Some test suites that test the web server
- Needs to be more fully fleshed out
### ConfigurationLib
- Library that holds configuration-related things that are common to the whole project.
- Things like `enums` and some of the more basic classes are defined here
### CSVLib
- Library that holds classes related to reading/writing CSV files
- Is used in the configuration tool to import/export some configuration settings
### DailyFileImport
- Command-line application to import a series of daily-files into a database
### DailyFileLib
- Library that holds classes related to reading/writing/importing daily files
- Daily files are a JSON file that contains all the data collected during a single day
### DatabaseLib
- Library that holds everything related to the COMMON database.
- The database is SQLite
- This project was started when EntityFramework for SQLite was still buggy, so we use string-based queries. We have paramaterized the queries, so that's good.
- Because the web-based GUI doesn't have any user-input we don't have to worry too much about SQL injection
### DatabasePrune
- Command-line app for pruning data from a database
- Is used to take a database that has large amounts of data and prune it down to a more typical amount of data
- Is used in our testbed
  - The testbed is set up to stress COMMON by collecting data at a far higher rate than is typical
  - We use this so we can also test with a typically-sized database
### DataLib
- Is the library that is used in data collection
- Collects pings, WMI, database, etc.
### DBTester
- Just a quick test program for putting data into a DB then clearing it out
- Hasn't been used in years--it could probably go away
### EncryptDecrypt
- Simple command-line app for encrypting/decrypting things using the encryption/decryption code in UtilitiesLib
### json
- Simple command-line utility to deal with JSON files
- Can convert a JSON to CSV
### Logging
- Library with code that handles our logging
- Can log to file or to the EventLog
### RequestResponseLib
- Simple library for having a generic way of asking for information, and providing that information
- Derive a class from `Request` to ask for data, then derive a class from `Responder` to provide that data
- Register your `Responder` with the `RequestBus`, then when a `Request` is placed on the bus, your `Responder` will be called and you can provide the data
- Is used so some communication between libraries can be done without direct dependencies
### UnitTest
- Has our XUnit unit tests
- Also needs to be more fully fleshed out
### UtilitiesLib
- Library that holds generic code that doesn't really fit anywhere else
- Contains some simple encryption/decryption code
  - Used to obfuscate some data that's stored in our database
  - Isn't really secure since the key is stored in the source code
  - Is just used to prevent prying eyes from easily seeing things

# Building
COMMON is built using Visual Studio 2019. The COMMONWeb part also uses NPM and Gulp.

There have been cases where Visual Studio doesn't properly do the NPM stuff, so you may need to install those separately.

We have a Jenkins CI builder that does these steps:  

    .nuget\nuget.exe restore COMMON.sln
    cd COMMONWeb
    npm install
    gulp
    cd ..
    MSBuild.exe COMMON.sln /t:ReBuild /p:Configuration=Release

May not need that NuGet restore in there, but it's still part of the build script for now.

# Installer
We use [Advanced Installer](https://www.advancedinstaller.com) (AI) to build our installation executable. The AI files are kept in the `src/Installers` folder
```
src/Installers/COMMONInstaller-1.5.2.aip
```

# Upgrading
After you make your code changes, upgrading involves updating the version numbers of the EXEs and DLLs. This is pretty simple and follows these steps:  
- Run the Python script `set_version.py` which updates the version number in each `AssemblyInfo.cs` file (replace the version with whatever is appropriate)  
    ```
    python set_version.py 1.5.2
    ```
- Update the Advanced Installer file with the new version #.  It's in the "Product Details" section. It will ask you to generate a new Product Code. Click 'Generate New'.
  - Save the updated Advanced Installer file with a new version # in the filename, using the 'Save As' menu option.
- You also need to tweak the installer build script to use the file you just saved, which is in `Installers/build.py` This Python script is run to build the installer executable. Tweak the `fullVersion` variable so it's the same version # of the AI file:
    ```
    fullVersion = "1.5.2"
    ```
That should do it!
