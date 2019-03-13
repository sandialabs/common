import { DeviceInfo } from "./devices";

export interface IGroup {
    id: number;
    name: string;
}

export class Group {
    id: number;
    name: string;
    devices: DeviceInfo[];
    hasAlarm: boolean;
    //panelIsOpen: boolean;

    constructor(group: IGroup) {
        this.id = group.id;
        this.name = group.name;
        this.devices = [];
        this.hasAlarm = false;
        //this.panelIsOpen = false;
    }

    public addDevice(di: DeviceInfo) {
        this.devices.push(di);
        this.devices.sort((a, b) => {
            return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
        });
    }

    public findDeviceFromName(name: string): DeviceInfo {
        let d: DeviceInfo = null;
        for (var i = 0; d === null && i < this.devices.length; ++i) {
            let device = this.devices[i];
            if (name.localeCompare(device.name) === 0)
                d = device;
        }
        return d;
    }

    public findDeviceFromCollectorID(collectorID: number): DeviceInfo {
        let device: DeviceInfo = null;
        for (var i = 0; device === null && i < this.devices.length; ++i) {
            let d = this.devices[i];
            let c = d.getCollector(collectorID);
            if (c)
                device = d;
        }
        return device;
    }

    public updateStatusFlags() {
        this.hasAlarm = false;
        for (var i = 0; this.hasAlarm === false && i < this.devices.length; ++i) {
            let d = this.devices[i];
            this.hasAlarm = this.hasAlarm || d.alarms.length > 0;
        }
    }
}
