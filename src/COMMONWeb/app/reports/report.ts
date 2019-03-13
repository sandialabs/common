import { DeviceInfo, DeviceManager } from '../classes/devices';
import { EDeviceTypes } from '../enums/devicetypes.enum';
import { ECollectorType, CollectorTypeExtensions } from '../enums/collectortype.enum';
import { Machine, EMachineParts, IMachineData } from '../classes/machine';
import { Utilities } from "../classes/utilities";
import { INetworkStatus, NetworkStatus, Network } from "../classes/network";
import { AllApplicationsHistory, IAllApplicationsHistory } from "../classes/applications";
import { IServices, Services } from "../classes/services";
import { DataService } from '../services/data.service';
import { DeviceManagerService } from '../services/devicemanager.service';
import { ConfigurationService } from '../services/configuration.service';
import { ISystemConfiguration, SystemConfiguration } from "../classes/systemconfiguration";

export enum EReportTypes {
    Server,
    Workstation,
    Network,
    CASLoad,
    Issues,
    SiteConfiguration,
    Site,
}

export enum EReportSubTypes {
    Memory,
    Disk,
    CPU,
    NIC,
}

export interface IReportSettings {
    deviceID: number;
    device: DeviceInfo;
    type: EReportTypes;
    startDate?: Date;
    endDate?: Date;
    reportName: string;
}

export abstract class Report {
    siteName: string;

    constructor(protected settings: IReportSettings, protected dataService: DataService, protected configurationService: ConfigurationService) {
        let t = this;
        configurationService.get()
            .then((config: SystemConfiguration) => {
                t.siteName = config.siteName;
            });
    }

    abstract build();
}

interface ICurrentPeakReportBase {
    currentPercentUsed: number;
    peakPercentUsed: number;
    peakTimestamp: string;
}

export interface ICPUReport extends ICurrentPeakReportBase {
}

abstract class CurrentPeakReportBase {
    currentPercentUsed: number;
    peakPercentUsed: number;
    peakTimestamp: Date;

    constructor(report: ICurrentPeakReportBase) {
        this.currentPercentUsed = report.currentPercentUsed;
        this.peakPercentUsed = report.peakPercentUsed;
        this.peakTimestamp = new Date(report.peakTimestamp);
    }
}

export interface IMemoryReport extends ICurrentPeakReportBase {
}

class MemoryReport extends CurrentPeakReportBase {
    constructor(report: IMemoryReport) {
        super(report);
    }
}

interface IDiskInfo extends ICurrentPeakReportBase {
    name: string;
}

class DiskInfo extends CurrentPeakReportBase {
    name: string;

    constructor(info: IDiskInfo) {
        super(info);
        this.name = info.name;
    }
}

export interface IDiskReport {
    disks: IDiskInfo[];
}

export interface INICReport extends ICurrentPeakReportBase {
    bps: number;
    peakBps: number;
}

class DiskReport {
    disks: DiskInfo[];

    constructor(report: IDiskReport) {
        this.disks = [];
        for (var i = 0; i < report.disks.length; ++i)
            this.disks.push(new DiskInfo(report.disks[i]));
    }
}

class CPUReport extends CurrentPeakReportBase {
    constructor(report: ICPUReport) {
        super(report);
    }
}

class NICReport extends CurrentPeakReportBase {
    bps: number;
    peakBps: number;

    constructor(report: INICReport) {
        super(report);
        this.bps = report.bps;
        this.peakBps = report.peakBps;
    }
}

export interface ISubReport {
    memory?: IMemoryReport;
    disk?: IDiskReport;
    cpu?: ICPUReport;
    nic?: INICReport;
}

export class MachineReport extends Report {
    machine: Machine;
    memory: MemoryReport;
    disks: DiskReport;
    cpu: CPUReport;
    nic: NICReport;
    allProcesses: string[];
    splitProcesses: [string, string][];
    services: Services;
    splitServices: [string, string][];
    appHistory: AllApplicationsHistory;

    constructor(settings: IReportSettings, dataService: DataService, configurationService: ConfigurationService) {
        super(settings, dataService, configurationService);

        this.machine = null;
        this.memory = null;
        this.disks = null;
        this.cpu = null;
        this.nic = null;
        this.allProcesses = [];
        this.splitProcesses = [];
        this.services = null;
        this.splitServices = [];
    }

    public build() {
        this.machine = new Machine(this.settings.device, this.dataService, this.settings.startDate,
            [
                EMachineParts.Applications,
                EMachineParts.Database,
                EMachineParts.ApplicationErrors,
                EMachineParts.SystemErrors
            ]);

        let t = this;
        this.dataService.getSubReport(this.settings.device.id, [EReportSubTypes.Memory, EReportSubTypes.Disk, EReportSubTypes.CPU, EReportSubTypes.NIC], this.settings.startDate, this.settings.endDate)
            .then((data: ISubReport) => {
                if (data.memory)
                    t.memory = new MemoryReport(data.memory);
                if (data.disk)
                    t.disks = new DiskReport(data.disk);
                if (data.cpu)
                    t.cpu = new CPUReport(data.cpu);
                if (data.nic)
                    t.nic = new NICReport(data.nic);
            });

        this.getProcesses();
        this.getServices();
        this.getAppHistory();
    }

    private getProcesses() {
        var t = this;
        this.allProcesses = [];
        this.splitProcesses = [];

        this.dataService.getAllProcesses(this.settings.device.id, this.settings.startDate, this.settings.endDate)
            .then((data: string[]) => {
                if (data) {
                    t.allProcesses = data;
                    t.splitProcesses = Utilities.chunkToTupleOf2(data);
                }
            });
    }

    private getServices() {
        var t = this;
        this.services = null;
        this.splitServices = [];
        this.dataService.getServicesData(this.settings.device.id)
            .then((data: IServices) => {
                if (!data)
                    return;
                t.services = new Services(data);
                if(t.services.services)
                    t.splitServices = Utilities.chunkToTupleOf2(t.services.services);
            });
    }

    private getAppHistory() {
        var t = this;
        this.appHistory = null;
        this.dataService.getAppChanges(this.settings.device.id, this.settings.startDate, this.settings.endDate)
            .then((data: IAllApplicationsHistory) => {
                t.appHistory = new AllApplicationsHistory(data);
            });
    }
}

export class NetworkReport extends Report {
    networkStatus: NetworkStatus[];
    mostRecentAttempt: Date;

    constructor(settings: IReportSettings, dataService: DataService, configurationService: ConfigurationService, private network: Network) {
        super(settings, dataService, configurationService);

        if (!network)
            return;

        this.networkStatus = network.data;
        this.mostRecentAttempt = network.maxDate;
    }

    public build() {
    }
}

export class CASLoadReport extends Report {
    devices: Machine[];

    constructor(settings: IReportSettings, dataService: DataService, configurationService: ConfigurationService, private datamanagerService: DeviceManagerService) {
        super(settings, dataService, configurationService);

        this.devices = [];
    }

    public build() {
        let t = this;
        t.datamanagerService.get()
            .then((dm: DeviceManager) => {
                for (var i = 0; i < dm.windowsDevices.length; ++i) {
                    var device = dm.windowsDevices[i];
                    var m = new Machine(device, this.dataService, this.settings.startDate,[
                        EMachineParts.Memory,
                        EMachineParts.DiskUsage,
                        EMachineParts.CPU,
                        EMachineParts.NIC
                    ]);
                    t.devices.push(m);
                }
            });
    }
}

export class IssuesReport extends Report {
    devices: DeviceInfo[];

    constructor(settings: IReportSettings, dataService: DataService, configurationService: ConfigurationService, private datamanagerService: DeviceManagerService) {
        super(settings, dataService, configurationService);

        this.devices = [];
    }

    public build() {
        let t = this;
        t.datamanagerService.get()
            .then((dm: DeviceManager) => {
                t.devices = dm.windowsDevices;
            });
    }
}

export class ConfigReport extends Report {
    configData: SystemConfiguration;

    constructor(settings: IReportSettings, dataService: DataService, configurationService: ConfigurationService) {
        super(settings, dataService, configurationService);

        this.configData = null;
    }

    public build() {
        var t = this;
        this.dataService.getConfiguration()
            .then(function (data: ISystemConfiguration) {
                t.configData = new SystemConfiguration(data);

                var collectorTypeExt = new CollectorTypeExtensions();
                for (var i = 0; i < t.configData.devices.length; ++i) {
                    var device = t.configData.devices[i];

                    var doomed: ECollectorType[] = [];
                    for (var j = 0; j < device.collectors.length; ++j) {
                        var collector = device.collectors[j];
                        if (collectorTypeExt.isHidden(collector.collectorType))
                            doomed.push(collector.collectorType);
                    }

                    for (var d = 0; d < doomed.length; ++d) {
                        for (var j = 0; j < device.collectors.length; ++j) {
                            var collector = device.collectors[j];
                            if (collector.collectorType == doomed[d]) {
                                device.collectors.splice(j, 1);
                                break;
                            }
                        }
                    }
                }
            });
    }
}

export class SiteReport extends Report {
    networkStatus: NetworkStatus[];

    constructor(settings: IReportSettings, dataService: DataService, configurationService: ConfigurationService) {
        super(settings, dataService, configurationService);

        this.networkStatus = [];
    }

    build() {
        let t = this;
        this.dataService.getNetworkStatus(this.settings.startDate, this.settings.endDate)
            .then((status: INetworkStatus[]) => {
                // The statuses should be coming back sorted by IP address,
                // which is what we want, so we won't re-order them.
                for (var i = 0; i < status.length; ++i)
                    t.networkStatus.push(new NetworkStatus(status[i]));
            });
    }
}