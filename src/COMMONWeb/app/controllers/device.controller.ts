/// <reference types="angular" />
/// <reference types="angular-route" />

import { DeviceInfo, DeviceManager } from "../classes/devices";
import { DeviceManagerService } from "../services/devicemanager.service";
import { IChartJSSettings, ChartJSSettings } from "../charts/chartjs";

export class DeviceController implements ng.IController {
    id: number;
    device: DeviceInfo;
    chartSettings: IChartJSSettings;

    constructor(private $routeParams: ng.route.IRouteParamsService, private deviceManager: DeviceManagerService) {
        this.id = parseInt($routeParams.id);
        this.chartSettings = new ChartJSSettings("Response time in ms", 125);

        let t = this;
        deviceManager.get()
            .then((dm: DeviceManager) => {
                t.device = dm.findDeviceFromID(t.id);
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = ($routeParams: ng.route.IRouteParamsService, deviceManager: DeviceManagerService): ng.IController => {
            return new DeviceController($routeParams, deviceManager);
        }
        factory.$inject = ['$routeParams', 'devicemanagerService'];
        return factory;
    }
}
