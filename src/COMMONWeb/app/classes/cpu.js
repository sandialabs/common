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
    var CPUSnapshot = /** @class */ (function () {
        function CPUSnapshot(data) {
            var currentCPU = JSON.parse(data.value)['Value'];
            this.timestamp = new Date(data.timeStamp);
            this.percent = Math.round(parseFloat(currentCPU));
        }
        return CPUSnapshot;
    }());
    exports.CPUSnapshot = CPUSnapshot;
    var CPUData = /** @class */ (function () {
        function CPUData() {
            this.current = this.peak = null;
            this.cpuData = [];
        }
        CPUData.prototype.update = function (data) {
            this.current = this.peak = null;
            this.cpuData = [];
            if (!data || data.length === 0)
                return;
            for (var i = 0; i < data.length; ++i) {
                var snapshot = new CPUSnapshot(data[i]);
                if (isNaN(snapshot.percent) === false) {
                    this.cpuData.push(snapshot);
                    if (this.peak === null || snapshot.percent > this.peak.percent)
                        this.peak = snapshot;
                }
            }
            this.current = this.cpuData[this.cpuData.length - 1];
        };
        return CPUData;
    }());
    exports.CPUData = CPUData;
    var CPUChartBridge = /** @class */ (function (_super) {
        __extends(CPUChartBridge, _super);
        function CPUChartBridge(cpuDataSource, settings, factory) {
            var _this = _super.call(this, cpuDataSource, factory) || this;
            _this.cpuDataSource = cpuDataSource;
            _this.settings = settings;
            _this.factory = factory;
            //console.log("CPUChartBridge constructor");
            _this.settings.valueRange = [0, 100];
            return _this;
        }
        CPUChartBridge.prototype.watchCollection = function () {
            return this.cpuDataSource.cpuData;
        };
        CPUChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.cpuDataSource.cpuData)
                return;
            for (var i = 0; i < this.cpuDataSource.cpuData.length; ++i) {
                var c = this.cpuDataSource.cpuData[i];
                this.addData(CPUChartBridge.convert(c));
            }
        };
        CPUChartBridge.convert = function (c) {
            return [new chartjs_1.ChartJSDataPoint({ x: c.timestamp, y: c.percent })];
        };
        return CPUChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.CPUChartBridge = CPUChartBridge;
});
//# sourceMappingURL=cpu.js.map