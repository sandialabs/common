define(["require", "exports", "./chartjs", "../classes/cpu", "../classes/database", "../classes/memory", "../classes/network", "../classes/nic", "../classes/processes", "../disk/disk", "../classes/ups"], function (require, exports, chartjs_1, cpu_1, database_1, memory_1, network_1, nic_1, processes_1, disk_1, ups_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ChartJSChartBridgeFactory = /** @class */ (function () {
        function ChartJSChartBridgeFactory() {
            this.factory = new chartjs_1.ChartJSChartFactory();
        }
        ChartJSChartBridgeFactory.prototype.createChartBridge = function (chart, settings) {
            var b = null;
            if (chart instanceof cpu_1.CPUData)
                b = new cpu_1.CPUChartBridge(chart, settings, this.factory);
            if (chart instanceof database_1.DatabaseHistory)
                b = new database_1.DatabaseHistoryChartBridge(chart, this.factory);
            if (chart instanceof memory_1.Memory)
                b = new memory_1.MemoryChartBridge(chart, settings, this.factory);
            if (chart instanceof network_1.NetworkStatus)
                b = new network_1.NetworkChartBridge(chart, settings, this.factory);
            if (chart instanceof nic_1.NICData)
                b = new nic_1.NICChartBridge(chart, settings, this.factory);
            if (chart instanceof processes_1.ProcessHistory)
                b = new processes_1.ProcessHistoryChartBridge(chart, this.factory);
            if (chart instanceof disk_1.DiskUsage)
                b = new disk_1.DiskUsageChartBridge(chart, settings, this.factory);
            if (chart instanceof disk_1.DiskPerformance)
                b = new disk_1.DiskPerformanceChartBridge(chart, this.factory);
            if (chart instanceof ups_1.UPSStatus)
                b = new ups_1.UPSChartBridge(chart, this.factory);
            return b;
        };
        ChartJSChartBridgeFactory.prototype.createChartSettings = function (yaxis, height) {
            if (height === void 0) { height = 125; }
            return new chartjs_1.ChartJSSettings(yaxis, height);
        };
        return ChartJSChartBridgeFactory;
    }());
    exports.ChartJSChartBridgeFactory = ChartJSChartBridgeFactory;
});
//# sourceMappingURL=chartbridgefactory.js.map