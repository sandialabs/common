var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "./windowsMachine.controller", "../charts/chartjs"], function (require, exports, windowsMachine_controller_1, chartjs_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var WorkstationController = /** @class */ (function (_super) {
        __extends(WorkstationController, _super);
        function WorkstationController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, network, $uibModal, config) {
            var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) || this;
            _this.dataService = dataService;
            _this.$routeParams = $routeParams;
            _this.$scope = $scope;
            _this.devicemanagerService = devicemanagerService;
            _this.chartBridgeFactoryService = chartBridgeFactoryService;
            _this.network = network;
            _this.$uibModal = $uibModal;
            _this.config = config;
            //console.log("WorkstationController constructor");
            _this.networkChartSettings = new chartjs_1.ChartJSSettings("Response time in ms", 125);
            var t = _this;
            if (_this.device) {
                _this.network.get()
                    .then(function (n) {
                    var ns = n.getNetworkStatusFromID(_this.device.id);
                    if (ns)
                        t.device.networkStatus = ns;
                });
            }
            _this.factory = WorkstationController.Factory;
            return _this;
        }
        WorkstationController.Factory = function () {
            var factory = function (dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, network, $uibModal, config) {
                return new WorkstationController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, network, $uibModal, config);
            };
            factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'chartBridgeFactoryService', 'networkService', '$uibModal', 'configurationService'];
            return factory;
        };
        return WorkstationController;
    }(windowsMachine_controller_1.WindowsMachineController));
    exports.WorkstationController = WorkstationController;
});
//# sourceMappingURL=workstation.controller.js.map