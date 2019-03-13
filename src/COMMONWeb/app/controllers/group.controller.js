/// <reference types="angular" />
/// <reference types="angular-route" />
define(["require", "exports", "../charts/chartjs"], function (require, exports, chartjs_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var GroupController = /** @class */ (function () {
        function GroupController($routeParams, devicemanagerService) {
            this.$routeParams = $routeParams;
            this.devicemanagerService = devicemanagerService;
            this.id = parseInt($routeParams.id);
            this.chartSettings = new chartjs_1.ChartJSSettings("Response time in ms", 125);
            var t = this;
            devicemanagerService.get()
                .then(function (dm) {
                t.group = dm.findGroup(t.id);
            });
        }
        GroupController.Factory = function () {
            var factory = function ($routeParams, devicemanagerService) {
                return new GroupController($routeParams, devicemanagerService);
            };
            factory.$inject = ['$routeParams', 'devicemanagerService'];
            return factory;
        };
        return GroupController;
    }());
    exports.GroupController = GroupController;
});
//# sourceMappingURL=group.controller.js.map