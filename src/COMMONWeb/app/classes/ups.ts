import { IDeviceData } from './idevicedata';
import { IChartFactory, ChartJSDataPoint } from '../charts/chartjs';
import { ChartBridge } from '../charts/chartbridge';

export class UPSSnapshot {
    public timestamp: Date;
    public name: string;
    public upsStatus: string;
    public batteryStatus: string;
    public batteryStatusInt: number;
    public estimatedRunTimeInMinutes: number;
    public estimatedChargeRemainingPercentage: number;
    public runningOnUPS: boolean;

    constructor(data: IDeviceData) {
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
}

export class UPSStatus  {
    public upsData: UPSSnapshot[];
    public current: UPSSnapshot;
    public name: string;

    constructor(data: IDeviceData[]) {
        this.upsData = [];

        if (!data || data.length === 0)
            return;

        for (var i = 0; i < data.length; ++i) {
            var snapshot = new UPSSnapshot(data[i]);
            this.upsData.push(snapshot);
        }
        this.current = this.upsData[this.upsData.length - 1];
    }
}

export class UPSChartBridge extends ChartBridge {
    constructor(private upsDataSource: UPSStatus, protected factory: IChartFactory) {
        super(upsDataSource, factory);
    }

    watchCollection(): any {
        return this.upsDataSource.upsData;
    }

    createChartData() {
        this.clearData();
        if (!this.upsDataSource.upsData)
            return;

        for (var i = 0; i < this.upsDataSource.upsData.length; ++i) {
            let c = this.upsDataSource.upsData[i];
            this.addData(UPSChartBridge.convert(c));
        }
    }

    private static convert(u: UPSSnapshot): ChartJSDataPoint[] {
        return [new ChartJSDataPoint({ x: u.timestamp, y: u.runningOnUPS ? 1.0 : 0.0 })];
    }
}
