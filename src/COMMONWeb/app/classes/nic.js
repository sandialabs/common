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
    var NICSnapshot = /** @class */ (function () {
        function NICSnapshot(data) {
            var nic = JSON.parse(data.value)['Value'];
            this.timestamp = new Date(data.timeStamp);
            this.bps = Math.round(parseFloat(nic['BPS']));
            this.capacity = Math.round(parseFloat(nic['Capacity']));
            this.percent = Math.round(parseFloat(nic['Avg']));
        }
        return NICSnapshot;
    }());
    exports.NICSnapshot = NICSnapshot;
    var NICData = /** @class */ (function () {
        function NICData() {
            this.current = this.peak = null;
            this.nicData = [];
        }
        NICData.prototype.update = function (data) {
            this.nicData = [];
            this.current = this.peak = null;
            if (!data || data.length === 0)
                return;
            // Default capacity is 1GB in bytes so we can handle old data that didn't include the capacity.
            // If the actual capacity is different, we'll update this.
            var capacity = 125000000;
            for (var i = 0; i < data.length; ++i) {
                var snapshot = new NICSnapshot(data[i]);
                if (isNaN(snapshot.bps) === false) {
                    this.nicData.push(snapshot);
                    if (this.peak === null || snapshot.bps > this.peak.bps)
                        this.peak = snapshot;
                }
            }
            this.current = this.nicData[this.nicData.length - 1];
        };
        return NICData;
    }());
    exports.NICData = NICData;
    var NICChartBridge = /** @class */ (function (_super) {
        __extends(NICChartBridge, _super);
        function NICChartBridge(nicDataSource, settings, factory) {
            var _this = _super.call(this, nicDataSource, factory) || this;
            _this.nicDataSource = nicDataSource;
            _this.settings = settings;
            _this.factory = factory;
            _this.settings.valueRange = [0, 100];
            return _this;
        }
        NICChartBridge.prototype.watchCollection = function () {
            return this.nicDataSource.nicData;
        };
        NICChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.nicDataSource.nicData)
                return;
            for (var i = 0; i < this.nicDataSource.nicData.length; ++i) {
                var c = this.nicDataSource.nicData[i];
                this.addData(NICChartBridge.convert(c));
            }
        };
        NICChartBridge.convert = function (n) {
            return [new chartjs_1.ChartJSDataPoint({ x: n.timestamp, y: n.percent })];
        };
        return NICChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.NICChartBridge = NICChartBridge;
});
//# sourceMappingURL=nic.js.map