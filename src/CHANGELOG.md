# 1.1 (Albuquerque)

## Updates/new features:
Add COMMON build version to the web interface  
Device page: Standardize the box size of each item  
Upper bound for graphs should be 100% for NIC  
Need to ping all configured devices  
Updated report format. It had been too dull.  
Config tool upgrade for configuration saving/uploading.  
Change daily file extension  
Device page: Change order of the boxes (Order by: CPU Use, Memory Use, Disk Use, Disk Performance, NIC Use, DB Size, Running Processes, Running Services, Installed Applications, Event Log Errors - System, Event Log Errors - Applications)  

## Bugs fixed:
Languages are blank (config tool)  
No applications were being retrieved from the workstations in Djibouti  
About page: Can't see the entire page, cuts off at bottom  
Service hung up when collecting pings  
Issue with failed pings in Djibouti  
PDF report of server says, "There is no report" for each disk.  


# 1.2 (Bodie)

## Updates/new features:
Show lanes in web pages  
Remember last chosen language  
Add site name to configuration filename (config tool)  
Change settings configuration file to JSON (config tool)  
Create x lanes (config tool)  

## Bugs fixed:
IP Address can't be tabbed to (config tool)  
Browser runs out of memory when rendering report with lots of system/application errors  
IP address isn't saved (config tool)  
IP addresses aren't loaded (config tool)  
Fix last quartet in IP address contol (config tool)  
Make sure password gets saved when adding a windows machine (config tool)  

# 1.3 (Cody)

## Updates/new features:
Make +/- buttons smaller (config tool)  
Align logos in about page vertically as well as horizontally  
Zip the daily files  
Change the logos  
Include disk volume label on the device status page  
Create a link to COMMON on the desktop (installer)  
Change default device name (config tool)  
Label the 'global' frequency field (config tool)  
Delete device confirmation (config tool)  
Remove Add* controls at bottom, and put that functionality within the tabs (config tool)  
Change tab wording (config tool)  
Add version # to the daily file  
Disable browser caching  

## Bugs fixed:
Crash when canceling out of load (config tool)  
Missing status messages  
Have to delete devices twice (config tool)  
Update configuration saving (config tool)  
Fix crash when clicking the - button when nothing's selected (config tool)  
PDF reports aren't displaying properly  
Database timestamp/count is missing  
Fix IP address field (config tool)  
Fix timing issues when using Google Chrome  


# 1.3.1

## Bugs fixed:
Changed default log file location and severity  
Unable to edit ping interval (config tool)  


# 1.4 (Deadwood)

## Updates/new features:
Update daily-file name (#148)  
Add network up/down graph (#152)  
Keep alert history (#139)  
Report size of COMMON DB (#135)  
Ping and present MAC address (#138)  
Show application history (#132)  
Remove 'Lanes' tab (config tool) (#176)  
Gather "Source" for application errors (#173)  
Gather event ID for system and application errors (#141)  
Removed device list from main site menu (config tool) (#170)  
Import devices from CSV file (config tool) (#153)  
Enter configuration snapshot into the daily file (#147)  
Display UPS providing power alert (#142)  
Have installer create event log source (#175)  
Record processes memory usage (#178)  
Remove username/password from the server's config screen (#159)  
Different default collection rates (#169)  
Added ability to install COMMON at a location other than C:\COMMON (#193)  
Standardized date/time format (#200)  
Disabled JSON import (#187)  


## Bugs fixed:
Make sure specified devices always appear in the ping list (#94, #137)  
Device name in addition to IP address in daily file (#133)  
Importing a config file that was exported from another site doesn't work (config tool) (#151)  
Slow collection of large #s of errors from the event log (#164)  
Report PDFs were missing some sections (#172)  
Initial page load didn't show errors or device data (#168)  
Encrypt database connection string in DB (#111)  
There doesn't seem to be a place to change the language of either the config tool or the UI (#185)  
Adding new windows devices is different if you click add when the main server is hightlighted vs another device  (config tool) (#184)  
Inconsistent alerts -- alerts show on home page but are missing from device page (#179)  
Fixed installer problem that kept the common_config.json and common_log_config.json files from being created (#193)  
Fixed problem with Site Configuration Report (#197)  
Fixed problem where changing a device's name wouldn't always work (#199)  
Fixed problem with missing languages in ConfigTool (#191)  
Fixed problem with UPS estimated time remaining while it was charging (#196)  

# 1.5 (Eureka)

## Updates/new features:
Improve main display (#275)  
Collect SMART data (#181, #235)  
Add collection feedback showing collection success/failure and when it will next occur (#156, #165, #167)  
Add ability to trigger a collection immediately (#242)  
Eliminate IIS dependency (#237)  
Put current configuration in first day of month's daily file (#227)  
Reorder daily file JSON so version is at the top (#225)  
Add network history page (#205)  
Incorporate COMMON icon into config tool and UI (#188)  
Allow only one running instance of config tool (#208)  
Added ability to group related items together (#274)  
Added "informational" alerts (things of note, but nothing too bad) (#248)  
Provide the ability to choose which disk volumes to monitor, and provide ability to test WMI connection (#228)  
Device bricks should show "peaks" (peak CPU usage, peak memory usage, etc.) (#291)  
Make sure the country code is specified (#268)  
Limit the number of errors in the reports (#281)  
Use browser print capabilities instead of having a PDF download (#279)  
Added ability to import daily files into a database  
Added ability to host multiple sites on a single machine  

## Bugs fixed:
Put Site information on reports (#223)  
Labels ordered consistently (#209)  
Fixed CPU calculations (#161)  
Service hangs when no devices to ping (#266)  
Added note to config tool to indicate username is of format MACHINENAME\USERNAME (#160)  
Inconsistent memory-usage reporting (#271)  
Reloading site from other than main page failed to load everything (#168)  
Add alerts to Device report (#322)  
Fix ambiguous times (use 24-hour times) (#326)  
