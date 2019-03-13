/// <reference types="angular" />
/// <reference types="angular-route" />
define(["require", "exports", "../charts/chartjs"], function (require, exports, chartjs_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DeviceController = /** @class */ (function () {
        function DeviceController($routeParams, deviceManager) {
            this.$routeParams = $routeParams;
            this.deviceManager = deviceManager;
            this.id = parseInt($routeParams.id);
            this.chartSettings = new chartjs_1.ChartJSSettings("Response time in ms", 125);
            var t = this;
            deviceManager.get()
                .then(function (dm) {
                t.device = dm.findDeviceFromID(t.id);
            });
        }
        DeviceController.Factory = function () {
            var factory = function ($routeParams, deviceManager) {
                return new DeviceController($routeParams, deviceManager);
            };
            factory.$inject = ['$routeParams', 'devicemanagerService'];
            return factory;
        };
        return DeviceController;
    }());
    exports.DeviceController = DeviceController;
});
//# sourceMappingURL=device.controller.js.map