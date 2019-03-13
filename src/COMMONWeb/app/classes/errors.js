define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ErrorInfo = /** @class */ (function () {
        function ErrorInfo(data) {
            this.message = data.message;
            this.timestamp = new Date(data.timestamp);
            this.count = data.count;
            this.firstTimestamp = this.lastTimestamp = null;
            if (data.firstTimestamp !== null)
                this.firstTimestamp = new Date(data.firstTimestamp);
            if (data.lastTimestamp !== null)
                this.lastTimestamp = new Date(data.lastTimestamp);
        }
        return ErrorInfo;
    }());
    var ErrorManager = /** @class */ (function () {
        function ErrorManager(data) {
            this.errors = [];
            if (data === undefined || data === null || data.errors.length === 0)
                return;
            for (var i = 0; i < data.errors.length; ++i) {
                var errorInfo = new ErrorInfo(data.errors[i]);
                this.errors.push(errorInfo);
            }
        }
        return ErrorManager;
    }());
    exports.ErrorManager = ErrorManager;
});
//# sourceMappingURL=errors.js.map