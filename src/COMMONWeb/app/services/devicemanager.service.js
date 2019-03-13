/// <reference types="angular" />
define(["require", "exports", "../classes/devices", "../classes/promisekeeper"], function (require, exports, devices_1, promisekeeper_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DeviceManagerService = /** @class */ (function () {
        function DeviceManagerService($q, dataService, $interval) {
            this.$q = $q;
            this.dataService = dataService;
            this.$interval = $interval;
            if (!DeviceManagerService.deviceManager)
                DeviceManagerService.deviceManager = new devices_1.DeviceManager(this.dataService, this.$interval);
            this.keeper = new promisekeeper_1.PromiseKeeper($q);
            this.configured = false;
            // console.log("NewDeviceManagerService.constructor");
        }
        DeviceManagerService.prototype.get = function (alwaysReturn) {
            if (alwaysReturn === void 0) { alwaysReturn = false; }
            var d = this.$q.defer();
            if (this.configured || alwaysReturn)
                d.resolve(DeviceManagerService.deviceManager);
            else
                this.keeper.push(d);
            return d.promise;
        };
        DeviceManagerService.prototype.setConfigured = function () {
            this.configured = true;
            this.keeper.resolve(DeviceManagerService.deviceManager);
        };
        DeviceManagerService.Factory = function () {
            var factory = function ($q, dataService, $interval) {
                return new DeviceManagerService($q, dataService, $interval);
            };
            factory.$inject = ['$q', 'dataService', '$interval'];
            return factory;
        };
        return DeviceManagerService;
    }());
    exports.DeviceManagerService = DeviceManagerService;
});
//# sourceMappingURL=devicemanager.service.js.map