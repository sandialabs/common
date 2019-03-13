define(["require", "exports", "../../lib/moment/min/moment-with-locales"], function (require, exports, moment) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var URL = /** @class */ (function () {
        function URL(url, d, http) {
            this.url = url;
            this.d = d;
            this.http = http;
            this.urlID = URL.nextID++;
            this.func = this.get;
        }
        URL.prototype.get = function () {
            var t = this;
            //console.log("Executing: " + t.toString());
            this.http.get(t.url)
                .then(function (response) {
                t.d.resolve(response.data);
                t.urlQueue.remove(t);
            })
                .catch(function (reason) {
                console.log("Rejected " + t.toString() + ": " + reason.toString());
                t.d.reject(reason);
                t.urlQueue.remove(t);
            });
        };
        URL.prototype.toString = function () {
            return "(" + this.urlID.toString() + ") " + this.url;
        };
        URL.nextID = 1;
        return URL;
    }());
    var URLQueue = /** @class */ (function () {
        function URLQueue(maxLength, timeoutInS) {
            this.maxLength = maxLength;
            this.held = [];
            this.executing = [];
            if (timeoutInS)
                this.timeoutInMS = timeoutInS * 1000;
        }
        URLQueue.prototype.push = function (url) {
            url.urlQueue = this;
            url.requestedAt = new Date();
            this.held.push(url);
            this.executeHeldURLs();
        };
        URLQueue.prototype.executeHeldURLs = function () {
            this.clearTimedOut();
            while (this.held.length > 0 && this.executing.length < this.maxLength) {
                var url = this.held.shift();
                url.transmittedAt = new Date();
                var ms = Math.abs(url.transmittedAt.getTime() - url.requestedAt.getTime());
                // if(ms > 500)
                //     console.log("Held for " + ms.toString() + " ms: " + url.toString());
                this.executing.push(url);
                url.get();
            }
        };
        URLQueue.prototype.remove = function (url) {
            for (var i = 0; i < this.executing.length; ++i) {
                if (this.executing[i].urlID == url.urlID) {
                    var url_1 = this.executing[i];
                    var ms = Math.abs(new Date().getTime() - url_1.transmittedAt.getTime());
                    // if(ms > 500)
                    //     console.log("Completed after " + ms.toString() + " ms: " + url.toString());
                    this.executing.splice(i, 1);
                    break;
                }
            }
        };
        URLQueue.prototype.clearTimedOut = function () {
            if (!this.timeoutInMS)
                return;
            var now = new Date().getTime();
            for (var i = 0; i < this.executing.length; ++i) {
                var doomed = this.executing[i];
                var ms = Math.abs(now - doomed.transmittedAt.getTime());
                if (ms >= this.timeoutInMS) {
                    // console.log("Doomed after " + ms.toString() + " ms: " + doomed.toString());
                    this.executing.splice(i--, 1);
                }
            }
        };
        return URLQueue;
    }());
    var DataService = /** @class */ (function () {
        function DataService($http, $q) {
            var _this = this;
            this.$http = $http;
            this.$q = $q;
            this.getConfiguration = function () { return _this.get('configurationdata'); };
            this.getDeviceStatus = function () { return _this.get('devicestatus'); };
            this.getAllCollectors = function () { return _this.get('allcollectors'); };
            this.getNetworkStatus = function (starting, ending) { return _this.get('networkstatus', DataService.getDatesTuple(starting, ending)); };
            this.collectNow = function (collectorID) { return _this.post('collectnow', [collectorID]); };
            this.getProcessHistory = function (id, processName) { return _this.get('processhistory', [id, processName]); };
            this.getAllProcesses = function (id, starting, ending) { return _this.get('allprocesses', DataService.getIDAndDatesTuple(id, starting, ending)); };
            this.getAppHistory = function (id, app) { return _this.get('apphistory', [id, app]); };
            this.getAppChanges = function (deviceID, starting, ending) { return _this.get('xyz', DataService.getIDAndDatesTuple(deviceID, starting, ending)); };
            // The database name might have some invalid characters, so let's encode it and let the server decode it
            this.getDatabaseHistory = function (id, databaseName) { return _this.get('databasehistory', [id, btoa(databaseName)]); };
            this.getServicesData = function (id) { return _this.get('servicesdata', [id]); };
            this.getDeviceDetails = function (id) { return _this.get('devicedetails', [id]); };
            this.getMachineData = function (id, parts, starting, ending) { return _this.get('machinedata', DataService.getIDAndDatesAndMachinePartsTuple(id, parts, starting, ending)); };
            this.getSubReport = function (id, types, starting, ending) { return _this.get('machinesubreport', DataService.getIDAndDAtesAndReportPartsTuple(id, types, starting, ending)); };
            //console.log("NewDataService.constructor");
            this.urlQueue = new URLQueue(8, 120);
        }
        // The problem is that Date.toIsoString() always formats the date to UTC, or like this:
        // 2019-02-03T12:23:34.123Z
        // The Z means UTC (zulu)
        // But the dates in the SQLite database are stored as strings, not actual dates, so if we do a query
        // using the zulu time it doesn't quite match up:
        // 2019-02-03T06:23:34.123-06:00 != 2019-02-03T12:23:34.123Z
        // even though those are the same times. The left one is the local
        // time with the offset to GMT, the other is in GMT.
        // The time used in the DB is the C# DateTimeOffset.ToString("o"), which puts the
        // local time with the offset to GMT.
        // So, for consistency, we need to use the same format that DateTimeOffset.ToString("o")
        // uses, but there isn't one built in to JavaScript.
        //
        // Fortunately, moment.format() does exactly what we want. Well, almost. format() doesn't
        // put the milliseconds in there, so we'll have to
        DataService.getIDAndDatesTuple = function (id, starting, ending) {
            var s = starting == null ? null : moment(starting).format(DataService.dateFormat);
            var e = ending == null ? null : moment(ending).format(DataService.dateFormat);
            return [id, s, e];
        };
        DataService.getDatesTuple = function (starting, ending) {
            var s = starting == null ? null : moment(starting).format(DataService.dateFormat);
            var e = ending == null ? null : moment(ending).format(DataService.dateFormat);
            return [s, e];
        };
        DataService.getIDAndDatesAndMachinePartsTuple = function (id, parts, starting, ending) {
            var mp = {
                machineParts: parts
            };
            var json = JSON.stringify(mp.machineParts);
            var t1 = DataService.getIDAndDatesTuple(id, starting, ending);
            return [t1[0], json, t1[1], t1[2]];
        };
        DataService.getIDAndDAtesAndReportPartsTuple = function (id, types, starting, ending) {
            var mp = {
                reportTypes: types
            };
            var json = JSON.stringify(mp.reportTypes);
            var t1 = DataService.getIDAndDatesTuple(id, starting, ending);
            return [t1[0], json, t1[1], t1[2]];
        };
        DataService.prototype.get = function (method, data) {
            var url = '/' + method;
            if (data) {
                for (var i = 0; i < data.length; ++i)
                    url += '/' + data[i];
            }
            // Push the URL into the queue, and it will automatically attempt
            // to execute it if there aren't too many already executing.
            var d = this.$q.defer();
            var u = new URL(url, d, this.$http);
            this.urlQueue.push(u);
            return d.promise;
        };
        DataService.prototype.post = function (method, data) {
            var url = '/' + method + '/' + data;
            // console.log(url);
            var d = this.$q.defer();
            this.$http.post(encodeURI(url), data)
                .then(function (response) {
                d.resolve(response.data);
            })
                .catch(function (reason) {
                d.reject(reason);
            });
            return d.promise;
        };
        DataService.Factory = function () {
            var factory = function ($http, $q) {
                return new DataService($http, $q);
            };
            factory.$inject = ['$http', '$q'];
            return factory;
        };
        // kk is 00-23 hours
        DataService.dateFormat = "YYYY-MM-DDTkk:mm:ss.SSSZ";
        return DataService;
    }());
    exports.DataService = DataService;
});
//# sourceMappingURL=data.service.js.map