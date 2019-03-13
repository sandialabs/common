define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    // A lot of these were taken from:
    // https://www.data-medics.com/forum/list-of-all-s-m-a-r-t-attributes-including-vendor-specific-t1476.html
    // We're using the generic attributes right now. Maybe we can get smarter and report the vendor-specific
    // ones later.
    var ESmartAttribute;
    (function (ESmartAttribute) {
        ESmartAttribute[ESmartAttribute["Invalid"] = 0] = "Invalid";
        ESmartAttribute[ESmartAttribute["RawReadErrorRate"] = 1] = "RawReadErrorRate";
        ESmartAttribute[ESmartAttribute["ThroughputPerformance"] = 2] = "ThroughputPerformance";
        ESmartAttribute[ESmartAttribute["SpinupTime"] = 3] = "SpinupTime";
        ESmartAttribute[ESmartAttribute["StartStopCount"] = 4] = "StartStopCount";
        ESmartAttribute[ESmartAttribute["ReallocatedSectorCount"] = 5] = "ReallocatedSectorCount";
        ESmartAttribute[ESmartAttribute["ReadChannelMargin"] = 6] = "ReadChannelMargin";
        ESmartAttribute[ESmartAttribute["SeekErrorRate"] = 7] = "SeekErrorRate";
        ESmartAttribute[ESmartAttribute["SeekTimerPerformance"] = 8] = "SeekTimerPerformance";
        ESmartAttribute[ESmartAttribute["PowerOnHoursCount"] = 9] = "PowerOnHoursCount";
        ESmartAttribute[ESmartAttribute["SpinupRetryCount"] = 10] = "SpinupRetryCount";
        ESmartAttribute[ESmartAttribute["CalibrationRetryCount"] = 11] = "CalibrationRetryCount";
        ESmartAttribute[ESmartAttribute["PowerCycleCount"] = 12] = "PowerCycleCount";
        ESmartAttribute[ESmartAttribute["SoftReadErrorRate1"] = 13] = "SoftReadErrorRate1";
        ESmartAttribute[ESmartAttribute["AvailableReservedSpace1"] = 170] = "AvailableReservedSpace1";
        ESmartAttribute[ESmartAttribute["ProgramFailCount"] = 171] = "ProgramFailCount";
        ESmartAttribute[ESmartAttribute["EraseFailBlockCount"] = 172] = "EraseFailBlockCount";
        ESmartAttribute[ESmartAttribute["WearLevelCount"] = 173] = "WearLevelCount";
        ESmartAttribute[ESmartAttribute["UnexpectedPowerLossCount"] = 174] = "UnexpectedPowerLossCount";
        ESmartAttribute[ESmartAttribute["SATADownshiftCount"] = 183] = "SATADownshiftCount";
        ESmartAttribute[ESmartAttribute["EndToEndError"] = 184] = "EndToEndError";
        ESmartAttribute[ESmartAttribute["UncorrectableErrorCount"] = 187] = "UncorrectableErrorCount";
        ESmartAttribute[ESmartAttribute["CommandTimeout"] = 188] = "CommandTimeout";
        ESmartAttribute[ESmartAttribute["HighFlyWrites"] = 189] = "HighFlyWrites";
        ESmartAttribute[ESmartAttribute["AirflowTemperature"] = 190] = "AirflowTemperature";
        ESmartAttribute[ESmartAttribute["GSenseErrorRate1"] = 191] = "GSenseErrorRate1";
        ESmartAttribute[ESmartAttribute["UnsafeShutdownCount"] = 192] = "UnsafeShutdownCount";
        ESmartAttribute[ESmartAttribute["LoadUnloadCycleCount"] = 193] = "LoadUnloadCycleCount";
        ESmartAttribute[ESmartAttribute["Temperature1"] = 194] = "Temperature1";
        ESmartAttribute[ESmartAttribute["HardwareECCRecovered"] = 195] = "HardwareECCRecovered";
        ESmartAttribute[ESmartAttribute["ReallocationCount"] = 196] = "ReallocationCount";
        ESmartAttribute[ESmartAttribute["CurrentPendingSectorCount"] = 197] = "CurrentPendingSectorCount";
        ESmartAttribute[ESmartAttribute["OfflineScanUncorrectableCount"] = 198] = "OfflineScanUncorrectableCount";
        ESmartAttribute[ESmartAttribute["InterfaceCRCErrorRate"] = 199] = "InterfaceCRCErrorRate";
        ESmartAttribute[ESmartAttribute["WriteErrorRate"] = 200] = "WriteErrorRate";
        ESmartAttribute[ESmartAttribute["SoftReadErrorRate2"] = 201] = "SoftReadErrorRate2";
        ESmartAttribute[ESmartAttribute["DataAddressMarkErrors"] = 202] = "DataAddressMarkErrors";
        ESmartAttribute[ESmartAttribute["RunOutCancel"] = 203] = "RunOutCancel";
        ESmartAttribute[ESmartAttribute["SoftECCCorrection"] = 204] = "SoftECCCorrection";
        ESmartAttribute[ESmartAttribute["ThermalAsperityRate"] = 205] = "ThermalAsperityRate";
        ESmartAttribute[ESmartAttribute["FlyingHeight"] = 206] = "FlyingHeight";
        ESmartAttribute[ESmartAttribute["SpinHighCurrent"] = 207] = "SpinHighCurrent";
        ESmartAttribute[ESmartAttribute["SpinBuzz"] = 208] = "SpinBuzz";
        ESmartAttribute[ESmartAttribute["OfflineSeekPerformance"] = 209] = "OfflineSeekPerformance";
        ESmartAttribute[ESmartAttribute["VibrationDuringWrite"] = 211] = "VibrationDuringWrite";
        ESmartAttribute[ESmartAttribute["ShockDuringWrite"] = 212] = "ShockDuringWrite";
        ESmartAttribute[ESmartAttribute["DiskShift"] = 220] = "DiskShift";
        ESmartAttribute[ESmartAttribute["GSenseErrorRate2"] = 221] = "GSenseErrorRate2";
        ESmartAttribute[ESmartAttribute["LoadedHours"] = 222] = "LoadedHours";
        ESmartAttribute[ESmartAttribute["LoadUnloadRetryCount"] = 223] = "LoadUnloadRetryCount";
        ESmartAttribute[ESmartAttribute["LoadFriction"] = 224] = "LoadFriction";
        ESmartAttribute[ESmartAttribute["HostWrites"] = 225] = "HostWrites";
        ESmartAttribute[ESmartAttribute["TimerWorkloadMediaWear"] = 226] = "TimerWorkloadMediaWear";
        ESmartAttribute[ESmartAttribute["TimerWorkloadReadWriteRatio"] = 227] = "TimerWorkloadReadWriteRatio";
        ESmartAttribute[ESmartAttribute["TimerWorkloadTimer"] = 228] = "TimerWorkloadTimer";
        ESmartAttribute[ESmartAttribute["GMRHeadAmplitude"] = 230] = "GMRHeadAmplitude";
        ESmartAttribute[ESmartAttribute["Temperature2"] = 231] = "Temperature2";
        ESmartAttribute[ESmartAttribute["AvailableReservedSpace2"] = 232] = "AvailableReservedSpace2";
        ESmartAttribute[ESmartAttribute["MediaWearoutIndicator"] = 233] = "MediaWearoutIndicator";
        ESmartAttribute[ESmartAttribute["HeadFlyingHours"] = 240] = "HeadFlyingHours";
        ESmartAttribute[ESmartAttribute["LifetimeWrites"] = 241] = "LifetimeWrites";
        ESmartAttribute[ESmartAttribute["LifetimeReads"] = 242] = "LifetimeReads";
        ESmartAttribute[ESmartAttribute["LifetimeWritesNAND"] = 249] = "LifetimeWritesNAND";
        ESmartAttribute[ESmartAttribute["ReadErrorRetryRate"] = 250] = "ReadErrorRetryRate";
        ESmartAttribute[ESmartAttribute["FreeFallProtection"] = 254] = "FreeFallProtection";
    })(ESmartAttribute || (ESmartAttribute = {}));
    var SmartAttribute = /** @class */ (function () {
        function SmartAttribute(attr) {
            this.attribute = attr.Attribute;
            this.name = attr.Name;
            this.value = attr.Value;
        }
        return SmartAttribute;
    }());
    exports.SmartAttribute = SmartAttribute;
    var HardDisk = /** @class */ (function () {
        function HardDisk(hd) {
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
        return HardDisk;
    }());
    exports.HardDisk = HardDisk;
    var SmartData = /** @class */ (function () {
        function SmartData(disks) {
            this.update(disks);
        }
        SmartData.prototype.selectHardDisk = function (driveLetter) {
            var hd = null;
            for (var i = 0; i < this.disks.length; ++i) {
                var disk = this.disks[i];
                for (var j = 0; j < disk.driveLetters.length; ++j) {
                    if (disk.driveLetters[j] === driveLetter) {
                        hd = disk;
                        break;
                    }
                }
            }
            this.selectedDisk = hd;
            return hd;
        };
        SmartData.prototype.update = function (disks) {
            this.disks = [];
            this.selectedDisk = null;
            var deviceID = this.selectedDisk === null ? null : this.selectedDisk.deviceID;
            for (var i = 0; i < disks.length; ++i) {
                if (!disks[i])
                    continue;
                var hd = new HardDisk(disks[i]);
                this.disks.push(hd);
                if (hd.deviceID === hd.deviceID)
                    this.selectedDisk = hd;
            }
        };
        return SmartData;
    }());
    exports.SmartData = SmartData;
});
//# sourceMappingURL=smart.js.map