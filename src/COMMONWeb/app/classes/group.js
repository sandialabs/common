define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Group = /** @class */ (function () {
        //panelIsOpen: boolean;
        function Group(group) {
            this.id = group.id;
            this.name = group.name;
            this.devices = [];
            this.hasAlarm = false;
            //this.panelIsOpen = false;
        }
        Group.prototype.addDevice = function (di) {
            this.devices.push(di);
            this.devices.sort(function (a, b) {
                return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
            });
        };
        Group.prototype.findDeviceFromName = function (name) {
            var d = null;
            for (var i = 0; d === null && i < this.devices.length; ++i) {
                var device = this.devices[i];
                if (name.localeCompare(device.name) === 0)
                    d = device;
            }
            return d;
        };
        Group.prototype.findDeviceFromCollectorID = function (collectorID) {
            var device = null;
            for (var i = 0; device === null && i < this.devices.length; ++i) {
                var d = this.devices[i];
                var c = d.getCollector(collectorID);
                if (c)
                    device = d;
            }
            return device;
        };
        Group.prototype.updateStatusFlags = function () {
            this.hasAlarm = false;
            for (var i = 0; this.hasAlarm === false && i < this.devices.length; ++i) {
                var d = this.devices[i];
                this.hasAlarm = this.hasAlarm || d.alarms.length > 0;
            }
        };
        return Group;
    }());
    exports.Group = Group;
});
//# sourceMappingURL=group.js.map