/// <reference types="angular" />

import { IAutoUpdatable, AutoUpdater } from './autoupdater';
import { DataService } from '../services/data.service';
import { DeviceManagerService } from '../services/devicemanager.service';
import { SystemConfiguration } from './systemconfiguration';
import { EChartJSColors, Color, IChartJSSettings, IChartFactory, ChartJSDataPoint } from '../charts/chartjs';
import { ChartBridge } from '../charts/chartbridge';

interface IPingAttempt {
    successful: boolean;
    timestamp: string;
    responseTimeMS: number;
}

export class PingAttempt {
    successful: boolean;
    timestamp: Date;
    responseTimeMS: number;

    constructor(attempt: IPingAttempt) {
        this.successful = attempt.successful;
        this.timestamp = new Date(attempt.timestamp);
        this.responseTimeMS = attempt.responseTimeMS;
    }
}

// See Models.cs/NetworkStatus
export interface INetworkStatus {
    name: string;
    deviceID: number;
    successfulPing: boolean;
    dateSuccessfulPingOccurred?: string;
    datePingAttempted: string;
    ipAddress: string;
    hasBeenPinged: boolean;
    attempts: IPingAttempt[];
}

export class NetworkStatus {
    public name: string;
    public deviceID: number;
    public successfulPing: boolean;
    public dateMostRecentStateChange?: Date;
    public dateSuccessfulPingOccurred?: Date;
    public datePingAttempted: Date;
    public ipAddress: string;
    public hasBeenPinged: boolean;
    public avgResponseTimeMS: number;
    public totalPingAttempts: number;
    public totalSuccessfulPingAttempts: number;
    public percentSuccessfulPings: number;
    public attempts: PingAttempt[];

    constructor(status: INetworkStatus) {
        this.name = status.name;
        this.deviceID = status.deviceID;
        this.ipAddress = status.ipAddress;
        this.attempts = [];

        this.avgResponseTimeMS =
            this.percentSuccessfulPings =
            this.totalPingAttempts =
            this.totalSuccessfulPingAttempts = 0;
        this.dateMostRecentStateChange = null;

        this.update(status);
    }

    public update(n: INetworkStatus) {
        this.successfulPing = n.successfulPing;
        this.hasBeenPinged = n.hasBeenPinged;

        // Because the JavaScript dates and C# dates are slightly different,
        // when we convert from one to the other we'll lose some of the
        // milliseconds. This is enough so that when the query to get new ping
        // attempts occurs, it will almost surely get one out of the DB that
        // we've already seen here.
        // So, before we make a new PingAttempt and push it into the attempts array,
        // make sure it's not already there. Compare the ping attempt times, and since
        // we'll be comparing JavaScript dates we should be fine.
        if (n.attempts.length > 0) {
            let totalResponseTimeMS = 0;
            for (var i = 0; i < n.attempts.length; ++i) {
                let pa: PingAttempt = new PingAttempt(n.attempts[i]);
                let exists: boolean = false;
                const paTime = pa.timestamp.getTime();
                // Let's go in reverse order for a bit better performance since the new
                // PingAttempt will likely be coming in after the most recent in this.attempts.
                for (var j = this.attempts.length - 1; exists == false && j >= 0; --j) {
                    let old = this.attempts[j];
                    exists = paTime - old.timestamp.getTime() == 0;
                }

                if (exists == false) {
                    if (pa.successful)
                        this.totalSuccessfulPingAttempts += 1;
                    totalResponseTimeMS += pa.responseTimeMS;
                    this.attempts.push(pa);
                }
            }
            this.totalPingAttempts = n.attempts.length;
            this.avgResponseTimeMS = this.totalPingAttempts > 0 ? totalResponseTimeMS / this.totalPingAttempts : 0;
            this.percentSuccessfulPings = this.totalPingAttempts > 0 ? this.totalSuccessfulPingAttempts / this.totalPingAttempts * 100.0 : 0;

            // The attempts come in "chunks" like the most-recent two weeks, then two-weeks before that, etc.
            // This means as the attempts are pushed on to the attempts array they won't be in order so
            // we need to sort.
            this.attempts.sort((x, y) => {
                let a = x.timestamp.getTime();
                let b = y.timestamp.getTime();
                return a - b;
            });
        }

        if (n.dateSuccessfulPingOccurred && n.dateSuccessfulPingOccurred !== "")
            this.dateSuccessfulPingOccurred = new Date(n.dateSuccessfulPingOccurred);
        this.datePingAttempted = new Date(n.datePingAttempted);
    }
}

export class NetworkChartBridge extends ChartBridge {
    public static red: Color = new Color(EChartJSColors.Red, EChartJSColors.LightRed);
    public static green: Color = new Color(EChartJSColors.Green, EChartJSColors.LightGreen);

    constructor(private networkDataSource: NetworkStatus, public settings: IChartJSSettings, protected factory: IChartFactory) {
        super(networkDataSource, factory, NetworkChartBridge.green);
        this.settings.valueRange = [0, 1000];
    }

    watchCollection(): any {
        return this.networkDataSource.attempts;
    }

    createChartData() {
        this.clearData();

        if (!this.networkDataSource || !this.networkDataSource.attempts)
            return;

        for (var i = 0; i < this.networkDataSource.attempts.length; ++i) {
            let a = this.networkDataSource.attempts[i];
            this.addData(NetworkChartBridge.convert(a));
        }
    }

    private static convert(pa: PingAttempt): ChartJSDataPoint[] {
        let c: Color = (pa.responseTimeMS >= 500) ? NetworkChartBridge.red : NetworkChartBridge.green;
        return [new ChartJSDataPoint({ x: pa.timestamp, y: pa.responseTimeMS }, c)];
    }
}

interface IIDToNetworkStatus {
    [index: number]: NetworkStatus;
}

interface IStringToNetworkStatus {
    [index: string]: NetworkStatus;
}

class NetworkRetrieval {
    public startingDate: Date;
    public endingDate: Date;
    public static msPerDay: number = 24 * 60 * 60 * 1000;
    public static daysPerWindow: number = 15;
    public static msPerWindow: number = NetworkRetrieval.daysPerWindow * NetworkRetrieval.msPerDay;

    constructor(private config: SystemConfiguration) {
        this.startingDate = this.endingDate = null;
    }

    // Returns the date range that should be retrieved next
    // When called the first time, it will go back from the
    // current time. Each subsequent call will go back from the
    // previous start.
    public retrieveNext() : [Date, Date] {
        let start: Date = null;
        let end: Date = null;

        // This will move the 'window' back by 15 days
        // each time we gather the data.
        if (this.startingDate == null) {
            if (this.config.mostRecentData)
                this.startingDate = this.config.mostRecentData;
            else
                this.startingDate = new Date();
            this.endingDate = null;
            end = null;
        }
        else {
            this.endingDate = this.startingDate;
            end = this.endingDate;
        }

        this.startingDate = new Date(this.startingDate.getTime() - NetworkRetrieval.msPerWindow);
        start = this.startingDate;

        console.log("Retrieving " + (start ? start.toDateString() : "null") + " to " + (end ? end.toDateString() : "null"));

        return [start, end];
    }
}

// Holds all of the NetworkStatus objects, and keeps track of the most-recent poll time in
// maxDate
export class Network implements IAutoUpdatable<Network> {
    public maxDate: Date;
    private idToData: IIDToNetworkStatus;
    private ipAddressToData: IStringToNetworkStatus;
    public data: NetworkStatus[];
    public selected: NetworkStatus;
    public autoUpdater: AutoUpdater<Network>;
    private retrieval: NetworkRetrieval;

    constructor(private dataService: DataService, private deviceManagerService: DeviceManagerService, private $interval: ng.IIntervalService, private config: SystemConfiguration) {
        this.maxDate = null;
        this.idToData = {};
        this.ipAddressToData = {};
        this.data = [];
        this.dataService = dataService;
        this.selected = null;
        this.deviceManagerService = deviceManagerService;

        // Get the new network data every 10 seconds
        this.autoUpdater = new AutoUpdater(10000, Network.gatherData, this, $interval);
        this.retrieval = new NetworkRetrieval(this.config);

        Network.gatherData(this);
    }

    public getNetworkStatusFromID(id: number): NetworkStatus {
        return this.idToData[id];
    }

    public getNetworkStatusFromIPAddress(name: string): NetworkStatus {
        return this.ipAddressToData[name];
    }

    public updateData(data: INetworkStatus[]) {
        if (!data)
            return;

        for (var i = 0; i < data.length; ++i) {
            var d: INetworkStatus = data[i];
            let existing: NetworkStatus = this.getNetworkStatusFromIPAddress(d.ipAddress);
            let ns: NetworkStatus = null;

            if (!existing) {
                ns = new NetworkStatus(d);

                this.data.push(ns);
                this.idToData[ns.deviceID] = ns;
                this.ipAddressToData[ns.ipAddress] = ns;
            }
            else {
                existing.update(d);
                ns = existing;
            }

            if (this.maxDate === null || ns.datePingAttempted > this.maxDate)
                this.maxDate = ns.datePingAttempted;
        }

        //this.startAutomaticUpdate();
    }

    public startAutomaticUpdate() {
        this.autoUpdater.start();
    }

    public stopAutomaticUpdate() {
        this.autoUpdater.stop();
    }

    // This is called when the user wants to see more data
    // on the chart.
    public getEarlierRange() {
        this.gather(this.retrieval.retrieveNext());
    }

    private gather(range: [Date, Date]) {
        let t: Network = this;
        //console.debug("Network.gather.getNetworkStatus", range);
        this.dataService.getNetworkStatus(range[0], range[1])
            .then((data: INetworkStatus[]) => {
                //console.debug("Network.gather.getNetworkStatus.return", data);
                if (!data)
                    return;

                t.updateData(data);
                t.deviceManagerService.get()
                    .then(function (deviceManager) {
                        deviceManager.updateNetwork(t);
                    });
            });
    }

    // This is called at startup, and periodically to get
    // the most recent data. When starting up, gather
    // a couple of weeks of data, then after that just
    // get the data that might have been collected since
    // startup.
    private static gatherData(t: Network) {
        let range: [Date, Date] = [null, null];

        if (t.retrieval.startingDate == null)
            range = t.retrieval.retrieveNext();
        else
            range = [t.maxDate, null];

        t.gather(range);
    }
}