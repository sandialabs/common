/// <reference types="angular" />
define(["require", "exports", "./services/data.service", "./services/devicemanager.service", "./services/network.service", "./services/languages.service", "./services/configuration.service", "./app.config", "./enums/collectortype.enum", "./enums/devicetypes.enum", "./controllers/overview.controller", "./controllers/header.controller", "./controllers/server.controller", "./controllers/workstation.controller", "./controllers/datacollection.controller", "./controllers/about.controller", "./controllers/admin.controller", "./controllers/help.controller", "./controllers/group.controller", "./reports/reports.controller", "./controllers/device.controller", "./controllers/networkhistory.controller", "./charts/graph.directive", "./charts/chartbridgefactory.service", "./reports/networktable.directive", "./reports/machine.snapshot.directive", "./reports/datablock.directive", "./reports/print.directive", "./directives/deviceinfo.directive", "./classes/machine"], function (require, exports, data_service_1, devicemanager_service_1, network_service_1, languages_service_1, configuration_service_1, app_config_1, collectortype_enum_1, devicetypes_enum_1, overview_controller_1, header_controller_1, server_controller_1, workstation_controller_1, datacollection_controller_1, about_controller_1, admin_controller_1, help_controller_1, group_controller_1, reports_controller_1, device_controller_1, networkhistory_controller_1, graph_directive_1, chartbridgefactory_service_1, networktable_directive_1, machine_snapshot_directive_1, datablock_directive_1, print_directive_1, deviceinfo_directive_1, machine_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var App = /** @class */ (function () {
        function App() {
            var _this = this;
            var t = this;
            var app = angular.module('app', [
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
            app.config(app_config_1.Configuration.Factory());
            app.run([
                '$rootScope',
                '$filter',
                'devicemanagerService',
                'networkService',
                'languagesService',
                'configurationService',
                function ($rootScope, $filter, devicemanagerService, networkService, languagesService, configurationService) {
                    var t = _this;
                    $rootScope.ECollectorType = collectortype_enum_1.ECollectorType;
                    $rootScope.EDeviceTypes = devicetypes_enum_1.EDeviceTypes;
                    $rootScope.EMachineParts = machine_1.EMachineParts;
                    // First thing to do is get the configuration, then update the device manager. We won't do that
                    // over and over since the site's configuration will rarely change, and when it does a reload is in order.
                    configurationService.get()
                        .then(function (data) {
                        if (!data)
                            return;
                        languagesService.get()
                            .then(function (ls) {
                            ls.updateLanguages(data);
                        });
                        devicemanagerService.get(true)
                            .then(function (deviceManager) {
                            deviceManager.setConfiguration(data);
                            deviceManager.startAutomaticUpdate();
                            networkService.get()
                                .then(function (n) {
                                n.startAutomaticUpdate();
                            });
                            devicemanagerService.setConfigured();
                        });
                    });
                    t.dateFilter = $filter('date');
                }
            ]);
            app.factory('dataService', data_service_1.DataService.Factory());
            app.factory('devicemanagerService', devicemanager_service_1.DeviceManagerService.Factory());
            app.factory('networkService', network_service_1.NetworkService.Factory());
            app.factory('languagesService', languages_service_1.LanguagesService.Factory());
            app.factory('configurationService', configuration_service_1.ConfigurationService.Factory());
            app.factory('chartBridgeFactoryService', chartbridgefactory_service_1.ChartBridgeFactoryService.Factory());
            app.controller('OverviewController', overview_controller_1.OverviewController.Factory());
            app.controller('HeaderController', header_controller_1.HeaderController.Factory());
            app.controller('ServerController', server_controller_1.ServerController.Factory());
            app.controller('WorkstationController', workstation_controller_1.WorkstationController.Factory());
            app.controller('DataCollectionController', datacollection_controller_1.DataCollectionController.Factory());
            app.controller('AboutController', about_controller_1.AboutController.Factory());
            app.controller('AdminController', admin_controller_1.AdminController.Factory());
            app.controller('DeviceController', device_controller_1.DeviceController.Factory());
            app.controller('HelpController', help_controller_1.HelpController.Factory());
            app.controller('GroupController', group_controller_1.GroupController.Factory());
            app.controller('SiteReportController', reports_controller_1.SiteReportController.Factory());
            app.controller('NetworkHistoryController', networkhistory_controller_1.NetworkHistoryController.Factory());
            app.controller('MachineReportController', reports_controller_1.MachineReportController.Factory());
            app.controller('NetworkReportController', reports_controller_1.NetworkReportController.Factory());
            app.controller('CASLoadReportController', reports_controller_1.CASLoadReportController.Factory());
            app.controller('IssuesReportController', reports_controller_1.IssuesReportController.Factory());
            app.controller('SiteConfigurationReportController', reports_controller_1.SiteConfigurationReportController.Factory());
            app.directive('cmGraph', graph_directive_1.GraphDirective.Factory());
            app.directive('cmNetworkTable', networktable_directive_1.NetworkTableDirective.Factory());
            app.directive('cmMachineSnapshot', machine_snapshot_directive_1.MachineSnapshotDirective.Factory());
            app.directive('cmDataBlock', datablock_directive_1.DataBlockDirective.Factory());
            app.directive('cmPrint', print_directive_1.PrintDirective.Factory());
            app.directive('cmDeviceInfo', deviceinfo_directive_1.DeviceInfoDirective.Factory());
            app.filter('commonDate', function () {
                var t = _this;
                return function (theDate) {
                    return t.dateFilter(theDate, 'yyyy-MM-dd HH:mm:ss');
                };
            });
            app.filter('commonDay', function () {
                var t = _this;
                return function (theDate) {
                    return t.dateFilter(theDate, 'yyyy-MM-dd');
                };
            });
            this.app = app;
        }
        return App;
    }());
    exports.App = App;
});
//# sourceMappingURL=app.module.js.map