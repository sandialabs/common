/// <reference types="angular" />

import { IChartContext, ChartJSData, ChartJSDataPoint, IChartFactory, ChartJSChart, Color } from "./chartjs";
import { IChartTypes } from "./icharttypes";

export type ChartBridgeConverterCallback<T> = (t: T) => ChartJSDataPoint[];

// Bridge between the data owner and the graph. Each data owner
// will have data in a format that's right for him, and each graph
// will want his data in his own format. Use this to bridge the
// two.
// It's an abstract class because at some point you'll need to
// write actual code to convert your data to what the chart wants.
export abstract class ChartBridge {
    chart: ChartJSChart;
    chartData: ChartJSData[];
    unWatch: Function;

    constructor(public dataSource: IChartTypes, protected factory: IChartFactory, protected defaultColor: Color = new Color()) {
        this.chart = null;
        this.chartData = [];
        this.unWatch = null;

        // Can't do this here because the child constructor hasn't finished and this
        // method may very well fail because the object isn't valid yet.
        //this.createChartData();
    }

    protected setupWatch(scope: ng.IScope) {
        // Watch for changes to the raw data and update the graph.
        // The data you want to watch, and what you want to do when the data
        // changes, is up to you in your child class.
        let t = this;
        this.unWatch = scope.$watchCollection(
            (s: ng.IScope) => {
                return t.watchCollection();
            },
            (newValue: any, oldValue: any, s: ng.IScope) => {
                t.onWatchedCollectionChanged(newValue, oldValue, s);
            });
    }

    protected addData(chartData: ChartJSDataPoint[]) {
        // Make sure the length of this.chartData is the same as the parameter chartData
        while (this.chartData.length < chartData.length)
            this.chartData.push(new ChartJSData(this.defaultColor));

        for (var i = 0; i < chartData.length; ++i)
            this.chartData[i].add(chartData[i]);
    }

    protected clearData() {
        for (var i = 0; i < this.chartData.length; ++i)
            this.chartData[i].clear();
    }

    makeChart(context: IChartContext) {
        this.chart = this.factory.makeChart(context, this.dataSource);
        this.refreshChart();

        this.setupWatch(context.scope);

        // Let's keep track of the directive's scope, so when it goes away (when the
        // graph is no longer being displayed) we can stop watching the data collection.
        let t = this;
        context.scope.$on('$destroy', () => {
            if (t.unWatch) {
                t.unWatch();
                t.unWatch = null;
            }
        });
    }

    protected onWatchedCollectionChanged(newValue: any, oldValue: any, scope: ng.IScope) {
        this.refreshChart();
    }

    // Method that's called when it's time to convert the owner's data to the chart's
    // data.
    protected abstract createChartData();

    // Return the collection that should be watched so the chart can be refreshed
    // when the data changes.
    abstract watchCollection(): any;

    protected refreshChart() {
        this.createChartData();
        if (this.chart)
            this.chart.refresh(this.chartData);
    }
}
