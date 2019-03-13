import { IChartFactory, ChartJSDataPoint } from "../charts/chartjs";
import { ValueInfo } from "./valueinfo";
import { IValueInfo } from "./ivalueinfo";
import { DataService } from "../services/data.service";
import { ChartBridge } from "../charts/chartbridge";

export interface IDatabaseToSize {
    name: string;
    size: number;
}

export interface IDatabaseDetail {
    sizeInMB: number;
    timestamp: string;
}

export class DatabaseDetail {
    sizeInMB: number;
    sizeInGB: number;
    timestamp: Date;

    constructor(detail: IDatabaseDetail) {
        this.timestamp = new Date(detail.timestamp);
        this.sizeInMB = detail.sizeInMB;
        this.sizeInGB = this.sizeInMB / 1000;
    }
}

export interface IDatabaseHistory {
    deviceID: number;
    databaseName: string;
    details: IDatabaseDetail[];
}

export class DatabaseHistory {
    deviceID: number;
    databaseName: string;
    details: DatabaseDetail[];
    maxSizeInMB: number;

    constructor() {
        this.deviceID = -1;
        this.databaseName = "";
        this.details = [];
        this.maxSizeInMB = 0;
    }

    update(data: IDatabaseHistory) {
        this.deviceID = data.deviceID;
        this.databaseName = data.databaseName;
        this.details = [];
        this.maxSizeInMB = 0;

        for (var i = 0; i < data.details.length; ++i) {
            let detail = new DatabaseDetail(data.details[i]);
            this.details.push(detail);
            this.maxSizeInMB = Math.max(this.maxSizeInMB, detail.sizeInMB);
        }
    }
}

export class DatabaseHistoryChartBridge extends ChartBridge {
    constructor(protected dbDataSource: DatabaseHistory, factory: IChartFactory) {
        super(dbDataSource, factory);
    }

    watchCollection(): any {
        if (this.dbDataSource)
            return this.dbDataSource.details;
        return null;
    }

    createChartData() {
        this.clearData();
        if (!this.dbDataSource || !this.dbDataSource.details)
            return;

        for (var i = 0; i < this.dbDataSource.details.length; ++i) {
            let d = this.dbDataSource.details[i];
            this.addData(DatabaseHistoryChartBridge.convert(d));
        }
    }

    private static convert(d: DatabaseDetail): ChartJSDataPoint[] {
        return [new ChartJSDataPoint({ x: d.timestamp, y: d.sizeInMB })];
    }
}

export class DatabaseManager {
    databases: ValueInfo;
    values: IDatabaseToSize[];
    dataService: any;
    databaseHistory: DatabaseHistory;

    constructor(data: IValueInfo, dataService: DataService) {
        this.databases = new ValueInfo(data);
        this.dataService = dataService;
        this.values = [];
        this.databaseHistory = null;

        var dict = JSON.parse(this.databases.value)['Value'];
        var sizeDictionary = {};
        for (var i = 0; i < dict.length; ++i) {
            var databaseInfo = dict[i];
            var size = databaseInfo["Size"];

            var dbs = [];
            if (sizeDictionary.hasOwnProperty(size)) {
                dbs = sizeDictionary[size];
            } else {
                sizeDictionary[size] = dbs;
            }
            dbs.push(databaseInfo);
        }

        // OK, now we have another dictionary mapping sizes to the databases that
        // have that size. Let's create a new array with the different size, and
        // sort that in reverse numeric order so the largest sizes are earliest in the list.
        var sizes = Object.keys(sizeDictionary);
        sizes.sort(function (x, y) {
            var xi = parseInt(x, 10);
            var yi = parseInt(y, 10);

            // Reverse sort
            return yi - xi;
        });

        // Now we can walk through sizes, getting the databases with that size out
        // of sizeDictionary, then fill up an array mapping the size to the database.
        // That array is what will be walked for display.
        for (var i = 0; i < sizes.length; ++i) {
            var s: number = parseInt(sizes[i]);
            var databases = sizeDictionary[s];
            for (var j = 0; j < databases.length; ++j) {
                var info = databases[j];
                this.values.push({ "size": s, "name": info.Name });
            }
        }
    }

    public onSelectProcess(process: string) {
        var t = this;
        this.dataService.getDatabaseHistory(this.databases.deviceID, process)
            .then((data: IDatabaseHistory) => {
                if (!t.databaseHistory)
                    t.databaseHistory = new DatabaseHistory();
                t.databaseHistory.update(data);
            });
    }
}
