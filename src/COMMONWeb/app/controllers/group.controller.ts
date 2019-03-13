/// <reference types="angular" />
/// <reference types="angular-route" />

import { DeviceManagerService } from "../services/devicemanager.service";
import { DeviceManager } from "../classes/devices";
import { Group } from "../classes/group";
import { IChartJSSettings, ChartJSSettings } from "../charts/chartjs";

export class GroupController implements ng.IController {
    id: number;
    group: Group;
    chartSettings: IChartJSSettings;

    constructor(private $routeParams: ng.route.IRouteParamsService, private devicemanagerService: DeviceManagerService) {
        this.id = parseInt($routeParams.id);
        this.chartSettings = new ChartJSSettings("Response time in ms", 125);

        let t = this;
        devicemanagerService.get()
            .then((dm: DeviceManager) => {
                t.group = dm.findGroup(t.id);
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = ($routeParams: ng.route.IRouteParamsService, devicemanagerService: DeviceManagerService): ng.IController => {
            return new GroupController($routeParams, devicemanagerService);
        }
        factory.$inject = ['$routeParams', 'devicemanagerService'];
        return factory;
    }
}
