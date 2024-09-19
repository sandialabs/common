import { NetworkStatus, Network } from './network';
import { CollectorInfo, ICollectorInfo } from './collectorinfo';
import { EDeviceTypes } from '../enums/devicetypes.enum';
import { IAutoUpdatable, AutoUpdater } from './autoupdater';
import { EDriveTypes } from '../enums/drivetypes.enum';
import { DataService } from '../services/data.service';
import { ISystemConfiguration, SystemConfiguration } from './systemconfiguration';
import { Group } from './group';

export enum EAlertLevel {
    Normal,
    Alert,
    Information,
}

interface IDeviceStatus {
    status: string;
    alertLevel: number;
    message: string;
}

class DeviceStatus {
    public status: string;
    public alertLevel: EAlertLevel;
    public message: string;

    constructor(s: IDeviceStatus) {
        this.status = s.status;
        this.alertLevel = s.alertLevel;
        this.message = s.message;
    }
}

interface ICollectorTypeToCollectorInfo {
    [collectorType: string]: CollectorInfo;
}

interface IDriveNames {
    [driveLetter: string]: string;
}

export interface IDriveInfo {
    letter: string;
    name: string;
    typeDescription: string
    type: number;
}

export class DriveInfo {
    letter: string;
    name: string;
    typeDescription: string
    type: EDriveTypes;

    constructor(d: IDriveInfo) {
        this.letter = d.letter;
        this.name = d.name;
        this.typeDescription = d.typeDescription;
        this.type = d.type;
    }
}

export interface IDeviceInfo {
    id: number;
    name: string;
    type: EDeviceTypes;
    ipAddress: string;
    username: string;
    password: string;
    deleted: boolean;
    collectors: ICollectorInfo[];
    driveNames: IDriveNames;
    monitoredDrives: IMonitoredDriveManager;
    groupID: number;
}

interface IStatusMap {
    [deviceID: number]: IDeviceStatus[];
}

export interface IFullDeviceStatus {
    fullStatus: IStatusMap;
}

export interface IFullStatusMap {
    [deviceID: number]: DeviceStatus[];
}

class FullDeviceStatus {
    fullStatus: IFullStatusMap;

    constructor(status: IFullDeviceStatus) {
        var t = this;
        this.fullStatus = {};
        var keys = Object.keys(status.fullStatus);
        keys.forEach(function (key, index) {
            var ids: IDeviceStatus[] = status.fullStatus[key];
            t.fullStatus[key] = [];
            for (var j = 0; j < ids.length; ++j) {
                var ids2 = ids[j];
                var ds = new DeviceStatus(ids2);
                t.fullStatus[key].push(ds);
            }
        });
    }
}

export class DeviceInfo {
    public name: string;
    public id: number;
    public type: EDeviceTypes;
    public ipAddress: string;
    public statuses: DeviceStatus[];
    public hasStatus: boolean;
    public alarms: DeviceStatus[];
    public networkStatus: NetworkStatus;
    public collectors: CollectorInfo[];
    public driveNames: IDriveNames;
    public groupID: number;
    public collectorToInfo: ICollectorTypeToCollectorInfo;
    public monitoredDrives: MonitoredDriveManager;
    //public panelIsOpen: boolean;

    constructor(iinfo: IDeviceInfo) {
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
        let t = this;
        Object.keys(iinfo.driveNames).forEach(key => {
            let driveLetter = key;
            let isMonitored = t.monitoredDrives.isDriveMonitored(driveLetter);
            if (isMonitored)
                t.driveNames[driveLetter] = iinfo.driveNames[driveLetter];
        });
    }

    public updateStatus(statuses: DeviceStatus[]) {
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
    }

    public updateCollectors(collectors: ICollectorInfo[]) {
        if (!collectors)
            return;

        this.collectors = [];
        this.collectorToInfo = {};
        for (var i = 0; i < collectors.length; ++i) {
            let collector = new CollectorInfo(collectors[i]);
            this.collectors.push(collector);
            this.collectorToInfo[collector.collectorType] = collector;
        }
    }

    public updateCollector(collector: ICollectorInfo) {
        for (var i = 0; i < this.collectors.length; ++i) {
            let c: CollectorInfo = this.collectors[i];
            if (c && c.id == collector.id) {
                let ci: CollectorInfo = new CollectorInfo(collector);
                this.collectors[i] = ci;
                this.collectorToInfo[ci.collectorType] = ci;
                break;
            }
        }
    }

    // Use string as the parameter type here because hasOwnProperty requires a string,
    // and this method is typically used from the html (i.e. ng-if="vm.device.isCollectorEnabled(0)")
    // so a string works good.
    public isCollectorEnabled(type: string): boolean {
        if (this.collectorToInfo.hasOwnProperty(type)) {
            var info: CollectorInfo = this.collectorToInfo[type];
            return info.isEnabled;
        }

        return false;
    }

    public getCollector(collectorID: number): CollectorInfo {
        let collector: CollectorInfo = null;
        for (let i: number = 0; collector === null && i < this.collectors.length; ++i) {
            if (this.collectors[i].id == collectorID)
                collector = this.collectors[i];
        }
        return collector;
    }

    public isWindowsDevice(): boolean {
        return this.type == EDeviceTypes.Server || this.type == EDeviceTypes.Workstation;
    }
}

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
export class DeviceManager implements IAutoUpdatable<DeviceManager> {
    public devices: DeviceInfo[];
    public windowsDevices: DeviceInfo[];
    public groups: Group[];
    public systemDevice: DeviceInfo;
    private allDevices: { [id: number]: DeviceInfo };

    // The collectors that will be collected next, in order of when they will
    // be collected next.
    public upNext: CollectorInfo[];

    // Used so we can indicate on the home page that there are alerts or not
    public hasAlarms: boolean;
    public hasStatus: boolean;

    public autoUpdater: AutoUpdater<DeviceManager>;
    private dataService: DataService;

    constructor(dataService: DataService, $interval: ng.IIntervalService) {
        this.devices = [];
        this.windowsDevices = [];
        this.allDevices = {};
        this.hasAlarms = this.hasStatus = false;
        this.groups = [];
        this.upNext = [];

        this.dataService = dataService;
        this.autoUpdater = new AutoUpdater(5000, DeviceManager.gatherData, this, $interval);
    }

    public setConfiguration(configuration: SystemConfiguration) {
        if (!configuration)
            return;

        this.groups = [];

        if (configuration.groups) {
            for (var i = 0; i < configuration.groups.length; ++i) {
                let g = new Group(configuration.groups[i]);
                this.groups.push(g);
            }
            this.groups.sort((a, b) => {
                return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
            });
        }

        var devices = configuration.devices;

        // We don't want the 'System' device to be in the list of all devices/groups

        this.allDevices = {};

        for (var i = 0; i < devices.length; ++i) {
            let device: DeviceInfo = devices[i];
            this.allDevices[device.id] = device;

            if (device.type > EDeviceTypes.Unknown && device.type !== EDeviceTypes.System) {
                let group: Group = this.findGroup(device.groupID);
                if (group)
                    group.addDevice(device);
                else
                    this.devices.push(device);
            }

            if (device.type === EDeviceTypes.System)
                this.systemDevice = device;
            if (device.isWindowsDevice())
                this.windowsDevices.push(device);
        }

        this.devices.sort((a: DeviceInfo, b: DeviceInfo) => {
            return a.name.localeCompare(b.name);
        });
        this.windowsDevices.sort((a: DeviceInfo, b: DeviceInfo) => {
            return a.name.localeCompare(b.name);
        });

        // Sort the devices within each group
        for (var i = 0; i < this.groups.length; ++i) {
            let g = this.groups[i];
            g.devices.sort((a: DeviceInfo, b: DeviceInfo) => {
                return a.name.localeCompare(b.name);
            })
        }

        this.updateUpNext();
    }

    public findDeviceFromID(id: number): DeviceInfo {
        return this.allDevices[id];
    }

    public findDeviceFromName(name: string): DeviceInfo {
        let t = this;
        let keys = Object.keys(t.allDevices);
        for (let i = 0; i < keys.length; ++i) {
            let d: DeviceInfo = t.allDevices[keys[i]];
            if (d.name === name)
                return d;
        }

        return null;
    }

    public findGroup(id: number): Group {
        let g: Group = null;

        if (!id || id < 0)
            return g;

        for (var i = 0; g === null && i < this.groups.length; ++i) {
            let group = this.groups[i];
            if (group.id === id)
                g = group;
        }
        return g;
    }

    public updateStatus(ifds: IFullDeviceStatus) {
        var t = this;
        var fds = new FullDeviceStatus(ifds);
        Object.keys(fds.fullStatus).forEach((key: string, _index) => {
            let statuses = fds.fullStatus[key];
            let device = t.allDevices[Number(key)];
            if (device)
                device.updateStatus(statuses);
        });
        this.updateStatusFlags();

        for (var i = 0; i < this.groups.length; ++i)
            this.groups[i].updateStatusFlags();
    }

    public updateCollectors(collectors: ICollectorInfo[]) {
        if (!collectors)
            return;

        // A map of device ID to their set of collectors
        let test = {};

        for (let i = 0; i < collectors.length; ++i) {
            let collector = collectors[i];
            if (test.hasOwnProperty(collector.deviceID) === false)
                test[collector.deviceID] = [];
            test[collector.deviceID].push(collector);
        }

        let t = this;
        for (var key in test) {
            let deviceID: number = Number(key);
            let cs: ICollectorInfo[] = test[deviceID];
            let device: DeviceInfo = t.findDeviceFromID(deviceID);
            if (device)
                device.updateCollectors(cs);
        }

        this.updateUpNext();
    }

    updateUpNext() {
        // A list of all the collectors. Will be sorted into time 
        // order based on the next collection time.
        let allCollectors: CollectorInfo[] = [];
        for (var i = 0; i < this.devices.length; ++i) {
            let dev: DeviceInfo = this.devices[i];
            for (var j = 0; dev.collectors && j < dev.collectors.length; ++j) {
                let collector: CollectorInfo = dev.collectors[j];
                if (collector.isEnabled)
                    allCollectors.push(collector);
            }
        }

        allCollectors.sort((x: CollectorInfo, y: CollectorInfo) => {
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
    }

    public getDevicesForDataCollection(): DeviceInfo[] {
        let devices: DeviceInfo[] = [];
        let keys = Object.keys(this.allDevices);
        for (let i = 0; i < keys.length; ++i) {
            let d: DeviceInfo = this.allDevices[keys[i]];

            // We want the system device first in the array, so don't
            // add it here and we'll put in at the very end.
            if (d !== this.systemDevice && d.collectors && d.collectors.length > 0)
                devices.push(d);
        }

        devices.sort((a: DeviceInfo, b: DeviceInfo) => {
            return a.name.localeCompare(b.name);
        });

        devices.splice(0, 0, this.systemDevice);

        return devices;
    }

    public collectNow(collectorID: number): void {
        let device: DeviceInfo = null;
        for (var i = 0; device == null && i < this.devices.length; ++i) {
            let d = this.devices[i];
            let c = d.getCollector(collectorID);
            if (c)
                device = d;
        }
        if (!device) {
            // Look in the groups for the device
            for (var i = 0; device == null && i < this.groups.length; ++i) {
                let g = this.groups[i];
                device = g.findDeviceFromCollectorID(collectorID);
            }
        }
        if (!device) {
            // See if the System device has it
            let c = this.systemDevice.getCollector(collectorID);
            if (c)
                device = this.systemDevice;
        }
        if (device) {
            this.dataService.collectNow(collectorID)
                .then((data: ICollectorInfo) => device.updateCollector(data));
        }
    }

    public collectAll(deviceID: number): void {
        let device: DeviceInfo = this.findDeviceFromID(deviceID);
        if (device) {
            for (var i = 0; i < device.collectors.length; ++i) {
                let collector: CollectorInfo = device.collectors[i];
                this.dataService.collectNow(collector.id)
                    .then(function (data: ICollectorInfo) {
                        device.updateCollector(data);
                    });
            }
        }
    }

    private updateStatusFlags() {
        // Have to do this because inside the forEach method 'this' doesn't
        // refer to the DeviceManager object.
        let t = this;
        this.hasAlarms = false;
        this.hasStatus = false;
        Object.keys(this.allDevices).forEach(function (key, index) {
            var d = t.allDevices[key];
            let hasAlarm = d.alarms.length > 0;
            t.hasAlarms = t.hasAlarms || hasAlarm;

            let keys = Object.keys(d.statuses);
            t.hasStatus = t.hasStatus || keys.length > 0;
        });
    }

    public updateNetwork(network: Network) {
        for (var i = 0; i < network.data.length; ++i) {
            var n : NetworkStatus = network.data[i];
            if (n.deviceID >= 0) {
                //console.log("updateNetwork: " + JSON.stringify(n));
                let device = this.allDevices[n.deviceID];
                if (device) {
                    //console.log("updateNetwork-A: " + JSON.stringify(device));
                    device.networkStatus = n;
                    //console.log("updateNetwork-B: " + JSON.stringify(device));
                }
            }
        }
    }

    //public closeAllPanels() {
    //    for (var i = 0; i < this.devices.length; ++i) {
    //        this.devices[i].panelIsOpen = false;
    //    }
    //    for (var i = 0; i < this.groups.length; ++i) {
    //        this.groups[i].panelIsOpen = false;
    //    }
    //}

    public startAutomaticUpdate() {
        this.autoUpdater.start();
    }

    stopAutomaticUpdate() {
        this.autoUpdater.stop();
    }

    private static gatherData(t: DeviceManager) {
        t.dataService.getDeviceStatus()
            .then((data: IFullDeviceStatus) => t.updateStatus(data))

        t.dataService.getAllCollectors()
            .then((collectors: ICollectorInfo[]) => t.updateCollectors(collectors));
    }
}

interface IMonitoredDrive extends IDriveInfo {
    isMonitored: boolean;
}

class MonitoredDrive extends DriveInfo {
    isMonitored: boolean;

    constructor(d: IMonitoredDrive) {
        super(d);

        this.isMonitored = d.isMonitored;
    }
}

export interface IMonitoredDriveManager {
    driveMap: { [driveLetter: string]: IMonitoredDrive };
}

export class MonitoredDriveManager {
    driveMap: { [driveLetter: string]: MonitoredDrive };

    constructor(manager: IMonitoredDriveManager) {
        this.driveMap = {};
        let t = this;
        Object.keys(manager.driveMap).forEach(key => {
            t.driveMap[key] = new MonitoredDrive(manager.driveMap[key]);
        });
    }

    isDriveMonitored(driveLetter: string): boolean {
        let isMonitored = true;
        let d: MonitoredDrive = this.driveMap[driveLetter];
        if (d)
            isMonitored = d.isMonitored;
        return isMonitored;
    }
}
