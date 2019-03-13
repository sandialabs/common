using System.ComponentModel;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// A lot of these were taken from:
    /// https://www.data-medics.com/forum/list-of-all-s-m-a-r-t-attributes-including-vendor-specific-t1476.html
    /// We're using the generic attributes right now. Maybe we can get smarter and report the vendor-specific
    /// ones later.
    /// </summary>
    public enum EResource : byte
    {
        [Description("Invalid")]
        Invalid = 0x00,
        [Description("Raw read error rate")]
        RawReadErrorRate = 0x01,
        [Description("Throughput performance")]
        ThroughputPerformance = 0x02,
        [Description("Spinup time")]
        SpinupTime = 0x03,
        [Description("Start/stop count")]
        StartStopCount = 0x04,
        [Description("Reallocated sector count")]
        ReallocatedSectorCount = 0x05,
        [Description("Read channel margin")]
        ReadChannelMargin = 0x06,
        [Description("Seek error rate")]
        SeekErrorRate = 0x07,
        [Description("Seek timer performance")]
        SeekTimerPerformance = 0x08,
        [Description("Power-on hours count")]
        PowerOnHoursCount = 0x09,
        [Description("Spinup retry count")]
        SpinupRetryCount = 0x0A,
        [Description("Calibration retry count")]
        CalibrationRetryCount = 0x0B,
        [Description("Power cycle count")]
        PowerCycleCount = 0x0C,
        [Description("Soft read error rate")]
        SoftReadErrorRate1 = 0x0D,

        [Description("Available reserved space")]
        AvailableReservedSpace1 = 0xAA,
        [Description("Program fail count")]
        ProgramFailCount = 0xAB,
        [Description("Erase fail block count")]
        EraseFailBlockCount = 0xAC,
        [Description("Wear level count")]
        WearLevelCount = 0xAD,
        [Description("Unexpected power loss count")]
        UnexpectedPowerLossCount = 0xAE,

        [Description("SATA downshift count")]
        SATADownshiftCount = 0xB7,
        [Description("End-to-End error")]
        EndToEndError = 0xB8,

        [Description("Uncorrectable error count")]
        UncorrectableErrorCount = 0xBB,
        [Description("Command timeout")]
        CommandTimeout = 0xBC,

        [Description("Airflow Temperature")]
        AirflowTemperature = 0xBE,
        [Description("G-sense error rate")]
        GSenseErrorRate1 = 0xBF,
        [Description("Unsafe shutdown count")]
        UnsafeShutdownCount = 0xC0,
        [Description("Load/Unload cycle count")]
        LoadUnloadCycleCount = 0xC1,
        [Description("Temperature")]
        Temperature1 = 0xC2,
        [Description("Hardware ECC recovered")]
        HardwareECCRecovered = 0xC3,
        [Description("Reallocation count")]
        ReallocationCount = 0xC4,
        [Description("Current pending sector count")]
        CurrentPendingSectorCount = 0xC5,
        [Description("Offline scan uncorrectable count")]
        OfflineScanUncorrectableCount = 0xC6,
        [Description("Interface CRC error rate")]
        InterfaceCRCErrorRate = 0xC7,
        [Description("Write error rate")]
        WriteErrorRate = 0xC8,
        [Description("Soft read error rate")]
        SoftReadErrorRate2 = 0xC9,
        [Description("Data Address Mark errors")]
        DataAddressMarkErrors = 0xCA,
        [Description("Run out cancel")]
        RunOutCancel = 0xCB,
        [Description("Soft ECC correction")]
        SoftECCCorrection = 0xCC,
        [Description("Thermal asperity rate (TAR)")]
        ThermalAsperityRate = 0xCD,
        [Description("Flying height")]
        FlyingHeight = 0xCE,
        [Description("Spin high current")]
        SpinHighCurrent = 0xCF,
        [Description("Spin buzz")]
        SpinBuzz = 0xD0,
        [Description("Offline seek performance")]
        OfflineSeekPerformance = 0xD1,

        [Description("Disk shift")]
        DiskShift = 0xDC,
        [Description("G-sense error rate")]
        GSenseErrorRate2 = 0xDD,
        [Description("Loaded hours")]
        LoadedHours = 0xDE,
        [Description("Load/unload retry count")]
        LoadUnloadRetryCount = 0xDF,
        [Description("Load friction")]
        LoadFriction = 0xE0,
        [Description("Host writes")]
        HostWrites = 0xE1,
        [Description("Timer workload media wear")]
        TimerWorkloadMediaWear = 0xE2,
        [Description("Timer workload read/write ratio")]
        TimerWorkloadReadWriteRatio = 0xE3,
        [Description("Timer workload timer")]
        TimerWorkloadTimer = 0xE4,

        [Description("GMR head amplitude")]
        GMRHeadAmplitude = 0xE6,
        [Description("Temperature")]
        Temperature2 = 0xE7,
        [Description("Available reserved space")]
        AvailableReservedSpace2 = 0xE8,
        [Description("Media wearout indicator")]
        MediaWearoutIndicator = 0xE9,

        [Description("Head flying hours")]
        HeadFlyingHours = 0xF0,
        [Description("Life time writes")]
        LifetimeWrites = 0xF1,
        [Description("Life time reads")]
        LifetimeReads = 0xF2,

        [Description("Life time writes (NAND)")]
        LifetimeWritesNAND = 0xF9,
        [Description("Read error retry rate")]
        ReadErrorRetryRate = 0xFA,
    }
}