import { DiskUsage } from "./disk";

// A lot of these were taken from:
// https://www.data-medics.com/forum/list-of-all-s-m-a-r-t-attributes-including-vendor-specific-t1476.html
// We're using the generic attributes right now. Maybe we can get smarter and report the vendor-specific
// ones later.

enum ESmartAttribute
{
    Invalid = 0x00,
    RawReadErrorRate = 0x01,
    ThroughputPerformance = 0x02,
    SpinupTime = 0x03,
    StartStopCount = 0x04,
    ReallocatedSectorCount = 0x05,
    ReadChannelMargin = 0x06,
    SeekErrorRate = 0x07,
    SeekTimerPerformance = 0x08,
    PowerOnHoursCount = 0x09,
    SpinupRetryCount = 0x0A,
    CalibrationRetryCount = 0x0B,
    PowerCycleCount = 0x0C,
    SoftReadErrorRate1 = 0x0D,

    AvailableReservedSpace1 = 0xAA,
    ProgramFailCount = 0xAB,
    EraseFailBlockCount = 0xAC,
    WearLevelCount = 0xAD,
    UnexpectedPowerLossCount = 0xAE,

    SATADownshiftCount = 0xB7,
    EndToEndError = 0xB8,

    UncorrectableErrorCount = 0xBB,
    CommandTimeout = 0xBC,
    HighFlyWrites = 0xBD,
    AirflowTemperature = 0xBE,
    GSenseErrorRate1 = 0xBF,
    UnsafeShutdownCount = 0xC0,
    LoadUnloadCycleCount = 0xC1,
    Temperature1 = 0xC2,
    HardwareECCRecovered = 0xC3,
    ReallocationCount = 0xC4,
    CurrentPendingSectorCount = 0xC5,
    OfflineScanUncorrectableCount = 0xC6,
    InterfaceCRCErrorRate = 0xC7,
    WriteErrorRate = 0xC8,
    SoftReadErrorRate2 = 0xC9,
    DataAddressMarkErrors = 0xCA,
    RunOutCancel = 0xCB,
    SoftECCCorrection = 0xCC,
    ThermalAsperityRate = 0xCD,
    FlyingHeight = 0xCE,
    SpinHighCurrent = 0xCF,
    SpinBuzz = 0xD0,
    OfflineSeekPerformance = 0xD1,
    VibrationDuringWrite = 0xD3,
    ShockDuringWrite = 0xD4,

    DiskShift = 0xDC,
    GSenseErrorRate2 = 0xDD,
    LoadedHours = 0xDE,
    LoadUnloadRetryCount = 0xDF,
    LoadFriction = 0xE0,
    HostWrites = 0xE1,
    TimerWorkloadMediaWear = 0xE2,
    TimerWorkloadReadWriteRatio = 0xE3,
    TimerWorkloadTimer = 0xE4,

    GMRHeadAmplitude = 0xE6,
    Temperature2 = 0xE7,
    AvailableReservedSpace2 = 0xE8,
    MediaWearoutIndicator = 0xE9,

    HeadFlyingHours = 0xF0,
    LifetimeWrites = 0xF1,
    LifetimeReads = 0xF2,

    LifetimeWritesNAND = 0xF9,
    ReadErrorRetryRate = 0xFA,

    FreeFallProtection = 0xFE,
}

export interface ISmartAttribute {
    Attribute: number;
    Name: string;
    Value: number;
}

export class SmartAttribute {
    attribute: ESmartAttribute;
    name: string;
    value: number;

    constructor(attr: ISmartAttribute) {
        this.attribute = attr.Attribute;
        this.name = attr.Name;
        this.value = attr.Value;
    }
}

export interface ISmartData {
    smartDisk: HardDisk;
}

export interface IHardDisk {
    DeviceID: string;
    PnpDeviceID: string;
    DriveLetters: string[];
    FailureIsPredicted?: boolean;
    Model: string;
    InterfaceType: string;
    SerialNum: string;
    SmartAttributes: ISmartAttribute[];
    Timestamp: string;
}

export class HardDisk {
    deviceID: string;
    pnpDeviceID: string;
    driveLetters: string[];
    driveLettersAsString: string;
    failureIsPredicted?: boolean;
    model: string;
    interfaceType: string;
    serialNum: string;
    attributes: SmartAttribute[];
    timestamp: Date;

    constructor(hd: IHardDisk) {
        this.deviceID = hd.DeviceID;
        this.pnpDeviceID = hd.PnpDeviceID;
        this.driveLetters = hd.DriveLetters.slice(0);
        this.driveLetters.sort();
        this.driveLettersAsString = "";
        for (var i = 0; i < this.driveLetters.length; ++i) {
            if (i > 0)
                this.driveLettersAsString += ", ";
            this.driveLettersAsString += this.driveLetters[i];
        }
        this.failureIsPredicted = hd.FailureIsPredicted;
        this.model = hd.Model;
        this.interfaceType = hd.InterfaceType;
        this.serialNum = hd.SerialNum;
        this.attributes = [];
        for (var i = 0; i < hd.SmartAttributes.length; ++i)
            this.attributes.push(new SmartAttribute(hd.SmartAttributes[i]));
        this.timestamp = new Date(hd.Timestamp);
    }
}

export class SmartData {
    disks: HardDisk[];
    selectedDisk: HardDisk;

    constructor(disks: IHardDisk[]) {
        this.update(disks);
    }

    selectHardDisk(driveLetter: string): HardDisk {
        let hd: HardDisk = null;
        for (var i = 0; i < this.disks.length; ++i) {
            let disk = this.disks[i];
            for (var j = 0; j < disk.driveLetters.length; ++j) {
                if (disk.driveLetters[j] === driveLetter) {
                    hd = disk;
                    break;
                }
            }
        }
        this.selectedDisk = hd;
        return hd;
    }

    update(disks: IHardDisk[]) {
        this.disks = [];
        this.selectedDisk = null;
        let deviceID = this.selectedDisk === null ? null : this.selectedDisk.deviceID;

        for (var i = 0; i < disks.length; ++i) {
            if (!disks[i])
                continue;

            let hd = new HardDisk(disks[i]);
            this.disks.push(hd);
            if (hd.deviceID === hd.deviceID)
                this.selectedDisk = hd;
        }
    }
}
