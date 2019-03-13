/// <reference types="angular" />

import { DeviceInfo } from "../classes/devices"
import { IChartJSSettings } from "../charts/chartjs";

export interface IDeviceInfoScope extends ng.IScope {
    device: DeviceInfo;
    networkChartSettings: IChartJSSettings;
    //isOpen?: boolean;
    groupID: number;
}

export class DeviceInfoDirective implements ng.IDirective {
    restrict: string = 'E';
    scope = {
        device: '=',
        networkChartSettings: '=',
        //isOpen: '=?',
    };
    templateUrl = 'app/views/partials/deviceinfo.partial.html';
    link: ng.IDirectiveLinkFn = (scope: IDeviceInfoScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
    };

    constructor() {
        console.log("DeviceInfoDirective.constructor");
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new DeviceInfoDirective();
        }
        return factory;
    }
}
