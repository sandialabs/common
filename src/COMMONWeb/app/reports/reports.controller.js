/// <reference types="angular" />
/// <reference types="angular-route" />
/// <reference types="angular-ui-bootstrap" />
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
define(["require", "exports", "./report", "../classes/utilities", "../enums/devicetypes.enum", "../charts/chartjs"], function (require, exports, report_1, utilities_1, devicetypes_enum_1, chartjs_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var download;
    var ReportsController = /** @class */ (function () {
        function ReportsController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, reportType, reportName) {
            var _this = this;
            this.dataService = dataService;
            this.$routeParams = $routeParams;
            this.$scope = $scope;
            this.devicemanagerService = devicemanagerService;
            this.network = network;
            this.configurationService = configurationService;
            this.reportName = reportName;
            this.deviceID = parseInt($routeParams.id);
            this.currentDate = new Date();
            this.daysToRetrieve = 15;
            this.daysToRetrieveChoices = [15, 30, 60, 90, 120, 150, 180];
            this.type = reportType;
            configurationService.get()
                .then(function (c) {
                _this.siteName = c.siteName;
            });
        }
        //downloadReport() {
        //    let t = this;
        //    this.dataService.downloadReport($('#report').html())
        //        .then((reportBytes: Uint8Array) => {
        //            //var decoded_bytes = atob(reportBytes);
        //            download(reportBytes, t.reportName + ".pdf", "application/pdf")
        //        });
        //}
        ReportsController.prototype.changeDaysToRetrieve = function (days) {
            this.daysToRetrieve = days;
            var now = new Date();
            this.startDate = new Date(now.getTime() - (days * 24 * 60 * 60 * 1000));
            this.endDate = null;
            this.build();
        };
        ReportsController.prototype.changeDateRange = function (start, end) {
            this.startDate = new Date(start);
            this.endDate = new Date(end);
            this.daysToRetrieve = null;
            this.build();
        };
        return ReportsController;
    }());
    var SiteReportController = /** @class */ (function (_super) {
        __extends(SiteReportController, _super);
        function SiteReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, chartBridgeFactoryService) {
            var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, network, configurationService, report_1.EReportTypes.Site, "Site") || this;
            _this.dataService = dataService;
            _this.$routeParams = $routeParams;
            _this.$scope = $scope;
            _this.devicemanagerService = devicemanagerService;
            _this.network = network;
            _this.configurationService = configurationService;
            _this.chartBridgeFactoryService = chartBridgeFactoryService;
            var t = _this;
            var fs = chartBridgeFactoryService.$get();
            _this.networkChartSettings = fs.createChartSettings("", 75);
            _this.networkChartSettings.displayAxes = _this.networkChartSettings.displayLegend = false;
            _this.popupIsOpen = {
                start: false,
                end: false
            };
            // Default the site report to the previous month. The end date
            // is not inclusive, so we go from midnight on the 1st of the
            // month to midnight on the 1st of the next month.
            var now = new Date();
            var year = now.getFullYear();
            // JavaScript month goes from 0-11
            var month = now.getMonth() - 1;
            if (month < 0) {
                month = 11;
                --year;
            }
            _this.startDate = new Date(year, month, 1, 0, 0, 0, 0);
            _this.endDate = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0, 0);
            _this.startDateOptions = {
                datepickerMode: 'month',
                showWeeks: false,
                maxDate: _this.endDate
            };
            _this.endDateOptions = {
                datepickerMode: 'month',
                showWeeks: false,
                minDate: _this.startDate
            };
            _this.popupStartDate = _this.startDate;
            _this.popupEndDate = _this.endDate;
            _this.machineReports = [];
            // onDateChange will be called whenever the user chooses
            // a new date. Until they do, let's start building now.
            t.build();
            return _this;
        }
        SiteReportController.prototype.build = function () {
            this.reportName = "Site Report";
            this.report = new report_1.SiteReport(this, this.dataService, this.configurationService);
            this.report.build();
            var t = this;
            this.machineReports = [];
            this.configurationService.get()
                .then(function (config) {
                for (var i = 0; i < config.devices.length; ++i) {
                    var device = config.devices[i];
                    if (device.type == devicetypes_enum_1.EDeviceTypes.Server || device.type == devicetypes_enum_1.EDeviceTypes.Workstation) {
                        var s = {
                            deviceID: device.id,
                            device: device,
                            type: device.type == devicetypes_enum_1.EDeviceTypes.Server ? report_1.EReportTypes.Server : report_1.EReportTypes.Workstation,
                            startDate: t.startDate,
                            endDate: t.endDate,
                            reportName: device.name,
                        };
                        var m = new report_1.MachineReport(s, t.dataService, t.configurationService);
                        t.machineReports.push(m);
                        m.build();
                    }
                }
            });
        };
        SiteReportController.prototype.openStartPopup = function () {
            this.popupIsOpen.start = true;
        };
        SiteReportController.prototype.openEndPopup = function () {
            this.popupIsOpen.end = true;
        };
        SiteReportController.prototype.onDateChange = function () {
            this.fixDateRange();
            this.startDate = utilities_1.Utilities.toMidnight(this.popupStartDate);
            this.endDateOptions.minDate = this.popupStartDate;
            // We want the end date to encompass the entire day, so that means
            // we want to add 1 day to the date to get it to midnight of the next day.
            this.endDate = new Date(utilities_1.Utilities.toMidnight(this.popupEndDate).getTime() + (24 * 60 * 60 * 1000));
            this.startDateOptions.maxDate = this.popupEndDate;
            this.build();
        };
        // Make sure start comes before end
        SiteReportController.prototype.fixDateRange = function () {
            if (this.popupEndDate && this.popupStartDate && this.popupEndDate.getTime() < this.popupStartDate.getTime()) {
                var temp = this.popupStartDate;
                this.popupStartDate = this.popupEndDate;
                this.popupEndDate = temp;
            }
        };
        SiteReportController.Factory = function () {
            var factory = function (dataService, $routeParams, $scope, devicemanagerService, network, configurationService, chartBridgeFactoryService) {
                return new SiteReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, chartBridgeFactoryService);
            };
            factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService', 'chartBridgeFactoryService'];
            return factory;
        };
        return SiteReportController;
    }(ReportsController));
    exports.SiteReportController = SiteReportController;
    var MachineReportController = /** @class */ (function (_super) {
        __extends(MachineReportController, _super);
        function MachineReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
            var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, network, configurationService, report_1.EReportTypes.Workstation, "Unknown Machine") || this;
            _this.dataService = dataService;
            _this.$routeParams = $routeParams;
            _this.$scope = $scope;
            _this.devicemanagerService = devicemanagerService;
            _this.network = network;
            _this.configurationService = configurationService;
            _this.build();
            return _this;
        }
        MachineReportController.prototype.build = function () {
            var t = this;
            this.devicemanagerService.get()
                .then(function (deviceManager) {
                var device = deviceManager.findDeviceFromID(t.deviceID);
                if (device) {
                    t.device = device;
                    t.reportName = device.name;
                    t.report = new report_1.MachineReport(t, t.dataService, t.configurationService);
                    t.report.build();
                }
            });
        };
        MachineReportController.Factory = function () {
            var factory = function (dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
                return new MachineReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
            };
            factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
            return factory;
        };
        return MachineReportController;
    }(ReportsController));
    exports.MachineReportController = MachineReportController;
    var NetworkReportController = /** @class */ (function (_super) {
        __extends(NetworkReportController, _super);
        function NetworkReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
            var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, network, configurationService, report_1.EReportTypes.Network, "Network Report") || this;
            _this.dataService = dataService;
            _this.$routeParams = $routeParams;
            _this.$scope = $scope;
            _this.devicemanagerService = devicemanagerService;
            _this.network = network;
            _this.configurationService = configurationService;
            _this.networkChartSettings = new chartjs_1.ChartJSSettings("", 75);
            _this.networkChartSettings.displayAxes = _this.networkChartSettings.displayLegend = false;
            _this.networkChartSettings.makeResponsive = false;
            // Remove the choices for the # of days to retrieve because this report is a current snapshot
            _this.daysToRetrieveChoices = [];
            _this.build();
            return _this;
        }
        NetworkReportController.prototype.build = function () {
            var t = this;
            this.network.get()
                .then(function (n) {
                t.report = new report_1.NetworkReport(t, t.dataService, t.configurationService, n);
                t.report.build();
            });
        };
        NetworkReportController.Factory = function () {
            var factory = function (dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
                return new NetworkReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
            };
            factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
            return factory;
        };
        return NetworkReportController;
    }(ReportsController));
    exports.NetworkReportController = NetworkReportController;
    var CASLoadReportController = /** @class */ (function (_super) {
        __extends(CASLoadReportController, _super);
        function CASLoadReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
            var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, network, configurationService, report_1.EReportTypes.CASLoad, "CAS Load Report") || this;
            _this.dataService = dataService;
            _this.$routeParams = $routeParams;
            _this.$scope = $scope;
            _this.devicemanagerService = devicemanagerService;
            _this.network = network;
            _this.configurationService = configurationService;
            // Remove the choices for the # of days to retrieve because this report is a current snapshot
            _this.daysToRetrieveChoices = [];
            // Get 30 days of data for the report; that should be plenty
            _this.startDate = new Date(new Date().getTime() - (30 * 24 * 60 * 60 * 1000));
            _this.build();
            return _this;
        }
        CASLoadReportController.prototype.build = function () {
            var t = this;
            this.network.get()
                .then(function (n) {
                t.report = new report_1.CASLoadReport(t, t.dataService, t.configurationService, t.devicemanagerService);
                t.report.build();
            });
        };
        CASLoadReportController.Factory = function () {
            var factory = function (dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
                return new CASLoadReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
            };
            factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
            return factory;
        };
        return CASLoadReportController;
    }(ReportsController));
    exports.CASLoadReportController = CASLoadReportController;
    var IssuesReportController = /** @class */ (function (_super) {
        __extends(IssuesReportController, _super);
        function IssuesReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
            var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, network, configurationService, report_1.EReportTypes.Issues, "Issues Report") || this;
            _this.dataService = dataService;
            _this.$routeParams = $routeParams;
            _this.$scope = $scope;
            _this.devicemanagerService = devicemanagerService;
            _this.network = network;
            _this.configurationService = configurationService;
            // Remove the choices for the # of days to retrieve because this report is a current snapshot
            _this.daysToRetrieveChoices = [];
            _this.build();
            return _this;
        }
        IssuesReportController.prototype.build = function () {
            var t = this;
            this.network.get()
                .then(function (n) {
                t.report = new report_1.IssuesReport(t, t.dataService, t.configurationService, t.devicemanagerService);
                t.report.build();
            });
        };
        IssuesReportController.Factory = function () {
            var factory = function (dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
                return new IssuesReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
            };
            factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
            return factory;
        };
        return IssuesReportController;
    }(ReportsController));
    exports.IssuesReportController = IssuesReportController;
    var SiteConfigurationReportController = /** @class */ (function (_super) {
        __extends(SiteConfigurationReportController, _super);
        function SiteConfigurationReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
            var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, network, configurationService, report_1.EReportTypes.SiteConfiguration, "Site Configuration Report") || this;
            _this.dataService = dataService;
            _this.$routeParams = $routeParams;
            _this.$scope = $scope;
            _this.devicemanagerService = devicemanagerService;
            _this.network = network;
            _this.configurationService = configurationService;
            // Remove the choices for the # of days to retrieve because this report is a current snapshot
            _this.daysToRetrieveChoices = [];
            _this.build();
            return _this;
        }
        SiteConfigurationReportController.prototype.build = function () {
            this.report = new report_1.ConfigReport(this, this.dataService, this.configurationService);
            this.report.build();
        };
        SiteConfigurationReportController.Factory = function () {
            var factory = function (dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
                return new SiteConfigurationReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
            };
            factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
            return factory;
        };
        return SiteConfigurationReportController;
    }(ReportsController));
    exports.SiteConfigurationReportController = SiteConfigurationReportController;
});
//# sourceMappingURL=reports.controller.js.map