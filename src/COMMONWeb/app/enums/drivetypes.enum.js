define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var EDriveTypes;
    (function (EDriveTypes) {
        EDriveTypes[EDriveTypes["Unknown"] = 0] = "Unknown";
        EDriveTypes[EDriveTypes["NoRootDirectory"] = 1] = "NoRootDirectory";
        EDriveTypes[EDriveTypes["RemovableDisk"] = 2] = "RemovableDisk";
        EDriveTypes[EDriveTypes["LocalDisk"] = 3] = "LocalDisk";
        EDriveTypes[EDriveTypes["NetworkDrive"] = 4] = "NetworkDrive";
        EDriveTypes[EDriveTypes["CompactDisc"] = 5] = "CompactDisc";
        EDriveTypes[EDriveTypes["RAMDisk"] = 6] = "RAMDisk";
    })(EDriveTypes = exports.EDriveTypes || (exports.EDriveTypes = {}));
});
//# sourceMappingURL=drivetypes.enum.js.map