/// <reference types="chart.js" />
import { Chart } from "../../lib/chart.js/Chart.js";

import { IChartTypes } from "./icharttypes.js";

export interface IDataScope extends ng.IScope {
    data: IChartTypes;
    settings?: IChartJSSettings;
}

export interface IChartContext {
    element: ng.IAugmentedJQuery;
    scope: IDataScope;
    rootScope: ng.IRootScopeService;
    window: ng.IWindowService;
}

export enum EChartJSColors {
    Blue,
    LightBlue,
    Green,
    LightGreen,
    Red,
    LightRed,
    Black,
    LightGray,
}

export class Color {
    border?: EChartJSColors;
    background?: EChartJSColors;

    // Default color scheme is blue
    constructor(border: EChartJSColors = EChartJSColors.Blue, background: EChartJSColors = EChartJSColors.LightBlue) {
        this.border = border;
        this.background = background;
    }

    static colors: Chart.ChartColor[] = [
        "#72b9ff",
        "#d8ebff",
        "#84cb6a",
        "#e1f2db",
        "#e57773",
        "#fae6e6",
        "#000000",
        "#dddddd"
    ];
}

export interface IChartJSSettings {
    xaxis: string;
    yaxis: string;
    yaxis2?: string;
    chartSizeInGB?: boolean;
    displayAxes?: boolean;
    displayLegend?: boolean;
    valueRange?: [number, number];
    height: number;
    makeResponsive: boolean;
}

export class ChartJSSettings implements IChartJSSettings {
    xaxis: string;
    yaxis: string;
    yaxis2?: string;
    chartSizeInGB?: boolean;
    displayAxes?: boolean;
    displayLegend?: boolean;
    valueRange?: [number, number];
    height: number;
    makeResponsive: boolean;

    constructor(yaxis: string, height: number) {
        this.xaxis = "";
        this.yaxis = yaxis;
        this.yaxis2 = null;
        this.chartSizeInGB = true;
        this.displayAxes = true;
        this.displayLegend = true;
        this.valueRange = null;
        this.height = height;   // pixels
        this.makeResponsive = true;
    }
}

export class ChartJSDataPoint {
    constructor(public point: Chart.ChartPoint, public color?: Color) {
    }
}

export class ChartJSData {
    chartPoints: Chart.ChartPoint[];

    // When all points in the chart should have the same color. Will
    // also be the color of the graph line.
    color: Color;

    // When each point in the chart might have a different color
    // In this case, the colors array should match the size of the
    // chartPoints array, specifying which color each chart point
    // should have.
    colors?: Color[];

    constructor(color: Color) {
        this.chartPoints = [];
        this.color = color;
        this.colors = null;
    }

    add(dataPoint: ChartJSDataPoint) {
        this.chartPoints.push(dataPoint.point);
        if (dataPoint.color) {
            if (!this.colors)
                this.colors = [];
            this.colors.push(dataPoint.color);
        }
    }

    clear() {
        this.chartPoints = [];
        // Don't change the color in case it's different than the default
        //this.color = new Color();
        this.colors = null;
    }

    // Returns a tuple of [borders[], backgrounds[]]
    asColors(): [Chart.ChartColor[], Chart.ChartColor[]] {
        let colors: [Chart.ChartColor[], Chart.ChartColor[]] = null;

        if (this.colors) {
            colors = [[], []];
            for (let i = 0; i < this.colors.length; ++i) {
                let c: Color = this.colors[i];
                colors[0].push(Color.colors[c.border]);
                colors[1].push(Color.colors[c.background]);
            }
        }
        return colors;
    }
}

export interface IChartJSChartOptions {
    context: IChartContext;
    settings: IChartJSSettings;
}

class ChartJSConfiguration implements Chart.ChartConfiguration {
    data: Chart.ChartData;
    options: Chart.ChartOptions;

    constructor(public type: Chart.ChartType, displayLegend: boolean) {
        this.data = {
            datasets: []
        };
        this.options = {
            responsive: true,
            maintainAspectRatio: false,
            title: {
                display: false,
            },
            scales: {
            },
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
        }
    }

    setData(data: Chart.ChartDataSets[]) {
        if (!data)
            return;
        this.data.datasets = data;
    }
}

export abstract class ChartJSChart {
    config: ChartJSConfiguration;
    chart: Chart;

    constructor(public chartContext: IChartContext, protected settings: IChartJSSettings, type: Chart.ChartType) {
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
            let t = this;

            // Make sure we are alerted that something changed so we can resize the graph
            chartContext.window.addEventListener('resize', function () {
                t.chartContext.rootScope.$apply();
            });

            chartContext.scope.$watch(function () {
                var parent = t.chartContext.element.parent();
                return {
                    height: Math.round(parent.height()),
                    width: Math.round(parent.width())
                }
            }, function () {
                var width = Math.round(t.chartContext.element.parent().width());
                var height = t.settings.height;

                t.resize(width, height);
            }, true);
        }
    }

    resize(width: number, height: number) {
        if (!this.chart)
            return;

        let e = this.chartContext.element;
        e.parent().height(height);
        e.width(width).height(height);

        let c = this.chart.canvas;
        c.width = width;
        c.height = height;
    }

    refresh(data: ChartJSData[]) {
        let dataSets: Chart.ChartDataSets[] = [];
        if (data.length > 0)
            dataSets.push(this.createDataSet(data[0], this.settings.yaxis, true));
        if (data.length > 1)
            dataSets.push(this.createDataSet(data[1], this.settings.yaxis2, false));

        this.config.setData(dataSets);

        if (this.chart)
            this.chart.update(this.config);
    }

    fixAxes() {
        let displayAxes = (this.settings.displayAxes !== undefined && this.settings.displayAxes !== null) ? this.settings.displayAxes : true;

        let y: Chart.ChartScales = this.createYScale(this.settings.yaxis, "left", true, displayAxes);
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
    }

    protected createYScale(label: string, position: 'left' | 'right', y1: boolean, displayAxes: boolean): Chart.ChartScales {
        let y: Chart.ChartScales = {
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
        (<any>y).id = y1 ? "y1" : "y2";
        return y;
    }

    protected createGraph(options: IChartJSChartOptions) {
        let element = this.chartContext.element.get(0).childNodes[0];
        this.chart = new Chart((<any>element).getContext('2d'), this.config);
    }

    protected createDataSet(data: ChartJSData, label: string, y1: boolean): Chart.ChartDataSets {
        let d: Chart.ChartDataSets = {
            data: data.chartPoints,
            label: label,
            backgroundColor: Color.colors[data.color.background],
            borderColor: Color.colors[data.color.border],
            yAxisID: y1 ? "y1" : "y2",
            fill: false
        };

        if (data.colors) {
            let colors = data.asColors();
            d.pointBorderColor = colors[0];
            d.pointBackgroundColor = colors[1];
        }

        return d;
    }
}

export class ChartJSLineChart extends ChartJSChart {
    constructor(options: IChartJSChartOptions) {
        super(options.context, options.settings, 'line');
        this.createGraph(options);
    }
}

// Not currently using any bar charts, but if we do it would go here
//export class ChartJSBarChart extends ChartJSChart {
//    constructor(options: IChartJSChartOptions, chartData: ChartJSData[]) {
//        super(options.context, options.settings, 'bar');
//        this.createGraph(options, chartData);
//    }
//}

export interface IChartFactory {
    makeChart(context: IChartContext, chart: IChartTypes): ChartJSChart;
}

export class ChartJSChartFactory implements IChartFactory {
    makeChart(context: IChartContext, chart: IChartTypes): ChartJSChart {
        let options: IChartJSChartOptions = {
            context: context,
            settings: context.scope.settings
        };

        // If we ever do bar charts, create that kind of chart here. We'll probably want to
        // determine that from something in IChartContext so the chart type can
        // be specified by a controller or something like that.
        var c: ChartJSChart = new ChartJSLineChart(options);
        return c;
    }
}
