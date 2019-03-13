// See ConfigurationLib/DeviceTypes.cs
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var EDeviceTypes;
    (function (EDeviceTypes) {
        EDeviceTypes[EDeviceTypes["Server"] = 0] = "Server";
        EDeviceTypes[EDeviceTypes["Workstation"] = 1] = "Workstation";
        EDeviceTypes[EDeviceTypes["Camera"] = 2] = "Camera";
        EDeviceTypes[EDeviceTypes["RPM"] = 3] = "RPM";
        EDeviceTypes[EDeviceTypes["System"] = 4] = "System";
        EDeviceTypes[EDeviceTypes["Generic"] = 5] = "Generic";
        EDeviceTypes[EDeviceTypes["Unknown"] = -1] = "Unknown";
    })(EDeviceTypes = exports.EDeviceTypes || (exports.EDeviceTypes = {}));
});
//# sourceMappingURL=devicetypes.enum.js.map