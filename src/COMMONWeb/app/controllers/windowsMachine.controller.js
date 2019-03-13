/// <reference types="angular" />
/// <reference types="angular-ui-bootstrap" />
/// <reference types="angular-route" />
define(["require", "exports", "../classes/machine"], function (require, exports, machine_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var WindowsMachineController = /** @class */ (function () {
        function WindowsMachineController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) {
            this.dataService = dataService;
            this.$routeParams = $routeParams;
            this.$scope = $scope;
            this.devicemanagerService = devicemanagerService;
            this.chartBridgeFactoryService = chartBridgeFactoryService;
            this.$uibModal = $uibModal;
            this.config = config;
            this.windowsMachineScope = this.$scope;
            this.windowsMachineScope.smartDisk = null;
            this.id = $routeParams.id;
            this.daysToRetrieveChoices = [15, 30, 60, 90, 120, 150, 180];
            var fs = this.chartBridgeFactoryService.$get();
            this.chartSettings = {
                cpu: fs.createChartSettings("% Used"),
                database: fs.createChartSettings("MB"),
                memory: fs.createChartSettings("GB Used"),
                nic: fs.createChartSettings("% Used"),
                processes: fs.createChartSettings("CPU %", 250),
                ups: fs.createChartSettings("Running on battery"),
                diskUsage: fs.createChartSettings("GB Used"),
                diskPerformance: fs.createChartSettings("% Time reading/writing"),
            };
            this.chartSettings.database.chartSizeInGB = false;
            this.chartSettings.processes.yaxis2 = "Memory in MB";
            this.chartSettings.processes.valueRange = [0, 100];
            this.changeDaysToRetrieve(15);
        }
        WindowsMachineController.prototype.changeDaysToRetrieve = function (days) {
            var _this = this;
            this.daysToRetrieve = days;
            var t = this;
            this.config.get()
                .then(function (c) {
                _this.devicemanagerService.get()
                    .then(function (dm) {
                    t.device = dm.findDeviceFromID(t.id);
                    if (t.device) {
                        var now = new Date();
                        var startDate = new Date(c.mostRecentData.getTime() - (days * 24 * 60 * 60 * 1000));
                        t.machine = new machine_1.Machine(t.device, t.dataService, startDate);
                    }
                });
            });
        };
        WindowsMachineController.prototype.getMoreCPU = function () {
            this.getMore(machine_1.EMachineParts.CPU);
        };
        WindowsMachineController.prototype.getMoreDiskUsage = function () {
            this.getMore(machine_1.EMachineParts.DiskUsage);
        };
        WindowsMachineController.prototype.getMoreDiskPerformance = function () {
            this.getMore(machine_1.EMachineParts.DiskPerformance);
        };
        WindowsMachineController.prototype.getMoreApplicationErrors = function () {
            this.getMore(machine_1.EMachineParts.ApplicationErrors);
        };
        WindowsMachineController.prototype.getMoreSystemErrors = function () {
            this.getMore(machine_1.EMachineParts.SystemErrors);
        };
        WindowsMachineController.prototype.getMoreMemory = function () {
            this.getMore(machine_1.EMachineParts.Memory);
        };
        WindowsMachineController.prototype.getMoreNIC = function () {
            this.getMore(machine_1.EMachineParts.NIC);
        };
        WindowsMachineController.prototype.getMoreUPS = function () {
            this.getMore(machine_1.EMachineParts.UPS);
        };
        WindowsMachineController.prototype.getMore = function (part) {
            if (this.machine)
                this.machine.getMoreDays(this.daysToRetrieve, part);
        };
        WindowsMachineController.prototype.showSMART = function (disk) {
            if (!disk)
                return;
            var t = this;
            t.windowsMachineScope.smartDisk = disk.smart;
            t.smartModal = this.$uibModal.open({
                templateUrl: 'app/disk/smart.modal.html',
                //controller: t.factory(),
                controllerAs: 'vm',
                scope: t.windowsMachineScope,
            });
        };
        return WindowsMachineController;
    }());
    exports.WindowsMachineController = WindowsMachineController;
});
//# sourceMappingURL=windowsMachine.controller.js.map