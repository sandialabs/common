import { IDeviceData } from './idevicedata';
import { IChartJSSettings, IChartFactory, ChartJSDataPoint } from '../charts/chartjs';
import { ChartBridge } from '../charts/chartbridge';

export class MemorySnapshot {
    public timestamp: Date;
    public capacity: number;
    public free: number;
    public used: number;
    public percentUsed: number;
    static byteSize: number = 0x40000000;  // 1GB

    constructor(data: IDeviceData) {
        var currentMemory = JSON.parse(data.value)['Value'];
        this.timestamp = new Date(data.timeStamp);
        this.capacity = (parseFloat(currentMemory['Memory Capacity']) / MemorySnapshot.byteSize);
        this.free = (parseFloat(currentMemory['Free Memory']) / MemorySnapshot.byteSize);
        this.used = (parseFloat(currentMemory['Memory Used']) / MemorySnapshot.byteSize);
        this.percentUsed = this.used / this.capacity * 100;
    }
}

export class Memory {
    public current: MemorySnapshot;
    public peak: MemorySnapshot;
    public memoryData: MemorySnapshot[];
    public capacity: number;
    public type: string;

    constructor() {
        this.current = this.peak = null;
        this.memoryData = [];
        this.capacity = 0;
        this.type = "GB";
    }

    update(data: IDeviceData[]) {
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
    }
}


export class MemoryChartBridge extends ChartBridge {
    constructor(private memoryDataSource: Memory, public settings: IChartJSSettings, protected factory: IChartFactory) {
        super(memoryDataSource, factory);
        this.settings.valueRange = [0, memoryDataSource.capacity];
    }

    watchCollection(): any {
        return this.memoryDataSource.memoryData;
    }

    createChartData() {
        this.clearData();
        if (!this.memoryDataSource.memoryData)
            return;

        for (var i = 0; i < this.memoryDataSource.memoryData.length; ++i) {
            let m = this.memoryDataSource.memoryData[i];
            this.addData(MemoryChartBridge.convert(m));
        }
    }

    private static convert(m: MemorySnapshot): ChartJSDataPoint[] {
        return [new ChartJSDataPoint({ x: m.timestamp, y: m.used })];
    }
}
