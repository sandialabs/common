import { IDeviceProcessInfo } from "./deviceprocessinfo";
import { DataService } from "../services/data.service";
import { ChartJSData, Color, EChartJSColors, IChartJSSettings, IChartFactory, ChartJSDataPoint } from "../charts/chartjs";
import { ChartBridge } from "../charts/chartbridge";

// See Database/Models.cs
export interface IProcessHistory {
    deviceID: number;
    processName: string;
    details: [string, number, number][];
}

// The tuple is the date, CPU%, and memory usage in MB
export type HistoryData = [Date, number, number];

export class ProcessHistory {
    deviceID: number;
    processName: string;
    details: HistoryData[];

    constructor() {
        this.deviceID = -1;
        this.processName = "";
        this.details = [];
    }

    update(history: IProcessHistory) {
        this.deviceID = history.deviceID;
        this.processName = history.processName;
        this.details = [];

        var mb = 1000000;
        for (var i = 0; i < history.details.length; ++i) {
            let tup = history.details[i];
            let detail: HistoryData = [new Date(tup[0]), tup[1], tup[2] / mb];
            this.details.push(detail);
        }
    }
}

export class ProcessHistoryChartBridge extends ChartBridge {
    constructor(private processHistoryDataSource: ProcessHistory, protected factory: IChartFactory) {
        super(processHistoryDataSource, factory);

        let line2: ChartJSData = new ChartJSData(new Color(EChartJSColors.Green, EChartJSColors.LightGreen));

        this.chartData.push(line2);
    }

    watchCollection(): any {
        return this.processHistoryDataSource.details;
    }

    protected createChartData() {
        this.clearData();
        if (!this.processHistoryDataSource.details)
            return;

        for (var i = 0; i < this.processHistoryDataSource.details.length; ++i) {
            let c = this.processHistoryDataSource.details[i];
            this.addData(ProcessHistoryChartBridge.convert(c));
        }
    }

    private static convert(p: HistoryData): ChartJSDataPoint[] {
        return [new ChartJSDataPoint({ x: p[0], y: p[1] }), new ChartJSDataPoint({ x: p[0], y: p[2] })];
    }
}

export class ProcessManager {
    processes: IDeviceProcessInfo;
    values: any[];
    dataService: DataService;
    processHistory: ProcessHistory;

    constructor(data: IDeviceProcessInfo, dataService: DataService) {
        this.processes = data;
        this.dataService = dataService;
        this.values = [];
        this.processHistory = null;

        var keys = Object.keys(this.processes.cpuToProcesses);
        // Sort in reverse order
        keys.sort((a: string, b: string) => {
            var a2 = parseInt(a);
            var b2 = parseInt(b);
            return b2 - a2;
        });

        for (var i = 0; i < keys.length; ++i) {
            var key = keys[i];
            var procs = this.processes.cpuToProcesses[key];
            for (var j = 0; j < procs.length; ++j) {
                this.values.push([key, procs[j]]);
            }
        }
    }

    public onSelectProcess(process: string) {
        var t = this;
        this.dataService.getProcessHistory(this.processes.deviceID, process)
            .then((data: IProcessHistory) => {
                if (!t.processHistory)
                    t.processHistory = new ProcessHistory();
                t.processHistory.update(data);
            });
    }
}
