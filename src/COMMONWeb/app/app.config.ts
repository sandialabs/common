/// <reference types="angular" />
/// <reference types="angular-route" />
/// <reference types="angular-translate" />

// When putting the route in the when() methods, it's expecting an IRoute.
// But I'm sticking an additional thing in there, called 'activeTab', so we can
// highlight which tab is appropriate. Typescript will complain unless I cast
// the struct to an <ng.route.IRoute>.

export class Configuration {
    //public static $inject: string[] = ['$routeProvider', '$translateProvider', '$locationProvider'];

    constructor(private $routeProvider: ng.route.IRouteProvider, private $translateProvider: ng.translate.ITranslateProvider, private $locationProvider: ng.ILocationProvider) {
        $routeProvider
            .when('/',
                <ng.route.IRoute>{
                    controller: 'OverviewController',
                    templateUrl: 'app/views/overview.html',
                    controllerAs: 'vm',
                    activeTab: 'Home'
                })
            .when('/server/:id',
                <ng.route.IRoute>{
                    controller: 'ServerController',
                    templateUrl: 'app/views/server.html',
                    controllerAs: 'vm',
                    activeTab: 'Devices'
                })
            .when('/workstation/:id',
                <ng.route.IRoute>{
                    controller: 'WorkstationController',
                    templateUrl: 'app/views/workstation.html',
                    controllerAs: 'vm',
                    activeTab: 'Devices'
                })
            .when('/device/:id',
                <ng.route.IRoute>{
                    controller: 'DeviceController',
                    templateUrl: 'app/views/device.html',
                    controllerAs: 'vm',
                    activeTab: 'Devices'
                })
            .when('/group/:id',
                <ng.route.IRoute>{
                    controller: 'GroupController',
                    templateUrl: 'app/views/group.html',
                    controllerAs: 'vm',
                    activeTab: 'Devices'
                })
            .when('/admin/',
                <ng.route.IRoute>{
                    controller: 'AdminController',
                    templateUrl: 'app/views/admin.html',
                    controllerAs: 'vm',
                    activeTab: 'Admin'
                })
            .when('/help/',
                <ng.route.IRoute>{
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
            .when('/about/',
                <ng.route.IRoute>{
                    controller: 'AboutController',
                    templateUrl: 'app/views/about.html',
                    controllerAs: 'vm',
                    activeTab: 'About'
                })
            .when('/datacollection/',
                <ng.route.IRoute>{
                    controller: 'DataCollectionController',
                    templateUrl: 'app/views/datacollection.html',
                    controllerAs: 'vm',
                    activeTab: 'Data Collection'
                })
            .when('/networkhistory/',
                <ng.route.IRoute>{
                    controller: 'NetworkHistoryController',
                    templateUrl: 'app/views/networkhistory.html',
                    controllerAs: 'vm',
                    activeTab: 'Network History'
                })
            .when('/sitereport/',
                <ng.route.IRoute>{
                    controller: 'SiteReportController',
                    templateUrl: 'app/reports/site.html',
                    controllerAs: 'vm',
                    activeTab: 'Reports'
                })
            .when('/computerreport/:id',
                <ng.route.IRoute>{
                    controller: 'MachineReportController',
                    templateUrl: 'app/reports/computer.html',
                    controllerAs: 'vm',
                    activeTab: 'Reports'
                })
            .when('/networkreport/',
                <ng.route.IRoute>{
                    controller: 'NetworkReportController',
                    templateUrl: 'app/reports/network.html',
                    controllerAs: 'vm',
                    activeTab: 'Reports'
                })
            .when('/casloadreport/',
                <ng.route.IRoute>{
                    controller: 'CASLoadReportController',
                    templateUrl: 'app/reports/casload.partial.html',
                    controllerAs: 'vm',
                    activeTab: 'Reports'
                })
            .when('/issuesreport/',
                <ng.route.IRoute>{
                    controller: 'IssuesReportController',
                    templateUrl: 'app/reports/issues.partial.html',
                    controllerAs: 'vm',
                    activeTab: 'Reports'
                })
            .when('/siteconfigurationreport/',
                <ng.route.IRoute>{
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

    public static Factory(): Function {
        let factory = ($routeProvider: ng.route.IRouteProvider, $translateProvider: ng.translate.ITranslateProvider, $locationProvider: ng.ILocationProvider) => {
            return new Configuration($routeProvider, $translateProvider, $locationProvider);
        }
        factory.$inject = ['$routeProvider', '$translateProvider', '$locationProvider'];
        return factory;
    }
}
