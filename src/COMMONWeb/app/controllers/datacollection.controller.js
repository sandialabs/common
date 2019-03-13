define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DataCollectionController = /** @class */ (function () {
        function DataCollectionController(dataService, $rootScope, $scope, devicemanagerService) {
            this.dataService = dataService;
            this.$rootScope = $rootScope;
            this.$scope = $scope;
            this.devicemanagerService = devicemanagerService;
            var t = this;
            this.devices = [];
            devicemanagerService.get()
                .then(function (deviceManager) {
                t.devices = deviceManager.getDevicesForDataCollection();
                t.deviceManager = deviceManager;
            });
        }
        DataCollectionController.prototype.onCollectNow = function (collectorID) {
            this.devicemanagerService.get()
                .then(function (deviceManager) {
                deviceManager.collectNow(collectorID);
            });
        };
        DataCollectionController.prototype.onCollectAll = function (deviceID) {
            this.devicemanagerService.get()
                .then(function (deviceManager) {
                deviceManager.collectAll(deviceID);
            });
        };
        DataCollectionController.Factory = function () {
            var factory = function (dataService, $rootScope, $scope, devicemanagerService) {
                return new DataCollectionController(dataService, $rootScope, $scope, devicemanagerService);
            };
            factory.$inject = ['dataService', '$rootScope', '$scope', 'devicemanagerService'];
            return factory;
        };
        return DataCollectionController;
    }());
    exports.DataCollectionController = DataCollectionController;
});
//# sourceMappingURL=datacollection.controller.js.map