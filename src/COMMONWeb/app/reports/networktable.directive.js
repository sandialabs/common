/// <reference types="angular" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    // Pass a NetworkStatus[] to the directive in the 'data' attribute.
    // Pass an optional IChartSettings to the chart-settings attribute,
    // if you want a chart displayed.
    var NetworkTableDirective = /** @class */ (function () {
        function NetworkTableDirective() {
            this.restrict = 'E';
            this.scope = {
                data: '=',
                chartSettings: '='
            };
            this.templateUrl = 'app/reports/networktable.partial.html';
            this.link = function (scope, element, attrs) {
            };
        }
        NetworkTableDirective.Factory = function () {
            var factory = function () {
                return new NetworkTableDirective();
            };
            return factory;
        };
        return NetworkTableDirective;
    }());
    exports.NetworkTableDirective = NetworkTableDirective;
});
//# sourceMappingURL=networktable.directive.js.map