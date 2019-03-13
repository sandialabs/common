/// <reference types="angular" />
define(["require", "exports", "../charts/chartjs"], function (require, exports, chartjs_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var NetworkHistoryController = /** @class */ (function () {
        function NetworkHistoryController(devicemanagerService, networkService) {
            this.devicemanagerService = devicemanagerService;
            this.networkService = networkService;
            this.devices = [];
            this.groups = [];
            this.chartSettings = new chartjs_1.ChartJSSettings("Response time in ms", 100);
            this.chartSettings.displayLegend = false;
            var t = this;
            devicemanagerService.get()
                .then(function (dm) {
                t.devices = dm.devices;
                t.groups = dm.groups;
            });
        }
        NetworkHistoryController.prototype.getEarlierRange = function () {
            this.networkService.get()
                .then(function (n) {
                n.getEarlierRange();
            });
        };
        NetworkHistoryController.Factory = function () {
            var factory = function (devicemanagerService, networkService) {
                return new NetworkHistoryController(devicemanagerService, networkService);
            };
            factory.$inject = ['devicemanagerService', 'networkService'];
            return factory;
        };
        return NetworkHistoryController;
    }());
    exports.NetworkHistoryController = NetworkHistoryController;
});
//# sourceMappingURL=networkhistory.controller.js.map