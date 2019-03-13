/// <reference types="angular" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DeviceInfoDirective = /** @class */ (function () {
        function DeviceInfoDirective() {
            this.restrict = 'E';
            this.scope = {
                device: '=',
                networkChartSettings: '=',
            };
            this.templateUrl = 'app/views/partials/deviceinfo.partial.html';
            this.link = function (scope, element, attrs) {
            };
            console.log("DeviceInfoDirective.constructor");
        }
        DeviceInfoDirective.Factory = function () {
            var factory = function () {
                return new DeviceInfoDirective();
            };
            return factory;
        };
        return DeviceInfoDirective;
    }());
    exports.DeviceInfoDirective = DeviceInfoDirective;
});
//# sourceMappingURL=deviceinfo.directive.js.map