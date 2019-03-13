import { ECollectorType } from "../enums/collectortype.enum";
import { IValueInfo } from "./ivalueinfo";

// See Models.cs
export class ValueInfo {
    deviceID: number;
    collectorType: ECollectorType;
    value: string;
    timestamp: Date;

    constructor(ivi: IValueInfo) {
        this.deviceID = ivi.deviceID;
        this.collectorType = ivi.collectorType;
        this.value = ivi.value;
        this.timestamp = new Date(ivi.timestamp);
    }

    public parseValue(): string[] {
        var value: string[] = [];
        if (this.value)
            value = JSON.parse(this.value)['Value'];
        return value;
    }
}
