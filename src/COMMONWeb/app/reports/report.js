var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "../enums/collectortype.enum", "../classes/machine", "../classes/utilities", "../classes/network", "../classes/applications", "../classes/services", "../classes/systemconfiguration"], function (require, exports, collectortype_enum_1, machine_1, utilities_1, network_1, applications_1, services_1, systemconfiguration_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var EReportTypes;
    (function (EReportTypes) {
        EReportTypes[EReportTypes["Server"] = 0] = "Server";
        EReportTypes[EReportTypes["Workstation"] = 1] = "Workstation";
        EReportTypes[EReportTypes["Network"] = 2] = "Network";
        EReportTypes[EReportTypes["CASLoad"] = 3] = "CASLoad";
        EReportTypes[EReportTypes["Issues"] = 4] = "Issues";
        EReportTypes[EReportTypes["SiteConfiguration"] = 5] = "SiteConfiguration";
        EReportTypes[EReportTypes["Site"] = 6] = "Site";
    })(EReportTypes = exports.EReportTypes || (exports.EReportTypes = {}));
    var EReportSubTypes;
    (function (EReportSubTypes) {
        EReportSubTypes[EReportSubTypes["Memory"] = 0] = "Memory";
        EReportSubTypes[EReportSubTypes["Disk"] = 1] = "Disk";
        EReportSubTypes[EReportSubTypes["CPU"] = 2] = "CPU";
        EReportSubTypes[EReportSubTypes["NIC"] = 3] = "NIC";
    })(EReportSubTypes = exports.EReportSubTypes || (exports.EReportSubTypes = {}));
    var Report = /** @class */ (function () {
        function Report(settings, dataService, configurationService) {
            this.settings = settings;
            this.dataService = dataService;
            this.configurationService = configurationService;
            var t = this;
            configurationService.get()
                .then(function (config) {
                t.siteName = config.siteName;
            });
        }
        return Report;
    }());
    exports.Report = Report;
    var CurrentPeakReportBase = /** @class */ (function () {
        function CurrentPeakReportBase(report) {
            this.currentPercentUsed = report.currentPercentUsed;
            this.peakPercentUsed = report.peakPercentUsed;
            this.peakTimestamp = new Date(report.peakTimestamp);
        }
        return CurrentPeakReportBase;
    }());
    var MemoryReport = /** @class */ (function (_super) {
        __extends(MemoryReport, _super);
        function MemoryReport(report) {
            return _super.call(this, report) || this;
        }
        return MemoryReport;
    }(CurrentPeakReportBase));
    var DiskInfo = /** @class */ (function (_super) {
        __extends(DiskInfo, _super);
        function DiskInfo(info) {
            var _this = _super.call(this, info) || this;
            _this.name = info.name;
            return _this;
        }
        return DiskInfo;
    }(CurrentPeakReportBase));
    var DiskReport = /** @class */ (function () {
        function DiskReport(report) {
            this.disks = [];
            for (var i = 0; i < report.disks.length; ++i)
                this.disks.push(new DiskInfo(report.disks[i]));
        }
        return DiskReport;
    }());
    var CPUReport = /** @class */ (function (_super) {
        __extends(CPUReport, _super);
        function CPUReport(report) {
            return _super.call(this, report) || this;
        }
        return CPUReport;
    }(CurrentPeakReportBase));
    var NICReport = /** @class */ (function (_super) {
        __extends(NICReport, _super);
        function NICReport(report) {
            var _this = _super.call(this, report) || this;
            _this.bps = report.bps;
            _this.peakBps = report.peakBps;
            return _this;
        }
        return NICReport;
    }(CurrentPeakReportBase));
    var MachineReport = /** @class */ (function (_super) {
        __extends(MachineReport, _super);
        function MachineReport(settings, dataService, configurationService) {
            var _this = _super.call(this, settings, dataService, configurationService) || this;
            _this.machine = null;
            _this.memory = null;
            _this.disks = null;
            _this.cpu = null;
            _this.nic = null;
            _this.allProcesses = [];
            _this.splitProcesses = [];
            _this.services = null;
            _this.splitServices = [];
            return _this;
        }
        MachineReport.prototype.build = function () {
            this.machine = new machine_1.Machine(this.settings.device, this.dataService, this.settings.startDate, [
                machine_1.EMachineParts.Applications,
                machine_1.EMachineParts.Database,
                machine_1.EMachineParts.ApplicationErrors,
                machine_1.EMachineParts.SystemErrors
            ]);
            var t = this;
            this.dataService.getSubReport(this.settings.device.id, [EReportSubTypes.Memory, EReportSubTypes.Disk, EReportSubTypes.CPU, EReportSubTypes.NIC], this.settings.startDate, this.settings.endDate)
                .then(function (data) {
                if (data.memory)
                    t.memory = new MemoryReport(data.memory);
                if (data.disk)
                    t.disks = new DiskReport(data.disk);
                if (data.cpu)
                    t.cpu = new CPUReport(data.cpu);
                if (data.nic)
                    t.nic = new NICReport(data.nic);
            });
            this.getProcesses();
            this.getServices();
            this.getAppHistory();
        };
        MachineReport.prototype.getProcesses = function () {
            var t = this;
            this.allProcesses = [];
            this.splitProcesses = [];
            this.dataService.getAllProcesses(this.settings.device.id, this.settings.startDate, this.settings.endDate)
                .then(function (data) {
                if (data) {
                    t.allProcesses = data;
                    t.splitProcesses = utilities_1.Utilities.chunkToTupleOf2(data);
                }
            });
        };
        MachineReport.prototype.getServices = function () {
            var t = this;
            this.services = null;
            this.splitServices = [];
            this.dataService.getServicesData(this.settings.device.id)
                .then(function (data) {
                if (!data)
                    return;
                t.services = new services_1.Services(data);
                if (t.services.services)
                    t.splitServices = utilities_1.Utilities.chunkToTupleOf2(t.services.services);
            });
        };
        MachineReport.prototype.getAppHistory = function () {
            var t = this;
            this.appHistory = null;
            this.dataService.getAppChanges(this.settings.device.id, this.settings.startDate, this.settings.endDate)
                .then(function (data) {
                t.appHistory = new applications_1.AllApplicationsHistory(data);
            });
        };
        return MachineReport;
    }(Report));
    exports.MachineReport = MachineReport;
    var NetworkReport = /** @class */ (function (_super) {
        __extends(NetworkReport, _super);
        function NetworkReport(settings, dataService, configurationService, network) {
            var _this = _super.call(this, settings, dataService, configurationService) || this;
            _this.network = network;
            if (!network)
                return _this;
            _this.networkStatus = network.data;
            _this.mostRecentAttempt = network.maxDate;
            return _this;
        }
        NetworkReport.prototype.build = function () {
        };
        return NetworkReport;
    }(Report));
    exports.NetworkReport = NetworkReport;
    var CASLoadReport = /** @class */ (function (_super) {
        __extends(CASLoadReport, _super);
        function CASLoadReport(settings, dataService, configurationService, datamanagerService) {
            var _this = _super.call(this, settings, dataService, configurationService) || this;
            _this.datamanagerService = datamanagerService;
            _this.devices = [];
            return _this;
        }
        CASLoadReport.prototype.build = function () {
            var _this = this;
            var t = this;
            t.datamanagerService.get()
                .then(function (dm) {
                for (var i = 0; i < dm.windowsDevices.length; ++i) {
                    var device = dm.windowsDevices[i];
                    var m = new machine_1.Machine(device, _this.dataService, _this.settings.startDate, [
                        machine_1.EMachineParts.Memory,
                        machine_1.EMachineParts.DiskUsage,
                        machine_1.EMachineParts.CPU,
                        machine_1.EMachineParts.NIC
                    ]);
                    t.devices.push(m);
                }
            });
        };
        return CASLoadReport;
    }(Report));
    exports.CASLoadReport = CASLoadReport;
    var IssuesReport = /** @class */ (function (_super) {
        __extends(IssuesReport, _super);
        function IssuesReport(settings, dataService, configurationService, datamanagerService) {
            var _this = _super.call(this, settings, dataService, configurationService) || this;
            _this.datamanagerService = datamanagerService;
            _this.devices = [];
            return _this;
        }
        IssuesReport.prototype.build = function () {
            var t = this;
            t.datamanagerService.get()
                .then(function (dm) {
                t.devices = dm.windowsDevices;
            });
        };
        return IssuesReport;
    }(Report));
    exports.IssuesReport = IssuesReport;
    var ConfigReport = /** @class */ (function (_super) {
        __extends(ConfigReport, _super);
        function ConfigReport(settings, dataService, configurationService) {
            var _this = _super.call(this, settings, dataService, configurationService) || this;
            _this.configData = null;
            return _this;
        }
        ConfigReport.prototype.build = function () {
            var t = this;
            this.dataService.getConfiguration()
                .then(function (data) {
                t.configData = new systemconfiguration_1.SystemConfiguration(data);
                var collectorTypeExt = new collectortype_enum_1.CollectorTypeExtensions();
                for (var i = 0; i < t.configData.devices.length; ++i) {
                    var device = t.configData.devices[i];
                    var doomed = [];
                    for (var j = 0; j < device.collectors.length; ++j) {
                        var collector = device.collectors[j];
                        if (collectorTypeExt.isHidden(collector.collectorType))
                            doomed.push(collector.collectorType);
                    }
                    for (var d = 0; d < doomed.length; ++d) {
                        for (var j = 0; j < device.collectors.length; ++j) {
                            var collector = device.collectors[j];
                            if (collector.collectorType == doomed[d]) {
                                device.collectors.splice(j, 1);
                                break;
                            }
                        }
                    }
                }
            });
        };
        return ConfigReport;
    }(Report));
    exports.ConfigReport = ConfigReport;
    var SiteReport = /** @class */ (function (_super) {
        __extends(SiteReport, _super);
        function SiteReport(settings, dataService, configurationService) {
            var _this = _super.call(this, settings, dataService, configurationService) || this;
            _this.networkStatus = [];
            return _this;
        }
        SiteReport.prototype.build = function () {
            var t = this;
            this.dataService.getNetworkStatus(this.settings.startDate, this.settings.endDate)
                .then(function (status) {
                // The statuses should be coming back sorted by IP address,
                // which is what we want, so we won't re-order them.
                for (var i = 0; i < status.length; ++i)
                    t.networkStatus.push(new network_1.NetworkStatus(status[i]));
            });
        };
        return SiteReport;
    }(Report));
    exports.SiteReport = SiteReport;
});
//# sourceMappingURL=report.js.map