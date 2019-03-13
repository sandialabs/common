define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DeviceApplications = /** @class */ (function () {
        function DeviceApplications(data) {
            this.deviceID = data.deviceID;
            this.applications = data.applications;
            this.timestamp = new Date(data.timestamp);
        }
        return DeviceApplications;
    }());
    var Snapshot = /** @class */ (function () {
        function Snapshot(data) {
            this.version = data.version;
            this.timestamp = new Date(data.timestamp);
        }
        return Snapshot;
    }());
    // See Database/Models.cs
    var ApplicationHistory = /** @class */ (function () {
        function ApplicationHistory() {
            this.name = "";
            this.history = [];
        }
        ApplicationHistory.prototype.update = function (data) {
            this.name = data.name;
            this.history = [];
            for (var i = 0; i < data.history.length; ++i) {
                var snapshot = new Snapshot(data.history[i]);
                this.history.push(snapshot);
            }
        };
        return ApplicationHistory;
    }());
    var ApplicationsHistoryMap = /** @class */ (function () {
        function ApplicationsHistoryMap(map) {
            this.history = {};
            var keys = Object.keys(map);
            for (var i = 0; i < keys.length; ++i) {
                var name_1 = keys[i];
                var history_1 = map[name_1];
                var appHistory = new ApplicationHistory();
                appHistory.update(history_1);
                this.history[name_1] = appHistory;
            }
        }
        return ApplicationsHistoryMap;
    }());
    var AllApplicationsHistory = /** @class */ (function () {
        function AllApplicationsHistory(history) {
            this.history = new ApplicationsHistoryMap(history.history);
            this.apps = Object.keys(this.history.history);
            this.apps.sort();
        }
        return AllApplicationsHistory;
    }());
    exports.AllApplicationsHistory = AllApplicationsHistory;
    var ApplicationManager = /** @class */ (function () {
        function ApplicationManager(data, dataService) {
            this.applications = new DeviceApplications(data);
            this.dataService = dataService;
            //this.values = [];
            this.applicationHistory = null;
        }
        ApplicationManager.prototype.onSelectApplication = function (app) {
            var t = this;
            this.dataService.getAppHistory(this.applications.deviceID, app)
                .then(function (data) {
                if (!t.applicationHistory)
                    t.applicationHistory = new ApplicationHistory();
                t.applicationHistory.update(data);
            });
        };
        return ApplicationManager;
    }());
    exports.ApplicationManager = ApplicationManager;
});
//# sourceMappingURL=applications.js.map