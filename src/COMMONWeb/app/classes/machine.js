define(["require", "exports", "./memory", "../disk/disk", "./nic", "./errors", "./cpu", "./processes", "./database", "./applications", "./services", "./ups"], function (require, exports, memory_1, disk_1, nic_1, errors_1, cpu_1, processes_1, database_1, applications_1, services_1, ups_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DeviceDetails = /** @class */ (function () {
        function DeviceDetails(data) {
            this.uptime = data.uptime;
            var uptimeRegex = /(\d+) (\d\d:\d\d:\d\d)/;
            var match = uptimeRegex.exec(data.uptime);
            if (match !== null) {
                this.uptime = match[1] + " days, " + match[2];
            }
            this.lastBootTime = null;
            if (data.lastBootTime && data.lastBootTime !== "")
                this.lastBootTime = new Date(data.lastBootTime);
        }
        return DeviceDetails;
    }());
    var EMachineParts;
    (function (EMachineParts) {
        EMachineParts[EMachineParts["CPU"] = 0] = "CPU";
        EMachineParts[EMachineParts["Memory"] = 1] = "Memory";
        EMachineParts[EMachineParts["DiskUsage"] = 2] = "DiskUsage";
        EMachineParts[EMachineParts["DiskPerformance"] = 3] = "DiskPerformance";
        EMachineParts[EMachineParts["NIC"] = 4] = "NIC";
        EMachineParts[EMachineParts["SystemErrors"] = 5] = "SystemErrors";
        EMachineParts[EMachineParts["ApplicationErrors"] = 6] = "ApplicationErrors";
        EMachineParts[EMachineParts["Processes"] = 7] = "Processes";
        EMachineParts[EMachineParts["Database"] = 8] = "Database";
        EMachineParts[EMachineParts["Applications"] = 9] = "Applications";
        EMachineParts[EMachineParts["Services"] = 10] = "Services";
        EMachineParts[EMachineParts["UPS"] = 11] = "UPS";
        //Network,
        //SMART,
        EMachineParts[EMachineParts["NumMachineParts"] = 12] = "NumMachineParts";
    })(EMachineParts = exports.EMachineParts || (exports.EMachineParts = {}));
    var Machine = /** @class */ (function () {
        /// Let's have a way to retrieve a subset of all things that we can retrieve.
        /// The third parameter will specify the parts to retrieve. If it's null, retrieve
        /// everything. If it's non-null, retrieve just those specified parts.
        function Machine(devInfo, dataService, startTime, partsToRetrieve) {
            var _this = this;
            this.devInfo = devInfo;
            this.dataService = dataService;
            this.getDeviceDetails = function () {
                var t = _this;
                _this.dataService.getDeviceDetails(_this.devInfo.id)
                    .then(function (data) {
                    if (data)
                        t.details = new DeviceDetails(data);
                });
            };
            this.getCPUData = function () {
                var startDate = _this.getStartDate(EMachineParts.CPU);
                if (!startDate)
                    return;
                _this.retrieveData(_this.devInfo.id, [EMachineParts.CPU], startDate, null);
            };
            this.getMemoryData = function () {
                var startDate = _this.getStartDate(EMachineParts.Memory);
                if (!startDate)
                    return;
                _this.retrieveData(_this.devInfo.id, [EMachineParts.Memory], startDate, null);
            };
            this.getDiskUsageData = function () {
                var startDate = _this.getStartDate(EMachineParts.DiskUsage);
                if (!startDate)
                    return;
                _this.retrieveData(_this.devInfo.id, [EMachineParts.DiskUsage], startDate, null);
            };
            this.onDiskUsageSelection = function (diskName) {
                if (_this.diskUsage !== null)
                    _this.diskUsage.selectDisk(diskName);
            };
            this.getDiskPerformanceData = function () {
                var startDate = _this.getStartDate(EMachineParts.DiskPerformance);
                if (!startDate)
                    return;
                _this.retrieveData(_this.devInfo.id, [EMachineParts.DiskPerformance], startDate, null);
            };
            this.onDiskPerformanceSelection = function (diskName) {
                if (_this.diskPerformance !== null)
                    _this.diskPerformance.selectDisk(diskName);
            };
            this.getNICData = function () {
                var startDate = _this.getStartDate(EMachineParts.NIC);
                if (!startDate)
                    return;
                _this.retrieveData(_this.devInfo.id, [EMachineParts.NIC], startDate, null);
            };
            this.getSystemErrors = function () {
                var startDate = _this.getStartDate(EMachineParts.SystemErrors);
                if (!startDate)
                    return;
                _this.retrieveData(_this.devInfo.id, [EMachineParts.SystemErrors], startDate, null);
            };
            this.getApplicationErrors = function () {
                var startDate = _this.getStartDate(EMachineParts.ApplicationErrors);
                if (!startDate)
                    return;
                _this.retrieveData(_this.devInfo.id, [EMachineParts.ApplicationErrors], startDate, null);
            };
            this.getProcesses = function () {
                _this.retrieveData(_this.devInfo.id, [EMachineParts.Processes], null, null);
            };
            this.getDatabases = function () {
                _this.retrieveData(_this.devInfo.id, [EMachineParts.Database], null, null);
            };
            this.getApplications = function () {
                _this.retrieveData(_this.devInfo.id, [EMachineParts.Applications], null, null);
            };
            this.getServices = function () {
                _this.retrieveData(_this.devInfo.id, [EMachineParts.Services], null, null);
            };
            this.getUPS = function () {
                var startDate = _this.getStartDate(EMachineParts.UPS);
                if (!startDate)
                    return;
                _this.retrieveData(_this.devInfo.id, [EMachineParts.UPS], null, null);
            };
            this.onProcessSelection = function (process) {
                if (_this.processes)
                    _this.processes.onSelectProcess(process);
            };
            this.name = devInfo.name;
            this.cpu = null;
            this.memory = null;
            this.diskUsage = null;
            this.diskPerformance = null;
            this.nic = null;
            this.systemErrors = this.applicationErrors = null;
            this.processes = null;
            this.database = null;
            this.applications = null;
            this.allParts = new Array(EMachineParts.NumMachineParts);
            this.allMethods = {};
            this.allMethods[EMachineParts.CPU] = this.getCPUData;
            this.allMethods[EMachineParts.Memory] = this.getMemoryData;
            this.allMethods[EMachineParts.DiskUsage] = this.getDiskUsageData;
            this.allMethods[EMachineParts.DiskPerformance] = this.getDiskPerformanceData;
            this.allMethods[EMachineParts.NIC] = this.getNICData;
            this.allMethods[EMachineParts.SystemErrors] = this.getSystemErrors;
            this.allMethods[EMachineParts.ApplicationErrors] = this.getApplicationErrors;
            this.allMethods[EMachineParts.Processes] = this.getProcesses;
            this.allMethods[EMachineParts.Database] = this.getDatabases;
            this.allMethods[EMachineParts.Applications] = this.getApplications;
            this.allMethods[EMachineParts.Services] = this.getServices;
            this.allMethods[EMachineParts.UPS] = this.getUPS;
            //this.allMethods[MachineParts.SMART] = this.getSMART;
            this.loading = {};
            // Another way of retrieving all the things in an Enum
            // https://stackoverflow.com/a/18112157/706747
            for (var e in EMachineParts) {
                var isNum = parseInt(e, 10) >= 0;
                if (isNum)
                    this.loading[e] = false;
            }
            if (!partsToRetrieve) {
                // Retrieve everything. But let's automatically populate the toRetrieve array
                // with everything in MachineParts, except for the NumMachineParts value
                //
                // Here's how to get all the values from MachineParts: http://stackoverflow.com/a/21294925
                var objValues = Object.keys(EMachineParts).map(function (k) { return EMachineParts[k]; });
                var value = objValues.filter(function (v) { return typeof v === "number"; });
                partsToRetrieve = value;
            }
            this.toRetrieve = partsToRetrieve;
            // Make sure we don't retrieve the last value from the enum, which is just used as a count
            var index = this.toRetrieve.indexOf(EMachineParts.NumMachineParts);
            if (index >= 0)
                this.toRetrieve.splice(index, 1);
            if (partsToRetrieve)
                this.batchGetEverythingFrom(startTime);
            else
                this.getEverythingFrom(startTime);
            //this.getEverything();
        }
        Machine.prototype.getEverythingFrom = function (starting) {
            for (var i = 0; i < this.allParts.length; ++i)
                this.allParts[i] = starting;
            this.getEverything();
        };
        Machine.prototype.getEverything = function () {
            this.getDeviceDetails();
            for (var i = 0; i < this.toRetrieve.length; ++i) {
                var part = this.toRetrieve[i];
                var method = this.allMethods[part];
                if (method) {
                    this.loading[part] = true;
                    method();
                }
            }
        };
        Machine.prototype.batchGetEverythingFrom = function (starting) {
            for (var i = 0; i < this.allParts.length; ++i)
                this.allParts[i] = starting;
            this.batchGetEverything(starting);
        };
        Machine.prototype.batchGetEverything = function (starting) {
            this.getDeviceDetails();
            var t = this;
            for (var i = 0; i < this.toRetrieve.length; ++i) {
                var e = this.toRetrieve[i];
                this.loading[e] = true;
            }
            this.dataService.getMachineData(this.devInfo.id, this.toRetrieve, starting)
                .then(function (data) {
                t.onReceivedData(data);
            });
        };
        Machine.prototype.isToRetrieve = function (part) {
            return this.toRetrieve.indexOf(part) >= 0;
        };
        Machine.prototype.getMoreDays = function (days, part) {
            var old_date = this.allParts[part];
            this.allParts[part] = new Date(old_date.getTime() - (days * 24 * 60 * 60 * 1000));
            if (this.isToRetrieve(part)) {
                var method = this.allMethods[part];
                if (method) {
                    this.loading[part] = true;
                    method();
                }
            }
        };
        Machine.prototype.retrieveData = function (id, parts, start, end) {
            var t = this;
            this.dataService.getMachineData(this.devInfo.id, parts, start, end)
                .then(function (data) {
                t.onReceivedData(data);
            });
        };
        Machine.prototype.onReceivedData = function (data) {
            if (!data)
                return;
            if (data.cpu) {
                if (!this.cpu)
                    this.cpu = new cpu_1.CPUData();
                this.cpu.update(data.cpu);
                this.loading[EMachineParts.CPU] = false;
            }
            if (data.memory) {
                if (!this.memory)
                    this.memory = new memory_1.Memory();
                this.memory.update(data.memory);
                this.loading[EMachineParts.Memory] = false;
            }
            if (data.diskUsage) {
                if (!this.diskUsage)
                    this.diskUsage = new disk_1.DiskUsageManager(this.devInfo);
                this.diskUsage.update(data.diskUsage);
                this.loading[EMachineParts.DiskUsage] = false;
            }
            if (data.diskPerformance) {
                if (!this.diskPerformance)
                    this.diskPerformance = new disk_1.DiskPerformanceManager(this.devInfo);
                this.diskPerformance.update(data.diskPerformance);
                this.loading[EMachineParts.DiskPerformance] = false;
            }
            if (data.nic) {
                if (!this.nic)
                    this.nic = new nic_1.NICData();
                this.nic.update(data.nic);
                this.loading[EMachineParts.NIC] = false;
            }
            if (data.systemErrors) {
                this.systemErrors = new errors_1.ErrorManager(data.systemErrors);
                this.loading[EMachineParts.SystemErrors] = false;
            }
            if (data.applicationErrors) {
                this.applicationErrors = new errors_1.ErrorManager(data.applicationErrors);
                this.loading[EMachineParts.ApplicationErrors] = false;
            }
            if (data.processes) {
                this.processes = new processes_1.ProcessManager(data.processes, this.dataService);
                this.loading[EMachineParts.Processes] = false;
            }
            if (data.database) {
                this.database = new database_1.DatabaseManager(data.database, this.dataService);
                this.loading[EMachineParts.Database] = false;
            }
            if (data.applications) {
                this.applications = new applications_1.ApplicationManager(data.applications, this.dataService);
                this.loading[EMachineParts.Applications] = false;
            }
            if (data.services) {
                this.services = new services_1.Services(data.services);
                this.loading[EMachineParts.Services] = false;
            }
            if (data.ups) {
                this.ups = new ups_1.UPSStatus(data.ups);
                this.loading[EMachineParts.UPS] = false;
            }
        };
        Machine.prototype.getStartDate = function (part) {
            var startDate = null;
            if (this.allParts[part])
                startDate = this.allParts[part];
            return startDate;
        };
        return Machine;
    }());
    exports.Machine = Machine;
});
//# sourceMappingURL=machine.js.map