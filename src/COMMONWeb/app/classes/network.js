/// <reference types="angular" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "./autoupdater", "../charts/chartjs", "../charts/chartbridge"], function (require, exports, autoupdater_1, chartjs_1, chartbridge_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var PingAttempt = /** @class */ (function () {
        function PingAttempt(attempt) {
            this.successful = attempt.successful;
            this.timestamp = new Date(attempt.timestamp);
            this.responseTimeMS = attempt.responseTimeMS;
        }
        return PingAttempt;
    }());
    exports.PingAttempt = PingAttempt;
    var NetworkStatus = /** @class */ (function () {
        function NetworkStatus(status) {
            this.name = status.name;
            this.deviceID = status.deviceID;
            this.ipAddress = status.ipAddress;
            this.attempts = [];
            this.avgResponseTimeMS =
                this.percentSuccessfulPings =
                    this.totalPingAttempts =
                        this.totalSuccessfulPingAttempts = 0;
            this.dateMostRecentStateChange = null;
            this.update(status);
        }
        NetworkStatus.prototype.update = function (n) {
            this.successfulPing = n.successfulPing;
            this.hasBeenPinged = n.hasBeenPinged;
            // Because the JavaScript dates and C# dates are slightly different,
            // when we convert from one to the other we'll lose some of the
            // milliseconds. This is enough so that when the query to get new ping
            // attempts occurs, it will almost surely get one out of the DB that
            // we've already seen here.
            // So, before we make a new PingAttempt and push it into the attempts array,
            // make sure it's not already there. Compare the ping attempt times, and since
            // we'll be comparing JavaScript dates we should be fine.
            if (n.attempts.length > 0) {
                var totalResponseTimeMS = 0;
                for (var i = 0; i < n.attempts.length; ++i) {
                    var pa = new PingAttempt(n.attempts[i]);
                    var exists = false;
                    var paTime = pa.timestamp.getTime();
                    // Let's go in reverse order for a bit better performance since the new
                    // PingAttempt will likely be coming in after the most recent in this.attempts.
                    for (var j = this.attempts.length - 1; exists == false && j >= 0; --j) {
                        var old = this.attempts[j];
                        exists = paTime - old.timestamp.getTime() == 0;
                    }
                    if (exists == false) {
                        if (pa.successful)
                            this.totalSuccessfulPingAttempts += 1;
                        totalResponseTimeMS += pa.responseTimeMS;
                        this.attempts.push(pa);
                    }
                }
                this.totalPingAttempts = n.attempts.length;
                this.avgResponseTimeMS = this.totalPingAttempts > 0 ? totalResponseTimeMS / this.totalPingAttempts : 0;
                this.percentSuccessfulPings = this.totalPingAttempts > 0 ? this.totalSuccessfulPingAttempts / this.totalPingAttempts * 100.0 : 0;
                // The attempts come in "chunks" like the most-recent two weeks, then two-weeks before that, etc.
                // This means as the attempts are pushed on to the attempts array they won't be in order so
                // we need to sort.
                this.attempts.sort(function (x, y) {
                    var a = x.timestamp.getTime();
                    var b = y.timestamp.getTime();
                    return a - b;
                });
            }
            if (n.dateSuccessfulPingOccurred && n.dateSuccessfulPingOccurred !== "")
                this.dateSuccessfulPingOccurred = new Date(n.dateSuccessfulPingOccurred);
            this.datePingAttempted = new Date(n.datePingAttempted);
        };
        return NetworkStatus;
    }());
    exports.NetworkStatus = NetworkStatus;
    var NetworkChartBridge = /** @class */ (function (_super) {
        __extends(NetworkChartBridge, _super);
        function NetworkChartBridge(networkDataSource, settings, factory) {
            var _this = _super.call(this, networkDataSource, factory, NetworkChartBridge.green) || this;
            _this.networkDataSource = networkDataSource;
            _this.settings = settings;
            _this.factory = factory;
            _this.settings.valueRange = [0, 1000];
            return _this;
        }
        NetworkChartBridge.prototype.watchCollection = function () {
            return this.networkDataSource.attempts;
        };
        NetworkChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.networkDataSource || !this.networkDataSource.attempts)
                return;
            for (var i = 0; i < this.networkDataSource.attempts.length; ++i) {
                var a = this.networkDataSource.attempts[i];
                this.addData(NetworkChartBridge.convert(a));
            }
        };
        NetworkChartBridge.convert = function (pa) {
            var c = (pa.responseTimeMS >= 500) ? NetworkChartBridge.red : NetworkChartBridge.green;
            return [new chartjs_1.ChartJSDataPoint({ x: pa.timestamp, y: pa.responseTimeMS }, c)];
        };
        NetworkChartBridge.red = new chartjs_1.Color(chartjs_1.EChartJSColors.Red, chartjs_1.EChartJSColors.LightRed);
        NetworkChartBridge.green = new chartjs_1.Color(chartjs_1.EChartJSColors.Green, chartjs_1.EChartJSColors.LightGreen);
        return NetworkChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.NetworkChartBridge = NetworkChartBridge;
    var NetworkRetrieval = /** @class */ (function () {
        function NetworkRetrieval(config) {
            this.config = config;
            this.startingDate = this.endingDate = null;
        }
        // Returns the date range that should be retrieved next
        // When called the first time, it will go back from the
        // current time. Each subsequent call will go back from the
        // previous start.
        NetworkRetrieval.prototype.retrieveNext = function () {
            var start = null;
            var end = null;
            // This will move the 'window' back by 15 days
            // each time we gather the data.
            if (this.startingDate == null) {
                if (this.config.mostRecentData)
                    this.startingDate = this.config.mostRecentData;
                else
                    this.startingDate = new Date();
                this.endingDate = null;
                end = null;
            }
            else {
                this.endingDate = this.startingDate;
                end = this.endingDate;
            }
            this.startingDate = new Date(this.startingDate.getTime() - NetworkRetrieval.msPerWindow);
            start = this.startingDate;
            console.log("Retrieving " + (start ? start.toDateString() : "null") + " to " + (end ? end.toDateString() : "null"));
            return [start, end];
        };
        NetworkRetrieval.msPerDay = 24 * 60 * 60 * 1000;
        NetworkRetrieval.daysPerWindow = 15;
        NetworkRetrieval.msPerWindow = NetworkRetrieval.daysPerWindow * NetworkRetrieval.msPerDay;
        return NetworkRetrieval;
    }());
    // Holds all of the NetworkStatus objects, and keeps track of the most-recent poll time in
    // maxDate
    var Network = /** @class */ (function () {
        function Network(dataService, deviceManagerService, $interval, config) {
            this.dataService = dataService;
            this.deviceManagerService = deviceManagerService;
            this.$interval = $interval;
            this.config = config;
            this.maxDate = null;
            this.idToData = {};
            this.ipAddressToData = {};
            this.data = [];
            this.dataService = dataService;
            this.selected = null;
            this.deviceManagerService = deviceManagerService;
            // Get the new network data every 10 seconds
            this.autoUpdater = new autoupdater_1.AutoUpdater(10000, Network.gatherData, this, $interval);
            this.retrieval = new NetworkRetrieval(this.config);
            Network.gatherData(this);
        }
        Network.prototype.getNetworkStatusFromID = function (id) {
            return this.idToData[id];
        };
        Network.prototype.getNetworkStatusFromIPAddress = function (name) {
            return this.ipAddressToData[name];
        };
        Network.prototype.updateData = function (data) {
            if (!data)
                return;
            for (var i = 0; i < data.length; ++i) {
                var d = data[i];
                var existing = this.getNetworkStatusFromIPAddress(d.ipAddress);
                var ns = null;
                if (!existing) {
                    ns = new NetworkStatus(d);
                    this.data.push(ns);
                    this.idToData[ns.deviceID] = ns;
                    this.ipAddressToData[ns.ipAddress] = ns;
                }
                else {
                    existing.update(d);
                    ns = existing;
                }
                if (this.maxDate === null || ns.datePingAttempted > this.maxDate)
                    this.maxDate = ns.datePingAttempted;
            }
        };
        Network.prototype.startAutomaticUpdate = function () {
            this.autoUpdater.start();
        };
        Network.prototype.stopAutomaticUpdate = function () {
            this.autoUpdater.stop();
        };
        // This is called when the user wants to see more data
        // on the chart.
        Network.prototype.getEarlierRange = function () {
            this.gather(this.retrieval.retrieveNext());
        };
        Network.prototype.gather = function (range) {
            var t = this;
            this.dataService.getNetworkStatus(range[0], range[1])
                .then(function (data) {
                if (!data)
                    return;
                t.updateData(data);
                t.deviceManagerService.get()
                    .then(function (deviceManager) {
                    deviceManager.updateNetwork(t);
                });
            });
        };
        // This is called at startup, and periodically to get
        // the most recent data. When starting up, gather
        // a couple of weeks of data, then after that just
        // get the data that might have been collected since
        // startup.
        Network.gatherData = function (t) {
            var range = [null, null];
            if (t.retrieval.startingDate == null)
                range = t.retrieval.retrieveNext();
            else
                range = [t.maxDate, null];
            t.gather(range);
        };
        return Network;
    }());
    exports.Network = Network;
});
//# sourceMappingURL=network.js.map