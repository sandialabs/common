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
define(["require", "exports", "../charts/chartjs", "../charts/chartbridge"], function (require, exports, chartjs_1, chartbridge_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ProcessHistory = /** @class */ (function () {
        function ProcessHistory() {
            this.deviceID = -1;
            this.processName = "";
            this.details = [];
        }
        ProcessHistory.prototype.update = function (history) {
            this.deviceID = history.deviceID;
            this.processName = history.processName;
            this.details = [];
            var mb = 1000000;
            for (var i = 0; i < history.details.length; ++i) {
                var tup = history.details[i];
                var detail = [new Date(tup[0]), tup[1], tup[2] / mb];
                this.details.push(detail);
            }
        };
        return ProcessHistory;
    }());
    exports.ProcessHistory = ProcessHistory;
    var ProcessHistoryChartBridge = /** @class */ (function (_super) {
        __extends(ProcessHistoryChartBridge, _super);
        function ProcessHistoryChartBridge(processHistoryDataSource, factory) {
            var _this = _super.call(this, processHistoryDataSource, factory) || this;
            _this.processHistoryDataSource = processHistoryDataSource;
            _this.factory = factory;
            var line2 = new chartjs_1.ChartJSData(new chartjs_1.Color(chartjs_1.EChartJSColors.Green, chartjs_1.EChartJSColors.LightGreen));
            _this.chartData.push(line2);
            return _this;
        }
        ProcessHistoryChartBridge.prototype.watchCollection = function () {
            return this.processHistoryDataSource.details;
        };
        ProcessHistoryChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.processHistoryDataSource.details)
                return;
            for (var i = 0; i < this.processHistoryDataSource.details.length; ++i) {
                var c = this.processHistoryDataSource.details[i];
                this.addData(ProcessHistoryChartBridge.convert(c));
            }
        };
        ProcessHistoryChartBridge.convert = function (p) {
            return [new chartjs_1.ChartJSDataPoint({ x: p[0], y: p[1] }), new chartjs_1.ChartJSDataPoint({ x: p[0], y: p[2] })];
        };
        return ProcessHistoryChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.ProcessHistoryChartBridge = ProcessHistoryChartBridge;
    var ProcessManager = /** @class */ (function () {
        function ProcessManager(data, dataService) {
            this.processes = data;
            this.dataService = dataService;
            this.values = [];
            this.processHistory = null;
            var keys = Object.keys(this.processes.cpuToProcesses);
            // Sort in reverse order
            keys.sort(function (a, b) {
                var a2 = parseInt(a);
                var b2 = parseInt(b);
                return b2 - a2;
            });
            for (var i = 0; i < keys.length; ++i) {
                var key = keys[i];
                var procs = this.processes.cpuToProcesses[key];
                for (var j = 0; j < procs.length; ++j) {
                    this.values.push([key, procs[j]]);
                }
            }
        }
        ProcessManager.prototype.onSelectProcess = function (process) {
            var t = this;
            this.dataService.getProcessHistory(this.processes.deviceID, process)
                .then(function (data) {
                if (!t.processHistory)
                    t.processHistory = new ProcessHistory();
                t.processHistory.update(data);
            });
        };
        return ProcessManager;
    }());
    exports.ProcessManager = ProcessManager;
});
//# sourceMappingURL=processes.js.map