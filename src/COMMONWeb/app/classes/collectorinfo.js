define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var CollectorInfo = /** @class */ (function () {
        function CollectorInfo(info) {
            this.id = info.id;
            this.deviceID = info.deviceID;
            this.name = this.fullName = info.name;
            this.collectorType = info.collectorType;
            this.isEnabled = info.isEnabled;
            this.frequencyInMinutes = info.frequencyInMinutes;
            this.lastCollectionAttempt = info.lastCollectionAttempt ? new Date(info.lastCollectionAttempt) : null;
            this.lastCollectedAt = info.lastCollectedAt ? new Date(info.lastCollectedAt) : null;
            this.nextCollectionTime = info.nextCollectionTime ? new Date(info.nextCollectionTime) : null;
            this.successfullyCollected = info.successfullyCollected;
            this.isBeingCollected = info.isBeingCollected;
            // Each collector's name is a combination of the device and the collector type,
            // like this: <devicename>.<collector>.
            // This finds the '.' between the "<devicename>.<collector>", then removes the "<devicename>." part
            var index = this.name.indexOf(".");
            if (index >= 0)
                this.name = this.name.substr(index + 1);
        }
        return CollectorInfo;
    }());
    exports.CollectorInfo = CollectorInfo;
});
//# sourceMappingURL=collectorinfo.js.map