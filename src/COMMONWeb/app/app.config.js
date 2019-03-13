/// <reference types="angular" />
/// <reference types="angular-route" />
/// <reference types="angular-translate" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    // When putting the route in the when() methods, it's expecting an IRoute.
    // But I'm sticking an additional thing in there, called 'activeTab', so we can
    // highlight which tab is appropriate. Typescript will complain unless I cast
    // the struct to an <ng.route.IRoute>.
    var Configuration = /** @class */ (function () {
        //public static $inject: string[] = ['$routeProvider', '$translateProvider', '$locationProvider'];
        function Configuration($routeProvider, $translateProvider, $locationProvider) {
            this.$routeProvider = $routeProvider;
            this.$translateProvider = $translateProvider;
            this.$locationProvider = $locationProvider;
            $routeProvider
                .when('/', {
                controller: 'OverviewController',
                templateUrl: 'app/views/overview.html',
                controllerAs: 'vm',
                activeTab: 'Home'
            })
                .when('/server/:id', {
                controller: 'ServerController',
                templateUrl: 'app/views/server.html',
                controllerAs: 'vm',
                activeTab: 'Devices'
            })
                .when('/workstation/:id', {
                controller: 'WorkstationController',
                templateUrl: 'app/views/workstation.html',
                controllerAs: 'vm',
                activeTab: 'Devices'
            })
                .when('/device/:id', {
                controller: 'DeviceController',
                templateUrl: 'app/views/device.html',
                controllerAs: 'vm',
                activeTab: 'Devices'
            })
                .when('/group/:id', {
                controller: 'GroupController',
                templateUrl: 'app/views/group.html',
                controllerAs: 'vm',
                activeTab: 'Devices'
            })
                .when('/admin/', {
                controller: 'AdminController',
                templateUrl: 'app/views/admin.html',
                controllerAs: 'vm',
                activeTab: 'Admin'
            })
                .when('/help/', {
                controller: 'HelpController',
                templateUrl: 'app/views/help.html',
                controllerAs: 'vm',
                activeTab: 'Help'
            })
                //.when('/reports/',
                //    <ng.route.IRoute>{
                //        controller: 'ReportsController',
                //        templateUrl: 'app/reports/reports.html',
                //        controllerAs: 'vm',
                //        activeTab: 'Reports'
                //    })
                .when('/about/', {
                controller: 'AboutController',
                templateUrl: 'app/views/about.html',
                controllerAs: 'vm',
                activeTab: 'About'
            })
                .when('/datacollection/', {
                controller: 'DataCollectionController',
                templateUrl: 'app/views/datacollection.html',
                controllerAs: 'vm',
                activeTab: 'Data Collection'
            })
                .when('/networkhistory/', {
                controller: 'NetworkHistoryController',
                templateUrl: 'app/views/networkhistory.html',
                controllerAs: 'vm',
                activeTab: 'Network History'
            })
                .when('/sitereport/', {
                controller: 'SiteReportController',
                templateUrl: 'app/reports/site.html',
                controllerAs: 'vm',
                activeTab: 'Reports'
            })
                .when('/computerreport/:id', {
                controller: 'MachineReportController',
                templateUrl: 'app/reports/computer.html',
                controllerAs: 'vm',
                activeTab: 'Reports'
            })
                .when('/networkreport/', {
                controller: 'NetworkReportController',
                templateUrl: 'app/reports/network.html',
                controllerAs: 'vm',
                activeTab: 'Reports'
            })
                .when('/casloadreport/', {
                controller: 'CASLoadReportController',
                templateUrl: 'app/reports/casload.partial.html',
                controllerAs: 'vm',
                activeTab: 'Reports'
            })
                .when('/issuesreport/', {
                controller: 'IssuesReportController',
                templateUrl: 'app/reports/issues.partial.html',
                controllerAs: 'vm',
                activeTab: 'Reports'
            })
                .when('/siteconfigurationreport/', {
                controller: 'SiteConfigurationReportController',
                templateUrl: 'app/reports/siteconfig.partial.html',
                controllerAs: 'vm',
                activeTab: 'Reports'
            })
                .otherwise({ redirectTo: '/' });
            $translateProvider.useStaticFilesLoader({
                prefix: '/languages/',
                suffix: '.json'
            }).fallbackLanguage('en');
            $translateProvider.useSanitizeValueStrategy('escape');
            // Between AngularJS 1.5 and 1.6 they did something with the '#' character in the URL, and
            // that messed things up. By doing this we can make it work the old way.
            $locationProvider.hashPrefix('');
        }
        Configuration.Factory = function () {
            var factory = function ($routeProvider, $translateProvider, $locationProvider) {
                return new Configuration($routeProvider, $translateProvider, $locationProvider);
            };
            factory.$inject = ['$routeProvider', '$translateProvider', '$locationProvider'];
            return factory;
        };
        return Configuration;
    }());
    exports.Configuration = Configuration;
});
//# sourceMappingURL=app.config.js.map