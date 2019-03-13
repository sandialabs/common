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
define(["require", "exports", "./windowsMachine.controller"], function (require, exports, windowsMachine_controller_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ServerController = /** @class */ (function (_super) {
        __extends(ServerController, _super);
        function ServerController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) {
            var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) || this;
            _this.dataService = dataService;
            _this.$routeParams = $routeParams;
            _this.$scope = $scope;
            _this.devicemanagerService = devicemanagerService;
            _this.chartBridgeFactoryService = chartBridgeFactoryService;
            _this.$uibModal = $uibModal;
            _this.config = config;
            // console.log("ServerController constructor");
            _this.factory = ServerController.Factory;
            return _this;
        }
        ServerController.Factory = function () {
            var factory = function (dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) {
                return new ServerController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config);
            };
            factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'chartBridgeFactoryService', '$uibModal', 'configurationService'];
            return factory;
        };
        return ServerController;
    }(windowsMachine_controller_1.WindowsMachineController));
    exports.ServerController = ServerController;
});
//# sourceMappingURL=server.controller.js.map