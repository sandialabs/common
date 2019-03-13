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
define(["require", "exports", "../../lib/chart.js/Chart.js"], function (require, exports, Chart_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var EChartJSColors;
    (function (EChartJSColors) {
        EChartJSColors[EChartJSColors["Blue"] = 0] = "Blue";
        EChartJSColors[EChartJSColors["LightBlue"] = 1] = "LightBlue";
        EChartJSColors[EChartJSColors["Green"] = 2] = "Green";
        EChartJSColors[EChartJSColors["LightGreen"] = 3] = "LightGreen";
        EChartJSColors[EChartJSColors["Red"] = 4] = "Red";
        EChartJSColors[EChartJSColors["LightRed"] = 5] = "LightRed";
        EChartJSColors[EChartJSColors["Black"] = 6] = "Black";
        EChartJSColors[EChartJSColors["LightGray"] = 7] = "LightGray";
    })(EChartJSColors = exports.EChartJSColors || (exports.EChartJSColors = {}));
    var Color = /** @class */ (function () {
        // Default color scheme is blue
        function Color(border, background) {
            if (border === void 0) { border = EChartJSColors.Blue; }
            if (background === void 0) { background = EChartJSColors.LightBlue; }
            this.border = border;
            this.background = background;
        }
        Color.colors = [
            "#72b9ff",
            "#d8ebff",
            "#84cb6a",
            "#e1f2db",
            "#e57773",
            "#fae6e6",
            "#000000",
            "#dddddd"
        ];
        return Color;
    }());
    exports.Color = Color;
    var ChartJSSettings = /** @class */ (function () {
        function ChartJSSettings(yaxis, height) {
            this.xaxis = "";
            this.yaxis = yaxis;
            this.yaxis2 = null;
            this.chartSizeInGB = true;
            this.displayAxes = true;
            this.displayLegend = true;
            this.valueRange = null;
            this.height = height; // pixels
            this.makeResponsive = true;
        }
        return ChartJSSettings;
    }());
    exports.ChartJSSettings = ChartJSSettings;
    var ChartJSDataPoint = /** @class */ (function () {
        function ChartJSDataPoint(point, color) {
            this.point = point;
            this.color = color;
        }
        return ChartJSDataPoint;
    }());
    exports.ChartJSDataPoint = ChartJSDataPoint;
    var ChartJSData = /** @class */ (function () {
        function ChartJSData(color) {
            this.chartPoints = [];
            this.color = color;
            this.colors = null;
        }
        ChartJSData.prototype.add = function (dataPoint) {
            this.chartPoints.push(dataPoint.point);
            if (dataPoint.color) {
                if (!this.colors)
                    this.colors = [];
                this.colors.push(dataPoint.color);
            }
        };
        ChartJSData.prototype.clear = function () {
            this.chartPoints = [];
            // Don't change the color in case it's different than the default
            //this.color = new Color();
            this.colors = null;
        };
        // Returns a tuple of [borders[], backgrounds[]]
        ChartJSData.prototype.asColors = function () {
            var colors = null;
            if (this.colors) {
                colors = [[], []];
                for (var i = 0; i < this.colors.length; ++i) {
                    var c = this.colors[i];
                    colors[0].push(Color.colors[c.border]);
                    colors[1].push(Color.colors[c.background]);
                }
            }
            return colors;
        };
        return ChartJSData;
    }());
    exports.ChartJSData = ChartJSData;
    var ChartJSConfiguration = /** @class */ (function () {
        function ChartJSConfiguration(type, displayLegend) {
            this.type = type;
            this.data = {
                datasets: []
            };
            this.options = {
                responsive: true,
                maintainAspectRatio: false,
                title: {
                    display: false,
                },
                scales: {},
                legend: {
                    display: displayLegend,
                },
                elements: {
                    point: {
                        radius: 3,
                    }
                },
                animation: {
                    duration: 0
                },
                spanGaps: false,
            };
        }
        ChartJSConfiguration.prototype.setData = function (data) {
            if (!data)
                return;
            this.data.datasets = data;
        };
        return ChartJSConfiguration;
    }());
    var ChartJSChart = /** @class */ (function () {
        function ChartJSChart(chartContext, settings, type) {
            this.chartContext = chartContext;
            this.settings = settings;
            //// We'll use displayAxes to also indicate whether the legend at the top of the graph should be displayed
            //var displayAxes: boolean = (settings.displayAxes !== undefined && settings.displayAxes !== null) ? settings.displayAxes : true;
            this.config = new ChartJSConfiguration(type, settings.displayLegend);
            this.fixAxes();
            // I'm calling it makeResponsive, but really it's just watching the size of the parent
            // of the chart, and resizing the chart if the parent size changes.
            // Doing this can lead to infinite loops when the parent changes its size based
            // on the child, such as when a chart is put inside a table. So, when that kind of
            // thing happens, set makeResponsive to false.
            if (settings.makeResponsive) {
                var t_1 = this;
                // Make sure we are alerted that something changed so we can resize the graph
                chartContext.window.addEventListener('resize', function () {
                    t_1.chartContext.rootScope.$apply();
                });
                chartContext.scope.$watch(function () {
                    var parent = t_1.chartContext.element.parent();
                    return {
                        height: Math.round(parent.height()),
                        width: Math.round(parent.width())
                    };
                }, function () {
                    var width = Math.round(t_1.chartContext.element.parent().width());
                    var height = t_1.settings.height;
                    t_1.resize(width, height);
                }, true);
            }
        }
        ChartJSChart.prototype.resize = function (width, height) {
            if (!this.chart)
                return;
            var e = this.chartContext.element;
            e.parent().height(height);
            e.width(width).height(height);
            var c = this.chart.canvas;
            c.width = width;
            c.height = height;
        };
        ChartJSChart.prototype.refresh = function (data) {
            var dataSets = [];
            if (data.length > 0)
                dataSets.push(this.createDataSet(data[0], this.settings.yaxis, true));
            if (data.length > 1)
                dataSets.push(this.createDataSet(data[1], this.settings.yaxis2, false));
            this.config.setData(dataSets);
            if (this.chart)
                this.chart.update(this.config);
        };
        ChartJSChart.prototype.fixAxes = function () {
            var displayAxes = (this.settings.displayAxes !== undefined && this.settings.displayAxes !== null) ? this.settings.displayAxes : true;
            var y = this.createYScale(this.settings.yaxis, "left", true, displayAxes);
            if (this.settings.valueRange)
                y.ticks = {
                    min: this.settings.valueRange[0],
                    max: this.settings.valueRange[1]
                };
            this.config.options.scales = {
                xAxes: [
                    {
                        type: 'time',
                        time: {
                            unit: 'day'
                        },
                        display: displayAxes,
                        scaleLabel: {
                            display: false,
                            labelString: 'Date'
                        },
                    }
                ],
                yAxes: [y]
            };
            if (this.settings.yaxis2)
                this.config.options.scales.yAxes.push(this.createYScale(this.settings.yaxis2, "right", false, displayAxes));
        };
        ChartJSChart.prototype.createYScale = function (label, position, y1, displayAxes) {
            var y = {
                display: displayAxes,
                scaleLabel: {
                    display: false,
                    labelString: label
                },
                ticks: {
                    beginAtZero: true
                },
                position: position
            };
            // It appears that ChartScales has an "id" field, but it's not in the DefinitelyTyped definitions,
            // and I haven't gotten the pull-request done yet and can't wait for it to be approved and put out there.
            y.id = y1 ? "y1" : "y2";
            return y;
        };
        ChartJSChart.prototype.createGraph = function (options) {
            var element = this.chartContext.element.get(0).childNodes[0];
            this.chart = new Chart_js_1.Chart(element.getContext('2d'), this.config);
        };
        ChartJSChart.prototype.createDataSet = function (data, label, y1) {
            var d = {
                data: data.chartPoints,
                label: label,
                backgroundColor: Color.colors[data.color.background],
                borderColor: Color.colors[data.color.border],
                yAxisID: y1 ? "y1" : "y2",
                fill: false
            };
            if (data.colors) {
                var colors = data.asColors();
                d.pointBorderColor = colors[0];
                d.pointBackgroundColor = colors[1];
            }
            return d;
        };
        return ChartJSChart;
    }());
    exports.ChartJSChart = ChartJSChart;
    var ChartJSLineChart = /** @class */ (function (_super) {
        __extends(ChartJSLineChart, _super);
        function ChartJSLineChart(options) {
            var _this = _super.call(this, options.context, options.settings, 'line') || this;
            _this.createGraph(options);
            return _this;
        }
        return ChartJSLineChart;
    }(ChartJSChart));
    exports.ChartJSLineChart = ChartJSLineChart;
    var ChartJSChartFactory = /** @class */ (function () {
        function ChartJSChartFactory() {
        }
        ChartJSChartFactory.prototype.makeChart = function (context, chart) {
            var options = {
                context: context,
                settings: context.scope.settings
            };
            // If we ever do bar charts, create that kind of chart here. We'll probably want to
            // determine that from something in IChartContext so the chart type can
            // be specified by a controller or something like that.
            var c = new ChartJSLineChart(options);
            return c;
        };
        return ChartJSChartFactory;
    }());
    exports.ChartJSChartFactory = ChartJSChartFactory;
});
//# sourceMappingURL=chartjs.js.map