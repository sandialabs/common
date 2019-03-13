/// <reference types="angular" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    // Pass a MachineReport to the directive in the 'data' attribute.
    var MachineSnapshotDirective = /** @class */ (function () {
        function MachineSnapshotDirective() {
            this.restrict = 'E';
            this.scope = {
                data: '=',
            };
            this.templateUrl = 'app/reports/machine.snapshot.partial.html';
            this.link = function (scope, element, attrs) {
            };
            // console.log("MachineSnapshotDirective.constructor");
        }
        MachineSnapshotDirective.Factory = function () {
            var factory = function () {
                return new MachineSnapshotDirective();
            };
            return factory;
        };
        return MachineSnapshotDirective;
    }());
    exports.MachineSnapshotDirective = MachineSnapshotDirective;
});
//# sourceMappingURL=machine.snapshot.directive.js.map