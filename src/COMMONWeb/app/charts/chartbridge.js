/// <reference types="angular" />
define(["require", "exports", "./chartjs"], function (require, exports, chartjs_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    // Bridge between the data owner and the graph. Each data owner
    // will have data in a format that's right for him, and each graph
    // will want his data in his own format. Use this to bridge the
    // two.
    // It's an abstract class because at some point you'll need to
    // write actual code to convert your data to what the chart wants.
    var ChartBridge = /** @class */ (function () {
        function ChartBridge(dataSource, factory, defaultColor) {
            if (defaultColor === void 0) { defaultColor = new chartjs_1.Color(); }
            this.dataSource = dataSource;
            this.factory = factory;
            this.defaultColor = defaultColor;
            this.chart = null;
            this.chartData = [];
            this.unWatch = null;
            // Can't do this here because the child constructor hasn't finished and this
            // method may very well fail because the object isn't valid yet.
            //this.createChartData();
        }
        ChartBridge.prototype.setupWatch = function (scope) {
            // Watch for changes to the raw data and update the graph.
            // The data you want to watch, and what you want to do when the data
            // changes, is up to you in your child class.
            var t = this;
            this.unWatch = scope.$watchCollection(function (s) {
                return t.watchCollection();
            }, function (newValue, oldValue, s) {
                t.onWatchedCollectionChanged(newValue, oldValue, s);
            });
        };
        ChartBridge.prototype.addData = function (chartData) {
            // Make sure the length of this.chartData is the same as the parameter chartData
            while (this.chartData.length < chartData.length)
                this.chartData.push(new chartjs_1.ChartJSData(this.defaultColor));
            for (var i = 0; i < chartData.length; ++i)
                this.chartData[i].add(chartData[i]);
        };
        ChartBridge.prototype.clearData = function () {
            for (var i = 0; i < this.chartData.length; ++i)
                this.chartData[i].clear();
        };
        ChartBridge.prototype.makeChart = function (context) {
            this.chart = this.factory.makeChart(context, this.dataSource);
            this.refreshChart();
            this.setupWatch(context.scope);
            // Let's keep track of the directive's scope, so when it goes away (when the
            // graph is no longer being displayed) we can stop watching the data collection.
            var t = this;
            context.scope.$on('$destroy', function () {
                if (t.unWatch) {
                    t.unWatch();
                    t.unWatch = null;
                }
            });
        };
        ChartBridge.prototype.onWatchedCollectionChanged = function (newValue, oldValue, scope) {
            this.refreshChart();
        };
        ChartBridge.prototype.refreshChart = function () {
            this.createChartData();
            if (this.chart)
                this.chart.refresh(this.chartData);
        };
        return ChartBridge;
    }());
    exports.ChartBridge = ChartBridge;
});
//# sourceMappingURL=chartbridge.js.map