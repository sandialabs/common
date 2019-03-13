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
    var UPSSnapshot = /** @class */ (function () {
        function UPSSnapshot(data) {
            var upsData = JSON.parse(data.value)['Value'];
            this.timestamp = new Date(data.timeStamp);
            this.name = (upsData['Name']);
            this.upsStatus = (upsData['Status']);
            this.batteryStatus = (upsData['BatteryStatus']);
            this.batteryStatusInt = (parseInt(upsData['BatteryStatusInt']));
            this.estimatedRunTimeInMinutes = (parseInt(upsData['EstimatedRunTime']));
            this.estimatedChargeRemainingPercentage = (parseInt(upsData['EstimatedChargeRemaining']));
            this.runningOnUPS = false;
            if (this.batteryStatusInt == 1) {
                this.runningOnUPS = true;
            }
        }
        return UPSSnapshot;
    }());
    exports.UPSSnapshot = UPSSnapshot;
    var UPSStatus = /** @class */ (function () {
        function UPSStatus(data) {
            this.upsData = [];
            if (!data || data.length === 0)
                return;
            for (var i = 0; i < data.length; ++i) {
                var snapshot = new UPSSnapshot(data[i]);
                this.upsData.push(snapshot);
            }
            this.current = this.upsData[this.upsData.length - 1];
        }
        return UPSStatus;
    }());
    exports.UPSStatus = UPSStatus;
    var UPSChartBridge = /** @class */ (function (_super) {
        __extends(UPSChartBridge, _super);
        function UPSChartBridge(upsDataSource, factory) {
            var _this = _super.call(this, upsDataSource, factory) || this;
            _this.upsDataSource = upsDataSource;
            _this.factory = factory;
            return _this;
        }
        UPSChartBridge.prototype.watchCollection = function () {
            return this.upsDataSource.upsData;
        };
        UPSChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.upsDataSource.upsData)
                return;
            for (var i = 0; i < this.upsDataSource.upsData.length; ++i) {
                var c = this.upsDataSource.upsData[i];
                this.addData(UPSChartBridge.convert(c));
            }
        };
        UPSChartBridge.convert = function (u) {
            return [new chartjs_1.ChartJSDataPoint({ x: u.timestamp, y: u.runningOnUPS ? 1.0 : 0.0 })];
        };
        return UPSChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.UPSChartBridge = UPSChartBridge;
});
//# sourceMappingURL=ups.js.map