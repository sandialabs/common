/// <reference types="angular" />
/// <reference types="angular-route" />
/// <reference types="angular-ui-bootstrap" />

import { DataService } from "../services/data.service";
import { DeviceManagerService } from "../services/devicemanager.service";
import { DeviceManager } from "../classes/devices";
import { DeviceInfo } from "../classes/devices";
import { IReportSettings, EReportTypes, Report, MachineReport, NetworkReport, CASLoadReport, IssuesReport, ConfigReport, SiteReport } from "./report";
import { NetworkService } from "../services/network.service";
import { Network } from "../classes/network";
import { ConfigurationService } from "../services/configuration.service";
import { SystemConfiguration } from "../classes/systemconfiguration";
import { Utilities } from "../classes/utilities";
import { EDeviceTypes } from "../enums/devicetypes.enum";
import { ChartBridgeFactoryService } from "../charts/chartbridgefactory.service";
import { IChartJSSettings, ChartJSSettings } from "../charts/chartjs";
import { ChartJSChartBridgeFactory } from "../charts/chartbridgefactory";

let download: any;

abstract class ReportsController implements ng.IController, IReportSettings {
    daysToRetrieveChoices: number[];
    deviceID: number;
    device: DeviceInfo;
    type: EReportTypes;
    daysToRetrieve?: number;
    startDate?: Date;
    endDate?: Date;
    siteName: string;
    currentDate: Date;

    // The public report object. The children classes will populate this with the appropriate type of report.
    report: Report;

    constructor(
        protected dataService: DataService,
        protected $routeParams: ng.route.IRouteParamsService,
        protected $scope: ng.IScope,
        protected devicemanagerService: DeviceManagerService,
        protected network: NetworkService,
        protected configurationService: ConfigurationService,
        reportType: EReportTypes,
        public reportName: string) {
        this.deviceID = parseInt($routeParams.id);
        this.currentDate = new Date();
        this.daysToRetrieve = 15;
        this.daysToRetrieveChoices = [15, 30, 60, 90, 120, 150, 180];
        this.type = reportType;

        configurationService.get()
            .then((c: SystemConfiguration) => {
                this.siteName = c.siteName;
            });
    }

    abstract build();

    //downloadReport() {
    //    let t = this;
    //    this.dataService.downloadReport($('#report').html())
    //        .then((reportBytes: Uint8Array) => {
    //            //var decoded_bytes = atob(reportBytes);
    //            download(reportBytes, t.reportName + ".pdf", "application/pdf")
    //        });
    //}

    changeDaysToRetrieve(days: number) {
        this.daysToRetrieve = days;
        var now = new Date();
        this.startDate = new Date(now.getTime() - (days * 24 * 60 * 60 * 1000));
        this.endDate = null;
        this.build();
    }

    changeDateRange(start: string, end: string) {
        this.startDate = new Date(start);
        this.endDate = new Date(end);
        this.daysToRetrieve = null;
        this.build();
    }
}

interface IPopupIsOpen {
    start: boolean;
    end: boolean;
}

export class SiteReportController extends ReportsController {
    networkChartSettings: IChartJSSettings;
    popupIsOpen: IPopupIsOpen;
    startDateOptions: angular.ui.bootstrap.IDatepickerPopupConfig;
    endDateOptions: angular.ui.bootstrap.IDatepickerPopupConfig;
    popupStartDate: Date;
    popupEndDate: Date;
    machineReports: MachineReport[];

    constructor(protected dataService: DataService,
        protected $routeParams: ng.route.IRouteParamsService,
        protected $scope: ng.IScope,
        protected devicemanagerService: DeviceManagerService,
        protected network: NetworkService,
        protected configurationService: ConfigurationService,
        protected chartBridgeFactoryService: ChartBridgeFactoryService) {
        super(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, EReportTypes.Site, "Site");

        let t = this;

        let fs: ChartJSChartBridgeFactory = chartBridgeFactoryService.$get();
        this.networkChartSettings = fs.createChartSettings("", 75);
        this.networkChartSettings.displayAxes = this.networkChartSettings.displayLegend = false;

        this.popupIsOpen = {
            start: false,
            end: false
        };

        // Default the site report to the previous month. The end date
        // is not inclusive, so we go from midnight on the 1st of the
        // month to midnight on the 1st of the next month.
        let now = new Date();
        let year = now.getFullYear();
        // JavaScript month goes from 0-11
        let month = now.getMonth() - 1;
        if (month < 0) {
            month = 11;
            --year;
        }
        this.startDate = new Date(year, month, 1, 0, 0, 0, 0);
        this.endDate = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0, 0);

        this.startDateOptions = {
            datepickerMode: 'month',
            showWeeks: false,
            maxDate: this.endDate
        };
        this.endDateOptions = {
            datepickerMode: 'month',
            showWeeks: false,
            minDate: this.startDate
        };

        this.popupStartDate = this.startDate;
        this.popupEndDate = this.endDate;

        this.machineReports = [];

        // onDateChange will be called whenever the user chooses
        // a new date. Until they do, let's start building now.
        t.build();
    }

    build() {
        this.reportName = "Site Report";
        this.report = new SiteReport(this, this.dataService, this.configurationService);
        this.report.build();

        let t = this;
        this.machineReports = [];
        this.configurationService.get()
            .then((config: SystemConfiguration) => {
                for (var i = 0; i < config.devices.length; ++i) {
                    let device: DeviceInfo = config.devices[i];
                    if (device.type == EDeviceTypes.Server || device.type == EDeviceTypes.Workstation) {
                        let s: IReportSettings = {
                            deviceID: device.id,
                            device: device,
                            type: device.type == EDeviceTypes.Server ? EReportTypes.Server : EReportTypes.Workstation,
                            startDate: t.startDate,
                            endDate: t.endDate,
                            reportName: device.name,
                        };
                        let m = new MachineReport(s, t.dataService, t.configurationService);
                        t.machineReports.push(m);
                        m.build();
                    }
                }
            });
    }

    openStartPopup() {
        this.popupIsOpen.start = true;
    }

    openEndPopup() {
        this.popupIsOpen.end = true;
    }

    onDateChange() {
        this.fixDateRange();

        this.startDate = Utilities.toMidnight(this.popupStartDate);
        this.endDateOptions.minDate = this.popupStartDate;

        // We want the end date to encompass the entire day, so that means
        // we want to add 1 day to the date to get it to midnight of the next day.
        this.endDate = new Date(Utilities.toMidnight(this.popupEndDate).getTime() + (24 * 60 * 60 * 1000));
        this.startDateOptions.maxDate = this.popupEndDate;

        this.build();
    }

    // Make sure start comes before end
    protected fixDateRange() {
        if (this.popupEndDate && this.popupStartDate && this.popupEndDate.getTime() < this.popupStartDate.getTime()) {
            let temp = this.popupStartDate;
            this.popupStartDate = this.popupEndDate;
            this.popupEndDate = temp;
        }
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $routeParams: ng.route.IRouteParamsService, $scope: ng.IScope, devicemanagerService: DeviceManagerService, network: NetworkService, configurationService: ConfigurationService, chartBridgeFactoryService: ChartBridgeFactoryService): ng.IController => {
            return new SiteReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, chartBridgeFactoryService);
        }
        factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService', 'chartBridgeFactoryService'];
        return factory;
    }
}

export class MachineReportController extends ReportsController {
    constructor(protected dataService: DataService, protected $routeParams: ng.route.IRouteParamsService, protected $scope: ng.IScope, protected devicemanagerService: DeviceManagerService, protected network: NetworkService, protected configurationService: ConfigurationService) {
        super(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, EReportTypes.Workstation, "Unknown Machine");

        this.build();
    }

    build() {
        let t = this;
        this.devicemanagerService.get()
            .then((deviceManager: DeviceManager) => {
                var device = deviceManager.findDeviceFromID(t.deviceID);
                if (device) {
                    t.device = device;
                    t.reportName = device.name;
                    t.report = new MachineReport(t, t.dataService, t.configurationService);
                    t.report.build();
                }
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $routeParams: ng.route.IRouteParamsService, $scope: ng.IScope, devicemanagerService: DeviceManagerService, network: NetworkService, configurationService: ConfigurationService): ng.IController => {
            return new MachineReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
        }
        factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
        return factory;
    }
}

export class NetworkReportController extends ReportsController {
    networkChartSettings: IChartJSSettings;

    constructor(protected dataService: DataService, protected $routeParams: ng.route.IRouteParamsService, protected $scope: ng.IScope, protected devicemanagerService: DeviceManagerService, protected network: NetworkService, protected configurationService: ConfigurationService) {
        super(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, EReportTypes.Network, "Network Report");

        this.networkChartSettings = new ChartJSSettings("", 75);
        this.networkChartSettings.displayAxes = this.networkChartSettings.displayLegend = false;
        this.networkChartSettings.makeResponsive = false;

        // Remove the choices for the # of days to retrieve because this report is a current snapshot
        this.daysToRetrieveChoices = [];
        this.build();
    }

    build() {
        let t = this;
        this.network.get()
            .then((n: Network) => {
                t.report = new NetworkReport(t, t.dataService, t.configurationService, n);
                t.report.build();
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $routeParams: ng.route.IRouteParamsService, $scope: ng.IScope, devicemanagerService: DeviceManagerService, network: NetworkService, configurationService: ConfigurationService): ng.IController => {
            return new NetworkReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
        }
        factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
        return factory;
    }
}

export class CASLoadReportController extends ReportsController {
    constructor(protected dataService: DataService, protected $routeParams: ng.route.IRouteParamsService, protected $scope: ng.IScope, protected devicemanagerService: DeviceManagerService, protected network: NetworkService, protected configurationService: ConfigurationService) {
        super(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, EReportTypes.CASLoad, "CAS Load Report");

        // Remove the choices for the # of days to retrieve because this report is a current snapshot
        this.daysToRetrieveChoices = [];

        // Get 30 days of data for the report; that should be plenty
        this.startDate = new Date(new Date().getTime() - (30 * 24 * 60 * 60 * 1000));

        this.build();
    }

    build() {
        let t = this;
        this.network.get()
            .then((n: Network) => {
                t.report = new CASLoadReport(t, t.dataService, t.configurationService, t.devicemanagerService);
                t.report.build();
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $routeParams: ng.route.IRouteParamsService, $scope: ng.IScope, devicemanagerService: DeviceManagerService, network: NetworkService, configurationService: ConfigurationService): ng.IController => {
            return new CASLoadReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
        }
        factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
        return factory;
    }
}

export class IssuesReportController extends ReportsController {
    constructor(protected dataService: DataService, protected $routeParams: ng.route.IRouteParamsService, protected $scope: ng.IScope, protected devicemanagerService: DeviceManagerService, protected network: NetworkService, protected configurationService: ConfigurationService) {
        super(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, EReportTypes.Issues, "Issues Report");

        // Remove the choices for the # of days to retrieve because this report is a current snapshot
        this.daysToRetrieveChoices = [];

        this.build();
    }

    build() {
        let t = this;
        this.network.get()
            .then((n: Network) => {
                t.report = new IssuesReport(t, t.dataService, t.configurationService, t.devicemanagerService);
                t.report.build();
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $routeParams: ng.route.IRouteParamsService, $scope: ng.IScope, devicemanagerService: DeviceManagerService, network: NetworkService, configurationService: ConfigurationService): ng.IController => {
            return new IssuesReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
        }
        factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
        return factory;
    }
}

export class SiteConfigurationReportController extends ReportsController {
    constructor(protected dataService: DataService, protected $routeParams: ng.route.IRouteParamsService, protected $scope: ng.IScope, protected devicemanagerService: DeviceManagerService, protected network: NetworkService, protected configurationService: ConfigurationService) {
        super(dataService, $routeParams, $scope, devicemanagerService, network, configurationService, EReportTypes.SiteConfiguration, "Site Configuration Report");

        // Remove the choices for the # of days to retrieve because this report is a current snapshot
        this.daysToRetrieveChoices = [];

        this.build();
    }

    build() {
        this.report = new ConfigReport(this, this.dataService, this.configurationService);
        this.report.build();
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $routeParams: ng.route.IRouteParamsService, $scope: ng.IScope, devicemanagerService: DeviceManagerService, network: NetworkService, configurationService: ConfigurationService): ng.IController => {
            return new SiteConfigurationReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService);
        }
        factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'networkService', 'configurationService'];
        return factory;
    }
}
