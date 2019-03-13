import { Memory } from './memory';
import { DiskUsageManager, DiskPerformanceManager, IDiskUsage } from '../disk/disk';
import { NICData } from './nic';
import { IDeviceErrors, ErrorManager } from './errors';
import { DeviceInfo } from './devices';
import { CPUData } from './cpu';
import { ProcessManager } from './processes';
import { DatabaseManager } from './database';
import { IDeviceApplications, ApplicationManager } from './applications';
import { IDeviceData, IDeviceDataDict } from "./idevicedata";
import { IDeviceProcessInfo } from "./deviceprocessinfo";
import { IServices, Services } from "./services";
import { UPSStatus } from "./ups";
import { DataService } from '../services/data.service';
import { IValueInfo } from './ivalueinfo';

// See Models.cs/DeviceDetails
export interface IDeviceDetails {
    deviceID: number;
    uptime: string;
    lastBootTime?: string;
}

class DeviceDetails {
    public uptime: string;
    public lastBootTime: Date;

    constructor(data: IDeviceDetails) {
        this.uptime = data.uptime;
        var uptimeRegex = /(\d+) (\d\d:\d\d:\d\d)/;
        var match = uptimeRegex.exec(data.uptime);
        if (match !== null) {
            this.uptime = match[1] + " days, " + match[2];
        }
        this.lastBootTime = null;
        if(data.lastBootTime && data.lastBootTime !== "")
            this.lastBootTime = new Date(data.lastBootTime);
    }
}

export enum EMachineParts {
    CPU,
    Memory,
    DiskUsage,
    DiskPerformance,
    NIC,
    SystemErrors,
    ApplicationErrors,
    Processes,
    Database,
    Applications,
    Services,
    UPS,
    //Network,
    //SMART,

    NumMachineParts
}

interface IMachinePartsToMethodMap {
    [part: number]: Function;
}

export interface IMachineData {
    cpu?: IDeviceData[];
    memory?: IDeviceData[];
    nic?: IDeviceData[];
    processes?: IDeviceProcessInfo;
    diskUsage?: IDiskUsage;
    diskPerformance?: IDeviceDataDict;
    systemErrors?: IDeviceErrors;
    applicationErrors?: IDeviceErrors;
    database?: IValueInfo;
    applications?: IDeviceApplications;
    services?: IServices;
    ups?: IDeviceData[];
}

export interface INumberToBoolean {
    [index: number]: boolean;
}

export class Machine {
    public name: string;
    public details: DeviceDetails;
    //public daysToRetrieve: number;
    public cpu: CPUData;
    public memory: Memory;
    public diskUsage: DiskUsageManager;
    public diskPerformance: DiskPerformanceManager;
    public nic: NICData;
    public systemErrors: ErrorManager;
    public applicationErrors: ErrorManager;
    public processes: ProcessManager;
    public database: DatabaseManager;
    public applications: ApplicationManager;
    public services: Services;
    public ups: UPSStatus;
    //private smart: SmartData;
    public loading: INumberToBoolean;
    private allParts: Date[];
    private allMethods: IMachinePartsToMethodMap;
    private toRetrieve: EMachineParts[];

    /// Let's have a way to retrieve a subset of all things that we can retrieve.
    /// The third parameter will specify the parts to retrieve. If it's null, retrieve
    /// everything. If it's non-null, retrieve just those specified parts.
    constructor(private devInfo: DeviceInfo, private dataService: DataService, startTime: Date, partsToRetrieve?: EMachineParts[]) {
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
        for (let e in EMachineParts) {
            let isNum: boolean = parseInt(e, 10) >= 0;
            if (isNum)
                this.loading[e] = false;
        }

        if (!partsToRetrieve) {
            // Retrieve everything. But let's automatically populate the toRetrieve array
            // with everything in MachineParts, except for the NumMachineParts value
            //
            // Here's how to get all the values from MachineParts: http://stackoverflow.com/a/21294925
            var objValues = Object.keys(EMachineParts).map(k => EMachineParts[k]);
            var value = objValues.filter(v => typeof v === "number") as EMachineParts[];

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

    public getEverythingFrom(starting: Date) {
        for (var i = 0; i < this.allParts.length; ++i)
            this.allParts[i] = starting;
        this.getEverything();
    }

    private getEverything() {
        this.getDeviceDetails();
        for (var i = 0; i < this.toRetrieve.length; ++i) {
            var part: EMachineParts = this.toRetrieve[i];
            var method = this.allMethods[part];
            if (method) {
                this.loading[part] = true;
                method();
            }
        }
    }

    private batchGetEverythingFrom(starting: Date) {
        for (var i = 0; i < this.allParts.length; ++i)
            this.allParts[i] = starting;
        this.batchGetEverything(starting);
    }

    private batchGetEverything(starting: Date) {
        this.getDeviceDetails();
        let t = this;
        for (let i = 0; i < this.toRetrieve.length; ++i) {
            let e: EMachineParts = this.toRetrieve[i];
            this.loading[e] = true;
        }
        this.dataService.getMachineData(this.devInfo.id, this.toRetrieve, starting)
            .then((data: IMachineData) => {
                t.onReceivedData(data);
            });
    }

    private isToRetrieve(part: EMachineParts): boolean {
        return this.toRetrieve.indexOf(part) >= 0;
    }

    public getMoreDays(days: number, part: EMachineParts) {
        let old_date = this.allParts[part];
        this.allParts[part] = new Date(old_date.getTime() - (days * 24 * 60 * 60 * 1000));

        if (this.isToRetrieve(part)) {
            let method = this.allMethods[part];
            if (method) {
                this.loading[part] = true;
                method();
            }
        }
    }

    public getDeviceDetails = () => {
        var t = this;
        this.dataService.getDeviceDetails(this.devInfo.id)
            .then((data: IDeviceDetails) => {
                if(data)
                    t.details = new DeviceDetails(data);
            });
    }

    private retrieveData(id: number, parts: EMachineParts[], start?: Date, end?: Date) {
        var t = this;
        this.dataService.getMachineData(this.devInfo.id, parts, start, end)
            .then((data: IMachineData) => {
                t.onReceivedData(data);
            });
    }

    private onReceivedData(data: IMachineData) {
        if (!data)
            return;

        if (data.cpu) {
            if (!this.cpu)
                this.cpu = new CPUData();
            this.cpu.update(data.cpu);
            this.loading[EMachineParts.CPU] = false;
        }
        if (data.memory) {
            if (!this.memory)
                this.memory = new Memory();
            this.memory.update(data.memory);
            this.loading[EMachineParts.Memory] = false;
        }
        if (data.diskUsage) {
            if (!this.diskUsage)
                this.diskUsage = new DiskUsageManager(this.devInfo);
            this.diskUsage.update(data.diskUsage);
            this.loading[EMachineParts.DiskUsage] = false;
        }
        if (data.diskPerformance) {
            if (!this.diskPerformance)
                this.diskPerformance = new DiskPerformanceManager(this.devInfo);
            this.diskPerformance.update(data.diskPerformance);
            this.loading[EMachineParts.DiskPerformance] = false;
        }
        if (data.nic) {
            if (!this.nic)
                this.nic = new NICData();
            this.nic.update(data.nic);
            this.loading[EMachineParts.NIC] = false;
        }
        if (data.systemErrors) {
            this.systemErrors = new ErrorManager(data.systemErrors);
            this.loading[EMachineParts.SystemErrors] = false;
        }
        if (data.applicationErrors) {
            this.applicationErrors = new ErrorManager(data.applicationErrors);
            this.loading[EMachineParts.ApplicationErrors] = false;
        }
        if (data.processes) {
            this.processes = new ProcessManager(data.processes, this.dataService);
            this.loading[EMachineParts.Processes] = false;
        }
        if (data.database) {
            this.database = new DatabaseManager(data.database, this.dataService);
            this.loading[EMachineParts.Database] = false;
        }
        if (data.applications) {
            this.applications = new ApplicationManager(data.applications, this.dataService);
            this.loading[EMachineParts.Applications] = false;
        }
        if (data.services) {
            this.services = new Services(data.services);
            this.loading[EMachineParts.Services] = false;
        }
        if (data.ups) {
            this.ups = new UPSStatus(data.ups);
            this.loading[EMachineParts.UPS] = false;
        }
    }

    public getCPUData = () => {
        let startDate: Date = this.getStartDate(EMachineParts.CPU);
        if (!startDate)
            return;

        this.retrieveData(this.devInfo.id, [EMachineParts.CPU], startDate, null);
    }

    public getMemoryData = () => {
        let startDate: Date = this.getStartDate(EMachineParts.Memory);
        if (!startDate)
            return;

        this.retrieveData(this.devInfo.id, [EMachineParts.Memory], startDate, null);
    }

    public getDiskUsageData = () => {
        let startDate: Date = this.getStartDate(EMachineParts.DiskUsage);
        if (!startDate)
            return;

        this.retrieveData(this.devInfo.id, [EMachineParts.DiskUsage], startDate, null);
    }

    public onDiskUsageSelection = (diskName: string) => {
        if (this.diskUsage !== null)
            this.diskUsage.selectDisk(diskName);
    }

    public getDiskPerformanceData = () => {
        let startDate: Date = this.getStartDate(EMachineParts.DiskPerformance);
        if (!startDate)
            return;

        this.retrieveData(this.devInfo.id, [EMachineParts.DiskPerformance], startDate, null);
    }

    public onDiskPerformanceSelection = (diskName: string) => {
        if (this.diskPerformance !== null)
            this.diskPerformance.selectDisk(diskName);
    }

    public getNICData = () => {
        let startDate: Date = this.getStartDate(EMachineParts.NIC);
        if (!startDate)
            return;

        this.retrieveData(this.devInfo.id, [EMachineParts.NIC], startDate, null);
    }

    public getSystemErrors = () => {
        let startDate: Date = this.getStartDate(EMachineParts.SystemErrors);
        if (!startDate)
            return;

        this.retrieveData(this.devInfo.id, [EMachineParts.SystemErrors], startDate, null);
    }

    public getApplicationErrors = () => {
        let startDate: Date = this.getStartDate(EMachineParts.ApplicationErrors);
        if (!startDate)
            return;

        this.retrieveData(this.devInfo.id, [EMachineParts.ApplicationErrors], startDate, null);
    }

    public getProcesses = () => {
        this.retrieveData(this.devInfo.id, [EMachineParts.Processes], null, null);
    }

    public getDatabases = () => {
        this.retrieveData(this.devInfo.id, [EMachineParts.Database], null, null);
    }

    public getApplications = () => {
        this.retrieveData(this.devInfo.id, [EMachineParts.Applications], null, null);
    }

    public getServices = () => {
        this.retrieveData(this.devInfo.id, [EMachineParts.Services], null, null);
    }

    public getUPS = () => {
        let startDate: Date = this.getStartDate(EMachineParts.UPS);
        if (!startDate)
            return;

        this.retrieveData(this.devInfo.id, [EMachineParts.UPS], null, null);
    }

    public onProcessSelection = (process: string) => {
        if (this.processes)
            this.processes.onSelectProcess(process);
    }

    private getStartDate(part: EMachineParts): Date {
        let startDate: Date = null;
        if (this.allParts[part])
            startDate = this.allParts[part];
        return startDate;
    }
}
