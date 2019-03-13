interface IApplicationInfo {
    name: string;
    version: string;
}

export interface IDeviceApplications {
    deviceID: number;
    applications: IApplicationInfo[];
    timestamp: string;
}

class DeviceApplications {
    deviceID: number;
    applications: IApplicationInfo[];
    timestamp: Date;

    constructor(data: IDeviceApplications) {
        this.deviceID = data.deviceID;
        this.applications = data.applications;
        this.timestamp = new Date(data.timestamp);
    }
}

interface ISnapshot {
    version: string;
    timestamp: string;
}

export interface IApplicationHistory {
    name: string;
    history: ISnapshot[];
}

interface IApplicationsMap {
    [app: string]: IApplicationHistory;
}

class Snapshot {
    version: string;
    timestamp: Date;

    constructor(data: ISnapshot) {
        this.version = data.version;
        this.timestamp = new Date(data.timestamp);
    }
}

// See Database/Models.cs
class ApplicationHistory {
    name: string;
    history: Snapshot[];

    constructor() {
        this.name = "";
        this.history = [];
    }

    update(data: IApplicationHistory) {
        this.name = data.name;
        this.history = [];

        for (var i = 0; i < data.history.length; ++i) {
            var snapshot = new Snapshot(data.history[i]);
            this.history.push(snapshot);
        }
    }
}

export interface IApplicationsHistoryMap {
    [name: string]: IApplicationHistory;
}

interface IApplicationsHistory {
    [name: string]: ApplicationHistory;
}

class ApplicationsHistoryMap {
    history: IApplicationsHistory;

    constructor(map: IApplicationsHistoryMap) {
        this.history = {};
        var keys = Object.keys(map);
        for (var i = 0; i < keys.length; ++i) {
            let name = keys[i];
            let history: IApplicationHistory = map[name];
            let appHistory = new ApplicationHistory();
            appHistory.update(history);
            this.history[name] = appHistory;
        }
    }
}

export interface IAllApplicationsHistory {
    history: IApplicationsMap;
}

export class AllApplicationsHistory {
    history: ApplicationsHistoryMap;
    apps: string[];

    constructor(history: IAllApplicationsHistory) {
        this.history = new ApplicationsHistoryMap(history.history);
        this.apps = Object.keys(this.history.history);
        this.apps.sort();
    }
}

export class ApplicationManager {
    applications: DeviceApplications;
    //values: any[];
    dataService: any;
    applicationHistory: ApplicationHistory;

    constructor(data: IDeviceApplications, dataService: any) {
        this.applications = new DeviceApplications(data);
        this.dataService = dataService;
        //this.values = [];
        this.applicationHistory = null;
    }

    public onSelectApplication(app: string) {
        var t = this;
        this.dataService.getAppHistory(this.applications.deviceID, app)
            .then((data: IApplicationHistory) => {
                if (!t.applicationHistory)
                    t.applicationHistory = new ApplicationHistory();
                t.applicationHistory.update(data);
            });
    }
}
