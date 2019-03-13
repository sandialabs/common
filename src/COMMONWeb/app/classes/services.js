define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Services = /** @class */ (function () {
        function Services(s) {
            this.services = s.services;
            this.timestamp = new Date(s.timestamp);
        }
        return Services;
    }());
    exports.Services = Services;
});
//# sourceMappingURL=services.js.map