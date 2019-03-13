import { IDeviceData } from './idevicedata';
import { IChartJSSettings, IChartFactory, ChartJSDataPoint } from '../charts/chartjs';
import { ChartBridge } from '../charts/chartbridge';

export class NICSnapshot {
    public timestamp: Date;
    public bps: number;
    public capacity: number;
    public percent: number

    constructor(data: IDeviceData) {
        var nic = JSON.parse(data.value)['Value'];
        this.timestamp = new Date(data.timeStamp);
        this.bps = Math.round(parseFloat(nic['BPS']));
        this.capacity = Math.round(parseFloat(nic['Capacity']));
        this.percent = Math.round(parseFloat(nic['Avg']));
    }
}

export class NICData {
    public current: NICSnapshot;
    public peak: NICSnapshot;
    public nicData: NICSnapshot[];

    constructor() {
        this.current = this.peak = null;
        this.nicData = [];
    }

    update(data: IDeviceData[]) {
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
    }
}

export class NICChartBridge extends ChartBridge {
    constructor(private nicDataSource: NICData, public settings: IChartJSSettings, protected factory: IChartFactory) {
        super(nicDataSource, factory);
        this.settings.valueRange = [0, 100];
    }

    watchCollection(): any {
        return this.nicDataSource.nicData;
    }

    createChartData() {
        this.clearData();
        if (!this.nicDataSource.nicData)
            return;

        for (var i = 0; i < this.nicDataSource.nicData.length; ++i) {
            let c = this.nicDataSource.nicData[i];
            this.addData(NICChartBridge.convert(c));
        }
    }

    private static convert(n: NICSnapshot): ChartJSDataPoint[] {
        return [new ChartJSDataPoint({ x: n.timestamp, y: n.percent })];
    }
}
