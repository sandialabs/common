![COMMON image](src/COMMONWeb/Content/common_header.png)

## NSDD - Nuclear Smuggling Detection and Deterrence
Used to monitor the communications systems at NSDD installations.

    Copyright 2018 National Technology & Engineering Solutions of Sandia, LLC (NTESS).  
    Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains  
    certain rights in this software.

# Installation

### Current Release
Download [here](https://github.com/sandialabs/common/releases)  
[Installation/Configuration Manual](docs/COMMONInstallationConfiguration_v1.5.pdf)  
[User's Manual](docs/COMMONUserManual_v1.5.pdf)  

# COMMON main pieces:
- A user-interface, accessed via a Web Browser, written in TypeScript and AngularJS (1.6.x).
  - As of COMMON 1.5 the web server is self-hosted using NancyFX
- A Windows service, written in C#, that collects data using WMI from other Windows machines, and also pings non-Windows machines to test network connectivity.
  - The collected data is stored in a SQLite database
  - The service hosts the main web site
  - Queries are run against the same database the service is storing data into
- A configuration tool, written in C# and WPF, which is used to set up the COMMON software
  - Most configuration is stored within the database
  - The configuration tool can only be run by an administrator
  - There are only two configuration files not kept within the database
    - The `common_config.json` file which indicates the location of the database file
    - The `common_log_config.json` file that indicates where to write log files
- A command-line tool for collecting and displaying the same data the service collects.
  - The collected data is displayed on-screen instead of putting it in the database.
  - Is commonly used to test that WMI data collection can be done remotely

# All projects
- COMMON
  - The Windows service
  - Collects data using the schedule as specified in the configuration
  - Writes daily files each day at 12:01 AM
    - Will write files for any day that has unwritten data
  - Also hosts the main web site
  - Is a command-line application when built Debug, becomes a service when built Release
- COMMONCLI
  - The command-line app
  - Collects data the same way the service does, but instead of storing it in a database it is displayed on the terminal
- COMMONConfig
  - The configuration tool
  - A WPF application that requires the user be an administrator
- COMMONWeb
  - The web-based GUI
  - All of the browser-side code is written in TypeScript
  - The server-side code is C#
- COMMONWebHost
  - A small command-line application that fires up an instance of the web site
  - Is used if you want to have a GUI but don't want to collect data, which the service does
  - The port to listen on and the database to use can be specified so multiple sites can be hosted at the same time on the same machine.
- COMMONWebTest
  - Some test suites that test the web server
  - Needs to be more fully fleshed out
- ConfigurationLib
  - Library that holds configuration-related things that are common to the whole project.
  - Things like `enums` and some of the more basic classes are defined here
- CSVLib
  - Library that holds classes related to reading/writing CSV files
  - Is used in the configuration tool to import/export some configuration settings
- DailyFileImport
  - Command-line application to import a series of daily-files into a database
- DailyFileLib
  - Library that holds classes related to reading/writing/importing daily files
  - Daily files are a JSON file that contains all the data collected during a single day
- DatabaseLib
  - Library that holds everything related to the COMMON database.
  - The database is SQLite
  - This project was started when EntityFramework for SQLite was still buggy, so we use string-based queries
  - Because the web-based GUI doesn't have any user-input we don't have to worry too much about SQL injection
  - But, I would still like to paramaterize the queries, or move to EntityFramework
    - TBD
- DatabasePrune
  - Command-line app for pruning data from a database
  - Is used to take a database that has large amounts of data and prune it down to a more typical amount of data
  - Is used in our testbed
    - The testbed is set up to stress COMMON by collecting data at a far higher rate than is typical
    - We use this so we can also test with a typically-sized database
- DataLib
  - Is the library that is used in data collection
  - Collects pings, WMI, database, etc.
- DBTester
  - Just a quick test program for putting data into a DB then clearing it out
  - Hasn't been used in years--it could probably go away
- EncryptDecrypt
  - Simple command-line app for encrypting/decrypting things using the encryption/decryption code in UtilitiesLib
- json
  - Simple command-line utility to deal with JSON files
  - Can convert a JSON to CSV
- Logging
  - Library with code that handles our logging
  - Can log to file or to the EventLog
- RequestResponseLib
  - Simple library for having a generic way of asking for information, and providing that information
  - Derive a class from `Request` to ask for data, then derive a class from `Responder` to provide that data
  - Register your `Responder` with the `RequestBus`, then when a `Request` is placed on the bus, your `Responder` will be called and you can provide the data
  - Is used so some communication between libraries can be done without direct dependencies
- UnitTest
  - Has our XUnit unit tests
  - Also needs to be more fully fleshed out
- UtilitiesLib
  - Library that holds generic code that doesn't really fit anywhere else
  - Contains some simple encryption/decryption code
    - Used to obfuscate some data that's stored in our database
    - Isn't really secure since the key is stored in the source code
    - Is just used to prevent prying eyes from easily seeing things


# Building
COMMON is built using Visual Studio 2017. The COMMONWeb part also uses Bower, NPM, and Gulp. Eventually, I plan to do away with Bower and just use NPM, but haven't done that yet.

There have been cases where Visual Studio doesn't properly do the Bower and NPM stuff, so you may need to install those separately.

We have a Jenkins CI builder that does these steps:  

    .nuget\nuget.exe restore COMMON.sln
    cd COMMONWeb
    bower install
    npm install
    gulp
    cd ..
    MSBuild.exe COMMON.sln /t:ReBuild /p:Configuration=Release

May not need that NuGet restore in there, but it's still part of the build script for now.
