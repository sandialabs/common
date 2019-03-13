import {IDeviceData} from "./idevicedata";

// See Models.cs/DeviceData
class DeviceData implements IDeviceData {
    public dataID: number;
    public collectorID: number;
    public value: string;
    public timeStamp: string;
}
