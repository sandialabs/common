define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    // See Models.cs
    var ValueInfo = /** @class */ (function () {
        function ValueInfo(ivi) {
            this.deviceID = ivi.deviceID;
            this.collectorType = ivi.collectorType;
            this.value = ivi.value;
            this.timestamp = new Date(ivi.timestamp);
        }
        ValueInfo.prototype.parseValue = function () {
            var value = [];
            if (this.value)
                value = JSON.parse(this.value)['Value'];
            return value;
        };
        return ValueInfo;
    }());
    exports.ValueInfo = ValueInfo;
});
//# sourceMappingURL=valueinfo.js.map