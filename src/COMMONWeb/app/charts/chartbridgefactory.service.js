/// <reference types="angular" />
define(["require", "exports", "./chartbridgefactory"], function (require, exports, chartbridgefactory_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ChartBridgeFactoryService = /** @class */ (function () {
        function ChartBridgeFactoryService() {
        }
        ChartBridgeFactoryService.prototype.$get = function () {
            return new chartbridgefactory_1.ChartJSChartBridgeFactory();
        };
        ChartBridgeFactoryService.Factory = function () {
            var factory = function () {
                return new ChartBridgeFactoryService();
            };
            return factory;
        };
        return ChartBridgeFactoryService;
    }());
    exports.ChartBridgeFactoryService = ChartBridgeFactoryService;
});
//# sourceMappingURL=chartbridgefactory.service.js.map