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

interface ICOMMONRootScope extends ng.IRootScopeService {
    ECollectorType: any;
    EDeviceTypes: any;
    EMachineParts: any;
}

export class App {
    app: ng.IModule;
    dateFilter: ng.IFilterDate;

    constructor() {
        let t = this;

        let app: ng.IModule = angular.module('app', [
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
        app.config(Configuration.Factory());
        app.run([
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
                                networkService.get()
                                    .then((n: Network) => {
                                        n.startAutomaticUpdate();
                                    });

                                devicemanagerService.setConfigured();
                            });
                    });

                t.dateFilter = $filter('date');
            }]
        );

        app.factory('dataService', DataService.Factory());
        app.factory('devicemanagerService', DeviceManagerService.Factory());
        app.factory('networkService', NetworkService.Factory());
        app.factory('languagesService', LanguagesService.Factory());
        app.factory('configurationService', ConfigurationService.Factory());
        app.factory('chartBridgeFactoryService', ChartBridgeFactoryService.Factory());

        app.controller('OverviewController', OverviewController.Factory());
        app.controller('HeaderController', HeaderController.Factory());
        app.controller('ServerController', ServerController.Factory());
        app.controller('WorkstationController', WorkstationController.Factory());
        app.controller('DataCollectionController', DataCollectionController.Factory());
        app.controller('AboutController', AboutController.Factory());
        app.controller('AdminController', AdminController.Factory());
        app.controller('DeviceController', DeviceController.Factory());
        app.controller('HelpController', HelpController.Factory());
        app.controller('GroupController', GroupController.Factory());
        app.controller('SiteReportController', SiteReportController.Factory());
        app.controller('NetworkHistoryController', NetworkHistoryController.Factory());
        app.controller('MachineReportController', MachineReportController.Factory());
        app.controller('NetworkReportController', NetworkReportController.Factory());
        app.controller('CASLoadReportController', CASLoadReportController.Factory());
        app.controller('IssuesReportController', IssuesReportController.Factory());
        app.controller('SiteConfigurationReportController', SiteConfigurationReportController.Factory());

        app.directive('cmGraph', GraphDirective.Factory());
        app.directive('cmNetworkTable', NetworkTableDirective.Factory());
        app.directive('cmMachineSnapshot', MachineSnapshotDirective.Factory());
        app.directive('cmDataBlock', DataBlockDirective.Factory());
        app.directive('cmPrint', PrintDirective.Factory());
        app.directive('cmDeviceInfo', DeviceInfoDirective.Factory());

        app.filter('commonDate', () => {
            let t = this;
            return (theDate: Date): string => {
                return t.dateFilter(theDate, 'yyyy-MM-dd HH:mm:ss');
            }
        });
        app.filter('commonDay', () => {
            let t = this;
            return (theDate: Date): string => {
                return t.dateFilter(theDate, 'yyyy-MM-dd');
            }
        });

        this.app = app;
    }
}
