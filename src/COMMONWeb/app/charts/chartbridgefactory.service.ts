/// <reference types="angular" />

import { ChartJSChartBridgeFactory } from "./chartbridgefactory";

export class ChartBridgeFactoryService implements ng.IServiceProvider {
    $get(): ChartJSChartBridgeFactory {
        return new ChartJSChartBridgeFactory();
    }

    public static Factory(): Function {
        let factory = (): ChartBridgeFactoryService => {
            return new ChartBridgeFactoryService();
        }
        return factory;
    }
}
