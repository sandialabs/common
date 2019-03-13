/// <reference types="angular" />

import { DeviceManagerService } from "../services/devicemanager.service";
import { DeviceInfo, DeviceManager } from "../classes/devices";
import { Group } from "../classes/group";
import { NetworkService } from "../services/network.service";
import { Network } from "../classes/network";
import { IChartJSSettings, ChartJSSettings } from "../charts/chartjs";

export class NetworkHistoryController implements ng.IController {
    devices: DeviceInfo[];
    groups: Group[];
    chartSettings: IChartJSSettings;

    constructor(private devicemanagerService: DeviceManagerService, private networkService: NetworkService) {
        this.devices = [];
        this.groups = [];
        this.chartSettings = new ChartJSSettings("Response time in ms", 100);
        this.chartSettings.displayLegend = false;

        let t = this;
        devicemanagerService.get()
            .then((dm: DeviceManager) => {
                t.devices = dm.devices;
                t.groups = dm.groups;
            });
    }

    getEarlierRange() {
        this.networkService.get()
            .then((n: Network) => {
                n.getEarlierRange();
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (devicemanagerService: DeviceManagerService, networkService: NetworkService): ng.IController => {
            return new NetworkHistoryController(devicemanagerService, networkService);
        }
        factory.$inject = ['devicemanagerService', 'networkService'];
        return factory;
    }
}
