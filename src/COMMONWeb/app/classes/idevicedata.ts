// See Models.cs/DeviceData
export interface IDeviceData {
    dataID: number;
    collectorID: number;
    value: string;
    timeStamp: string;
}

export interface IDeviceDataDict {
    [driveLetter: string]: IDeviceData[];
}
