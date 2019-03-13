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
define(["require", "exports", "../charts/chartjs", "./valueinfo", "../charts/chartbridge"], function (require, exports, chartjs_1, valueinfo_1, chartbridge_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DatabaseDetail = /** @class */ (function () {
        function DatabaseDetail(detail) {
            this.timestamp = new Date(detail.timestamp);
            this.sizeInMB = detail.sizeInMB;
            this.sizeInGB = this.sizeInMB / 1000;
        }
        return DatabaseDetail;
    }());
    exports.DatabaseDetail = DatabaseDetail;
    var DatabaseHistory = /** @class */ (function () {
        function DatabaseHistory() {
            this.deviceID = -1;
            this.databaseName = "";
            this.details = [];
            this.maxSizeInMB = 0;
        }
        DatabaseHistory.prototype.update = function (data) {
            this.deviceID = data.deviceID;
            this.databaseName = data.databaseName;
            this.details = [];
            this.maxSizeInMB = 0;
            for (var i = 0; i < data.details.length; ++i) {
                var detail = new DatabaseDetail(data.details[i]);
                this.details.push(detail);
                this.maxSizeInMB = Math.max(this.maxSizeInMB, detail.sizeInMB);
            }
        };
        return DatabaseHistory;
    }());
    exports.DatabaseHistory = DatabaseHistory;
    var DatabaseHistoryChartBridge = /** @class */ (function (_super) {
        __extends(DatabaseHistoryChartBridge, _super);
        function DatabaseHistoryChartBridge(dbDataSource, factory) {
            var _this = _super.call(this, dbDataSource, factory) || this;
            _this.dbDataSource = dbDataSource;
            return _this;
        }
        DatabaseHistoryChartBridge.prototype.watchCollection = function () {
            if (this.dbDataSource)
                return this.dbDataSource.details;
            return null;
        };
        DatabaseHistoryChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.dbDataSource || !this.dbDataSource.details)
                return;
            for (var i = 0; i < this.dbDataSource.details.length; ++i) {
                var d = this.dbDataSource.details[i];
                this.addData(DatabaseHistoryChartBridge.convert(d));
            }
        };
        DatabaseHistoryChartBridge.convert = function (d) {
            return [new chartjs_1.ChartJSDataPoint({ x: d.timestamp, y: d.sizeInMB })];
        };
        return DatabaseHistoryChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.DatabaseHistoryChartBridge = DatabaseHistoryChartBridge;
    var DatabaseManager = /** @class */ (function () {
        function DatabaseManager(data, dataService) {
            this.databases = new valueinfo_1.ValueInfo(data);
            this.dataService = dataService;
            this.values = [];
            this.databaseHistory = null;
            var dict = JSON.parse(this.databases.value)['Value'];
            var sizeDictionary = {};
            for (var i = 0; i < dict.length; ++i) {
                var databaseInfo = dict[i];
                var size = databaseInfo["Size"];
                var dbs = [];
                if (sizeDictionary.hasOwnProperty(size)) {
                    dbs = sizeDictionary[size];
                }
                else {
                    sizeDictionary[size] = dbs;
                }
                dbs.push(databaseInfo);
            }
            // OK, now we have another dictionary mapping sizes to the databases that
            // have that size. Let's create a new array with the different size, and
            // sort that in reverse numeric order so the largest sizes are earliest in the list.
            var sizes = Object.keys(sizeDictionary);
            sizes.sort(function (x, y) {
                var xi = parseInt(x, 10);
                var yi = parseInt(y, 10);
                // Reverse sort
                return yi - xi;
            });
            // Now we can walk through sizes, getting the databases with that size out
            // of sizeDictionary, then fill up an array mapping the size to the database.
            // That array is what will be walked for display.
            for (var i = 0; i < sizes.length; ++i) {
                var s = parseInt(sizes[i]);
                var databases = sizeDictionary[s];
                for (var j = 0; j < databases.length; ++j) {
                    var info = databases[j];
                    this.values.push({ "size": s, "name": info.Name });
                }
            }
        }
        DatabaseManager.prototype.onSelectProcess = function (process) {
            var t = this;
            this.dataService.getDatabaseHistory(this.databases.deviceID, process)
                .then(function (data) {
                if (!t.databaseHistory)
                    t.databaseHistory = new DatabaseHistory();
                t.databaseHistory.update(data);
            });
        };
        return DatabaseManager;
    }());
    exports.DatabaseManager = DatabaseManager;
});
//# sourceMappingURL=database.js.map