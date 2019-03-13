import { ECollectorType } from '../enums/collectortype.enum';

export interface ICollectorInfo {
    id: number;
    deviceID: number;
    name: string;
    collectorType: ECollectorType;
    isEnabled: boolean;
    frequencyInMinutes: number;
    lastCollectionAttempt: string;
    lastCollectedAt: string;
    nextCollectionTime: string;
    successfullyCollected: boolean;
    isBeingCollected: boolean;
}

export class CollectorInfo {
    public id: number;
    public deviceID: number;
    public name: string;
    public fullName: string;    // device name + collector name
    public collectorType: ECollectorType;
    public isEnabled: boolean;
    public frequencyInMinutes: number;
    public lastCollectionAttempt: Date;
    public lastCollectedAt: Date;
    public nextCollectionTime: Date;
    public successfullyCollected: boolean;
    public isBeingCollected: boolean;

    constructor(info: ICollectorInfo) {
        this.id = info.id;
        this.deviceID = info.deviceID;
        this.name = this.fullName = info.name;
        this.collectorType = info.collectorType;
        this.isEnabled = info.isEnabled;
        this.frequencyInMinutes = info.frequencyInMinutes;
        this.lastCollectionAttempt = info.lastCollectionAttempt ? new Date(info.lastCollectionAttempt) : null;
        this.lastCollectedAt = info.lastCollectedAt ? new Date(info.lastCollectedAt) : null;
        this.nextCollectionTime = info.nextCollectionTime ? new Date(info.nextCollectionTime) : null;
        this.successfullyCollected = info.successfullyCollected;
        this.isBeingCollected = info.isBeingCollected;

        // Each collector's name is a combination of the device and the collector type,
        // like this: <devicename>.<collector>.
        // This finds the '.' between the "<devicename>.<collector>", then removes the "<devicename>." part
        let index = this.name.indexOf(".");
        if (index >= 0)
            this.name = this.name.substr(index + 1);
    }
}