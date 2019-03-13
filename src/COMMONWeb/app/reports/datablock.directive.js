/// <reference types="angular" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DataBlockDirective = /** @class */ (function () {
        function DataBlockDirective() {
            this.restrict = 'E';
            this.scope = {
                heading: '@',
                line1: '@',
                line2: '@?'
            };
            this.templateUrl = 'app/reports/datablock.partial.html';
            this.link = function (scope, element, attrs) {
            };
            this.replace = false;
            // console.log("DataBlockDirective");
        }
        DataBlockDirective.Factory = function () {
            var factory = function () {
                return new DataBlockDirective();
            };
            return factory;
        };
        return DataBlockDirective;
    }());
    exports.DataBlockDirective = DataBlockDirective;
});
//# sourceMappingURL=datablock.directive.js.map