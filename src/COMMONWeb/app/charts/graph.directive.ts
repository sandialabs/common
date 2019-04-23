import { ChartBridgeFactoryService } from "./chartbridgefactory.service";
import { IDataScope, IChartContext } from "./chartjs";
import { ChartJSChartBridgeFactory } from "./chartbridgefactory";
import { ChartBridge } from "./chartbridge";

export class GraphDirective implements ng.IDirective {
    restrict: string = 'E';
    scope = {
        data: '<',
        settings: '<'
    };
    template = '<canvas></canvas>';
    link: ng.IDirectiveLinkFn = (scope: IDataScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
        let context: IChartContext = {
            element: element,
            rootScope: this.$rootScope,
            scope: scope,
            window: this.$window
        };
        //let start: number = new Date().getTime();
        let f: ChartJSChartBridgeFactory = this.chartBridgeFactoryService.$get();
        let cb: ChartBridge = f.createChartBridge(scope.data, scope.settings);

        if (cb)
            cb.makeChart(context);
        //let duration = new Date().getTime() - start;
        //console.log("makeChart: " + duration.toString() + " ms");
    };

    constructor(private $window: ng.IWindowService, private $rootScope: ng.IRootScopeService, private chartBridgeFactoryService: ChartBridgeFactoryService) {
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = ($window: ng.IWindowService, $rootScope: ng.IRootScopeService, chartBridgeService: ChartBridgeFactoryService): ng.IDirective => {
            return new GraphDirective($window, $rootScope, chartBridgeService);
        }
        factory.$inject = ['$window', '$rootScope', 'chartBridgeFactoryService'];
        return factory;
    }
}
