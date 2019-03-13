var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "./smart", "../charts/chartjs", "../charts/chartbridge"], function (require, exports, smart_1, chartjs_1, chartbridge_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DiskUsageData = /** @class */ (function () {
        function DiskUsageData(data) {
            this.dataID = data.dataID;
            this.collectorID = data.collectorID;
            this.timeStamp = new Date(data.timeStamp);
            this.capacity = data.capacity;
            this.free = data.free;
            this.used = data.used;
        }
        return DiskUsageData;
    }());
    exports.DiskUsageData = DiskUsageData;
    var DiskUsageWithSmartData = /** @class */ (function () {
        function DiskUsageWithSmartData(du) {
            this.driveLetter = du.driveLetter;
            this.diskUsage = [];
            for (var i = 0; i < du.diskUsage.length; ++i)
                this.diskUsage.push(new DiskUsageData(du.diskUsage[i]));
            this.diskUsage.sort(function (a, b) {
                return a.timeStamp.getTime() - b.timeStamp.getTime();
            });
            this.smartData = new smart_1.HardDisk(du.smartData);
        }
        return DiskUsageWithSmartData;
    }());
    exports.DiskUsageWithSmartData = DiskUsageWithSmartData;
    var DiskUsageSnapshot = /** @class */ (function () {
        function DiskUsageSnapshot(data) {
            this.timestamp = new Date(data.timeStamp);
            this.capacity = (data.capacity / DiskUsageSnapshot.byteSize);
            this.free = (data.free / DiskUsageSnapshot.byteSize);
            this.used = (data.used / DiskUsageSnapshot.byteSize);
            this.percentUsed = this.used / this.capacity * 100;
        }
        DiskUsageSnapshot.byteSize = 0x40000000; // 1GB
        return DiskUsageSnapshot;
    }());
    exports.DiskUsageSnapshot = DiskUsageSnapshot;
    var DiskUsage = /** @class */ (function () {
        function DiskUsage(data) {
            this.driveLetter = data.driveLetter;
            this.isActive = false;
            this.update(data);
        }
        DiskUsage.prototype.update = function (data) {
            this.diskData = [];
            this.current = this.peak = null;
            this.smart = data.smartData ? new smart_1.HardDisk(data.smartData) : null;
            if (!data || !data.diskUsage || data.diskUsage.length === 0)
                return;
            for (var i = 0; i < data.diskUsage.length; ++i) {
                var snapshot = new DiskUsageSnapshot(data.diskUsage[i]);
                if (isNaN(snapshot.capacity) === false) {
                    this.diskData.push(snapshot);
                    if (this.peak === null || snapshot.used > this.peak.used)
                        this.peak = snapshot;
                }
            }
            this.diskData.sort(function (a, b) {
                return a.timestamp.getTime() - b.timestamp.getTime();
            });
            this.current = this.diskData[this.diskData.length - 1];
        };
        return DiskUsage;
    }());
    exports.DiskUsage = DiskUsage;
    var DiskUsageChartBridge = /** @class */ (function (_super) {
        __extends(DiskUsageChartBridge, _super);
        function DiskUsageChartBridge(diskUsageDataSource, settings, factory) {
            var _this = _super.call(this, diskUsageDataSource, factory) || this;
            _this.diskUsageDataSource = diskUsageDataSource;
            _this.settings = settings;
            _this.factory = factory;
            _this.settings.valueRange = [0, diskUsageDataSource.current.capacity];
            return _this;
        }
        DiskUsageChartBridge.prototype.watchCollection = function () {
            return this.diskUsageDataSource.diskData;
        };
        DiskUsageChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.diskUsageDataSource.diskData)
                return;
            for (var i = 0; i < this.diskUsageDataSource.diskData.length; ++i) {
                var c = this.diskUsageDataSource.diskData[i];
                this.addData(DiskUsageChartBridge.convert(c));
            }
        };
        DiskUsageChartBridge.convert = function (d) {
            return [new chartjs_1.ChartJSDataPoint({ x: d.timestamp, y: d.used })];
        };
        return DiskUsageChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.DiskUsageChartBridge = DiskUsageChartBridge;
    var DiskUsageManager = /** @class */ (function () {
        function DiskUsageManager(devInfo) {
            this.devInfo = devInfo;
            this.selectedDriveLetter = "";
            this.selectedDisk = null;
            this.disks = [];
            this.diskMap = {};
            this.type = "GB";
            this.driveLetters = [];
        }
        DiskUsageManager.prototype.update = function (data) {
            var _this = this;
            var t = this;
            var diskNames = [];
            var names = Object.keys(data);
            names.forEach(function (name) {
                var isMonitored = _this.devInfo.monitoredDrives.isDriveMonitored(name);
                if (isMonitored)
                    diskNames.push(name);
            });
            if (this.driveLetters.length != diskNames.length) {
                this.driveLetters = diskNames;
                this.driveLetters.sort();
            }
            this.driveLetters.forEach(function (driveLetter) {
                var existingDisk = null;
                for (var i = 0; existingDisk === null && i < t.disks.length; ++i) {
                    var d = t.disks[i];
                    if (d.driveLetter === driveLetter)
                        existingDisk = d;
                }
                if (existingDisk)
                    existingDisk.update(data[driveLetter]);
                else {
                    var disk = new DiskUsage(data[driveLetter]);
                    t.disks.push(disk);
                    t.diskMap[driveLetter] = disk;
                }
            });
            if (this.driveLetters.length > 0 && this.selectedDisk === null)
                this.selectDisk(this.driveLetters[0]);
        };
        DiskUsageManager.prototype.selectDisk = function (disk) {
            this.selectedDriveLetter = disk;
            this.selectedDisk = this.diskMap[disk];
            for (var i = 0; i < this.disks.length; ++i)
                this.disks[i].isActive = this.disks[i].driveLetter === disk;
        };
        return DiskUsageManager;
    }());
    exports.DiskUsageManager = DiskUsageManager;
    // Disk performance (how much time it spends queuing data)
    var DiskPerformanceSnapshot = /** @class */ (function () {
        function DiskPerformanceSnapshot(data) {
            var current = JSON.parse(data.value)['Value'];
            this.timestamp = new Date(data.timeStamp);
            this.percentTime = Number(current['Disk Time %']);
            this.avgDiskQLength = Number(current['Avg Disk Q Length']);
        }
        return DiskPerformanceSnapshot;
    }());
    exports.DiskPerformanceSnapshot = DiskPerformanceSnapshot;
    var DiskPerformance = /** @class */ (function () {
        function DiskPerformance(data, driveLetter) {
            this.driveLetter = driveLetter;
            this.isActive = false;
            this.update(data);
        }
        DiskPerformance.prototype.update = function (data) {
            this.diskData = [];
            this.current = this.peak = null;
            if (!data || data.length === 0)
                return;
            for (var i = 0; i < data.length; ++i) {
                var snapshot = new DiskPerformanceSnapshot(data[i]);
                this.diskData.push(snapshot);
                if (!this.peak || snapshot.avgDiskQLength > this.peak.avgDiskQLength)
                    this.peak = snapshot;
            }
            this.diskData.sort(function (a, b) {
                return a.timestamp.getTime() - b.timestamp.getTime();
            });
            this.current = this.diskData[this.diskData.length - 1];
        };
        return DiskPerformance;
    }());
    exports.DiskPerformance = DiskPerformance;
    var DiskPerformanceChartBridge = /** @class */ (function (_super) {
        __extends(DiskPerformanceChartBridge, _super);
        function DiskPerformanceChartBridge(diskPerformanceDataSource, factory) {
            var _this = _super.call(this, diskPerformanceDataSource, factory) || this;
            _this.diskPerformanceDataSource = diskPerformanceDataSource;
            _this.factory = factory;
            return _this;
        }
        DiskPerformanceChartBridge.prototype.watchCollection = function () {
            return this.diskPerformanceDataSource.diskData;
        };
        DiskPerformanceChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.diskPerformanceDataSource.diskData)
                return;
            for (var i = 0; i < this.diskPerformanceDataSource.diskData.length; ++i) {
                var c = this.diskPerformanceDataSource.diskData[i];
                this.addData(DiskPerformanceChartBridge.convert(c));
            }
        };
        DiskPerformanceChartBridge.convert = function (dps) {
            return [new chartjs_1.ChartJSDataPoint({ x: dps.timestamp, y: dps.percentTime })];
        };
        return DiskPerformanceChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.DiskPerformanceChartBridge = DiskPerformanceChartBridge;
    var DiskPerformanceManager = /** @class */ (function () {
        function DiskPerformanceManager(devInfo) {
            this.devInfo = devInfo;
            this.selectedDriveLetter = "";
            this.selectedDisk = null;
            this.disks = [];
            this.diskMap = {};
            this.type = "GB";
            this.driveLetters = [];
        }
        DiskPerformanceManager.prototype.update = function (data) {
            var _this = this;
            var t = this;
            this.driveLetters = [];
            var names = Object.keys(data);
            names.forEach(function (name) {
                var isMonitored = _this.devInfo.monitoredDrives.isDriveMonitored(name);
                if (isMonitored)
                    t.driveLetters.push(name);
            });
            this.driveLetters.sort();
            this.driveLetters.forEach(function (diskName) {
                var existingDisk = null;
                for (var i = 0; existingDisk === null && i < t.disks.length; ++i) {
                    var d = t.disks[i];
                    if (d.driveLetter === diskName)
                        existingDisk = d;
                }
                if (existingDisk)
                    existingDisk.update(data[diskName]);
                else {
                    var allData = data[diskName];
                    var disk = new DiskPerformance(allData, diskName);
                    t.disks.push(disk);
                    t.diskMap[diskName] = disk;
                }
            });
            if (this.driveLetters.length > 0 && this.selectedDisk === null)
                this.selectDisk(this.driveLetters[0]);
        };
        DiskPerformanceManager.prototype.selectDisk = function (disk) {
            this.selectedDriveLetter = disk;
            this.selectedDisk = this.diskMap[disk];
            for (var i = 0; i < this.disks.length; ++i)
                this.disks[i].isActive = this.disks[i].driveLetter === disk;
        };
        return DiskPerformanceManager;
    }());
    exports.DiskPerformanceManager = DiskPerformanceManager;
});
//# sourceMappingURL=disk.js.map