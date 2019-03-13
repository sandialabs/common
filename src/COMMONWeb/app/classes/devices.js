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
define(["require", "exports", "./collectorinfo", "../enums/devicetypes.enum", "./autoupdater", "./group"], function (require, exports, collectorinfo_1, devicetypes_enum_1, autoupdater_1, group_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var EAlertLevel;
    (function (EAlertLevel) {
        EAlertLevel[EAlertLevel["Normal"] = 0] = "Normal";
        EAlertLevel[EAlertLevel["Alert"] = 1] = "Alert";
        EAlertLevel[EAlertLevel["Information"] = 2] = "Information";
    })(EAlertLevel = exports.EAlertLevel || (exports.EAlertLevel = {}));
    var DeviceStatus = /** @class */ (function () {
        function DeviceStatus(s) {
            this.status = s.status;
            this.alertLevel = s.alertLevel;
            this.message = s.message;
        }
        return DeviceStatus;
    }());
    var DriveInfo = /** @class */ (function () {
        function DriveInfo(d) {
            this.letter = d.letter;
            this.name = d.name;
            this.typeDescription = d.typeDescription;
            this.type = d.type;
        }
        return DriveInfo;
    }());
    exports.DriveInfo = DriveInfo;
    var FullDeviceStatus = /** @class */ (function () {
        function FullDeviceStatus(status) {
            var t = this;
            this.fullStatus = {};
            var keys = Object.keys(status.fullStatus);
            keys.forEach(function (key, index) {
                var ids = status.fullStatus[key];
                t.fullStatus[key] = [];
                for (var j = 0; j < ids.length; ++j) {
                    var ids2 = ids[j];
                    var ds = new DeviceStatus(ids2);
                    t.fullStatus[key].push(ds);
                }
            });
        }
        return FullDeviceStatus;
    }());
    var DeviceInfo = /** @class */ (function () {
        //public panelIsOpen: boolean;
        function DeviceInfo(iinfo) {
            this.name = iinfo.name;
            this.id = iinfo.id;
            this.type = iinfo.type;
            this.ipAddress = iinfo.ipAddress;
            this.statuses = [];
            this.hasStatus = false;
            this.alarms = [];
            this.networkStatus = null;
            this.groupID = iinfo.groupID;
            this.monitoredDrives = new MonitoredDriveManager(iinfo.monitoredDrives);
            //this.panelIsOpen = false;
            this.updateCollectors(iinfo.collectors);
            // We hide the drives that aren't being monitored. Only put drives into
            // the driveNames map if they're being monitored.
            this.driveNames = {};
            var t = this;
            Object.keys(iinfo.driveNames).forEach(function (key) {
                var driveLetter = key;
                var isMonitored = t.monitoredDrives.isDriveMonitored(driveLetter);
                if (isMonitored)
                    t.driveNames[driveLetter] = iinfo.driveNames[driveLetter];
            });
        }
        DeviceInfo.prototype.updateStatus = function (statuses) {
            this.statuses = [];
            this.alarms = [];
            for (var i = 0; i < statuses.length; ++i) {
                var status = statuses[i];
                if (status.alertLevel == EAlertLevel.Alert)
                    this.alarms.push(status);
                this.statuses.push(status);
            }
            this.hasStatus = statuses.length > 0;
            this.statuses.sort(function (a, b) {
                return a.status.localeCompare(b.status);
            });
            this.alarms.sort();
        };
        DeviceInfo.prototype.updateCollectors = function (collectors) {
            if (!collectors)
                return;
            this.collectors = [];
            this.collectorToInfo = {};
            for (var i = 0; i < collectors.length; ++i) {
                var collector = new collectorinfo_1.CollectorInfo(collectors[i]);
                this.collectors.push(collector);
                this.collectorToInfo[collector.collectorType] = collector;
            }
        };
        DeviceInfo.prototype.updateCollector = function (collector) {
            for (var i = 0; i < this.collectors.length; ++i) {
                var c = this.collectors[i];
                if (c && c.id == collector.id) {
                    var ci = new collectorinfo_1.CollectorInfo(collector);
                    this.collectors[i] = ci;
                    this.collectorToInfo[ci.collectorType] = ci;
                    break;
                }
            }
        };
        // Use string as the parameter type here because hasOwnProperty requires a string,
        // and this method is typically used from the html (i.e. ng-if="vm.device.isCollectorEnabled(0)")
        // so a string works good.
        DeviceInfo.prototype.isCollectorEnabled = function (type) {
            if (this.collectorToInfo.hasOwnProperty(type)) {
                var info = this.collectorToInfo[type];
                return info.isEnabled;
            }
            return false;
        };
        DeviceInfo.prototype.getCollector = function (collectorID) {
            var collector = null;
            for (var i = 0; collector === null && i < this.collectors.length; ++i) {
                if (this.collectors[i].id == collectorID)
                    collector = this.collectors[i];
            }
            return collector;
        };
        DeviceInfo.prototype.isWindowsDevice = function () {
            return this.type == devicetypes_enum_1.EDeviceTypes.Server || this.type == devicetypes_enum_1.EDeviceTypes.Workstation;
        };
        return DeviceInfo;
    }());
    exports.DeviceInfo = DeviceInfo;
    // Group lane-related things together. Keys off the RPM device.
    //class Lane {
    //    public name: string;
    //    public rpm: DeviceInfo;
    //    public devices: DeviceInfo[];
    //    constructor(name: string) {
    //        this.name = name;
    //        this.rpm = null;
    //        this.devices = [];
    //    }
    //    public hasAlarm(): boolean {
    //        var hasAlarm = this.rpm && this.rpm.alarms.length > 0;
    //        for (var i = 0; hasAlarm === false && i < this.devices.length; ++i) {
    //            var device = this.devices[i];
    //            hasAlarm = device.alarms.length > 0;
    //        }
    //        return hasAlarm;
    //    }
    //}
    // Used to group lane things together. Keys off the ' ' character separating the name of
    // things, like "Lane001 RPM", "Lane001 Camera1", "Lane001 Camera2", etc.
    //class Grouping {
    //    public grouping: string;
    //    public name: string;
    //    constructor(s: string) {
    //        let index = s.indexOf(' ');
    //        if (index >= 0) {
    //            this.grouping = s.substr(0, index);
    //            this.name = s.substr(index + 1);
    //        } else {
    //            this.grouping = '';
    //            this.name = s;
    //        }
    //    }
    //}
    // Keeps track of all the devices. Each device is in a DeviceInfo object, and each one of
    // those keeps track of each device's collectors.
    // If a device has been assigned to a Group, it will be kept in the appropriate Group
    // object. Otherwise, it will be kept in the devices list.
    // All devices, regardlesss of whether they're grouped or not, will be kept
    // in the allDevices struct, which maps the unique device ID to the DeviceInfo object.
    var DeviceManager = /** @class */ (function () {
        function DeviceManager(dataService, $interval) {
            this.devices = [];
            this.windowsDevices = [];
            this.allDevices = {};
            this.hasAlarms = this.hasStatus = false;
            this.groups = [];
            this.upNext = [];
            this.dataService = dataService;
            this.autoUpdater = new autoupdater_1.AutoUpdater(5000, DeviceManager.gatherData, this, $interval);
        }
        DeviceManager.prototype.setConfiguration = function (configuration) {
            if (!configuration)
                return;
            this.groups = [];
            if (configuration.groups) {
                for (var i = 0; i < configuration.groups.length; ++i) {
                    var g = new group_1.Group(configuration.groups[i]);
                    this.groups.push(g);
                }
                this.groups.sort(function (a, b) {
                    return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
                });
            }
            var devices = configuration.devices;
            // We don't want the 'System' device to be in the list of all devices/groups
            this.allDevices = {};
            for (var i = 0; i < devices.length; ++i) {
                var device = devices[i];
                this.allDevices[device.id] = device;
                if (device.type > devicetypes_enum_1.EDeviceTypes.Unknown && device.type !== devicetypes_enum_1.EDeviceTypes.System) {
                    var group = this.findGroup(device.groupID);
                    if (group)
                        group.addDevice(device);
                    else
                        this.devices.push(device);
                }
                if (device.type === devicetypes_enum_1.EDeviceTypes.System)
                    this.systemDevice = device;
                if (device.isWindowsDevice())
                    this.windowsDevices.push(device);
            }
            this.devices.sort(function (a, b) {
                return a.name.localeCompare(b.name);
            });
            this.windowsDevices.sort(function (a, b) {
                return a.name.localeCompare(b.name);
            });
            // Sort the devices within each group
            for (var i = 0; i < this.groups.length; ++i) {
                var g = this.groups[i];
                g.devices.sort(function (a, b) {
                    return a.name.localeCompare(b.name);
                });
            }
            this.updateUpNext();
        };
        DeviceManager.prototype.findDeviceFromID = function (id) {
            return this.allDevices[id];
        };
        DeviceManager.prototype.findDeviceFromName = function (name) {
            var t = this;
            var keys = Object.keys(t.allDevices);
            for (var i = 0; i < keys.length; ++i) {
                var d = t.allDevices[keys[i]];
                if (d.name === name)
                    return d;
            }
            return null;
        };
        DeviceManager.prototype.findGroup = function (id) {
            var g = null;
            if (!id || id < 0)
                return g;
            for (var i = 0; g === null && i < this.groups.length; ++i) {
                var group = this.groups[i];
                if (group.id === id)
                    g = group;
            }
            return g;
        };
        DeviceManager.prototype.updateStatus = function (ifds) {
            var t = this;
            var fds = new FullDeviceStatus(ifds);
            Object.keys(fds.fullStatus).forEach(function (key, _index) {
                var statuses = fds.fullStatus[key];
                var device = t.allDevices[Number(key)];
                if (device)
                    device.updateStatus(statuses);
            });
            this.updateStatusFlags();
            for (var i = 0; i < this.groups.length; ++i)
                this.groups[i].updateStatusFlags();
        };
        DeviceManager.prototype.updateCollectors = function (collectors) {
            if (!collectors)
                return;
            // A map of device ID to their set of collectors
            var test = {};
            for (var i = 0; i < collectors.length; ++i) {
                var collector = collectors[i];
                if (test.hasOwnProperty(collector.deviceID) === false)
                    test[collector.deviceID] = [];
                test[collector.deviceID].push(collector);
            }
            var t = this;
            for (var key in test) {
                var deviceID = Number(key);
                var cs = test[deviceID];
                var device = t.findDeviceFromID(deviceID);
                if (device)
                    device.updateCollectors(cs);
            }
            this.updateUpNext();
        };
        DeviceManager.prototype.updateUpNext = function () {
            // A list of all the collectors. Will be sorted into time 
            // order based on the next collection time.
            var allCollectors = [];
            for (var i = 0; i < this.devices.length; ++i) {
                var dev = this.devices[i];
                for (var j = 0; dev.collectors && j < dev.collectors.length; ++j) {
                    var collector = dev.collectors[j];
                    if (collector.isEnabled)
                        allCollectors.push(collector);
                }
            }
            allCollectors.sort(function (x, y) {
                if (!x.nextCollectionTime && !y.nextCollectionTime)
                    return 0;
                else if (!x.nextCollectionTime && y.nextCollectionTime)
                    return -1;
                else if (x.nextCollectionTime && !y.nextCollectionTime)
                    return 1;
                var a = x.nextCollectionTime.getTime();
                var b = y.nextCollectionTime.getTime();
                return a - b;
            });
            this.upNext = allCollectors;
        };
        DeviceManager.prototype.getDevicesForDataCollection = function () {
            var devices = [];
            var keys = Object.keys(this.allDevices);
            for (var i = 0; i < keys.length; ++i) {
                var d = this.allDevices[keys[i]];
                // We want the system device first in the array, so don't
                // add it here and we'll put in at the very end.
                if (d !== this.systemDevice && d.collectors && d.collectors.length > 0)
                    devices.push(d);
            }
            devices.sort(function (a, b) {
                return a.name.localeCompare(b.name);
            });
            devices.splice(0, 0, this.systemDevice);
            return devices;
        };
        DeviceManager.prototype.collectNow = function (collectorID) {
            var device = null;
            for (var i = 0; device == null && i < this.devices.length; ++i) {
                var d = this.devices[i];
                var c = d.getCollector(collectorID);
                if (c)
                    device = d;
            }
            if (!device) {
                // Look in the groups for the device
                for (var i = 0; device == null && i < this.groups.length; ++i) {
                    var g = this.groups[i];
                    device = g.findDeviceFromCollectorID(collectorID);
                }
            }
            if (!device) {
                // See if the System device has it
                var c = this.systemDevice.getCollector(collectorID);
                if (c)
                    device = this.systemDevice;
            }
            if (device) {
                this.dataService.collectNow(collectorID)
                    .then(function (data) { return device.updateCollector(data); });
            }
        };
        DeviceManager.prototype.collectAll = function (deviceID) {
            var device = this.findDeviceFromID(deviceID);
            if (device) {
                for (var i = 0; i < device.collectors.length; ++i) {
                    var collector = device.collectors[i];
                    this.dataService.collectNow(collector.id)
                        .then(function (data) {
                        device.updateCollector(data);
                    });
                }
            }
        };
        DeviceManager.prototype.updateStatusFlags = function () {
            // Have to do this because inside the forEach method 'this' doesn't
            // refer to the DeviceManager object.
            var t = this;
            this.hasAlarms = false;
            this.hasStatus = false;
            Object.keys(this.allDevices).forEach(function (key, index) {
                var d = t.allDevices[key];
                var hasAlarm = d.alarms.length > 0;
                t.hasAlarms = t.hasAlarms || hasAlarm;
                var keys = Object.keys(d.statuses);
                t.hasStatus = t.hasStatus || keys.length > 0;
            });
        };
        DeviceManager.prototype.updateNetwork = function (network) {
            for (var i = 0; i < network.data.length; ++i) {
                var n = network.data[i];
                if (n.deviceID >= 0) {
                    //console.log("updateNetwork: " + JSON.stringify(n));
                    var device = this.allDevices[n.deviceID];
                    if (device) {
                        //console.log("updateNetwork-A: " + JSON.stringify(device));
                        device.networkStatus = n;
                        //console.log("updateNetwork-B: " + JSON.stringify(device));
                    }
                }
            }
        };
        //public closeAllPanels() {
        //    for (var i = 0; i < this.devices.length; ++i) {
        //        this.devices[i].panelIsOpen = false;
        //    }
        //    for (var i = 0; i < this.groups.length; ++i) {
        //        this.groups[i].panelIsOpen = false;
        //    }
        //}
        DeviceManager.prototype.startAutomaticUpdate = function () {
            this.autoUpdater.start();
        };
        DeviceManager.prototype.stopAutomaticUpdate = function () {
            this.autoUpdater.stop();
        };
        DeviceManager.gatherData = function (t) {
            t.dataService.getDeviceStatus()
                .then(function (data) { return t.updateStatus(data); });
            t.dataService.getAllCollectors()
                .then(function (collectors) { return t.updateCollectors(collectors); });
        };
        return DeviceManager;
    }());
    exports.DeviceManager = DeviceManager;
    var MonitoredDrive = /** @class */ (function (_super) {
        __extends(MonitoredDrive, _super);
        function MonitoredDrive(d) {
            var _this = _super.call(this, d) || this;
            _this.isMonitored = d.isMonitored;
            return _this;
        }
        return MonitoredDrive;
    }(DriveInfo));
    var MonitoredDriveManager = /** @class */ (function () {
        function MonitoredDriveManager(manager) {
            this.driveMap = {};
            var t = this;
            Object.keys(manager.driveMap).forEach(function (key) {
                t.driveMap[key] = new MonitoredDrive(manager.driveMap[key]);
            });
        }
        MonitoredDriveManager.prototype.isDriveMonitored = function (driveLetter) {
            var isMonitored = false;
            var d = this.driveMap[driveLetter];
            if (d)
                isMonitored = d.isMonitored;
            return isMonitored;
        };
        return MonitoredDriveManager;
    }());
    exports.MonitoredDriveManager = MonitoredDriveManager;
});
//# sourceMappingURL=devices.js.map