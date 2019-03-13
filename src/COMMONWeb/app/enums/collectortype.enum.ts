export enum ECollectorType {
    Memory,                 // 0
    Disk,
    CPUUsage,
    NICUsage,
    Uptime,
    LastBootTime,           // 5
    Processes,
    Ping,
    InstalledApplications,
    Services,
    //Database,               // 10
    SystemErrors = 11,
    ApplicationErrors,
    DatabaseSize,
    UPS,
    DiskSpeed,              // 15
    Configuration,

    Unknown = -1,
}

export class CollectorTypeExtensions {
    public isHidden(type: ECollectorType): boolean {
        var isHidden = false;
        switch (type) {
            case ECollectorType.Configuration:
                isHidden = true;
                break;
        }
        return isHidden;
    }
}