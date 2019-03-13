/// <reference types="angular" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var AutoUpdater = /** @class */ (function () {
        // frequency: in milliseconds
        // callback: the function to call (a static-type function, not an object method), that takes
        // one parameter of type T
        // t: something of type T to pass to the parameter of callback
        // $interval: the angular interval service
        function AutoUpdater(frequency, callback, t, $interval) {
            this.frequency = frequency;
            this.callback = callback;
            this.t = t;
            this.$interval = $interval;
            this.autoUpdateTimer = null;
        }
        AutoUpdater.prototype.start = function () {
            if (this.autoUpdateTimer !== null)
                return;
            this.autoUpdateTimer = this.$interval(this.callback, this.frequency, 0, true, this.t);
            // Call it once so it get's called immediately upon starting. Setting up the timer doesn't
            // call it until the interval has expired.
            this.callback(this.t);
        };
        AutoUpdater.prototype.stop = function () {
            if (this.autoUpdateTimer === null)
                return;
            this.$interval.cancel(this.autoUpdateTimer);
            this.autoUpdateTimer = null;
        };
        AutoUpdater.prototype.changeFrequency = function (frequency) {
            this.stop();
            this.frequency = frequency;
            this.start();
        };
        return AutoUpdater;
    }());
    exports.AutoUpdater = AutoUpdater;
});
//# sourceMappingURL=autoupdater.js.map