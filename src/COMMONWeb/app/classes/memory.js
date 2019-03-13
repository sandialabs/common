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
    var MemorySnapshot = /** @class */ (function () {
        function MemorySnapshot(data) {
            var currentMemory = JSON.parse(data.value)['Value'];
            this.timestamp = new Date(data.timeStamp);
            this.capacity = (parseFloat(currentMemory['Memory Capacity']) / MemorySnapshot.byteSize);
            this.free = (parseFloat(currentMemory['Free Memory']) / MemorySnapshot.byteSize);
            this.used = (parseFloat(currentMemory['Memory Used']) / MemorySnapshot.byteSize);
            this.percentUsed = this.used / this.capacity * 100;
        }
        MemorySnapshot.byteSize = 0x40000000; // 1GB
        return MemorySnapshot;
    }());
    exports.MemorySnapshot = MemorySnapshot;
    var Memory = /** @class */ (function () {
        function Memory() {
            this.current = this.peak = null;
            this.memoryData = [];
            this.capacity = 0;
            this.type = "GB";
        }
        Memory.prototype.update = function (data) {
            this.memoryData = [];
            this.current = this.peak = null;
            this.capacity = 0;
            if (!data || data.length === 0)
                return;
            for (var i = 0; i < data.length; ++i) {
                var snapshot = new MemorySnapshot(data[i]);
                if (isNaN(snapshot.capacity) === false) {
                    this.memoryData.push(snapshot);
                    if (this.peak === null || snapshot.used > this.peak.used)
                        this.peak = snapshot;
                    this.capacity = Math.max(this.capacity, snapshot.capacity);
                }
            }
            this.current = this.memoryData[this.memoryData.length - 1];
        };
        return Memory;
    }());
    exports.Memory = Memory;
    var MemoryChartBridge = /** @class */ (function (_super) {
        __extends(MemoryChartBridge, _super);
        function MemoryChartBridge(memoryDataSource, settings, factory) {
            var _this = _super.call(this, memoryDataSource, factory) || this;
            _this.memoryDataSource = memoryDataSource;
            _this.settings = settings;
            _this.factory = factory;
            _this.settings.valueRange = [0, memoryDataSource.capacity];
            return _this;
        }
        MemoryChartBridge.prototype.watchCollection = function () {
            return this.memoryDataSource.memoryData;
        };
        MemoryChartBridge.prototype.createChartData = function () {
            this.clearData();
            if (!this.memoryDataSource.memoryData)
                return;
            for (var i = 0; i < this.memoryDataSource.memoryData.length; ++i) {
                var m = this.memoryDataSource.memoryData[i];
                this.addData(MemoryChartBridge.convert(m));
            }
        };
        MemoryChartBridge.convert = function (m) {
            return [new chartjs_1.ChartJSDataPoint({ x: m.timestamp, y: m.used })];
        };
        return MemoryChartBridge;
    }(chartbridge_1.ChartBridge));
    exports.MemoryChartBridge = MemoryChartBridge;
});
//# sourceMappingURL=memory.js.map