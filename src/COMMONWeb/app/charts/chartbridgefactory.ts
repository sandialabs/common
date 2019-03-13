import { IChartFactory, ChartJSChartFactory, IChartJSSettings, ChartJSSettings } from "./chartjs";
import { IChartTypes } from "./icharttypes";
import { ChartBridge } from "./chartbridge";
import { CPUData, CPUChartBridge } from "../classes/cpu";
import { DatabaseHistory, DatabaseHistoryChartBridge } from "../classes/database";
import { Memory, MemoryChartBridge } from "../classes/memory";
import { NetworkStatus, NetworkChartBridge } from "../classes/network";
import { NICData, NICChartBridge } from "../classes/nic";
import { ProcessHistory, ProcessHistoryChartBridge } from "../classes/processes";
import { DiskUsage, DiskUsageChartBridge, DiskPerformance, DiskPerformanceChartBridge } from "../disk/disk";
import { UPSStatus, UPSChartBridge } from "../classes/ups";

export class ChartJSChartBridgeFactory {
    private factory: IChartFactory;

    constructor() {
        this.factory = new ChartJSChartFactory();
    }

    createChartBridge(chart: IChartTypes, settings: IChartJSSettings): ChartBridge {
        let b: ChartBridge = null;

        if (chart instanceof CPUData)
            b = new CPUChartBridge(chart, settings, this.factory);
        if (chart instanceof DatabaseHistory)
            b = new DatabaseHistoryChartBridge(chart, this.factory);
        if (chart instanceof Memory)
            b = new MemoryChartBridge(chart, settings, this.factory);
        if (chart instanceof NetworkStatus)
            b = new NetworkChartBridge(chart, settings, this.factory);
        if (chart instanceof NICData)
            b = new NICChartBridge(chart, settings, this.factory);
        if (chart instanceof ProcessHistory)
            b = new ProcessHistoryChartBridge(chart, this.factory);
        if (chart instanceof DiskUsage)
            b = new DiskUsageChartBridge(chart, settings, this.factory);
        if (chart instanceof DiskPerformance)
            b = new DiskPerformanceChartBridge(chart, this.factory);
        if (chart instanceof UPSStatus)
            b = new UPSChartBridge(chart, this.factory);

        return b;
    }

    createChartSettings(yaxis: string, height: number = 125): IChartJSSettings {
        return new ChartJSSettings(yaxis, height);
    }
}
