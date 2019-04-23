/// <reference types="angular" />

import { DeviceInfo } from "../classes/devices"

export interface IOverviewDeviceAlertScope extends ng.IScope {
    device: DeviceInfo;
}

export class OverviewDeviceAlertDirective implements ng.IDirective {
    restrict: string = 'E';
    scope = {
        device: '<',
    };
    templateUrl = 'app/views/partials/overview.devicealert.partial.html';
    link: ng.IDirectiveLinkFn = (scope: IOverviewDeviceAlertScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
    };

    constructor() {
        //console.log("OverviewDeviceAlertDirective.constructor");
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new OverviewDeviceAlertDirective();
        }
        return factory;
    }
}
