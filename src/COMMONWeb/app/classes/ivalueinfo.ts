import { ECollectorType } from '../enums/collectortype.enum';

// See Models.cs
export interface IValueInfo {
    deviceID: number;
    collectorType: ECollectorType;
    value: string;
    timestamp: string;
}