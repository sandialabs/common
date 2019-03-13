/// <reference types="angular" />

import { NetworkStatus } from "../classes/network";
import { IChartJSSettings } from "../charts/chartjs";

export interface INetworkDataScope extends ng.IScope {
    data: NetworkStatus[];
    chartSettings: IChartJSSettings;
}

// Pass a NetworkStatus[] to the directive in the 'data' attribute.
// Pass an optional IChartSettings to the chart-settings attribute,
// if you want a chart displayed.
export class NetworkTableDirective implements ng.IDirective {
    restrict: string = 'E';
    scope = {
        data: '=',
        chartSettings: '='
    };
    templateUrl = 'app/reports/networktable.partial.html';
    link: ng.IDirectiveLinkFn = (scope: INetworkDataScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
    };

    constructor() {
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new NetworkTableDirective();
        }
        return factory;
    }
}
