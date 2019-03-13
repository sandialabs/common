import { IFullDeviceStatus } from "../classes/devices";
import { ISystemConfiguration } from "../classes/systemconfiguration";
import { ICollectorInfo } from "../classes/collectorinfo";
import { INetworkStatus } from "../classes/network";
import { IProcessHistory } from "../classes/processes";
import { IApplicationHistory, IAllApplicationsHistory } from "../classes/applications";
import { IDatabaseHistory } from "../classes/database";
import { IDeviceDetails, EMachineParts, IMachineData } from "../classes/machine";
import { EReportSubTypes, ISubReport } from "../reports/report";
import { IServices } from "../classes/services";
import * as moment from '../../lib/moment/min/moment-with-locales';

interface resultCallback<T> { (result: T): void }

interface IMachineParts {
    machineParts: EMachineParts[];
}

interface IReportSubTypes {
    reportTypes: EReportSubTypes[];
}

class URL<T> {
    urlID: number;
    requestedAt: Date;
    transmittedAt: Date;
    urlQueue: URLQueue;
    private func: Function;

    static nextID: number = 1;

    constructor(public url: string, private d: ng.IDeferred<T>, private http: ng.IHttpService) {
        this.urlID = URL.nextID++;
        this.func = this.get;
    }

    public get(): void {
        let t = this;
        //console.log("Executing: " + t.toString());
        this.http.get<T>(t.url)
            .then((response: ng.IHttpResponse<T>) => {
                t.d.resolve(response.data);
                t.urlQueue.remove(t);
            })
            .catch((reason: any) => {
                console.log("Rejected " + t.toString() + ": " + reason.toString());
                t.d.reject(reason);
                t.urlQueue.remove(t);
            });
    }

    public toString(): string {
        return "(" + this.urlID.toString() + ") " + this.url;
    }
}

class URLQueue {
    held: URL<any>[];
    executing: URL<any>[];
    private timeoutInMS?: number;

    constructor(private maxLength: number, timeoutInS?: number) {
        this.held = [];
        this.executing = [];

        if (timeoutInS)
            this.timeoutInMS = timeoutInS * 1000;
    }

    public push(url: URL<any>) {
        url.urlQueue = this;
        url.requestedAt = new Date();
        this.held.push(url);
        this.executeHeldURLs();
    }

    public executeHeldURLs(): void {
        this.clearTimedOut();

        while (this.held.length > 0 && this.executing.length < this.maxLength) {
            let url = this.held.shift();
            url.transmittedAt = new Date();
            let ms = Math.abs(url.transmittedAt.getTime() - url.requestedAt.getTime());
            // if(ms > 500)
            //     console.log("Held for " + ms.toString() + " ms: " + url.toString());
            this.executing.push(url);
            url.get();
        }
    }

    public remove(url: URL<any>) {
        for (let i = 0; i < this.executing.length; ++i) {
            if (this.executing[i].urlID == url.urlID) {
                let url = this.executing[i];
                let ms = Math.abs(new Date().getTime() - url.transmittedAt.getTime());
                // if(ms > 500)
                //     console.log("Completed after " + ms.toString() + " ms: " + url.toString());
                this.executing.splice(i, 1);
                break;
            }
        }
    }

    private clearTimedOut() {
        if (!this.timeoutInMS)
            return;

        let now: number = new Date().getTime();
        for (let i = 0; i < this.executing.length; ++i) {
            let doomed = this.executing[i];
            let ms = Math.abs(now - doomed.transmittedAt.getTime());
            if (ms >= this.timeoutInMS) {
                // console.log("Doomed after " + ms.toString() + " ms: " + doomed.toString());
                this.executing.splice(i--, 1);
            }
        }
    }
}

export class DataService {
    urlQueue: URLQueue;

    // kk is 00-23 hours
    private static dateFormat: string = "YYYY-MM-DDTkk:mm:ss.SSSZ";

    constructor(private $http: ng.IHttpService, private $q: ng.IQService) {
        //console.log("NewDataService.constructor");
        this.urlQueue = new URLQueue(8, 120);
    }

    getConfiguration = (): ng.IPromise<ISystemConfiguration> => this.get('configurationdata');
    getDeviceStatus = (): ng.IPromise<IFullDeviceStatus> => this.get('devicestatus');
    getAllCollectors = (): ng.IPromise<ICollectorInfo[]> => this.get('allcollectors');
    getNetworkStatus = (starting?: Date, ending?: Date): ng.IPromise<INetworkStatus[]> => this.get('networkstatus', DataService.getDatesTuple(starting, ending));
    collectNow = (collectorID: number): ng.IPromise<ICollectorInfo> => this.post('collectnow', [collectorID]);
    getProcessHistory = (id: number, processName): ng.IPromise<IProcessHistory> => this.get('processhistory', [id, processName]);
    getAllProcesses = (id: number, starting?: Date, ending?: Date): ng.IPromise<string[]> => this.get('allprocesses', DataService.getIDAndDatesTuple(id, starting, ending));
    getAppHistory = (id: number, app): ng.IPromise<IApplicationHistory> => this.get('apphistory', [id, app]);
    getAppChanges = (deviceID: number, starting?: Date, ending?: Date): ng.IPromise<IAllApplicationsHistory> => this.get('xyz', DataService.getIDAndDatesTuple(deviceID, starting, ending));
    // The database name might have some invalid characters, so let's encode it and let the server decode it
    getDatabaseHistory = (id: number, databaseName: string): ng.IPromise<IDatabaseHistory> => this.get('databasehistory', [id, btoa(databaseName)]);
    getServicesData = (id: number): ng.IPromise<IServices> => this.get('servicesdata', [id]);
    getDeviceDetails = (id: number): ng.IPromise<IDeviceDetails> => this.get('devicedetails', [id]);
    getMachineData = (id: number, parts: EMachineParts[], starting?: Date, ending?: Date): ng.IPromise<IMachineData> => this.get('machinedata', DataService.getIDAndDatesAndMachinePartsTuple(id, parts, starting, ending));
    getSubReport = (id: number, types: EReportSubTypes[], starting?: Date, ending?: Date): ng.IPromise<ISubReport> => this.get('machinesubreport', DataService.getIDAndDAtesAndReportPartsTuple(id, types, starting, ending));

    // The problem is that Date.toIsoString() always formats the date to UTC, or like this:
    // 2019-02-03T12:23:34.123Z
    // The Z means UTC (zulu)
    // But the dates in the SQLite database are stored as strings, not actual dates, so if we do a query
    // using the zulu time it doesn't quite match up:
    // 2019-02-03T06:23:34.123-06:00 != 2019-02-03T12:23:34.123Z
    // even though those are the same times. The left one is the local
    // time with the offset to GMT, the other is in GMT.
    // The time used in the DB is the C# DateTimeOffset.ToString("o"), which puts the
    // local time with the offset to GMT.
    // So, for consistency, we need to use the same format that DateTimeOffset.ToString("o")
    // uses, but there isn't one built in to JavaScript.
    //
    // Fortunately, moment.format() does exactly what we want. Well, almost. format() doesn't
    // put the milliseconds in there, so we'll have to

    private static getIDAndDatesTuple(id: number, starting?: Date, ending?: Date): [number, string, string] {
        let s = starting == null ? null : moment(starting).format(DataService.dateFormat);
        let e = ending == null ? null : moment(ending).format(DataService.dateFormat);
        return [id, s, e];
    }

    private static getDatesTuple(starting?: Date, ending?: Date): [string, string] {
        let s = starting == null ? null : moment(starting).format(DataService.dateFormat);
        let e = ending == null ? null : moment(ending).format(DataService.dateFormat);
        return [s, e];
    }

    private static getIDAndDatesAndMachinePartsTuple(id: number, parts: EMachineParts[], starting?: Date, ending?: Date): [number, string, string, string] {
        let mp: IMachineParts = {
            machineParts: parts
        };
        let json = JSON.stringify(mp.machineParts);
        let t1 = DataService.getIDAndDatesTuple(id, starting, ending);
        return [t1[0], json, t1[1], t1[2]];
    }

    private static getIDAndDAtesAndReportPartsTuple(id: number, types: EReportSubTypes[], starting?: Date, ending?: Date): [number, string, string, string] {
        let mp: IReportSubTypes = {
            reportTypes: types
        };
        let json = JSON.stringify(mp.reportTypes);
        let t1 = DataService.getIDAndDatesTuple(id, starting, ending);
        return [t1[0], json, t1[1], t1[2]];
    }

    private get<T>(method: string, data?: any): ng.IPromise<T> {
        let url = '/' + method;

        if (data) {
            for (var i = 0; i < data.length; ++i)
                url += '/' + data[i];
        }

        // Push the URL into the queue, and it will automatically attempt
        // to execute it if there aren't too many already executing.
        let d: ng.IDeferred<T> = this.$q.defer<T>();
        let u = new URL<T>(url, d, this.$http);
        this.urlQueue.push(u);
        return d.promise;
    }

    private post<T>(method: string, data?: any): ng.IPromise<T> {
        let url = '/' + method + '/' + data;

        // console.log(url);

        let d: ng.IDeferred<T> = this.$q.defer<T>();
        this.$http.post<T>(encodeURI(url), data)
            .then((response: ng.IHttpResponse<T>) => {
                d.resolve(response.data);
            })
            .catch((reason: any) => {
                d.reject(reason);
            });
        return d.promise;
    }

    public static Factory(): Function {
        let factory = ($http: ng.IHttpService, $q: ng.IQService) => {
            return new DataService($http, $q);
        }
        factory.$inject = ['$http', '$q'];
        return factory;
    }
}
