interface ICPUToProcesses {
    [index: number]: string[];
}

export interface IDeviceProcessInfo {
    deviceID: number;
    cpuToProcesses: ICPUToProcesses;
    timestamp: Date;
}

// See DeviceProcessInfo in DatabaseLib/Models.cs

export class DeviceProcessInfo implements IDeviceProcessInfo {
    deviceID: number;
    cpuToProcesses: ICPUToProcesses;
    timestamp: Date;

    constructor() {
        this.deviceID = -1;
        this.cpuToProcesses = {};
        this.timestamp = null;
    }
}
