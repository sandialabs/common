/// <reference types="angular" />

import { DeviceManager, DeviceInfo } from "../classes/devices";
import { DeviceManagerService } from "../services/devicemanager.service";
import { Group } from "../classes/group";
import { IChartJSSettings, ChartJSSettings } from "../charts/chartjs";

export class OverviewController {
    deviceManager: DeviceManager;
    devices: DeviceInfo[];
    groups: Group[];
    networkChartSettings: IChartJSSettings;

    constructor(private devicemanagerService: DeviceManagerService) {
        var vm = this;

        this.networkChartSettings = new ChartJSSettings("Response time in ms", 100);

        devicemanagerService.get()
            .then((deviceManager: DeviceManager) => {
                vm.deviceManager = deviceManager;

                vm.devices = vm.deviceManager.devices;
                vm.groups = vm.deviceManager.groups;

                //deviceManager.closeAllPanels();
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (devicemanagerService: DeviceManagerService) => {
            return new OverviewController(devicemanagerService);
        }
        factory.$inject = ['devicemanagerService'];
        return factory;
    }
}
