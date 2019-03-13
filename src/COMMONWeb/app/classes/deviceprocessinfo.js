define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    // See DeviceProcessInfo in DatabaseLib/Models.cs
    var DeviceProcessInfo = /** @class */ (function () {
        function DeviceProcessInfo() {
            this.deviceID = -1;
            this.cpuToProcesses = {};
            this.timestamp = null;
        }
        return DeviceProcessInfo;
    }());
    exports.DeviceProcessInfo = DeviceProcessInfo;
});
//# sourceMappingURL=deviceprocessinfo.js.map