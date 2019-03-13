define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var PromiseKeeper = /** @class */ (function () {
        function PromiseKeeper($q) {
            this.$q = $q;
            this.held = [];
        }
        PromiseKeeper.prototype.push = function (d) {
            this.held.push(d);
        };
        PromiseKeeper.prototype.resolve = function (t) {
            while (this.held.length > 0) {
                var d = this.held.shift();
                d.resolve(t);
            }
        };
        return PromiseKeeper;
    }());
    exports.PromiseKeeper = PromiseKeeper;
});
//# sourceMappingURL=promisekeeper.js.map