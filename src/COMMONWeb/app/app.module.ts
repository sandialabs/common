/// <reference types="angular" />

// Have to do this bit of ugliness to make TypeScript work with RequireJS.
// If you don't do this, you'll get an error saying to use
// import { isDefined } from 'angular'
// So far, so good: tsc compiles fine. But then when you load the site, RequireJS starts looking
// for ./angular.js which doesn't exist.

declare var angular;

import { DataService } from "./services/data.service";
import { DeviceManagerService } from "./services/devicemanager.service";
import { NetworkService } from "./services/network.service";
import { LanguagesService } from "./services/languages.service";
import { ConfigurationService } from "./services/configuration.service";
import { Configuration } from "./app.config";
import { Languages } from "./classes/languages";
import { Network } from "./classes/network";
import { ECollectorType } from './enums/collectortype.enum';
import { EDeviceTypes } from './enums/devicetypes.enum';
import { DeviceManager } from "./classes/devices";
import { OverviewController } from "./controllers/overview.controller";
import { HeaderController } from "./controllers/header.controller";
import { ServerController } from "./controllers/server.controller";
import { WorkstationController } from "./controllers/workstation.controller";
import { DataCollectionController } from "./controllers/datacollection.controller";
import { AboutController } from "./controllers/about.controller";
import { AdminController } from "./controllers/admin.controller";
import { HelpController } from "./controllers/help.controller";
import { GroupController } from "./controllers/group.controller";
import { MachineReportController, NetworkReportController, CASLoadReportController, IssuesReportController, SiteConfigurationReportController, SiteReportController } from "./reports/reports.controller";
import { DeviceController } from "./controllers/device.controller";
import { NetworkHistoryController } from "./controllers/networkhistory.controller";
import { GraphDirective } from "./charts/graph.directive";
import { SystemConfiguration } from "./classes/systemconfiguration";
import { ChartBridgeFactoryService } from "./charts/chartbridgefactory.service";
import { NetworkTableDirective } from "./reports/networktable.directive";
import { MachineSnapshotDirective } from "./reports/machine.snapshot.directive";
import { DataBlockDirective } from "./reports/datablock.directive";
import { PrintDirective } from "./reports/print.directive";
import { DeviceInfoDirective } from "./directives/deviceinfo.directive";
import { EMachineParts } from "./classes/machine";
import { OverviewDeviceAlertDirective } from "./directives/overview.devicealert.directive";
import { CPUBrickDirective, MemoryBrickDirective, DiskBrickDirective, DiskSpeedBrickDirective, NICBrickDirective, ServicesBrickDirective, DatabaseBrickDirective, ProcessesBrickDirective, ApplicationsBrickDirective, UPSBrickDirective, SystemErrorsBrickDirective, ApplicationErrorsBrickDirective } from "./directives/collector.brick.directive";
import { DaysToRetrieveDirective } from "./directives/daystoretrieve.directive";

interface ICOMMONRootScope extends ng.IRootScopeService {
    ECollectorType: any;
    EDeviceTypes: any;
    EMachineParts: any;
}

export class App {
    app: ng.IModule;
    dateFilter: ng.IFilterDate;

    constructor() {
        this.app = angular.module('app', [
            'ngRoute',
            'ui.bootstrap',
            'ngAnimate',
            'ngMaterial',
            'ngMessages',
            'angularUtils.directives.dirPagination',
            'pascalprecht.translate',
            'ngCookies',
            'treasure-overlay-spinner'
        ]);
        this.app.config(Configuration.Factory());
    }

    public Run() {
        this.app.run([
            '$rootScope',
            '$filter',
            'devicemanagerService',
            'networkService',
            'languagesService',
            'configurationService',
            ($rootScope: ICOMMONRootScope,
                $filter: ng.IFilterService,
                devicemanagerService: DeviceManagerService,
                networkService: NetworkService,
                languagesService: LanguagesService,
                configurationService: ConfigurationService) => {
                let t = this;

                $rootScope.ECollectorType = ECollectorType;
                $rootScope.EDeviceTypes = EDeviceTypes;
                $rootScope.EMachineParts = EMachineParts;

                // First thing to do is get the configuration, then update the device manager. We won't do that
                // over and over since the site's configuration will rarely change, and when it does a reload is in order.
                configurationService.get()
                    .then((data: SystemConfiguration) => {
                        if (!data)
                            return;

                        languagesService.get()
                            .then((ls: Languages) => {
                                ls.updateLanguages(data);
                            });

                        devicemanagerService.get(true)
                            .then((deviceManager: DeviceManager) => {
                                deviceManager.setConfiguration(data);

                                deviceManager.startAutomaticUpdate();
                                //networkService.get()
                                //    .then((n: Network) => {
                                //        n.startAutomaticUpdate();
                                //    });

                                devicemanagerService.setConfigured();
                            });
                    });

                t.dateFilter = $filter('date');
            }]
        );

        this.app.factory('dataService', DataService.Factory());
        this.app.factory('devicemanagerService', DeviceManagerService.Factory());
        this.app.factory('networkService', NetworkService.Factory());
        this.app.factory('languagesService', LanguagesService.Factory());
        this.app.factory('configurationService', ConfigurationService.Factory());
        this.app.factory('chartBridgeFactoryService', ChartBridgeFactoryService.Factory());

        this.app.controller('OverviewController', OverviewController.Factory());
        this.app.controller('HeaderController', HeaderController.Factory());
        this.app.controller('ServerController', ServerController.Factory());
        this.app.controller('WorkstationController', WorkstationController.Factory());
        this.app.controller('DataCollectionController', DataCollectionController.Factory());
        this.app.controller('AboutController', AboutController.Factory());
        this.app.controller('AdminController', AdminController.Factory());
        this.app.controller('DeviceController', DeviceController.Factory());
        this.app.controller('HelpController', HelpController.Factory());
        this.app.controller('GroupController', GroupController.Factory());
        this.app.controller('SiteReportController', SiteReportController.Factory());
        this.app.controller('NetworkHistoryController', NetworkHistoryController.Factory());
        this.app.controller('MachineReportController', MachineReportController.Factory());
        this.app.controller('NetworkReportController', NetworkReportController.Factory());
        this.app.controller('CASLoadReportController', CASLoadReportController.Factory());
        this.app.controller('IssuesReportController', IssuesReportController.Factory());
        this.app.controller('SiteConfigurationReportController', SiteConfigurationReportController.Factory());

        this.app.directive('cmGraph', GraphDirective.Factory());
        this.app.directive('cmNetworkTable', NetworkTableDirective.Factory());
        this.app.directive('cmMachineSnapshot', MachineSnapshotDirective.Factory());
        this.app.directive('cmDataBlock', DataBlockDirective.Factory());
        this.app.directive('cmPrint', PrintDirective.Factory());
        this.app.directive('cmDeviceInfo', DeviceInfoDirective.Factory());
        this.app.directive('cmOverviewDeviceAlert', OverviewDeviceAlertDirective.Factory());
        this.app.directive('cmCpuBrick', CPUBrickDirective.Factory());
        this.app.directive('cmMemoryBrick', MemoryBrickDirective.Factory());
        this.app.directive('cmDiskBrick', DiskBrickDirective.Factory());
        this.app.directive('cmDiskSpeedBrick', DiskSpeedBrickDirective.Factory());
        this.app.directive('cmNicBrick', NICBrickDirective.Factory());
        this.app.directive('cmServicesBrick', ServicesBrickDirective.Factory());
        this.app.directive('cmDatabaseBrick', DatabaseBrickDirective.Factory());
        this.app.directive('cmProcessesBrick', ProcessesBrickDirective.Factory());
        this.app.directive('cmApplicationsBrick', ApplicationsBrickDirective.Factory());
        this.app.directive('cmUpsBrick', UPSBrickDirective.Factory());
        this.app.directive('cmSystemErrorsBrick', SystemErrorsBrickDirective.Factory());
        this.app.directive('cmApplicationErrorsBrick', ApplicationErrorsBrickDirective.Factory());
        this.app.directive('cmDaysToRetrieve', DaysToRetrieveDirective.Factory());

        this.app.filter('commonDate', () => {
            let t = this;
            return (theDate: Date): string => {
                return t.dateFilter(theDate, 'yyyy-MM-dd HH:mm:ss');
            }
        });
        this.app.filter('commonDay', () => {
            let t = this;
            return (theDate: Date): string => {
                return t.dateFilter(theDate, 'yyyy-MM-dd');
            }
        });
    }
}
