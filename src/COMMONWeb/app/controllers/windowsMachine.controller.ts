/// <reference types="angular" />
/// <reference types="angular-ui-bootstrap" />
/// <reference types="angular-route" />

import { DataService } from "../services/data.service";
import { DeviceManagerService } from "../services/devicemanager.service";
import { ISmartData } from "../disk/smart";
import { DeviceManager, DeviceInfo } from "../classes/devices";
import { Machine, EMachineParts } from "../classes/machine";
import { DiskUsage } from "../disk/disk";
import { ChartBridgeFactoryService } from "../charts/chartbridgefactory.service";
import { IChartJSSettings } from "../charts/chartjs";
import { ChartJSChartBridgeFactory } from "../charts/chartbridgefactory";
import { ConfigurationService } from "../services/configuration.service";
import { SystemConfiguration } from "../classes/systemconfiguration";

interface IWindowsMachineScope extends ng.IScope, ISmartData {
}

interface IWindowsMachineChartSettings {
    cpu: IChartJSSettings;
    database: IChartJSSettings;
    memory: IChartJSSettings;
    nic: IChartJSSettings;
    processes: IChartJSSettings;
    ups: IChartJSSettings;
    diskUsage: IChartJSSettings;
    diskPerformance: IChartJSSettings;
}

export abstract class WindowsMachineController implements ng.IController {
    device: DeviceInfo;
    machine: Machine;
    id: number;
    daysToRetrieve: number;
    daysToRetrieveChoices: number[];
    windowsMachineScope: IWindowsMachineScope;
    smartModal: ng.ui.bootstrap.IModalInstanceService;
    factory: any;
    chartSettings: IWindowsMachineChartSettings

    constructor(protected dataService: DataService,
        protected $routeParams: ng.route.IRouteParamsService,
        protected $scope: ng.IScope,
        protected devicemanagerService: DeviceManagerService,
        protected chartBridgeFactoryService: ChartBridgeFactoryService,
        protected $uibModal: ng.ui.bootstrap.IModalService,
        protected config: ConfigurationService) {
        this.windowsMachineScope = <IWindowsMachineScope>this.$scope;
        this.windowsMachineScope.smartDisk = null;
        this.id = $routeParams.id;
        this.daysToRetrieveChoices = [15, 30, 60, 90, 120, 150, 180];

        let fs: ChartJSChartBridgeFactory = this.chartBridgeFactoryService.$get();

        this.chartSettings = {
            cpu: fs.createChartSettings("% Used"),
            database: fs.createChartSettings("MB"),
            memory: fs.createChartSettings("GB Used"),
            nic: fs.createChartSettings("% Used"),
            processes: fs.createChartSettings("CPU %", 250),
            ups: fs.createChartSettings("Running on battery"),
            diskUsage: fs.createChartSettings("GB Used"),
            diskPerformance: fs.createChartSettings("% Time reading/writing"),
        }
        this.chartSettings.database.chartSizeInGB = false;
        this.chartSettings.processes.yaxis2 = "Memory in MB";
        this.chartSettings.processes.valueRange = [0, 100];

        this.changeDaysToRetrieve(15);
    }

    changeDaysToRetrieve(days: number) {
        this.daysToRetrieve = days;

        let t = this;
        this.config.get()
            .then((c: SystemConfiguration) => {
                this.devicemanagerService.get()
                    .then((dm: DeviceManager) => {
                        t.device = dm.findDeviceFromID(t.id);
                        if (t.device) {
                            let now: Date = new Date();
                            let startDate: Date = new Date(c.mostRecentData.getTime() - (days * 24 * 60 * 60 * 1000));
                            t.machine = new Machine(t.device, t.dataService, startDate);
                        }
                    });
            });
    }

    getMoreCPU() {
        this.getMore(EMachineParts.CPU);
    }

    getMoreDiskUsage() {
        this.getMore(EMachineParts.DiskUsage);
    }

    getMoreDiskPerformance() {
        this.getMore(EMachineParts.DiskPerformance);
    }

    getMoreApplicationErrors() {
        this.getMore(EMachineParts.ApplicationErrors);
    }

    getMoreSystemErrors() {
        this.getMore(EMachineParts.SystemErrors);
    }

    getMoreMemory() {
        this.getMore(EMachineParts.Memory);
    }

    getMoreNIC() {
        this.getMore(EMachineParts.NIC);
    }

    getMoreUPS() {
        this.getMore(EMachineParts.UPS);
    }

    private getMore(part: EMachineParts) {
        if (this.machine)
            this.machine.getMoreDays(this.daysToRetrieve, part);
    }

    showSMART(disk: DiskUsage) {
        if (!disk)
            return;

        let t = this;
        t.windowsMachineScope.smartDisk = disk.smart;
        t.smartModal = this.$uibModal.open({
            templateUrl: 'app/disk/smart.modal.html',
            //controller: t.factory(),
            controllerAs: 'vm',
            scope: t.windowsMachineScope,
        });
    }
}