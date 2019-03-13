/// <reference types="angular" />
define(["require", "exports", "../charts/chartjs"], function (require, exports, chartjs_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var OverviewController = /** @class */ (function () {
        function OverviewController(devicemanagerService) {
            this.devicemanagerService = devicemanagerService;
            var vm = this;
            this.networkChartSettings = new chartjs_1.ChartJSSettings("Response time in ms", 100);
            devicemanagerService.get()
                .then(function (deviceManager) {
                vm.deviceManager = deviceManager;
                vm.devices = vm.deviceManager.devices;
                vm.groups = vm.deviceManager.groups;
                //deviceManager.closeAllPanels();
            });
        }
        OverviewController.Factory = function () {
            var factory = function (devicemanagerService) {
                return new OverviewController(devicemanagerService);
            };
            factory.$inject = ['devicemanagerService'];
            return factory;
        };
        return OverviewController;
    }());
    exports.OverviewController = OverviewController;
});
//# sourceMappingURL=overview.controller.js.map