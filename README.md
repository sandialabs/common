![COMMON image](src/COMMONWeb/Content/common_header.png)

Communications systems are a frequent cause of problems in radiation detection systems. These problems create headaches for partner countries’ staff and the local maintenance providers responsible for keeping commerce moving. These problems also threaten the ability of radiation detection systems to identify and signal the potential illicit movement of nuclear and radiological materials.

Sandia National Laboratories’ COMMON software is designed to help by pointing to system issues before they affect system operation. This free, easy to use tool helps partner countries monitor hardware and operating system software for potential problems, then pinpoint those problems to make them easier to fix. **The result:** Less downtime resulting in improved smuggling deterrence.

> Copyright 2018 National Technology & Engineering Solutions of Sandia, LLC (NTESS). Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

# Current Release

The current version is [1.5.2](https://github.com/sandialabs/common/releases/tag/v1.5.2). Older versions can be found [here](https://github.com/sandialabs/common/releases).

# Release History

| Version | Date | Comment |
| ------- | ---- | ---- |
| [1.5.2](https://github.com/sandialabs/common/releases/tag/v1.5.2) | 2019/07/22 | `Fixed an issue that could prevent COMMON from collecting WMI data from the server` |
| 1.5.1 | 2019/04/25 | `Fixed missing 'Uptime %' in reports`</br>`Performance improvements` |
| 1.5.0 | 2019/03/13 | `Initial Commit` |

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

# Development

For information about building COMMON, or contributing to the project, see the [Development](DEVELOPMENT.md) page.
