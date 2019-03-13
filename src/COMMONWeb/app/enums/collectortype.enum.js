define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ECollectorType;
    (function (ECollectorType) {
        ECollectorType[ECollectorType["Memory"] = 0] = "Memory";
        ECollectorType[ECollectorType["Disk"] = 1] = "Disk";
        ECollectorType[ECollectorType["CPUUsage"] = 2] = "CPUUsage";
        ECollectorType[ECollectorType["NICUsage"] = 3] = "NICUsage";
        ECollectorType[ECollectorType["Uptime"] = 4] = "Uptime";
        ECollectorType[ECollectorType["LastBootTime"] = 5] = "LastBootTime";
        ECollectorType[ECollectorType["Processes"] = 6] = "Processes";
        ECollectorType[ECollectorType["Ping"] = 7] = "Ping";
        ECollectorType[ECollectorType["InstalledApplications"] = 8] = "InstalledApplications";
        ECollectorType[ECollectorType["Services"] = 9] = "Services";
        //Database,               // 10
        ECollectorType[ECollectorType["SystemErrors"] = 11] = "SystemErrors";
        ECollectorType[ECollectorType["ApplicationErrors"] = 12] = "ApplicationErrors";
        ECollectorType[ECollectorType["DatabaseSize"] = 13] = "DatabaseSize";
        ECollectorType[ECollectorType["UPS"] = 14] = "UPS";
        ECollectorType[ECollectorType["DiskSpeed"] = 15] = "DiskSpeed";
        ECollectorType[ECollectorType["Configuration"] = 16] = "Configuration";
        ECollectorType[ECollectorType["Unknown"] = -1] = "Unknown";
    })(ECollectorType = exports.ECollectorType || (exports.ECollectorType = {}));
    var CollectorTypeExtensions = /** @class */ (function () {
        function CollectorTypeExtensions() {
        }
        CollectorTypeExtensions.prototype.isHidden = function (type) {
            var isHidden = false;
            switch (type) {
                case ECollectorType.Configuration:
                    isHidden = true;
                    break;
            }
            return isHidden;
        };
        return CollectorTypeExtensions;
    }());
    exports.CollectorTypeExtensions = CollectorTypeExtensions;
});
//# sourceMappingURL=collectortype.enum.js.map