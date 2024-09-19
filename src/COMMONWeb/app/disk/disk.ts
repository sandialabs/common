//import * as moment from '../../lib/moment/min/moment-with-locales';
import { IDeviceData, IDeviceDataDict } from '../classes/idevicedata';
import { DeviceInfo } from '../classes/devices';
import { IHardDisk, HardDisk } from './smart';
import { IChartJSSettings, IChartFactory, ChartJSDataPoint } from '../charts/chartjs';
import { ChartBridge } from '../charts/chartbridge';

// Disk usage (amount used/free)

interface IDiskUsageData {
    dataID: number;
    collectorID: number;
    timeStamp: string;
    capacity: number;
    free: number;
    used: number;
}

export class DiskUsageData {
    dataID: number;
    collectorID: number;
    timeStamp: Date;
    capacity: number;
    free: number;
    used: number;

    constructor(data: IDiskUsageData) {
        this.dataID = data.dataID;
        this.collectorID = data.collectorID;
        this.timeStamp = new Date(data.timeStamp);
        this.capacity = data.capacity;
        this.free = data.free;
        this.used = data.used;
    }
}

interface IDiskUsageWithSmartData {
    driveLetter: string;
    diskUsage: IDiskUsageData[];
    smartData: IHardDisk;
}

export class DiskUsageWithSmartData {
    driveLetter: string;
    diskUsage: DiskUsageData[];
    smartData: HardDisk;

    constructor(du: IDiskUsageWithSmartData) {
        this.driveLetter = du.driveLetter;
        this.diskUsage = [];
        for (var i = 0; i < du.diskUsage.length; ++i)
            this.diskUsage.push(new DiskUsageData(du.diskUsage[i]));
        this.diskUsage.sort((a, b) => {
            return a.timeStamp.getTime() - b.timeStamp.getTime();
        });
        this.smartData = new HardDisk(du.smartData);
    }
}

export interface IDiskUsage {
    [driveLetter: string]: IDiskUsageWithSmartData;
}

export class DiskUsageSnapshot {
    public timestamp: Date;
    public capacity: number;
    public free: number;
    public used: number;
    public percentUsed: number;
    static byteSize: number = 0x40000000;  // 1GB

    constructor(data: IDiskUsageData) {
        this.timestamp = new Date(data.timeStamp);
        this.capacity = (data.capacity / DiskUsageSnapshot.byteSize);
        this.free = (data.free / DiskUsageSnapshot.byteSize);
        this.used = (data.used / DiskUsageSnapshot.byteSize);
        this.percentUsed = this.used / this.capacity * 100;
    }
}

export class DiskUsage {
    public driveLetter: string;
    public current: DiskUsageSnapshot;
    public peak: DiskUsageSnapshot;
    public diskData: DiskUsageSnapshot[];
    public smart: HardDisk;
    public isActive: boolean;

    constructor(data: IDiskUsageWithSmartData) {
        this.driveLetter = data.driveLetter;
        this.isActive = false;

        this.update(data);
    }

    public update(data: IDiskUsageWithSmartData) {
        this.diskData = [];
        this.current = this.peak = null;
        this.smart = data.smartData ? new HardDisk(data.smartData) : null;

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

        this.diskData.sort((a, b) => {
            return a.timestamp.getTime() - b.timestamp.getTime();
        });
        this.current = this.diskData[this.diskData.length - 1];
    }
}

export class DiskUsageChartBridge extends ChartBridge {
    constructor(private diskUsageDataSource: DiskUsage, public settings: IChartJSSettings, protected factory: IChartFactory) {
        super(diskUsageDataSource, factory);
        this.settings.valueRange = [0, diskUsageDataSource.current.capacity];
    }

    watchCollection(): any {
        return this.diskUsageDataSource.diskData;
    }

    createChartData() {
        this.clearData();
        if (!this.diskUsageDataSource.diskData)
            return;

        for (var i = 0; i < this.diskUsageDataSource.diskData.length; ++i) {
            let c = this.diskUsageDataSource.diskData[i];
            this.addData(DiskUsageChartBridge.convert(c));
        }
    }

    private static convert(d: DiskUsageSnapshot): ChartJSDataPoint[] {
        return [new ChartJSDataPoint({ x: d.timestamp, y: d.used })];
    }
}

interface IDiskNameToDiskUsage {
    [index: string]: DiskUsage;
}

export class DiskUsageManager {
    public driveLetters: string[];
    public selectedDriveLetter: string;
    public selectedDisk: DiskUsage;
    public disks: DiskUsage[];
    public diskMap: IDiskNameToDiskUsage;
    public daysRetrieved: number;
    public type: string;

    constructor(private devInfo: DeviceInfo) {
        this.selectedDriveLetter = "";
        this.selectedDisk = null;
        this.disks = [];
        this.diskMap = {};
        this.type = "GB";
        this.driveLetters = [];
    }

    public update(data: IDiskUsage) {
        let t = this;
        let diskNames = [];
        let names = Object.keys(data);
        names.forEach(name => {
            let isMonitored = this.devInfo.monitoredDrives.isDriveMonitored(name);
            if (isMonitored)
                diskNames.push(name);
        });

        if (this.driveLetters.length != diskNames.length) {
            this.driveLetters = diskNames;
            this.driveLetters.sort();
        }

        this.driveLetters.forEach((driveLetter: string) => {
            let existingDisk = null;
            for (var i = 0; existingDisk === null && i < t.disks.length; ++i) {
                let d: DiskUsage = t.disks[i];
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
    }

    public selectDisk(disk: string) {
        this.selectedDriveLetter = disk;
        this.selectedDisk = this.diskMap[disk];
        for (var i = 0; i < this.disks.length; ++i)
            this.disks[i].isActive = this.disks[i].driveLetter === disk;
    }
}

// Disk performance (how much time it spends queuing data)

export class DiskPerformanceSnapshot {
    public timestamp: Date;
    public percentTime: number;
    public avgDiskQLength: number;

    constructor(data: IDeviceData) {
        var current = JSON.parse(data.value)['Value'];
        this.timestamp = new Date(data.timeStamp);
        this.percentTime = Number(current['Disk Time %']);
        this.avgDiskQLength = Number(current['Avg Disk Q Length']);
    }
}

export class DiskPerformance {
    public driveLetter: string;
    public current: DiskPerformanceSnapshot;
    public peak: DiskPerformanceSnapshot;
    public diskData: DiskPerformanceSnapshot[];
    public isActive: boolean;

    constructor(data: IDeviceData[], driveLetter: string) {
        this.driveLetter = driveLetter;
        this.isActive = false;

        this.update(data);
    }

    public update(data: IDeviceData[]) {
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

        this.diskData.sort((a, b) => {
            return a.timestamp.getTime() - b.timestamp.getTime();
        });
        this.current = this.diskData[this.diskData.length - 1];
    }
}

export class DiskPerformanceChartBridge extends ChartBridge {
    constructor(private diskPerformanceDataSource: DiskPerformance, protected factory: IChartFactory) {
        super(diskPerformanceDataSource, factory);
    }

    watchCollection(): any {
        return this.diskPerformanceDataSource.diskData;
    }

    createChartData() {
        this.clearData();
        if (!this.diskPerformanceDataSource.diskData)
            return;

        for (var i = 0; i < this.diskPerformanceDataSource.diskData.length; ++i) {
            let c = this.diskPerformanceDataSource.diskData[i];
            this.addData(DiskPerformanceChartBridge.convert(c));
        }
    }

    private static convert(dps: DiskPerformanceSnapshot): ChartJSDataPoint[] {
        return [new ChartJSDataPoint({ x: dps.timestamp, y: dps.percentTime })];
    }
}

interface IDriveLetterToDiskPerformance {
    [index: string]: DiskPerformance;
}

export class DiskPerformanceManager {
    public driveLetters: string[];
    public selectedDriveLetter: string;
    public selectedDisk: DiskPerformance;
    public disks: DiskPerformance[];
    public diskMap: IDriveLetterToDiskPerformance;
    public type: string;

    constructor(private devInfo: DeviceInfo) {
        this.selectedDriveLetter = "";
        this.selectedDisk = null;
        this.disks = [];
        this.diskMap = {};
        this.type = "GB";
        this.driveLetters = [];
    }

    update(data: IDeviceDataDict) {
        let t = this;

        this.driveLetters = [];
        let names = Object.keys(data);
        names.forEach(name => {
            let isMonitored = this.devInfo.monitoredDrives.isDriveMonitored(name);
            if (isMonitored)
                t.driveLetters.push(name);
        });
        this.driveLetters.sort();

        //console.log("DiskUsageDiskPerformanceManager.update", data, this.driveLetters);

        this.driveLetters.forEach((diskName: string) => {
            let existingDisk = null;
            for (let i = 0; existingDisk === null && i < t.disks.length; ++i) {
                let d: DiskPerformance = t.disks[i];
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
    }

    public selectDisk(disk: string) {
        this.selectedDriveLetter = disk;
        this.selectedDisk = this.diskMap[disk];
        for (var i = 0; i < this.disks.length; ++i)
            this.disks[i].isActive = this.disks[i].driveLetter === disk;
    }
}
