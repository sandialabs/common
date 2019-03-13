import { IDeviceData } from "./idevicedata";
import { IChartJSSettings, IChartFactory, ChartJSDataPoint } from "../charts/chartjs";
import { ChartBridge } from "../charts/chartbridge";

export class CPUSnapshot {
    public timestamp: Date;
    public percent: number;

    constructor(data: IDeviceData) {
        var currentCPU = JSON.parse(data.value)['Value'];
        this.timestamp = new Date(data.timeStamp);
        this.percent = Math.round(parseFloat(currentCPU));
    }
}

export class CPUData {
    public current: CPUSnapshot;
    public peak: CPUSnapshot;
    public cpuData: CPUSnapshot[];

    constructor() {
        this.current = this.peak = null;
        this.cpuData = [];
    }

    update(data: IDeviceData[]) {
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
    }
}

export class CPUChartBridge extends ChartBridge {
    constructor(private cpuDataSource: CPUData, public settings: IChartJSSettings, protected factory: IChartFactory) {
        super(cpuDataSource, factory);
        //console.log("CPUChartBridge constructor");
        this.settings.valueRange = [0, 100];
    }

    watchCollection(): any {
        return this.cpuDataSource.cpuData;
    }

    createChartData() {
        this.clearData();
        if (!this.cpuDataSource.cpuData)
            return;

        for (var i = 0; i < this.cpuDataSource.cpuData.length; ++i) {
            let c = this.cpuDataSource.cpuData[i];
            this.addData(CPUChartBridge.convert(c));
        }
    }

    private static convert(c: CPUSnapshot): ChartJSDataPoint[] {
        return [new ChartJSDataPoint({ x: c.timestamp, y: c.percent })];
    }
}
