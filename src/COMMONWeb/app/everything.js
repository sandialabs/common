var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
/// <reference types="angular" />
/// <reference types="angular-route" />
/// <reference types="angular-translate" />
System.register("app.config", [], function (exports_1, context_1) {
    "use strict";
    var Configuration;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            /// <reference types="angular-route" />
            /// <reference types="angular-translate" />
            // When putting the route in the when() methods, it's expecting an IRoute.
            // But I'm sticking an additional thing in there, called 'activeTab', so we can
            // highlight which tab is appropriate. Typescript will complain unless I cast
            // the struct to an <ng.route.IRoute>.
            Configuration = /** @class */ (function () {
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
            exports_1("Configuration", Configuration);
        }
    };
});
/// <reference types="angular" />
System.register("classes/autoupdater", [], function (exports_2, context_2) {
    "use strict";
    var AutoUpdater;
    var __moduleName = context_2 && context_2.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            AutoUpdater = /** @class */ (function () {
                // frequency: in milliseconds
                // callback: the function to call (a static-type function, not an object method), that takes
                // one parameter of type T
                // t: something of type T to pass to the parameter of callback
                // $interval: the angular interval service
                function AutoUpdater(frequency, callback, t, $interval) {
                    this.frequency = frequency;
                    this.callback = callback;
                    this.t = t;
                    this.$interval = $interval;
                    this.autoUpdateTimer = null;
                }
                AutoUpdater.prototype.start = function () {
                    if (this.autoUpdateTimer !== null)
                        return;
                    this.autoUpdateTimer = this.$interval(this.callback, this.frequency, 0, true, this.t);
                    // Call it once so it get's called immediately upon starting. Setting up the timer doesn't
                    // call it until the interval has expired.
                    this.callback(this.t);
                };
                AutoUpdater.prototype.stop = function () {
                    if (this.autoUpdateTimer === null)
                        return;
                    this.$interval.cancel(this.autoUpdateTimer);
                    this.autoUpdateTimer = null;
                };
                AutoUpdater.prototype.changeFrequency = function (frequency) {
                    this.stop();
                    this.frequency = frequency;
                    this.start();
                };
                return AutoUpdater;
            }());
            exports_2("AutoUpdater", AutoUpdater);
        }
    };
});
System.register("classes/promisekeeper", [], function (exports_3, context_3) {
    "use strict";
    var PromiseKeeper;
    var __moduleName = context_3 && context_3.id;
    return {
        setters: [],
        execute: function () {
            PromiseKeeper = /** @class */ (function () {
                function PromiseKeeper($q) {
                    this.$q = $q;
                    this.held = [];
                }
                PromiseKeeper.prototype.push = function (d) {
                    this.held.push(d);
                };
                PromiseKeeper.prototype.resolve = function (t) {
                    while (this.held.length > 0) {
                        var d = this.held.shift();
                        d.resolve(t);
                    }
                };
                return PromiseKeeper;
            }());
            exports_3("PromiseKeeper", PromiseKeeper);
        }
    };
});
/// <reference types="angular" />
System.register("services/devicemanager.service", ["classes/devices", "classes/promisekeeper"], function (exports_4, context_4) {
    "use strict";
    var devices_1, promisekeeper_1, DeviceManagerService;
    var __moduleName = context_4 && context_4.id;
    return {
        setters: [
            function (devices_1_1) {
                devices_1 = devices_1_1;
            },
            function (promisekeeper_1_1) {
                promisekeeper_1 = promisekeeper_1_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            DeviceManagerService = /** @class */ (function () {
                function DeviceManagerService($q, dataService, $interval) {
                    this.$q = $q;
                    this.dataService = dataService;
                    this.$interval = $interval;
                    if (!DeviceManagerService.deviceManager)
                        DeviceManagerService.deviceManager = new devices_1.DeviceManager(this.dataService, this.$interval);
                    this.keeper = new promisekeeper_1.PromiseKeeper($q);
                    this.configured = false;
                    // console.log("NewDeviceManagerService.constructor");
                }
                DeviceManagerService.prototype.get = function (alwaysReturn) {
                    if (alwaysReturn === void 0) { alwaysReturn = false; }
                    var d = this.$q.defer();
                    if (this.configured || alwaysReturn)
                        d.resolve(DeviceManagerService.deviceManager);
                    else
                        this.keeper.push(d);
                    return d.promise;
                };
                DeviceManagerService.prototype.setConfigured = function () {
                    this.configured = true;
                    this.keeper.resolve(DeviceManagerService.deviceManager);
                };
                DeviceManagerService.Factory = function () {
                    var factory = function ($q, dataService, $interval) {
                        return new DeviceManagerService($q, dataService, $interval);
                    };
                    factory.$inject = ['$q', 'dataService', '$interval'];
                    return factory;
                };
                return DeviceManagerService;
            }());
            exports_4("DeviceManagerService", DeviceManagerService);
        }
    };
});
System.register("classes/group", [], function (exports_5, context_5) {
    "use strict";
    var Group;
    var __moduleName = context_5 && context_5.id;
    return {
        setters: [],
        execute: function () {
            Group = /** @class */ (function () {
                //panelIsOpen: boolean;
                function Group(group) {
                    this.id = group.id;
                    this.name = group.name;
                    this.devices = [];
                    this.hasAlarm = false;
                    //this.panelIsOpen = false;
                }
                Group.prototype.addDevice = function (di) {
                    this.devices.push(di);
                    this.devices.sort(function (a, b) {
                        return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
                    });
                };
                Group.prototype.findDeviceFromName = function (name) {
                    var d = null;
                    for (var i = 0; d === null && i < this.devices.length; ++i) {
                        var device = this.devices[i];
                        if (name.localeCompare(device.name) === 0)
                            d = device;
                    }
                    return d;
                };
                Group.prototype.findDeviceFromCollectorID = function (collectorID) {
                    var device = null;
                    for (var i = 0; device === null && i < this.devices.length; ++i) {
                        var d = this.devices[i];
                        var c = d.getCollector(collectorID);
                        if (c)
                            device = d;
                    }
                    return device;
                };
                Group.prototype.updateStatusFlags = function () {
                    this.hasAlarm = false;
                    for (var i = 0; this.hasAlarm === false && i < this.devices.length; ++i) {
                        var d = this.devices[i];
                        this.hasAlarm = this.hasAlarm || d.alarms.length > 0;
                    }
                };
                return Group;
            }());
            exports_5("Group", Group);
        }
    };
});
System.register("classes/systemconfiguration", ["classes/devices", "classes/group"], function (exports_6, context_6) {
    "use strict";
    var devices_2, group_1, ConfigurationData, LanguageConfiguration, SystemConfiguration;
    var __moduleName = context_6 && context_6.id;
    return {
        setters: [
            function (devices_2_1) {
                devices_2 = devices_2_1;
            },
            function (group_1_1) {
                group_1 = group_1_1;
            }
        ],
        execute: function () {
            ConfigurationData = /** @class */ (function () {
                function ConfigurationData(config) {
                    this.configID = config.configID;
                    this.path = config.path;
                    this.value = config.value;
                    this.deleted = config.deleted;
                }
                return ConfigurationData;
            }());
            LanguageConfiguration = /** @class */ (function () {
                function LanguageConfiguration(config) {
                    this.languageCode = config.languageCode;
                    this.language = config.language;
                    this.isEnabled = config.isEnabled;
                }
                return LanguageConfiguration;
            }());
            SystemConfiguration = /** @class */ (function () {
                function SystemConfiguration(config) {
                    this.configuration = {};
                    var keys = Object.keys(config.configuration);
                    for (var i = 0; i < keys.length; ++i) {
                        var key = keys[i];
                        var iconfig = config.configuration[key];
                        var configuration = new ConfigurationData(iconfig);
                        this.configuration[key] = configuration;
                    }
                    this.groups = [];
                    for (var i = 0; i < config.groups.length; ++i) {
                        this.groups.push(new group_1.Group(config.groups[i]));
                    }
                    this.devices = [];
                    for (var i = 0; i < config.devices.length; ++i)
                        this.devices.push(new devices_2.DeviceInfo(config.devices[i]));
                    this.devices.sort(function (a, b) {
                        return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
                    });
                    this.languages = [];
                    for (var i = 0; i < config.languages.length; ++i) {
                        this.languages.push(new LanguageConfiguration(config.languages[i]));
                    }
                    this.siteName = this.configuration['site.name'].value;
                    this.softwareVersion = config.softwareVersion;
                    // Instead of 1.5.0.23, for example, make it 1.5.0
                    var values = this.softwareVersion.split('.');
                    if (values.length >= 3)
                        this.softwareVersionShort = values[0] + '.' + values[1] + '.' + values[2];
                    if (config.mostRecentData)
                        this.mostRecentData = new Date(config.mostRecentData);
                }
                SystemConfiguration.prototype.getGroup = function (id) {
                    var g = null;
                    for (var i = 0; g === null && i < this.groups.length; ++i) {
                        var group = this.groups[i];
                        if (group.id === id)
                            g = group;
                    }
                    return g;
                };
                return SystemConfiguration;
            }());
            exports_6("SystemConfiguration", SystemConfiguration);
        }
    };
});
System.register("classes/idevicedata", [], function (exports_7, context_7) {
    "use strict";
    var __moduleName = context_7 && context_7.id;
    return {
        setters: [],
        execute: function () {
        }
    };
});
/// <reference types="angular" />
System.register("charts/chartbridge", ["charts/chartjs"], function (exports_8, context_8) {
    "use strict";
    var chartjs_1, ChartBridge;
    var __moduleName = context_8 && context_8.id;
    return {
        setters: [
            function (chartjs_1_1) {
                chartjs_1 = chartjs_1_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            // Bridge between the data owner and the graph. Each data owner
            // will have data in a format that's right for him, and each graph
            // will want his data in his own format. Use this to bridge the
            // two.
            // It's an abstract class because at some point you'll need to
            // write actual code to convert your data to what the chart wants.
            ChartBridge = /** @class */ (function () {
                function ChartBridge(dataSource, factory, defaultColor) {
                    if (defaultColor === void 0) { defaultColor = new chartjs_1.Color(); }
                    this.dataSource = dataSource;
                    this.factory = factory;
                    this.defaultColor = defaultColor;
                    this.chart = null;
                    this.chartData = [];
                    this.unWatch = null;
                    // Can't do this here because the child constructor hasn't finished and this
                    // method may very well fail because the object isn't valid yet.
                    //this.createChartData();
                }
                ChartBridge.prototype.setupWatch = function (scope) {
                    // Watch for changes to the raw data and update the graph.
                    // The data you want to watch, and what you want to do when the data
                    // changes, is up to you in your child class.
                    var t = this;
                    this.unWatch = scope.$watchCollection(function (s) {
                        return t.watchCollection();
                    }, function (newValue, oldValue, s) {
                        t.onWatchedCollectionChanged(newValue, oldValue, s);
                    });
                };
                ChartBridge.prototype.addData = function (chartData) {
                    // Make sure the length of this.chartData is the same as the parameter chartData
                    while (this.chartData.length < chartData.length)
                        this.chartData.push(new chartjs_1.ChartJSData(this.defaultColor));
                    for (var i = 0; i < chartData.length; ++i)
                        this.chartData[i].add(chartData[i]);
                };
                ChartBridge.prototype.clearData = function () {
                    for (var i = 0; i < this.chartData.length; ++i)
                        this.chartData[i].clear();
                };
                ChartBridge.prototype.makeChart = function (context) {
                    this.chart = this.factory.makeChart(context, this.dataSource);
                    this.refreshChart();
                    this.setupWatch(context.scope);
                    // Let's keep track of the directive's scope, so when it goes away (when the
                    // graph is no longer being displayed) we can stop watching the data collection.
                    var t = this;
                    context.scope.$on('$destroy', function () {
                        if (t.unWatch) {
                            t.unWatch();
                            t.unWatch = null;
                        }
                    });
                };
                ChartBridge.prototype.onWatchedCollectionChanged = function (newValue, oldValue, scope) {
                    this.refreshChart();
                };
                ChartBridge.prototype.refreshChart = function () {
                    this.createChartData();
                    if (this.chart)
                        this.chart.refresh(this.chartData);
                };
                return ChartBridge;
            }());
            exports_8("ChartBridge", ChartBridge);
        }
    };
});
System.register("classes/cpu", ["charts/chartjs", "charts/chartbridge"], function (exports_9, context_9) {
    "use strict";
    var chartjs_2, chartbridge_1, CPUSnapshot, CPUData, CPUChartBridge;
    var __moduleName = context_9 && context_9.id;
    return {
        setters: [
            function (chartjs_2_1) {
                chartjs_2 = chartjs_2_1;
            },
            function (chartbridge_1_1) {
                chartbridge_1 = chartbridge_1_1;
            }
        ],
        execute: function () {
            CPUSnapshot = /** @class */ (function () {
                function CPUSnapshot(data) {
                    var currentCPU = JSON.parse(data.value)['Value'];
                    this.timestamp = new Date(data.timeStamp);
                    this.percent = Math.round(parseFloat(currentCPU));
                }
                return CPUSnapshot;
            }());
            exports_9("CPUSnapshot", CPUSnapshot);
            CPUData = /** @class */ (function () {
                function CPUData() {
                    this.current = this.peak = null;
                    this.cpuData = [];
                }
                CPUData.prototype.update = function (data) {
                    this.current = this.peak = null;
                    this.cpuData = [];
                    if (!data || data.length === 0)
                        return;
                    for (var i = 0; i < data.length; ++i) {
                        var snapshot = new CPUSnapshot(data[i]);
                        if (isNaN(snapshot.percent) === false) {
                            this.cpuData.push(snapshot);
                            if (this.peak === null || snapshot.percent > this.peak.percent)
                                this.peak = snapshot;
                        }
                    }
                    this.current = this.cpuData[this.cpuData.length - 1];
                };
                return CPUData;
            }());
            exports_9("CPUData", CPUData);
            CPUChartBridge = /** @class */ (function (_super) {
                __extends(CPUChartBridge, _super);
                function CPUChartBridge(cpuDataSource, settings, factory) {
                    var _this = _super.call(this, cpuDataSource, factory) || this;
                    _this.cpuDataSource = cpuDataSource;
                    _this.settings = settings;
                    _this.factory = factory;
                    //console.log("CPUChartBridge constructor");
                    _this.settings.valueRange = [0, 100];
                    return _this;
                }
                CPUChartBridge.prototype.watchCollection = function () {
                    return this.cpuDataSource.cpuData;
                };
                CPUChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.cpuDataSource.cpuData)
                        return;
                    for (var i = 0; i < this.cpuDataSource.cpuData.length; ++i) {
                        var c = this.cpuDataSource.cpuData[i];
                        this.addData(CPUChartBridge.convert(c));
                    }
                };
                CPUChartBridge.convert = function (c) {
                    return [new chartjs_2.ChartJSDataPoint({ x: c.timestamp, y: c.percent })];
                };
                return CPUChartBridge;
            }(chartbridge_1.ChartBridge));
            exports_9("CPUChartBridge", CPUChartBridge);
        }
    };
});
System.register("enums/collectortype.enum", [], function (exports_10, context_10) {
    "use strict";
    var ECollectorType, CollectorTypeExtensions;
    var __moduleName = context_10 && context_10.id;
    return {
        setters: [],
        execute: function () {
            (function (ECollectorType) {
                ECollectorType[ECollectorType["Memory"] = 0] = "Memory";
                ECollectorType[ECollectorType["Disk"] = 1] = "Disk";
                ECollectorType[ECollectorType["CPUUsage"] = 2] = "CPUUsage";
                ECollectorType[ECollectorType["NICUsage"] = 3] = "NICUsage";
                ECollectorType[ECollectorType["Uptime"] = 4] = "Uptime";
                ECollectorType[ECollectorType["LastBootTime"] = 5] = "LastBootTime";
                ECollectorType[ECollectorType["Processes"] = 6] = "Processes";
                ECollectorType[ECollectorType["Ping"] = 7] = "Ping";
                ECollectorType[ECollectorType["InstalledApplications"] = 8] = "InstalledApplications";
                ECollectorType[ECollectorType["Services"] = 9] = "Services";
                //Database,               // 10
                ECollectorType[ECollectorType["SystemErrors"] = 11] = "SystemErrors";
                ECollectorType[ECollectorType["ApplicationErrors"] = 12] = "ApplicationErrors";
                ECollectorType[ECollectorType["DatabaseSize"] = 13] = "DatabaseSize";
                ECollectorType[ECollectorType["UPS"] = 14] = "UPS";
                ECollectorType[ECollectorType["DiskSpeed"] = 15] = "DiskSpeed";
                ECollectorType[ECollectorType["Configuration"] = 16] = "Configuration";
                ECollectorType[ECollectorType["Unknown"] = -1] = "Unknown";
            })(ECollectorType || (ECollectorType = {}));
            exports_10("ECollectorType", ECollectorType);
            CollectorTypeExtensions = /** @class */ (function () {
                function CollectorTypeExtensions() {
                }
                CollectorTypeExtensions.prototype.isHidden = function (type) {
                    var isHidden = false;
                    switch (type) {
                        case ECollectorType.Configuration:
                            isHidden = true;
                            break;
                    }
                    return isHidden;
                };
                return CollectorTypeExtensions;
            }());
            exports_10("CollectorTypeExtensions", CollectorTypeExtensions);
        }
    };
});
System.register("classes/ivalueinfo", [], function (exports_11, context_11) {
    "use strict";
    var __moduleName = context_11 && context_11.id;
    return {
        setters: [],
        execute: function () {
        }
    };
});
System.register("classes/valueinfo", [], function (exports_12, context_12) {
    "use strict";
    var ValueInfo;
    var __moduleName = context_12 && context_12.id;
    return {
        setters: [],
        execute: function () {
            // See Models.cs
            ValueInfo = /** @class */ (function () {
                function ValueInfo(ivi) {
                    this.deviceID = ivi.deviceID;
                    this.collectorType = ivi.collectorType;
                    this.value = ivi.value;
                    this.timestamp = new Date(ivi.timestamp);
                }
                ValueInfo.prototype.parseValue = function () {
                    var value = [];
                    if (this.value)
                        value = JSON.parse(this.value)['Value'];
                    return value;
                };
                return ValueInfo;
            }());
            exports_12("ValueInfo", ValueInfo);
        }
    };
});
System.register("classes/database", ["charts/chartjs", "classes/valueinfo", "charts/chartbridge"], function (exports_13, context_13) {
    "use strict";
    var chartjs_3, valueinfo_1, chartbridge_2, DatabaseDetail, DatabaseHistory, DatabaseHistoryChartBridge, DatabaseManager;
    var __moduleName = context_13 && context_13.id;
    return {
        setters: [
            function (chartjs_3_1) {
                chartjs_3 = chartjs_3_1;
            },
            function (valueinfo_1_1) {
                valueinfo_1 = valueinfo_1_1;
            },
            function (chartbridge_2_1) {
                chartbridge_2 = chartbridge_2_1;
            }
        ],
        execute: function () {
            DatabaseDetail = /** @class */ (function () {
                function DatabaseDetail(detail) {
                    this.timestamp = new Date(detail.timestamp);
                    this.sizeInMB = detail.sizeInMB;
                    this.sizeInGB = this.sizeInMB / 1000;
                }
                return DatabaseDetail;
            }());
            exports_13("DatabaseDetail", DatabaseDetail);
            DatabaseHistory = /** @class */ (function () {
                function DatabaseHistory() {
                    this.deviceID = -1;
                    this.databaseName = "";
                    this.details = [];
                    this.maxSizeInMB = 0;
                }
                DatabaseHistory.prototype.update = function (data) {
                    this.deviceID = data.deviceID;
                    this.databaseName = data.databaseName;
                    this.details = [];
                    this.maxSizeInMB = 0;
                    for (var i = 0; i < data.details.length; ++i) {
                        var detail = new DatabaseDetail(data.details[i]);
                        this.details.push(detail);
                        this.maxSizeInMB = Math.max(this.maxSizeInMB, detail.sizeInMB);
                    }
                };
                return DatabaseHistory;
            }());
            exports_13("DatabaseHistory", DatabaseHistory);
            DatabaseHistoryChartBridge = /** @class */ (function (_super) {
                __extends(DatabaseHistoryChartBridge, _super);
                function DatabaseHistoryChartBridge(dbDataSource, factory) {
                    var _this = _super.call(this, dbDataSource, factory) || this;
                    _this.dbDataSource = dbDataSource;
                    return _this;
                }
                DatabaseHistoryChartBridge.prototype.watchCollection = function () {
                    if (this.dbDataSource)
                        return this.dbDataSource.details;
                    return null;
                };
                DatabaseHistoryChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.dbDataSource || !this.dbDataSource.details)
                        return;
                    for (var i = 0; i < this.dbDataSource.details.length; ++i) {
                        var d = this.dbDataSource.details[i];
                        this.addData(DatabaseHistoryChartBridge.convert(d));
                    }
                };
                DatabaseHistoryChartBridge.convert = function (d) {
                    return [new chartjs_3.ChartJSDataPoint({ x: d.timestamp, y: d.sizeInMB })];
                };
                return DatabaseHistoryChartBridge;
            }(chartbridge_2.ChartBridge));
            exports_13("DatabaseHistoryChartBridge", DatabaseHistoryChartBridge);
            DatabaseManager = /** @class */ (function () {
                function DatabaseManager(data, dataService) {
                    this.databases = new valueinfo_1.ValueInfo(data);
                    this.dataService = dataService;
                    this.values = [];
                    this.databaseHistory = null;
                    var dict = JSON.parse(this.databases.value)['Value'];
                    var sizeDictionary = {};
                    for (var i = 0; i < dict.length; ++i) {
                        var databaseInfo = dict[i];
                        var size = databaseInfo["Size"];
                        var dbs = [];
                        if (sizeDictionary.hasOwnProperty(size)) {
                            dbs = sizeDictionary[size];
                        }
                        else {
                            sizeDictionary[size] = dbs;
                        }
                        dbs.push(databaseInfo);
                    }
                    // OK, now we have another dictionary mapping sizes to the databases that
                    // have that size. Let's create a new array with the different size, and
                    // sort that in reverse numeric order so the largest sizes are earliest in the list.
                    var sizes = Object.keys(sizeDictionary);
                    sizes.sort(function (x, y) {
                        var xi = parseInt(x, 10);
                        var yi = parseInt(y, 10);
                        // Reverse sort
                        return yi - xi;
                    });
                    // Now we can walk through sizes, getting the databases with that size out
                    // of sizeDictionary, then fill up an array mapping the size to the database.
                    // That array is what will be walked for display.
                    for (var i = 0; i < sizes.length; ++i) {
                        var s = parseInt(sizes[i]);
                        var databases = sizeDictionary[s];
                        for (var j = 0; j < databases.length; ++j) {
                            var info = databases[j];
                            this.values.push({ "size": s, "name": info.Name });
                        }
                    }
                }
                DatabaseManager.prototype.onSelectProcess = function (process) {
                    var t = this;
                    this.dataService.getDatabaseHistory(this.databases.deviceID, process)
                        .then(function (data) {
                        if (!t.databaseHistory)
                            t.databaseHistory = new DatabaseHistory();
                        t.databaseHistory.update(data);
                    });
                };
                return DatabaseManager;
            }());
            exports_13("DatabaseManager", DatabaseManager);
        }
    };
});
System.register("classes/memory", ["charts/chartjs", "charts/chartbridge"], function (exports_14, context_14) {
    "use strict";
    var chartjs_4, chartbridge_3, MemorySnapshot, Memory, MemoryChartBridge;
    var __moduleName = context_14 && context_14.id;
    return {
        setters: [
            function (chartjs_4_1) {
                chartjs_4 = chartjs_4_1;
            },
            function (chartbridge_3_1) {
                chartbridge_3 = chartbridge_3_1;
            }
        ],
        execute: function () {
            MemorySnapshot = /** @class */ (function () {
                function MemorySnapshot(data) {
                    var currentMemory = JSON.parse(data.value)['Value'];
                    this.timestamp = new Date(data.timeStamp);
                    this.capacity = (parseFloat(currentMemory['Memory Capacity']) / MemorySnapshot.byteSize);
                    this.free = (parseFloat(currentMemory['Free Memory']) / MemorySnapshot.byteSize);
                    this.used = (parseFloat(currentMemory['Memory Used']) / MemorySnapshot.byteSize);
                    this.percentUsed = this.used / this.capacity * 100;
                }
                MemorySnapshot.byteSize = 0x40000000; // 1GB
                return MemorySnapshot;
            }());
            exports_14("MemorySnapshot", MemorySnapshot);
            Memory = /** @class */ (function () {
                function Memory() {
                    this.current = this.peak = null;
                    this.memoryData = [];
                    this.capacity = 0;
                    this.type = "GB";
                }
                Memory.prototype.update = function (data) {
                    this.memoryData = [];
                    this.current = this.peak = null;
                    this.capacity = 0;
                    if (!data || data.length === 0)
                        return;
                    for (var i = 0; i < data.length; ++i) {
                        var snapshot = new MemorySnapshot(data[i]);
                        if (isNaN(snapshot.capacity) === false) {
                            this.memoryData.push(snapshot);
                            if (this.peak === null || snapshot.used > this.peak.used)
                                this.peak = snapshot;
                            this.capacity = Math.max(this.capacity, snapshot.capacity);
                        }
                    }
                    this.current = this.memoryData[this.memoryData.length - 1];
                };
                return Memory;
            }());
            exports_14("Memory", Memory);
            MemoryChartBridge = /** @class */ (function (_super) {
                __extends(MemoryChartBridge, _super);
                function MemoryChartBridge(memoryDataSource, settings, factory) {
                    var _this = _super.call(this, memoryDataSource, factory) || this;
                    _this.memoryDataSource = memoryDataSource;
                    _this.settings = settings;
                    _this.factory = factory;
                    _this.settings.valueRange = [0, memoryDataSource.capacity];
                    return _this;
                }
                MemoryChartBridge.prototype.watchCollection = function () {
                    return this.memoryDataSource.memoryData;
                };
                MemoryChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.memoryDataSource.memoryData)
                        return;
                    for (var i = 0; i < this.memoryDataSource.memoryData.length; ++i) {
                        var m = this.memoryDataSource.memoryData[i];
                        this.addData(MemoryChartBridge.convert(m));
                    }
                };
                MemoryChartBridge.convert = function (m) {
                    return [new chartjs_4.ChartJSDataPoint({ x: m.timestamp, y: m.used })];
                };
                return MemoryChartBridge;
            }(chartbridge_3.ChartBridge));
            exports_14("MemoryChartBridge", MemoryChartBridge);
        }
    };
});
System.register("classes/nic", ["charts/chartjs", "charts/chartbridge"], function (exports_15, context_15) {
    "use strict";
    var chartjs_5, chartbridge_4, NICSnapshot, NICData, NICChartBridge;
    var __moduleName = context_15 && context_15.id;
    return {
        setters: [
            function (chartjs_5_1) {
                chartjs_5 = chartjs_5_1;
            },
            function (chartbridge_4_1) {
                chartbridge_4 = chartbridge_4_1;
            }
        ],
        execute: function () {
            NICSnapshot = /** @class */ (function () {
                function NICSnapshot(data) {
                    var nic = JSON.parse(data.value)['Value'];
                    this.timestamp = new Date(data.timeStamp);
                    this.bps = Math.round(parseFloat(nic['BPS']));
                    this.capacity = Math.round(parseFloat(nic['Capacity']));
                    this.percent = Math.round(parseFloat(nic['Avg']));
                }
                return NICSnapshot;
            }());
            exports_15("NICSnapshot", NICSnapshot);
            NICData = /** @class */ (function () {
                function NICData() {
                    this.current = this.peak = null;
                    this.nicData = [];
                }
                NICData.prototype.update = function (data) {
                    this.nicData = [];
                    this.current = this.peak = null;
                    if (!data || data.length === 0)
                        return;
                    // Default capacity is 1GB in bytes so we can handle old data that didn't include the capacity.
                    // If the actual capacity is different, we'll update this.
                    var capacity = 125000000;
                    for (var i = 0; i < data.length; ++i) {
                        var snapshot = new NICSnapshot(data[i]);
                        if (isNaN(snapshot.bps) === false) {
                            this.nicData.push(snapshot);
                            if (this.peak === null || snapshot.bps > this.peak.bps)
                                this.peak = snapshot;
                        }
                    }
                    this.current = this.nicData[this.nicData.length - 1];
                };
                return NICData;
            }());
            exports_15("NICData", NICData);
            NICChartBridge = /** @class */ (function (_super) {
                __extends(NICChartBridge, _super);
                function NICChartBridge(nicDataSource, settings, factory) {
                    var _this = _super.call(this, nicDataSource, factory) || this;
                    _this.nicDataSource = nicDataSource;
                    _this.settings = settings;
                    _this.factory = factory;
                    _this.settings.valueRange = [0, 100];
                    return _this;
                }
                NICChartBridge.prototype.watchCollection = function () {
                    return this.nicDataSource.nicData;
                };
                NICChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.nicDataSource.nicData)
                        return;
                    for (var i = 0; i < this.nicDataSource.nicData.length; ++i) {
                        var c = this.nicDataSource.nicData[i];
                        this.addData(NICChartBridge.convert(c));
                    }
                };
                NICChartBridge.convert = function (n) {
                    return [new chartjs_5.ChartJSDataPoint({ x: n.timestamp, y: n.percent })];
                };
                return NICChartBridge;
            }(chartbridge_4.ChartBridge));
            exports_15("NICChartBridge", NICChartBridge);
        }
    };
});
System.register("classes/deviceprocessinfo", [], function (exports_16, context_16) {
    "use strict";
    var DeviceProcessInfo;
    var __moduleName = context_16 && context_16.id;
    return {
        setters: [],
        execute: function () {
            // See DeviceProcessInfo in DatabaseLib/Models.cs
            DeviceProcessInfo = /** @class */ (function () {
                function DeviceProcessInfo() {
                    this.deviceID = -1;
                    this.cpuToProcesses = {};
                    this.timestamp = null;
                }
                return DeviceProcessInfo;
            }());
            exports_16("DeviceProcessInfo", DeviceProcessInfo);
        }
    };
});
System.register("classes/processes", ["charts/chartjs", "charts/chartbridge"], function (exports_17, context_17) {
    "use strict";
    var chartjs_6, chartbridge_5, ProcessHistory, ProcessHistoryChartBridge, ProcessManager;
    var __moduleName = context_17 && context_17.id;
    return {
        setters: [
            function (chartjs_6_1) {
                chartjs_6 = chartjs_6_1;
            },
            function (chartbridge_5_1) {
                chartbridge_5 = chartbridge_5_1;
            }
        ],
        execute: function () {
            ProcessHistory = /** @class */ (function () {
                function ProcessHistory() {
                    this.deviceID = -1;
                    this.processName = "";
                    this.details = [];
                }
                ProcessHistory.prototype.update = function (history) {
                    this.deviceID = history.deviceID;
                    this.processName = history.processName;
                    this.details = [];
                    var mb = 1000000;
                    for (var i = 0; i < history.details.length; ++i) {
                        var tup = history.details[i];
                        var detail = [new Date(tup[0]), tup[1], tup[2] / mb];
                        this.details.push(detail);
                    }
                };
                return ProcessHistory;
            }());
            exports_17("ProcessHistory", ProcessHistory);
            ProcessHistoryChartBridge = /** @class */ (function (_super) {
                __extends(ProcessHistoryChartBridge, _super);
                function ProcessHistoryChartBridge(processHistoryDataSource, factory) {
                    var _this = _super.call(this, processHistoryDataSource, factory) || this;
                    _this.processHistoryDataSource = processHistoryDataSource;
                    _this.factory = factory;
                    var line2 = new chartjs_6.ChartJSData(new chartjs_6.Color(chartjs_6.EChartJSColors.Green, chartjs_6.EChartJSColors.LightGreen));
                    _this.chartData.push(line2);
                    return _this;
                }
                ProcessHistoryChartBridge.prototype.watchCollection = function () {
                    return this.processHistoryDataSource.details;
                };
                ProcessHistoryChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.processHistoryDataSource.details)
                        return;
                    for (var i = 0; i < this.processHistoryDataSource.details.length; ++i) {
                        var c = this.processHistoryDataSource.details[i];
                        this.addData(ProcessHistoryChartBridge.convert(c));
                    }
                };
                ProcessHistoryChartBridge.convert = function (p) {
                    return [new chartjs_6.ChartJSDataPoint({ x: p[0], y: p[1] }), new chartjs_6.ChartJSDataPoint({ x: p[0], y: p[2] })];
                };
                return ProcessHistoryChartBridge;
            }(chartbridge_5.ChartBridge));
            exports_17("ProcessHistoryChartBridge", ProcessHistoryChartBridge);
            ProcessManager = /** @class */ (function () {
                function ProcessManager(data, dataService) {
                    this.processes = data;
                    this.dataService = dataService;
                    this.values = [];
                    this.processHistory = null;
                    var keys = Object.keys(this.processes.cpuToProcesses);
                    // Sort in reverse order
                    keys.sort(function (a, b) {
                        var a2 = parseInt(a);
                        var b2 = parseInt(b);
                        return b2 - a2;
                    });
                    for (var i = 0; i < keys.length; ++i) {
                        var key = keys[i];
                        var procs = this.processes.cpuToProcesses[key];
                        for (var j = 0; j < procs.length; ++j) {
                            this.values.push([key, procs[j]]);
                        }
                    }
                }
                ProcessManager.prototype.onSelectProcess = function (process) {
                    var t = this;
                    this.dataService.getProcessHistory(this.processes.deviceID, process)
                        .then(function (data) {
                        if (!t.processHistory)
                            t.processHistory = new ProcessHistory();
                        t.processHistory.update(data);
                    });
                };
                return ProcessManager;
            }());
            exports_17("ProcessManager", ProcessManager);
        }
    };
});
System.register("disk/smart", [], function (exports_18, context_18) {
    "use strict";
    var ESmartAttribute, SmartAttribute, HardDisk, SmartData;
    var __moduleName = context_18 && context_18.id;
    return {
        setters: [],
        execute: function () {
            // A lot of these were taken from:
            // https://www.data-medics.com/forum/list-of-all-s-m-a-r-t-attributes-including-vendor-specific-t1476.html
            // We're using the generic attributes right now. Maybe we can get smarter and report the vendor-specific
            // ones later.
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
            SmartAttribute = /** @class */ (function () {
                function SmartAttribute(attr) {
                    this.attribute = attr.Attribute;
                    this.name = attr.Name;
                    this.value = attr.Value;
                }
                return SmartAttribute;
            }());
            exports_18("SmartAttribute", SmartAttribute);
            HardDisk = /** @class */ (function () {
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
            exports_18("HardDisk", HardDisk);
            SmartData = /** @class */ (function () {
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
            exports_18("SmartData", SmartData);
        }
    };
});
System.register("disk/disk", ["disk/smart", "charts/chartjs", "charts/chartbridge"], function (exports_19, context_19) {
    "use strict";
    var smart_1, chartjs_7, chartbridge_6, DiskUsageData, DiskUsageWithSmartData, DiskUsageSnapshot, DiskUsage, DiskUsageChartBridge, DiskUsageManager, DiskPerformanceSnapshot, DiskPerformance, DiskPerformanceChartBridge, DiskPerformanceManager;
    var __moduleName = context_19 && context_19.id;
    return {
        setters: [
            function (smart_1_1) {
                smart_1 = smart_1_1;
            },
            function (chartjs_7_1) {
                chartjs_7 = chartjs_7_1;
            },
            function (chartbridge_6_1) {
                chartbridge_6 = chartbridge_6_1;
            }
        ],
        execute: function () {
            DiskUsageData = /** @class */ (function () {
                function DiskUsageData(data) {
                    this.dataID = data.dataID;
                    this.collectorID = data.collectorID;
                    this.timeStamp = new Date(data.timeStamp);
                    this.capacity = data.capacity;
                    this.free = data.free;
                    this.used = data.used;
                }
                return DiskUsageData;
            }());
            exports_19("DiskUsageData", DiskUsageData);
            DiskUsageWithSmartData = /** @class */ (function () {
                function DiskUsageWithSmartData(du) {
                    this.driveLetter = du.driveLetter;
                    this.diskUsage = [];
                    for (var i = 0; i < du.diskUsage.length; ++i)
                        this.diskUsage.push(new DiskUsageData(du.diskUsage[i]));
                    this.diskUsage.sort(function (a, b) {
                        return a.timeStamp.getTime() - b.timeStamp.getTime();
                    });
                    this.smartData = new smart_1.HardDisk(du.smartData);
                }
                return DiskUsageWithSmartData;
            }());
            exports_19("DiskUsageWithSmartData", DiskUsageWithSmartData);
            DiskUsageSnapshot = /** @class */ (function () {
                function DiskUsageSnapshot(data) {
                    this.timestamp = new Date(data.timeStamp);
                    this.capacity = (data.capacity / DiskUsageSnapshot.byteSize);
                    this.free = (data.free / DiskUsageSnapshot.byteSize);
                    this.used = (data.used / DiskUsageSnapshot.byteSize);
                    this.percentUsed = this.used / this.capacity * 100;
                }
                DiskUsageSnapshot.byteSize = 0x40000000; // 1GB
                return DiskUsageSnapshot;
            }());
            exports_19("DiskUsageSnapshot", DiskUsageSnapshot);
            DiskUsage = /** @class */ (function () {
                function DiskUsage(data) {
                    this.driveLetter = data.driveLetter;
                    this.isActive = false;
                    this.update(data);
                }
                DiskUsage.prototype.update = function (data) {
                    this.diskData = [];
                    this.current = this.peak = null;
                    this.smart = data.smartData ? new smart_1.HardDisk(data.smartData) : null;
                    if (!data || !data.diskUsage || data.diskUsage.length === 0)
                        return;
                    for (var i = 0; i < data.diskUsage.length; ++i) {
                        var snapshot = new DiskUsageSnapshot(data.diskUsage[i]);
                        if (isNaN(snapshot.capacity) === false) {
                            this.diskData.push(snapshot);
                            if (this.peak === null || snapshot.used > this.peak.used)
                                this.peak = snapshot;
                        }
                    }
                    this.diskData.sort(function (a, b) {
                        return a.timestamp.getTime() - b.timestamp.getTime();
                    });
                    this.current = this.diskData[this.diskData.length - 1];
                };
                return DiskUsage;
            }());
            exports_19("DiskUsage", DiskUsage);
            DiskUsageChartBridge = /** @class */ (function (_super) {
                __extends(DiskUsageChartBridge, _super);
                function DiskUsageChartBridge(diskUsageDataSource, settings, factory) {
                    var _this = _super.call(this, diskUsageDataSource, factory) || this;
                    _this.diskUsageDataSource = diskUsageDataSource;
                    _this.settings = settings;
                    _this.factory = factory;
                    _this.settings.valueRange = [0, diskUsageDataSource.current.capacity];
                    return _this;
                }
                DiskUsageChartBridge.prototype.watchCollection = function () {
                    return this.diskUsageDataSource.diskData;
                };
                DiskUsageChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.diskUsageDataSource.diskData)
                        return;
                    for (var i = 0; i < this.diskUsageDataSource.diskData.length; ++i) {
                        var c = this.diskUsageDataSource.diskData[i];
                        this.addData(DiskUsageChartBridge.convert(c));
                    }
                };
                DiskUsageChartBridge.convert = function (d) {
                    return [new chartjs_7.ChartJSDataPoint({ x: d.timestamp, y: d.used })];
                };
                return DiskUsageChartBridge;
            }(chartbridge_6.ChartBridge));
            exports_19("DiskUsageChartBridge", DiskUsageChartBridge);
            DiskUsageManager = /** @class */ (function () {
                function DiskUsageManager(devInfo) {
                    this.devInfo = devInfo;
                    this.selectedDriveLetter = "";
                    this.selectedDisk = null;
                    this.disks = [];
                    this.diskMap = {};
                    this.type = "GB";
                    this.driveLetters = [];
                }
                DiskUsageManager.prototype.update = function (data) {
                    var _this = this;
                    var t = this;
                    var diskNames = [];
                    var names = Object.keys(data);
                    names.forEach(function (name) {
                        var isMonitored = _this.devInfo.monitoredDrives.isDriveMonitored(name);
                        if (isMonitored)
                            diskNames.push(name);
                    });
                    if (this.driveLetters.length != diskNames.length) {
                        this.driveLetters = diskNames;
                        this.driveLetters.sort();
                    }
                    this.driveLetters.forEach(function (driveLetter) {
                        var existingDisk = null;
                        for (var i = 0; existingDisk === null && i < t.disks.length; ++i) {
                            var d = t.disks[i];
                            if (d.driveLetter === driveLetter)
                                existingDisk = d;
                        }
                        if (existingDisk)
                            existingDisk.update(data[driveLetter]);
                        else {
                            var disk = new DiskUsage(data[driveLetter]);
                            t.disks.push(disk);
                            t.diskMap[driveLetter] = disk;
                        }
                    });
                    if (this.driveLetters.length > 0 && this.selectedDisk === null)
                        this.selectDisk(this.driveLetters[0]);
                };
                DiskUsageManager.prototype.selectDisk = function (disk) {
                    this.selectedDriveLetter = disk;
                    this.selectedDisk = this.diskMap[disk];
                    for (var i = 0; i < this.disks.length; ++i)
                        this.disks[i].isActive = this.disks[i].driveLetter === disk;
                };
                return DiskUsageManager;
            }());
            exports_19("DiskUsageManager", DiskUsageManager);
            // Disk performance (how much time it spends queuing data)
            DiskPerformanceSnapshot = /** @class */ (function () {
                function DiskPerformanceSnapshot(data) {
                    var current = JSON.parse(data.value)['Value'];
                    this.timestamp = new Date(data.timeStamp);
                    this.percentTime = Number(current['Disk Time %']);
                    this.avgDiskQLength = Number(current['Avg Disk Q Length']);
                }
                return DiskPerformanceSnapshot;
            }());
            exports_19("DiskPerformanceSnapshot", DiskPerformanceSnapshot);
            DiskPerformance = /** @class */ (function () {
                function DiskPerformance(data, driveLetter) {
                    this.driveLetter = driveLetter;
                    this.isActive = false;
                    this.update(data);
                }
                DiskPerformance.prototype.update = function (data) {
                    this.diskData = [];
                    this.current = this.peak = null;
                    if (!data || data.length === 0)
                        return;
                    for (var i = 0; i < data.length; ++i) {
                        var snapshot = new DiskPerformanceSnapshot(data[i]);
                        this.diskData.push(snapshot);
                        if (!this.peak || snapshot.avgDiskQLength > this.peak.avgDiskQLength)
                            this.peak = snapshot;
                    }
                    this.diskData.sort(function (a, b) {
                        return a.timestamp.getTime() - b.timestamp.getTime();
                    });
                    this.current = this.diskData[this.diskData.length - 1];
                };
                return DiskPerformance;
            }());
            exports_19("DiskPerformance", DiskPerformance);
            DiskPerformanceChartBridge = /** @class */ (function (_super) {
                __extends(DiskPerformanceChartBridge, _super);
                function DiskPerformanceChartBridge(diskPerformanceDataSource, factory) {
                    var _this = _super.call(this, diskPerformanceDataSource, factory) || this;
                    _this.diskPerformanceDataSource = diskPerformanceDataSource;
                    _this.factory = factory;
                    return _this;
                }
                DiskPerformanceChartBridge.prototype.watchCollection = function () {
                    return this.diskPerformanceDataSource.diskData;
                };
                DiskPerformanceChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.diskPerformanceDataSource.diskData)
                        return;
                    for (var i = 0; i < this.diskPerformanceDataSource.diskData.length; ++i) {
                        var c = this.diskPerformanceDataSource.diskData[i];
                        this.addData(DiskPerformanceChartBridge.convert(c));
                    }
                };
                DiskPerformanceChartBridge.convert = function (dps) {
                    return [new chartjs_7.ChartJSDataPoint({ x: dps.timestamp, y: dps.percentTime })];
                };
                return DiskPerformanceChartBridge;
            }(chartbridge_6.ChartBridge));
            exports_19("DiskPerformanceChartBridge", DiskPerformanceChartBridge);
            DiskPerformanceManager = /** @class */ (function () {
                function DiskPerformanceManager(devInfo) {
                    this.devInfo = devInfo;
                    this.selectedDriveLetter = "";
                    this.selectedDisk = null;
                    this.disks = [];
                    this.diskMap = {};
                    this.type = "GB";
                    this.driveLetters = [];
                }
                DiskPerformanceManager.prototype.update = function (data) {
                    var _this = this;
                    var t = this;
                    this.driveLetters = [];
                    var names = Object.keys(data);
                    names.forEach(function (name) {
                        var isMonitored = _this.devInfo.monitoredDrives.isDriveMonitored(name);
                        if (isMonitored)
                            t.driveLetters.push(name);
                    });
                    this.driveLetters.sort();
                    this.driveLetters.forEach(function (diskName) {
                        var existingDisk = null;
                        for (var i = 0; existingDisk === null && i < t.disks.length; ++i) {
                            var d = t.disks[i];
                            if (d.driveLetter === diskName)
                                existingDisk = d;
                        }
                        if (existingDisk)
                            existingDisk.update(data[diskName]);
                        else {
                            var allData = data[diskName];
                            var disk = new DiskPerformance(allData, diskName);
                            t.disks.push(disk);
                            t.diskMap[diskName] = disk;
                        }
                    });
                    if (this.driveLetters.length > 0 && this.selectedDisk === null)
                        this.selectDisk(this.driveLetters[0]);
                };
                DiskPerformanceManager.prototype.selectDisk = function (disk) {
                    this.selectedDriveLetter = disk;
                    this.selectedDisk = this.diskMap[disk];
                    for (var i = 0; i < this.disks.length; ++i)
                        this.disks[i].isActive = this.disks[i].driveLetter === disk;
                };
                return DiskPerformanceManager;
            }());
            exports_19("DiskPerformanceManager", DiskPerformanceManager);
        }
    };
});
System.register("classes/ups", ["charts/chartjs", "charts/chartbridge"], function (exports_20, context_20) {
    "use strict";
    var chartjs_8, chartbridge_7, UPSSnapshot, UPSStatus, UPSChartBridge;
    var __moduleName = context_20 && context_20.id;
    return {
        setters: [
            function (chartjs_8_1) {
                chartjs_8 = chartjs_8_1;
            },
            function (chartbridge_7_1) {
                chartbridge_7 = chartbridge_7_1;
            }
        ],
        execute: function () {
            UPSSnapshot = /** @class */ (function () {
                function UPSSnapshot(data) {
                    var upsData = JSON.parse(data.value)['Value'];
                    this.timestamp = new Date(data.timeStamp);
                    this.name = (upsData['Name']);
                    this.upsStatus = (upsData['Status']);
                    this.batteryStatus = (upsData['BatteryStatus']);
                    this.batteryStatusInt = (parseInt(upsData['BatteryStatusInt']));
                    this.estimatedRunTimeInMinutes = (parseInt(upsData['EstimatedRunTime']));
                    this.estimatedChargeRemainingPercentage = (parseInt(upsData['EstimatedChargeRemaining']));
                    this.runningOnUPS = false;
                    if (this.batteryStatusInt == 1) {
                        this.runningOnUPS = true;
                    }
                }
                return UPSSnapshot;
            }());
            exports_20("UPSSnapshot", UPSSnapshot);
            UPSStatus = /** @class */ (function () {
                function UPSStatus(data) {
                    this.upsData = [];
                    if (!data || data.length === 0)
                        return;
                    for (var i = 0; i < data.length; ++i) {
                        var snapshot = new UPSSnapshot(data[i]);
                        this.upsData.push(snapshot);
                    }
                    this.current = this.upsData[this.upsData.length - 1];
                }
                return UPSStatus;
            }());
            exports_20("UPSStatus", UPSStatus);
            UPSChartBridge = /** @class */ (function (_super) {
                __extends(UPSChartBridge, _super);
                function UPSChartBridge(upsDataSource, factory) {
                    var _this = _super.call(this, upsDataSource, factory) || this;
                    _this.upsDataSource = upsDataSource;
                    _this.factory = factory;
                    return _this;
                }
                UPSChartBridge.prototype.watchCollection = function () {
                    return this.upsDataSource.upsData;
                };
                UPSChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.upsDataSource.upsData)
                        return;
                    for (var i = 0; i < this.upsDataSource.upsData.length; ++i) {
                        var c = this.upsDataSource.upsData[i];
                        this.addData(UPSChartBridge.convert(c));
                    }
                };
                UPSChartBridge.convert = function (u) {
                    return [new chartjs_8.ChartJSDataPoint({ x: u.timestamp, y: u.runningOnUPS ? 1.0 : 0.0 })];
                };
                return UPSChartBridge;
            }(chartbridge_7.ChartBridge));
            exports_20("UPSChartBridge", UPSChartBridge);
        }
    };
});
System.register("charts/icharttypes", [], function (exports_21, context_21) {
    "use strict";
    var __moduleName = context_21 && context_21.id;
    return {
        setters: [],
        execute: function () {
        }
    };
});
System.register("charts/chartjs", ["chart.js"], function (exports_22, context_22) {
    "use strict";
    var chart_js_1, EChartJSColors, Color, ChartJSSettings, ChartJSDataPoint, ChartJSData, ChartJSConfiguration, ChartJSChart, ChartJSLineChart, ChartJSChartFactory;
    var __moduleName = context_22 && context_22.id;
    return {
        setters: [
            function (chart_js_1_1) {
                chart_js_1 = chart_js_1_1;
            }
        ],
        execute: function () {
            (function (EChartJSColors) {
                EChartJSColors[EChartJSColors["Blue"] = 0] = "Blue";
                EChartJSColors[EChartJSColors["LightBlue"] = 1] = "LightBlue";
                EChartJSColors[EChartJSColors["Green"] = 2] = "Green";
                EChartJSColors[EChartJSColors["LightGreen"] = 3] = "LightGreen";
                EChartJSColors[EChartJSColors["Red"] = 4] = "Red";
                EChartJSColors[EChartJSColors["LightRed"] = 5] = "LightRed";
                EChartJSColors[EChartJSColors["Black"] = 6] = "Black";
                EChartJSColors[EChartJSColors["LightGray"] = 7] = "LightGray";
            })(EChartJSColors || (EChartJSColors = {}));
            exports_22("EChartJSColors", EChartJSColors);
            Color = /** @class */ (function () {
                // Default color scheme is blue
                function Color(border, background) {
                    if (border === void 0) { border = EChartJSColors.Blue; }
                    if (background === void 0) { background = EChartJSColors.LightBlue; }
                    this.border = border;
                    this.background = background;
                }
                Color.colors = [
                    "#72b9ff",
                    "#d8ebff",
                    "#84cb6a",
                    "#e1f2db",
                    "#e57773",
                    "#fae6e6",
                    "#000000",
                    "#dddddd"
                ];
                return Color;
            }());
            exports_22("Color", Color);
            ChartJSSettings = /** @class */ (function () {
                function ChartJSSettings(yaxis, height) {
                    this.xaxis = "";
                    this.yaxis = yaxis;
                    this.yaxis2 = null;
                    this.chartSizeInGB = true;
                    this.displayAxes = true;
                    this.displayLegend = true;
                    this.valueRange = null;
                    this.height = height; // pixels
                    this.makeResponsive = true;
                }
                return ChartJSSettings;
            }());
            exports_22("ChartJSSettings", ChartJSSettings);
            ChartJSDataPoint = /** @class */ (function () {
                function ChartJSDataPoint(point, color) {
                    this.point = point;
                    this.color = color;
                }
                return ChartJSDataPoint;
            }());
            exports_22("ChartJSDataPoint", ChartJSDataPoint);
            ChartJSData = /** @class */ (function () {
                function ChartJSData(color) {
                    this.chartPoints = [];
                    this.color = color;
                    this.colors = null;
                }
                ChartJSData.prototype.add = function (dataPoint) {
                    this.chartPoints.push(dataPoint.point);
                    if (dataPoint.color) {
                        if (!this.colors)
                            this.colors = [];
                        this.colors.push(dataPoint.color);
                    }
                };
                ChartJSData.prototype.clear = function () {
                    this.chartPoints = [];
                    // Don't change the color in case it's different than the default
                    //this.color = new Color();
                    this.colors = null;
                };
                // Returns a tuple of [borders[], backgrounds[]]
                ChartJSData.prototype.asColors = function () {
                    var colors = null;
                    if (this.colors) {
                        colors = [[], []];
                        for (var i = 0; i < this.colors.length; ++i) {
                            var c = this.colors[i];
                            colors[0].push(Color.colors[c.border]);
                            colors[1].push(Color.colors[c.background]);
                        }
                    }
                    return colors;
                };
                return ChartJSData;
            }());
            exports_22("ChartJSData", ChartJSData);
            ChartJSConfiguration = /** @class */ (function () {
                function ChartJSConfiguration(type, displayLegend) {
                    this.type = type;
                    chart_js_1.default.defaults.global.animation.duration = 0;
                    var t = this;
                    this.data = {
                        datasets: []
                    };
                    this.options = {
                        responsive: true,
                        maintainAspectRatio: false,
                        title: {
                            display: false,
                        },
                        scales: {},
                        legend: {
                            display: displayLegend,
                        },
                        elements: {
                            point: {
                                radius: 2,
                            },
                            line: {
                                tension: 0 // disable bezier curves
                            }
                        },
                        animation: {
                            duration: 0,
                        },
                        responsiveAnimationDuration: 0,
                        spanGaps: false,
                        showLines: false
                    };
                }
                ChartJSConfiguration.prototype.setData = function (data) {
                    if (!data)
                        return;
                    this.data.datasets = data;
                };
                return ChartJSConfiguration;
            }());
            ChartJSChart = /** @class */ (function () {
                function ChartJSChart(chartContext, settings, type) {
                    this.chartContext = chartContext;
                    this.settings = settings;
                    //// We'll use displayAxes to also indicate whether the legend at the top of the graph should be displayed
                    //var displayAxes: boolean = (settings.displayAxes !== undefined && settings.displayAxes !== null) ? settings.displayAxes : true;
                    this.config = new ChartJSConfiguration(type, settings.displayLegend);
                    this.fixAxes();
                    this.id = ChartJSChart.next_id++;
                    // I'm calling it makeResponsive, but really it's just watching the size of the parent
                    // of the chart, and resizing the chart if the parent size changes.
                    // Doing this can lead to infinite loops when the parent changes its size based
                    // on the child, such as when a chart is put inside a table. So, when that kind of
                    // thing happens, set makeResponsive to false.
                    if (settings.makeResponsive) {
                        var t_1 = this;
                        // Make sure we are alerted that something changed so we can resize the graph
                        chartContext.window.addEventListener('resize', function () {
                            t_1.chartContext.rootScope.$apply();
                        });
                        chartContext.scope.$watch(function () {
                            var parent = t_1.chartContext.element.parent();
                            return {
                                height: Math.round(parent.height()),
                                width: Math.round(parent.width())
                            };
                        }, function () {
                            var width = Math.round(t_1.chartContext.element.parent().width());
                            var height = t_1.settings.height;
                            t_1.resize(width, height);
                        }, true);
                    }
                }
                ChartJSChart.prototype.resize = function (width, height) {
                    if (!this.chart)
                        return;
                    //console.log("Resizing " + this.id.toString());
                    var e = this.chartContext.element;
                    e.parent().height(height);
                    e.width(width).height(height);
                    var c = this.chart.canvas;
                    c.width = width;
                    c.height = height;
                };
                ChartJSChart.prototype.refresh = function (data) {
                    //console.log("Refreshing " + this.id.toString());
                    var dataSets = [];
                    if (data.length > 0)
                        dataSets.push(this.createDataSet(data[0], this.settings.yaxis, true));
                    if (data.length > 1)
                        dataSets.push(this.createDataSet(data[1], this.settings.yaxis2, false));
                    this.config.setData(dataSets);
                    if (this.chart) {
                        //console.log("Refresh/update " + this.id.toString());
                        this.chart.update();
                    }
                };
                ChartJSChart.prototype.fixAxes = function () {
                    var displayAxes = (this.settings.displayAxes !== undefined && this.settings.displayAxes !== null) ? this.settings.displayAxes : true;
                    var y = this.createYScale(this.settings.yaxis, "left", true, displayAxes);
                    if (this.settings.valueRange)
                        y.ticks = {
                            min: this.settings.valueRange[0],
                            max: this.settings.valueRange[1]
                        };
                    this.config.options.scales = {
                        xAxes: [
                            {
                                type: 'time',
                                time: {
                                    unit: 'day'
                                },
                                display: displayAxes,
                                scaleLabel: {
                                    display: false,
                                    labelString: 'Date'
                                },
                            }
                        ],
                        yAxes: [y]
                    };
                    if (this.settings.yaxis2)
                        this.config.options.scales.yAxes.push(this.createYScale(this.settings.yaxis2, "right", false, displayAxes));
                };
                ChartJSChart.prototype.createYScale = function (label, position, y1, displayAxes) {
                    var y = {
                        display: displayAxes,
                        scaleLabel: {
                            display: false,
                            labelString: label
                        },
                        ticks: {
                            beginAtZero: true
                        },
                        position: position
                    };
                    // It appears that ChartScales has an "id" field, but it's not in the DefinitelyTyped definitions,
                    // and I haven't gotten the pull-request done yet and can't wait for it to be approved and put out there.
                    y.id = y1 ? "y1" : "y2";
                    return y;
                };
                ChartJSChart.prototype.createGraph = function (options) {
                    var element = this.chartContext.element.get(0).childNodes[0];
                    this.chart = new chart_js_1.default(element.getContext('2d'), this.config);
                };
                ChartJSChart.prototype.createDataSet = function (data, label, y1) {
                    var d = {
                        data: data.chartPoints,
                        label: label,
                        backgroundColor: Color.colors[data.color.background],
                        borderColor: Color.colors[data.color.border],
                        yAxisID: y1 ? "y1" : "y2",
                        fill: false
                    };
                    if (data.colors) {
                        var colors = data.asColors();
                        d.pointBorderColor = colors[0];
                        d.pointBackgroundColor = colors[1];
                    }
                    return d;
                };
                ChartJSChart.next_id = 1;
                return ChartJSChart;
            }());
            exports_22("ChartJSChart", ChartJSChart);
            ChartJSLineChart = /** @class */ (function (_super) {
                __extends(ChartJSLineChart, _super);
                function ChartJSLineChart(options) {
                    var _this = _super.call(this, options.context, options.settings, 'line') || this;
                    _this.createGraph(options);
                    return _this;
                }
                return ChartJSLineChart;
            }(ChartJSChart));
            exports_22("ChartJSLineChart", ChartJSLineChart);
            ChartJSChartFactory = /** @class */ (function () {
                function ChartJSChartFactory() {
                }
                ChartJSChartFactory.prototype.makeChart = function (context, chart) {
                    var options = {
                        context: context,
                        settings: context.scope.settings
                    };
                    // If we ever do bar charts, create that kind of chart here. We'll probably want to
                    // determine that from something in IChartContext so the chart type can
                    // be specified by a controller or something like that.
                    var c = new ChartJSLineChart(options);
                    return c;
                };
                return ChartJSChartFactory;
            }());
            exports_22("ChartJSChartFactory", ChartJSChartFactory);
        }
    };
});
/// <reference types="angular" />
System.register("classes/network", ["classes/autoupdater", "charts/chartjs", "charts/chartbridge"], function (exports_23, context_23) {
    "use strict";
    var autoupdater_1, chartjs_9, chartbridge_8, PingAttempt, NetworkStatus, NetworkChartBridge, NetworkRetrieval, Network;
    var __moduleName = context_23 && context_23.id;
    return {
        setters: [
            function (autoupdater_1_1) {
                autoupdater_1 = autoupdater_1_1;
            },
            function (chartjs_9_1) {
                chartjs_9 = chartjs_9_1;
            },
            function (chartbridge_8_1) {
                chartbridge_8 = chartbridge_8_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            PingAttempt = /** @class */ (function () {
                function PingAttempt(attempt) {
                    this.successful = attempt.successful;
                    this.timestamp = new Date(attempt.timestamp);
                    this.responseTimeMS = attempt.responseTimeMS;
                }
                return PingAttempt;
            }());
            exports_23("PingAttempt", PingAttempt);
            NetworkStatus = /** @class */ (function () {
                function NetworkStatus(status) {
                    this.name = status.name;
                    this.deviceID = status.deviceID;
                    this.ipAddress = status.ipAddress;
                    this.attempts = [];
                    this.avgResponseTimeMS =
                        this.percentSuccessfulPings =
                            this.totalPingAttempts =
                                this.totalSuccessfulPingAttempts = 0;
                    this.dateMostRecentStateChange = null;
                    this.update(status);
                }
                NetworkStatus.prototype.update = function (n) {
                    this.successfulPing = n.successfulPing;
                    this.hasBeenPinged = n.hasBeenPinged;
                    // Because the JavaScript dates and C# dates are slightly different,
                    // when we convert from one to the other we'll lose some of the
                    // milliseconds. This is enough so that when the query to get new ping
                    // attempts occurs, it will almost surely get one out of the DB that
                    // we've already seen here.
                    // So, before we make a new PingAttempt and push it into the attempts array,
                    // make sure it's not already there. Compare the ping attempt times, and since
                    // we'll be comparing JavaScript dates we should be fine.
                    if (n.attempts.length > 0) {
                        var totalResponseTimeMS = 0;
                        for (var i = 0; i < n.attempts.length; ++i) {
                            var pa = new PingAttempt(n.attempts[i]);
                            var exists = false;
                            var paTime = pa.timestamp.getTime();
                            // Let's go in reverse order for a bit better performance since the new
                            // PingAttempt will likely be coming in after the most recent in this.attempts.
                            for (var j = this.attempts.length - 1; exists == false && j >= 0; --j) {
                                var old = this.attempts[j];
                                exists = paTime - old.timestamp.getTime() == 0;
                            }
                            if (exists == false) {
                                if (pa.successful)
                                    this.totalSuccessfulPingAttempts += 1;
                                totalResponseTimeMS += pa.responseTimeMS;
                                this.attempts.push(pa);
                            }
                        }
                        this.totalPingAttempts = n.attempts.length;
                        this.avgResponseTimeMS = this.totalPingAttempts > 0 ? totalResponseTimeMS / this.totalPingAttempts : 0;
                        this.percentSuccessfulPings = this.totalPingAttempts > 0 ? this.totalSuccessfulPingAttempts / this.totalPingAttempts * 100.0 : 0;
                        // The attempts come in "chunks" like the most-recent two weeks, then two-weeks before that, etc.
                        // This means as the attempts are pushed on to the attempts array they won't be in order so
                        // we need to sort.
                        this.attempts.sort(function (x, y) {
                            var a = x.timestamp.getTime();
                            var b = y.timestamp.getTime();
                            return a - b;
                        });
                    }
                    if (n.dateSuccessfulPingOccurred && n.dateSuccessfulPingOccurred !== "")
                        this.dateSuccessfulPingOccurred = new Date(n.dateSuccessfulPingOccurred);
                    this.datePingAttempted = new Date(n.datePingAttempted);
                };
                return NetworkStatus;
            }());
            exports_23("NetworkStatus", NetworkStatus);
            NetworkChartBridge = /** @class */ (function (_super) {
                __extends(NetworkChartBridge, _super);
                function NetworkChartBridge(networkDataSource, settings, factory) {
                    var _this = _super.call(this, networkDataSource, factory, NetworkChartBridge.green) || this;
                    _this.networkDataSource = networkDataSource;
                    _this.settings = settings;
                    _this.factory = factory;
                    _this.settings.valueRange = [0, 1000];
                    return _this;
                }
                NetworkChartBridge.prototype.watchCollection = function () {
                    return this.networkDataSource.attempts;
                };
                NetworkChartBridge.prototype.createChartData = function () {
                    this.clearData();
                    if (!this.networkDataSource || !this.networkDataSource.attempts)
                        return;
                    for (var i = 0; i < this.networkDataSource.attempts.length; ++i) {
                        var a = this.networkDataSource.attempts[i];
                        this.addData(NetworkChartBridge.convert(a));
                    }
                };
                NetworkChartBridge.convert = function (pa) {
                    var c = (pa.responseTimeMS >= 500) ? NetworkChartBridge.red : NetworkChartBridge.green;
                    return [new chartjs_9.ChartJSDataPoint({ x: pa.timestamp, y: pa.responseTimeMS }, c)];
                };
                NetworkChartBridge.red = new chartjs_9.Color(chartjs_9.EChartJSColors.Red, chartjs_9.EChartJSColors.LightRed);
                NetworkChartBridge.green = new chartjs_9.Color(chartjs_9.EChartJSColors.Green, chartjs_9.EChartJSColors.LightGreen);
                return NetworkChartBridge;
            }(chartbridge_8.ChartBridge));
            exports_23("NetworkChartBridge", NetworkChartBridge);
            NetworkRetrieval = /** @class */ (function () {
                function NetworkRetrieval(config) {
                    this.config = config;
                    this.startingDate = this.endingDate = null;
                }
                // Returns the date range that should be retrieved next
                // When called the first time, it will go back from the
                // current time. Each subsequent call will go back from the
                // previous start.
                NetworkRetrieval.prototype.retrieveNext = function () {
                    var start = null;
                    var end = null;
                    // This will move the 'window' back by 15 days
                    // each time we gather the data.
                    if (this.startingDate == null) {
                        if (this.config.mostRecentData)
                            this.startingDate = this.config.mostRecentData;
                        else
                            this.startingDate = new Date();
                        this.endingDate = null;
                        end = null;
                    }
                    else {
                        this.endingDate = this.startingDate;
                        end = this.endingDate;
                    }
                    this.startingDate = new Date(this.startingDate.getTime() - NetworkRetrieval.msPerWindow);
                    start = this.startingDate;
                    console.log("Retrieving " + (start ? start.toDateString() : "null") + " to " + (end ? end.toDateString() : "null"));
                    return [start, end];
                };
                NetworkRetrieval.msPerDay = 24 * 60 * 60 * 1000;
                NetworkRetrieval.daysPerWindow = 15;
                NetworkRetrieval.msPerWindow = NetworkRetrieval.daysPerWindow * NetworkRetrieval.msPerDay;
                return NetworkRetrieval;
            }());
            // Holds all of the NetworkStatus objects, and keeps track of the most-recent poll time in
            // maxDate
            Network = /** @class */ (function () {
                function Network(dataService, deviceManagerService, $interval, config) {
                    this.dataService = dataService;
                    this.deviceManagerService = deviceManagerService;
                    this.$interval = $interval;
                    this.config = config;
                    this.maxDate = null;
                    this.idToData = {};
                    this.ipAddressToData = {};
                    this.data = [];
                    this.dataService = dataService;
                    this.selected = null;
                    this.deviceManagerService = deviceManagerService;
                    // Get the new network data every 10 seconds
                    this.autoUpdater = new autoupdater_1.AutoUpdater(10000, Network.gatherData, this, $interval);
                    this.retrieval = new NetworkRetrieval(this.config);
                    Network.gatherData(this);
                }
                Network.prototype.getNetworkStatusFromID = function (id) {
                    return this.idToData[id];
                };
                Network.prototype.getNetworkStatusFromIPAddress = function (name) {
                    return this.ipAddressToData[name];
                };
                Network.prototype.updateData = function (data) {
                    if (!data)
                        return;
                    for (var i = 0; i < data.length; ++i) {
                        var d = data[i];
                        var existing = this.getNetworkStatusFromIPAddress(d.ipAddress);
                        var ns = null;
                        if (!existing) {
                            ns = new NetworkStatus(d);
                            this.data.push(ns);
                            this.idToData[ns.deviceID] = ns;
                            this.ipAddressToData[ns.ipAddress] = ns;
                        }
                        else {
                            existing.update(d);
                            ns = existing;
                        }
                        if (this.maxDate === null || ns.datePingAttempted > this.maxDate)
                            this.maxDate = ns.datePingAttempted;
                    }
                    //this.startAutomaticUpdate();
                };
                Network.prototype.startAutomaticUpdate = function () {
                    this.autoUpdater.start();
                };
                Network.prototype.stopAutomaticUpdate = function () {
                    this.autoUpdater.stop();
                };
                // This is called when the user wants to see more data
                // on the chart.
                Network.prototype.getEarlierRange = function () {
                    this.gather(this.retrieval.retrieveNext());
                };
                Network.prototype.gather = function (range) {
                    var t = this;
                    this.dataService.getNetworkStatus(range[0], range[1])
                        .then(function (data) {
                        if (!data)
                            return;
                        t.updateData(data);
                        t.deviceManagerService.get()
                            .then(function (deviceManager) {
                            deviceManager.updateNetwork(t);
                        });
                    });
                };
                // This is called at startup, and periodically to get
                // the most recent data. When starting up, gather
                // a couple of weeks of data, then after that just
                // get the data that might have been collected since
                // startup.
                Network.gatherData = function (t) {
                    var range = [null, null];
                    if (t.retrieval.startingDate == null)
                        range = t.retrieval.retrieveNext();
                    else
                        range = [t.maxDate, null];
                    t.gather(range);
                };
                return Network;
            }());
            exports_23("Network", Network);
        }
    };
});
System.register("classes/collectorinfo", [], function (exports_24, context_24) {
    "use strict";
    var CollectorInfo;
    var __moduleName = context_24 && context_24.id;
    return {
        setters: [],
        execute: function () {
            CollectorInfo = /** @class */ (function () {
                function CollectorInfo(info) {
                    this.id = info.id;
                    this.deviceID = info.deviceID;
                    this.name = this.fullName = info.name;
                    this.collectorType = info.collectorType;
                    this.isEnabled = info.isEnabled;
                    this.frequencyInMinutes = info.frequencyInMinutes;
                    this.lastCollectionAttempt = info.lastCollectionAttempt ? new Date(info.lastCollectionAttempt) : null;
                    this.lastCollectedAt = info.lastCollectedAt ? new Date(info.lastCollectedAt) : null;
                    this.nextCollectionTime = info.nextCollectionTime ? new Date(info.nextCollectionTime) : null;
                    this.successfullyCollected = info.successfullyCollected;
                    this.isBeingCollected = info.isBeingCollected;
                    // Each collector's name is a combination of the device and the collector type,
                    // like this: <devicename>.<collector>.
                    // This finds the '.' between the "<devicename>.<collector>", then removes the "<devicename>." part
                    var index = this.name.indexOf(".");
                    if (index >= 0)
                        this.name = this.name.substr(index + 1);
                }
                return CollectorInfo;
            }());
            exports_24("CollectorInfo", CollectorInfo);
        }
    };
});
// See ConfigurationLib/DeviceTypes.cs
System.register("enums/devicetypes.enum", [], function (exports_25, context_25) {
    "use strict";
    var EDeviceTypes;
    var __moduleName = context_25 && context_25.id;
    return {
        setters: [],
        execute: function () {// See ConfigurationLib/DeviceTypes.cs
            (function (EDeviceTypes) {
                EDeviceTypes[EDeviceTypes["Server"] = 0] = "Server";
                EDeviceTypes[EDeviceTypes["Workstation"] = 1] = "Workstation";
                EDeviceTypes[EDeviceTypes["Camera"] = 2] = "Camera";
                EDeviceTypes[EDeviceTypes["RPM"] = 3] = "RPM";
                EDeviceTypes[EDeviceTypes["System"] = 4] = "System";
                EDeviceTypes[EDeviceTypes["Generic"] = 5] = "Generic";
                EDeviceTypes[EDeviceTypes["Unknown"] = -1] = "Unknown";
            })(EDeviceTypes || (EDeviceTypes = {}));
            exports_25("EDeviceTypes", EDeviceTypes);
        }
    };
});
System.register("enums/drivetypes.enum", [], function (exports_26, context_26) {
    "use strict";
    var EDriveTypes;
    var __moduleName = context_26 && context_26.id;
    return {
        setters: [],
        execute: function () {
            (function (EDriveTypes) {
                EDriveTypes[EDriveTypes["Unknown"] = 0] = "Unknown";
                EDriveTypes[EDriveTypes["NoRootDirectory"] = 1] = "NoRootDirectory";
                EDriveTypes[EDriveTypes["RemovableDisk"] = 2] = "RemovableDisk";
                EDriveTypes[EDriveTypes["LocalDisk"] = 3] = "LocalDisk";
                EDriveTypes[EDriveTypes["NetworkDrive"] = 4] = "NetworkDrive";
                EDriveTypes[EDriveTypes["CompactDisc"] = 5] = "CompactDisc";
                EDriveTypes[EDriveTypes["RAMDisk"] = 6] = "RAMDisk";
            })(EDriveTypes || (EDriveTypes = {}));
            exports_26("EDriveTypes", EDriveTypes);
        }
    };
});
System.register("classes/devices", ["classes/collectorinfo", "enums/devicetypes.enum", "classes/autoupdater", "classes/group"], function (exports_27, context_27) {
    "use strict";
    var collectorinfo_1, devicetypes_enum_1, autoupdater_2, group_2, EAlertLevel, DeviceStatus, DriveInfo, FullDeviceStatus, DeviceInfo, DeviceManager, MonitoredDrive, MonitoredDriveManager;
    var __moduleName = context_27 && context_27.id;
    return {
        setters: [
            function (collectorinfo_1_1) {
                collectorinfo_1 = collectorinfo_1_1;
            },
            function (devicetypes_enum_1_1) {
                devicetypes_enum_1 = devicetypes_enum_1_1;
            },
            function (autoupdater_2_1) {
                autoupdater_2 = autoupdater_2_1;
            },
            function (group_2_1) {
                group_2 = group_2_1;
            }
        ],
        execute: function () {
            (function (EAlertLevel) {
                EAlertLevel[EAlertLevel["Normal"] = 0] = "Normal";
                EAlertLevel[EAlertLevel["Alert"] = 1] = "Alert";
                EAlertLevel[EAlertLevel["Information"] = 2] = "Information";
            })(EAlertLevel || (EAlertLevel = {}));
            exports_27("EAlertLevel", EAlertLevel);
            DeviceStatus = /** @class */ (function () {
                function DeviceStatus(s) {
                    this.status = s.status;
                    this.alertLevel = s.alertLevel;
                    this.message = s.message;
                }
                return DeviceStatus;
            }());
            DriveInfo = /** @class */ (function () {
                function DriveInfo(d) {
                    this.letter = d.letter;
                    this.name = d.name;
                    this.typeDescription = d.typeDescription;
                    this.type = d.type;
                }
                return DriveInfo;
            }());
            exports_27("DriveInfo", DriveInfo);
            FullDeviceStatus = /** @class */ (function () {
                function FullDeviceStatus(status) {
                    var t = this;
                    this.fullStatus = {};
                    var keys = Object.keys(status.fullStatus);
                    keys.forEach(function (key, index) {
                        var ids = status.fullStatus[key];
                        t.fullStatus[key] = [];
                        for (var j = 0; j < ids.length; ++j) {
                            var ids2 = ids[j];
                            var ds = new DeviceStatus(ids2);
                            t.fullStatus[key].push(ds);
                        }
                    });
                }
                return FullDeviceStatus;
            }());
            DeviceInfo = /** @class */ (function () {
                //public panelIsOpen: boolean;
                function DeviceInfo(iinfo) {
                    this.name = iinfo.name;
                    this.id = iinfo.id;
                    this.type = iinfo.type;
                    this.ipAddress = iinfo.ipAddress;
                    this.statuses = [];
                    this.hasStatus = false;
                    this.alarms = [];
                    this.networkStatus = null;
                    this.groupID = iinfo.groupID;
                    this.monitoredDrives = new MonitoredDriveManager(iinfo.monitoredDrives);
                    //this.panelIsOpen = false;
                    this.updateCollectors(iinfo.collectors);
                    // We hide the drives that aren't being monitored. Only put drives into
                    // the driveNames map if they're being monitored.
                    this.driveNames = {};
                    var t = this;
                    Object.keys(iinfo.driveNames).forEach(function (key) {
                        var driveLetter = key;
                        var isMonitored = t.monitoredDrives.isDriveMonitored(driveLetter);
                        if (isMonitored)
                            t.driveNames[driveLetter] = iinfo.driveNames[driveLetter];
                    });
                }
                DeviceInfo.prototype.updateStatus = function (statuses) {
                    this.statuses = [];
                    this.alarms = [];
                    for (var i = 0; i < statuses.length; ++i) {
                        var status = statuses[i];
                        if (status.alertLevel == EAlertLevel.Alert)
                            this.alarms.push(status);
                        this.statuses.push(status);
                    }
                    this.hasStatus = statuses.length > 0;
                    this.statuses.sort(function (a, b) {
                        return a.status.localeCompare(b.status);
                    });
                    this.alarms.sort();
                };
                DeviceInfo.prototype.updateCollectors = function (collectors) {
                    if (!collectors)
                        return;
                    this.collectors = [];
                    this.collectorToInfo = {};
                    for (var i = 0; i < collectors.length; ++i) {
                        var collector = new collectorinfo_1.CollectorInfo(collectors[i]);
                        this.collectors.push(collector);
                        this.collectorToInfo[collector.collectorType] = collector;
                    }
                };
                DeviceInfo.prototype.updateCollector = function (collector) {
                    for (var i = 0; i < this.collectors.length; ++i) {
                        var c = this.collectors[i];
                        if (c && c.id == collector.id) {
                            var ci = new collectorinfo_1.CollectorInfo(collector);
                            this.collectors[i] = ci;
                            this.collectorToInfo[ci.collectorType] = ci;
                            break;
                        }
                    }
                };
                // Use string as the parameter type here because hasOwnProperty requires a string,
                // and this method is typically used from the html (i.e. ng-if="vm.device.isCollectorEnabled(0)")
                // so a string works good.
                DeviceInfo.prototype.isCollectorEnabled = function (type) {
                    if (this.collectorToInfo.hasOwnProperty(type)) {
                        var info = this.collectorToInfo[type];
                        return info.isEnabled;
                    }
                    return false;
                };
                DeviceInfo.prototype.getCollector = function (collectorID) {
                    var collector = null;
                    for (var i = 0; collector === null && i < this.collectors.length; ++i) {
                        if (this.collectors[i].id == collectorID)
                            collector = this.collectors[i];
                    }
                    return collector;
                };
                DeviceInfo.prototype.isWindowsDevice = function () {
                    return this.type == devicetypes_enum_1.EDeviceTypes.Server || this.type == devicetypes_enum_1.EDeviceTypes.Workstation;
                };
                return DeviceInfo;
            }());
            exports_27("DeviceInfo", DeviceInfo);
            // Group lane-related things together. Keys off the RPM device.
            //class Lane {
            //    public name: string;
            //    public rpm: DeviceInfo;
            //    public devices: DeviceInfo[];
            //    constructor(name: string) {
            //        this.name = name;
            //        this.rpm = null;
            //        this.devices = [];
            //    }
            //    public hasAlarm(): boolean {
            //        var hasAlarm = this.rpm && this.rpm.alarms.length > 0;
            //        for (var i = 0; hasAlarm === false && i < this.devices.length; ++i) {
            //            var device = this.devices[i];
            //            hasAlarm = device.alarms.length > 0;
            //        }
            //        return hasAlarm;
            //    }
            //}
            // Used to group lane things together. Keys off the ' ' character separating the name of
            // things, like "Lane001 RPM", "Lane001 Camera1", "Lane001 Camera2", etc.
            //class Grouping {
            //    public grouping: string;
            //    public name: string;
            //    constructor(s: string) {
            //        let index = s.indexOf(' ');
            //        if (index >= 0) {
            //            this.grouping = s.substr(0, index);
            //            this.name = s.substr(index + 1);
            //        } else {
            //            this.grouping = '';
            //            this.name = s;
            //        }
            //    }
            //}
            // Keeps track of all the devices. Each device is in a DeviceInfo object, and each one of
            // those keeps track of each device's collectors.
            // If a device has been assigned to a Group, it will be kept in the appropriate Group
            // object. Otherwise, it will be kept in the devices list.
            // All devices, regardlesss of whether they're grouped or not, will be kept
            // in the allDevices struct, which maps the unique device ID to the DeviceInfo object.
            DeviceManager = /** @class */ (function () {
                function DeviceManager(dataService, $interval) {
                    this.devices = [];
                    this.windowsDevices = [];
                    this.allDevices = {};
                    this.hasAlarms = this.hasStatus = false;
                    this.groups = [];
                    this.upNext = [];
                    this.dataService = dataService;
                    this.autoUpdater = new autoupdater_2.AutoUpdater(5000, DeviceManager.gatherData, this, $interval);
                }
                DeviceManager.prototype.setConfiguration = function (configuration) {
                    if (!configuration)
                        return;
                    this.groups = [];
                    if (configuration.groups) {
                        for (var i = 0; i < configuration.groups.length; ++i) {
                            var g = new group_2.Group(configuration.groups[i]);
                            this.groups.push(g);
                        }
                        this.groups.sort(function (a, b) {
                            return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
                        });
                    }
                    var devices = configuration.devices;
                    // We don't want the 'System' device to be in the list of all devices/groups
                    this.allDevices = {};
                    for (var i = 0; i < devices.length; ++i) {
                        var device = devices[i];
                        this.allDevices[device.id] = device;
                        if (device.type > devicetypes_enum_1.EDeviceTypes.Unknown && device.type !== devicetypes_enum_1.EDeviceTypes.System) {
                            var group = this.findGroup(device.groupID);
                            if (group)
                                group.addDevice(device);
                            else
                                this.devices.push(device);
                        }
                        if (device.type === devicetypes_enum_1.EDeviceTypes.System)
                            this.systemDevice = device;
                        if (device.isWindowsDevice())
                            this.windowsDevices.push(device);
                    }
                    this.devices.sort(function (a, b) {
                        return a.name.localeCompare(b.name);
                    });
                    this.windowsDevices.sort(function (a, b) {
                        return a.name.localeCompare(b.name);
                    });
                    // Sort the devices within each group
                    for (var i = 0; i < this.groups.length; ++i) {
                        var g = this.groups[i];
                        g.devices.sort(function (a, b) {
                            return a.name.localeCompare(b.name);
                        });
                    }
                    this.updateUpNext();
                };
                DeviceManager.prototype.findDeviceFromID = function (id) {
                    return this.allDevices[id];
                };
                DeviceManager.prototype.findDeviceFromName = function (name) {
                    var t = this;
                    var keys = Object.keys(t.allDevices);
                    for (var i = 0; i < keys.length; ++i) {
                        var d = t.allDevices[keys[i]];
                        if (d.name === name)
                            return d;
                    }
                    return null;
                };
                DeviceManager.prototype.findGroup = function (id) {
                    var g = null;
                    if (!id || id < 0)
                        return g;
                    for (var i = 0; g === null && i < this.groups.length; ++i) {
                        var group = this.groups[i];
                        if (group.id === id)
                            g = group;
                    }
                    return g;
                };
                DeviceManager.prototype.updateStatus = function (ifds) {
                    var t = this;
                    var fds = new FullDeviceStatus(ifds);
                    Object.keys(fds.fullStatus).forEach(function (key, _index) {
                        var statuses = fds.fullStatus[key];
                        var device = t.allDevices[Number(key)];
                        if (device)
                            device.updateStatus(statuses);
                    });
                    this.updateStatusFlags();
                    for (var i = 0; i < this.groups.length; ++i)
                        this.groups[i].updateStatusFlags();
                };
                DeviceManager.prototype.updateCollectors = function (collectors) {
                    if (!collectors)
                        return;
                    // A map of device ID to their set of collectors
                    var test = {};
                    for (var i = 0; i < collectors.length; ++i) {
                        var collector = collectors[i];
                        if (test.hasOwnProperty(collector.deviceID) === false)
                            test[collector.deviceID] = [];
                        test[collector.deviceID].push(collector);
                    }
                    var t = this;
                    for (var key in test) {
                        var deviceID = Number(key);
                        var cs = test[deviceID];
                        var device = t.findDeviceFromID(deviceID);
                        if (device)
                            device.updateCollectors(cs);
                    }
                    this.updateUpNext();
                };
                DeviceManager.prototype.updateUpNext = function () {
                    // A list of all the collectors. Will be sorted into time 
                    // order based on the next collection time.
                    var allCollectors = [];
                    for (var i = 0; i < this.devices.length; ++i) {
                        var dev = this.devices[i];
                        for (var j = 0; dev.collectors && j < dev.collectors.length; ++j) {
                            var collector = dev.collectors[j];
                            if (collector.isEnabled)
                                allCollectors.push(collector);
                        }
                    }
                    allCollectors.sort(function (x, y) {
                        if (!x.nextCollectionTime && !y.nextCollectionTime)
                            return 0;
                        else if (!x.nextCollectionTime && y.nextCollectionTime)
                            return -1;
                        else if (x.nextCollectionTime && !y.nextCollectionTime)
                            return 1;
                        var a = x.nextCollectionTime.getTime();
                        var b = y.nextCollectionTime.getTime();
                        return a - b;
                    });
                    this.upNext = allCollectors;
                };
                DeviceManager.prototype.getDevicesForDataCollection = function () {
                    var devices = [];
                    var keys = Object.keys(this.allDevices);
                    for (var i = 0; i < keys.length; ++i) {
                        var d = this.allDevices[keys[i]];
                        // We want the system device first in the array, so don't
                        // add it here and we'll put in at the very end.
                        if (d !== this.systemDevice && d.collectors && d.collectors.length > 0)
                            devices.push(d);
                    }
                    devices.sort(function (a, b) {
                        return a.name.localeCompare(b.name);
                    });
                    devices.splice(0, 0, this.systemDevice);
                    return devices;
                };
                DeviceManager.prototype.collectNow = function (collectorID) {
                    var device = null;
                    for (var i = 0; device == null && i < this.devices.length; ++i) {
                        var d = this.devices[i];
                        var c = d.getCollector(collectorID);
                        if (c)
                            device = d;
                    }
                    if (!device) {
                        // Look in the groups for the device
                        for (var i = 0; device == null && i < this.groups.length; ++i) {
                            var g = this.groups[i];
                            device = g.findDeviceFromCollectorID(collectorID);
                        }
                    }
                    if (!device) {
                        // See if the System device has it
                        var c = this.systemDevice.getCollector(collectorID);
                        if (c)
                            device = this.systemDevice;
                    }
                    if (device) {
                        this.dataService.collectNow(collectorID)
                            .then(function (data) { return device.updateCollector(data); });
                    }
                };
                DeviceManager.prototype.collectAll = function (deviceID) {
                    var device = this.findDeviceFromID(deviceID);
                    if (device) {
                        for (var i = 0; i < device.collectors.length; ++i) {
                            var collector = device.collectors[i];
                            this.dataService.collectNow(collector.id)
                                .then(function (data) {
                                device.updateCollector(data);
                            });
                        }
                    }
                };
                DeviceManager.prototype.updateStatusFlags = function () {
                    // Have to do this because inside the forEach method 'this' doesn't
                    // refer to the DeviceManager object.
                    var t = this;
                    this.hasAlarms = false;
                    this.hasStatus = false;
                    Object.keys(this.allDevices).forEach(function (key, index) {
                        var d = t.allDevices[key];
                        var hasAlarm = d.alarms.length > 0;
                        t.hasAlarms = t.hasAlarms || hasAlarm;
                        var keys = Object.keys(d.statuses);
                        t.hasStatus = t.hasStatus || keys.length > 0;
                    });
                };
                DeviceManager.prototype.updateNetwork = function (network) {
                    for (var i = 0; i < network.data.length; ++i) {
                        var n = network.data[i];
                        if (n.deviceID >= 0) {
                            //console.log("updateNetwork: " + JSON.stringify(n));
                            var device = this.allDevices[n.deviceID];
                            if (device) {
                                //console.log("updateNetwork-A: " + JSON.stringify(device));
                                device.networkStatus = n;
                                //console.log("updateNetwork-B: " + JSON.stringify(device));
                            }
                        }
                    }
                };
                //public closeAllPanels() {
                //    for (var i = 0; i < this.devices.length; ++i) {
                //        this.devices[i].panelIsOpen = false;
                //    }
                //    for (var i = 0; i < this.groups.length; ++i) {
                //        this.groups[i].panelIsOpen = false;
                //    }
                //}
                DeviceManager.prototype.startAutomaticUpdate = function () {
                    this.autoUpdater.start();
                };
                DeviceManager.prototype.stopAutomaticUpdate = function () {
                    this.autoUpdater.stop();
                };
                DeviceManager.gatherData = function (t) {
                    t.dataService.getDeviceStatus()
                        .then(function (data) { return t.updateStatus(data); });
                    t.dataService.getAllCollectors()
                        .then(function (collectors) { return t.updateCollectors(collectors); });
                };
                return DeviceManager;
            }());
            exports_27("DeviceManager", DeviceManager);
            MonitoredDrive = /** @class */ (function (_super) {
                __extends(MonitoredDrive, _super);
                function MonitoredDrive(d) {
                    var _this = _super.call(this, d) || this;
                    _this.isMonitored = d.isMonitored;
                    return _this;
                }
                return MonitoredDrive;
            }(DriveInfo));
            MonitoredDriveManager = /** @class */ (function () {
                function MonitoredDriveManager(manager) {
                    this.driveMap = {};
                    var t = this;
                    Object.keys(manager.driveMap).forEach(function (key) {
                        t.driveMap[key] = new MonitoredDrive(manager.driveMap[key]);
                    });
                }
                MonitoredDriveManager.prototype.isDriveMonitored = function (driveLetter) {
                    var isMonitored = false;
                    var d = this.driveMap[driveLetter];
                    if (d)
                        isMonitored = d.isMonitored;
                    return isMonitored;
                };
                return MonitoredDriveManager;
            }());
            exports_27("MonitoredDriveManager", MonitoredDriveManager);
        }
    };
});
System.register("classes/applications", [], function (exports_28, context_28) {
    "use strict";
    var DeviceApplications, Snapshot, ApplicationHistory, ApplicationsHistoryMap, AllApplicationsHistory, ApplicationManager;
    var __moduleName = context_28 && context_28.id;
    return {
        setters: [],
        execute: function () {
            DeviceApplications = /** @class */ (function () {
                function DeviceApplications(data) {
                    this.deviceID = data.deviceID;
                    this.applications = data.applications;
                    this.timestamp = new Date(data.timestamp);
                }
                return DeviceApplications;
            }());
            Snapshot = /** @class */ (function () {
                function Snapshot(data) {
                    this.version = data.version;
                    this.timestamp = new Date(data.timestamp);
                }
                return Snapshot;
            }());
            // See Database/Models.cs
            ApplicationHistory = /** @class */ (function () {
                function ApplicationHistory() {
                    this.name = "";
                    this.history = [];
                }
                ApplicationHistory.prototype.update = function (data) {
                    this.name = data.name;
                    this.history = [];
                    for (var i = 0; i < data.history.length; ++i) {
                        var snapshot = new Snapshot(data.history[i]);
                        this.history.push(snapshot);
                    }
                };
                return ApplicationHistory;
            }());
            ApplicationsHistoryMap = /** @class */ (function () {
                function ApplicationsHistoryMap(map) {
                    this.history = {};
                    var keys = Object.keys(map);
                    for (var i = 0; i < keys.length; ++i) {
                        var name_1 = keys[i];
                        var history_1 = map[name_1];
                        var appHistory = new ApplicationHistory();
                        appHistory.update(history_1);
                        this.history[name_1] = appHistory;
                    }
                }
                return ApplicationsHistoryMap;
            }());
            AllApplicationsHistory = /** @class */ (function () {
                function AllApplicationsHistory(history) {
                    this.history = new ApplicationsHistoryMap(history.history);
                    this.apps = Object.keys(this.history.history);
                    this.apps.sort();
                }
                return AllApplicationsHistory;
            }());
            exports_28("AllApplicationsHistory", AllApplicationsHistory);
            ApplicationManager = /** @class */ (function () {
                function ApplicationManager(data, dataService) {
                    this.applications = new DeviceApplications(data);
                    this.dataService = dataService;
                    //this.values = [];
                    this.applicationHistory = null;
                }
                ApplicationManager.prototype.onSelectApplication = function (app) {
                    var t = this;
                    this.dataService.getAppHistory(this.applications.deviceID, app)
                        .then(function (data) {
                        if (!t.applicationHistory)
                            t.applicationHistory = new ApplicationHistory();
                        t.applicationHistory.update(data);
                    });
                };
                return ApplicationManager;
            }());
            exports_28("ApplicationManager", ApplicationManager);
        }
    };
});
System.register("classes/errors", [], function (exports_29, context_29) {
    "use strict";
    var ErrorInfo, ErrorManager;
    var __moduleName = context_29 && context_29.id;
    return {
        setters: [],
        execute: function () {
            ErrorInfo = /** @class */ (function () {
                function ErrorInfo(data) {
                    this.message = data.message;
                    this.timestamp = new Date(data.timestamp);
                    this.count = data.count;
                    this.firstTimestamp = this.lastTimestamp = null;
                    if (data.firstTimestamp !== null)
                        this.firstTimestamp = new Date(data.firstTimestamp);
                    if (data.lastTimestamp !== null)
                        this.lastTimestamp = new Date(data.lastTimestamp);
                }
                return ErrorInfo;
            }());
            ErrorManager = /** @class */ (function () {
                function ErrorManager(data) {
                    this.errors = [];
                    if (data === undefined || data === null || data.errors.length === 0)
                        return;
                    for (var i = 0; i < data.errors.length; ++i) {
                        var errorInfo = new ErrorInfo(data.errors[i]);
                        this.errors.push(errorInfo);
                    }
                }
                return ErrorManager;
            }());
            exports_29("ErrorManager", ErrorManager);
        }
    };
});
System.register("classes/services", [], function (exports_30, context_30) {
    "use strict";
    var Services;
    var __moduleName = context_30 && context_30.id;
    return {
        setters: [],
        execute: function () {
            Services = /** @class */ (function () {
                function Services(s) {
                    this.services = s.services;
                    this.timestamp = new Date(s.timestamp);
                }
                return Services;
            }());
            exports_30("Services", Services);
        }
    };
});
System.register("classes/machine", ["classes/memory", "disk/disk", "classes/nic", "classes/errors", "classes/cpu", "classes/processes", "classes/database", "classes/applications", "classes/services", "classes/ups"], function (exports_31, context_31) {
    "use strict";
    var memory_1, disk_1, nic_1, errors_1, cpu_1, processes_1, database_1, applications_1, services_1, ups_1, DeviceDetails, EMachineParts, Machine;
    var __moduleName = context_31 && context_31.id;
    return {
        setters: [
            function (memory_1_1) {
                memory_1 = memory_1_1;
            },
            function (disk_1_1) {
                disk_1 = disk_1_1;
            },
            function (nic_1_1) {
                nic_1 = nic_1_1;
            },
            function (errors_1_1) {
                errors_1 = errors_1_1;
            },
            function (cpu_1_1) {
                cpu_1 = cpu_1_1;
            },
            function (processes_1_1) {
                processes_1 = processes_1_1;
            },
            function (database_1_1) {
                database_1 = database_1_1;
            },
            function (applications_1_1) {
                applications_1 = applications_1_1;
            },
            function (services_1_1) {
                services_1 = services_1_1;
            },
            function (ups_1_1) {
                ups_1 = ups_1_1;
            }
        ],
        execute: function () {
            DeviceDetails = /** @class */ (function () {
                function DeviceDetails(data) {
                    this.uptime = data.uptime;
                    var uptimeRegex = /(\d+) (\d\d:\d\d:\d\d)/;
                    var match = uptimeRegex.exec(data.uptime);
                    if (match !== null) {
                        this.uptime = match[1] + " days, " + match[2];
                    }
                    this.lastBootTime = null;
                    if (data.lastBootTime && data.lastBootTime !== "")
                        this.lastBootTime = new Date(data.lastBootTime);
                }
                return DeviceDetails;
            }());
            (function (EMachineParts) {
                EMachineParts[EMachineParts["CPU"] = 0] = "CPU";
                EMachineParts[EMachineParts["Memory"] = 1] = "Memory";
                EMachineParts[EMachineParts["DiskUsage"] = 2] = "DiskUsage";
                EMachineParts[EMachineParts["DiskPerformance"] = 3] = "DiskPerformance";
                EMachineParts[EMachineParts["NIC"] = 4] = "NIC";
                EMachineParts[EMachineParts["SystemErrors"] = 5] = "SystemErrors";
                EMachineParts[EMachineParts["ApplicationErrors"] = 6] = "ApplicationErrors";
                EMachineParts[EMachineParts["Processes"] = 7] = "Processes";
                EMachineParts[EMachineParts["Database"] = 8] = "Database";
                EMachineParts[EMachineParts["Applications"] = 9] = "Applications";
                EMachineParts[EMachineParts["Services"] = 10] = "Services";
                EMachineParts[EMachineParts["UPS"] = 11] = "UPS";
                //Network,
                //SMART,
                EMachineParts[EMachineParts["NumMachineParts"] = 12] = "NumMachineParts";
            })(EMachineParts || (EMachineParts = {}));
            exports_31("EMachineParts", EMachineParts);
            Machine = /** @class */ (function () {
                /// Let's have a way to retrieve a subset of all things that we can retrieve.
                /// The third parameter will specify the parts to retrieve. If it's null, retrieve
                /// everything. If it's non-null, retrieve just those specified parts.
                function Machine(devInfo, dataService, startTime, partsToRetrieve) {
                    var _this = this;
                    this.devInfo = devInfo;
                    this.dataService = dataService;
                    this.getDeviceDetails = function () {
                        var t = _this;
                        _this.dataService.getDeviceDetails(_this.devInfo.id)
                            .then(function (data) {
                            if (data)
                                t.details = new DeviceDetails(data);
                        });
                    };
                    this.getCPUData = function () {
                        var startDate = _this.getStartDate(EMachineParts.CPU);
                        if (!startDate)
                            return;
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.CPU], startDate, null);
                    };
                    this.getMemoryData = function () {
                        var startDate = _this.getStartDate(EMachineParts.Memory);
                        if (!startDate)
                            return;
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.Memory], startDate, null);
                    };
                    this.getDiskUsageData = function () {
                        var startDate = _this.getStartDate(EMachineParts.DiskUsage);
                        if (!startDate)
                            return;
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.DiskUsage], startDate, null);
                    };
                    this.onDiskUsageSelection = function (diskName) {
                        if (_this.diskUsage !== null)
                            _this.diskUsage.selectDisk(diskName);
                    };
                    this.getDiskPerformanceData = function () {
                        var startDate = _this.getStartDate(EMachineParts.DiskPerformance);
                        if (!startDate)
                            return;
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.DiskPerformance], startDate, null);
                    };
                    this.onDiskPerformanceSelection = function (diskName) {
                        if (_this.diskPerformance !== null)
                            _this.diskPerformance.selectDisk(diskName);
                    };
                    this.getNICData = function () {
                        var startDate = _this.getStartDate(EMachineParts.NIC);
                        if (!startDate)
                            return;
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.NIC], startDate, null);
                    };
                    this.getSystemErrors = function () {
                        var startDate = _this.getStartDate(EMachineParts.SystemErrors);
                        if (!startDate)
                            return;
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.SystemErrors], startDate, null);
                    };
                    this.getApplicationErrors = function () {
                        var startDate = _this.getStartDate(EMachineParts.ApplicationErrors);
                        if (!startDate)
                            return;
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.ApplicationErrors], startDate, null);
                    };
                    this.getProcesses = function () {
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.Processes], null, null);
                    };
                    this.getDatabases = function () {
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.Database], null, null);
                    };
                    this.getApplications = function () {
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.Applications], null, null);
                    };
                    this.getServices = function () {
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.Services], null, null);
                    };
                    this.getUPS = function () {
                        var startDate = _this.getStartDate(EMachineParts.UPS);
                        if (!startDate)
                            return;
                        _this.retrieveData(_this.devInfo.id, [EMachineParts.UPS], null, null);
                    };
                    this.onProcessSelection = function (process) {
                        if (_this.processes)
                            _this.processes.onSelectProcess(process);
                    };
                    this.name = devInfo.name;
                    this.cpu = null;
                    this.memory = null;
                    this.diskUsage = null;
                    this.diskPerformance = null;
                    this.nic = null;
                    this.systemErrors = this.applicationErrors = null;
                    this.processes = null;
                    this.database = null;
                    this.applications = null;
                    this.allParts = new Array(EMachineParts.NumMachineParts);
                    this.allMethods = {};
                    this.allMethods[EMachineParts.CPU] = this.getCPUData;
                    this.allMethods[EMachineParts.Memory] = this.getMemoryData;
                    this.allMethods[EMachineParts.DiskUsage] = this.getDiskUsageData;
                    this.allMethods[EMachineParts.DiskPerformance] = this.getDiskPerformanceData;
                    this.allMethods[EMachineParts.NIC] = this.getNICData;
                    this.allMethods[EMachineParts.SystemErrors] = this.getSystemErrors;
                    this.allMethods[EMachineParts.ApplicationErrors] = this.getApplicationErrors;
                    this.allMethods[EMachineParts.Processes] = this.getProcesses;
                    this.allMethods[EMachineParts.Database] = this.getDatabases;
                    this.allMethods[EMachineParts.Applications] = this.getApplications;
                    this.allMethods[EMachineParts.Services] = this.getServices;
                    this.allMethods[EMachineParts.UPS] = this.getUPS;
                    //this.allMethods[MachineParts.SMART] = this.getSMART;
                    this.loading = {};
                    // Another way of retrieving all the things in an Enum
                    // https://stackoverflow.com/a/18112157/706747
                    for (var e in EMachineParts) {
                        var isNum = parseInt(e, 10) >= 0;
                        if (isNum)
                            this.loading[e] = false;
                    }
                    if (!partsToRetrieve) {
                        // Retrieve everything. But let's automatically populate the toRetrieve array
                        // with everything in MachineParts, except for the NumMachineParts value
                        //
                        // Here's how to get all the values from MachineParts: http://stackoverflow.com/a/21294925
                        var objValues = Object.keys(EMachineParts).map(function (k) { return EMachineParts[k]; });
                        var value = objValues.filter(function (v) { return typeof v === "number"; });
                        partsToRetrieve = value;
                    }
                    this.toRetrieve = partsToRetrieve;
                    // Make sure we don't retrieve the last value from the enum, which is just used as a count
                    var index = this.toRetrieve.indexOf(EMachineParts.NumMachineParts);
                    if (index >= 0)
                        this.toRetrieve.splice(index, 1);
                    if (partsToRetrieve)
                        this.batchGetEverythingFrom(startTime);
                    else
                        this.getEverythingFrom(startTime);
                    //this.getEverything();
                }
                Machine.prototype.getEverythingFrom = function (starting) {
                    for (var i = 0; i < this.allParts.length; ++i)
                        this.allParts[i] = starting;
                    this.getEverything();
                };
                Machine.prototype.getEverything = function () {
                    this.getDeviceDetails();
                    for (var i = 0; i < this.toRetrieve.length; ++i) {
                        var part = this.toRetrieve[i];
                        var method = this.allMethods[part];
                        if (method) {
                            this.loading[part] = true;
                            method();
                        }
                    }
                };
                Machine.prototype.batchGetEverythingFrom = function (starting) {
                    for (var i = 0; i < this.allParts.length; ++i)
                        this.allParts[i] = starting;
                    this.batchGetEverything(starting);
                };
                Machine.prototype.batchGetEverything = function (starting) {
                    this.getDeviceDetails();
                    var t = this;
                    for (var i = 0; i < this.toRetrieve.length; ++i) {
                        var e = this.toRetrieve[i];
                        this.loading[e] = true;
                    }
                    this.dataService.getMachineData(this.devInfo.id, this.toRetrieve, starting)
                        .then(function (data) {
                        t.onReceivedData(data);
                    });
                };
                Machine.prototype.isToRetrieve = function (part) {
                    return this.toRetrieve.indexOf(part) >= 0;
                };
                Machine.prototype.getMoreDays = function (days, part) {
                    var old_date = this.allParts[part];
                    this.allParts[part] = new Date(old_date.getTime() - (days * 24 * 60 * 60 * 1000));
                    if (this.isToRetrieve(part)) {
                        var method = this.allMethods[part];
                        if (method) {
                            this.loading[part] = true;
                            method();
                        }
                    }
                };
                Machine.prototype.retrieveData = function (id, parts, start, end) {
                    var t = this;
                    this.dataService.getMachineData(this.devInfo.id, parts, start, end)
                        .then(function (data) {
                        t.onReceivedData(data);
                    });
                };
                Machine.prototype.onReceivedData = function (data) {
                    if (!data)
                        return;
                    if (data.cpu) {
                        if (!this.cpu)
                            this.cpu = new cpu_1.CPUData();
                        this.cpu.update(data.cpu);
                        this.loading[EMachineParts.CPU] = false;
                    }
                    if (data.memory) {
                        if (!this.memory)
                            this.memory = new memory_1.Memory();
                        this.memory.update(data.memory);
                        this.loading[EMachineParts.Memory] = false;
                    }
                    if (data.diskUsage) {
                        if (!this.diskUsage)
                            this.diskUsage = new disk_1.DiskUsageManager(this.devInfo);
                        this.diskUsage.update(data.diskUsage);
                        this.loading[EMachineParts.DiskUsage] = false;
                    }
                    if (data.diskPerformance) {
                        if (!this.diskPerformance)
                            this.diskPerformance = new disk_1.DiskPerformanceManager(this.devInfo);
                        this.diskPerformance.update(data.diskPerformance);
                        this.loading[EMachineParts.DiskPerformance] = false;
                    }
                    if (data.nic) {
                        if (!this.nic)
                            this.nic = new nic_1.NICData();
                        this.nic.update(data.nic);
                        this.loading[EMachineParts.NIC] = false;
                    }
                    if (data.systemErrors) {
                        this.systemErrors = new errors_1.ErrorManager(data.systemErrors);
                        this.loading[EMachineParts.SystemErrors] = false;
                    }
                    if (data.applicationErrors) {
                        this.applicationErrors = new errors_1.ErrorManager(data.applicationErrors);
                        this.loading[EMachineParts.ApplicationErrors] = false;
                    }
                    if (data.processes) {
                        this.processes = new processes_1.ProcessManager(data.processes, this.dataService);
                        this.loading[EMachineParts.Processes] = false;
                    }
                    if (data.database) {
                        this.database = new database_1.DatabaseManager(data.database, this.dataService);
                        this.loading[EMachineParts.Database] = false;
                    }
                    if (data.applications) {
                        this.applications = new applications_1.ApplicationManager(data.applications, this.dataService);
                        this.loading[EMachineParts.Applications] = false;
                    }
                    if (data.services) {
                        this.services = new services_1.Services(data.services);
                        this.loading[EMachineParts.Services] = false;
                    }
                    if (data.ups) {
                        this.ups = new ups_1.UPSStatus(data.ups);
                        this.loading[EMachineParts.UPS] = false;
                    }
                };
                Machine.prototype.getStartDate = function (part) {
                    var startDate = null;
                    if (this.allParts[part])
                        startDate = this.allParts[part];
                    return startDate;
                };
                return Machine;
            }());
            exports_31("Machine", Machine);
        }
    };
});
System.register("classes/utilities", [], function (exports_32, context_32) {
    "use strict";
    var Utilities;
    var __moduleName = context_32 && context_32.id;
    return {
        setters: [],
        execute: function () {
            Utilities = /** @class */ (function () {
                function Utilities() {
                }
                Utilities.chunk = function (data, size) {
                    var newArr = [];
                    if (size > 0) {
                        for (var i = 0; i < data.length; i += size) {
                            newArr.push(data.slice(i, i + size));
                        }
                    }
                    return newArr;
                };
                Utilities.chunkToTupleOf2 = function (data) {
                    var chunks = Utilities.chunk(data, 2);
                    var arr = [];
                    if (chunks && chunks.length > 0) {
                        for (var i = 0; i < chunks[0].length; ++i)
                            arr.push([chunks[0][i], null]);
                        for (var i = 0; i < chunks[1].length; ++i)
                            arr[1][i] = chunks[1][i];
                    }
                    return arr;
                };
                Utilities.toMidnight = function (date) {
                    var y = date.getFullYear();
                    var m = date.getMonth();
                    var d = date.getDate();
                    return new Date(y, m, d, 0, 0, 0, 0);
                };
                return Utilities;
            }());
            exports_32("Utilities", Utilities);
        }
    };
});
/// <reference types="angular" />
System.register("services/configuration.service", ["classes/systemconfiguration", "classes/promisekeeper"], function (exports_33, context_33) {
    "use strict";
    var systemconfiguration_1, promisekeeper_2, ConfigurationService;
    var __moduleName = context_33 && context_33.id;
    return {
        setters: [
            function (systemconfiguration_1_1) {
                systemconfiguration_1 = systemconfiguration_1_1;
            },
            function (promisekeeper_2_1) {
                promisekeeper_2 = promisekeeper_2_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            ConfigurationService = /** @class */ (function () {
                function ConfigurationService(ds, $q) {
                    this.ds = ds;
                    this.$q = $q;
                    this.keeper = new promisekeeper_2.PromiseKeeper($q);
                    this.requested = false;
                    // console.log("ConfigurationService.constructor");
                }
                ConfigurationService.prototype.get = function () {
                    var deferred = this.$q.defer();
                    if (ConfigurationService.config)
                        deferred.resolve(ConfigurationService.config);
                    else {
                        this.keeper.push(deferred);
                        if (this.requested == false) {
                            this.requested = true;
                            var t_2 = this;
                            this.ds.getConfiguration()
                                .then(function (result) {
                                var config = new systemconfiguration_1.SystemConfiguration(result);
                                t_2.keeper.resolve(config);
                                ConfigurationService.config = config;
                            });
                        }
                    }
                    return deferred.promise;
                };
                ConfigurationService.Factory = function () {
                    var factory = function (ds, $q) {
                        return new ConfigurationService(ds, $q);
                    };
                    factory.$inject = ['dataService', '$q'];
                    return factory;
                };
                return ConfigurationService;
            }());
            exports_33("ConfigurationService", ConfigurationService);
        }
    };
});
System.register("reports/report", ["enums/collectortype.enum", "classes/machine", "classes/utilities", "classes/network", "classes/applications", "classes/services", "classes/systemconfiguration"], function (exports_34, context_34) {
    "use strict";
    var collectortype_enum_1, machine_1, utilities_1, network_1, applications_2, services_2, systemconfiguration_2, EReportTypes, EReportSubTypes, Report, CurrentPeakReportBase, MemoryReport, DiskInfo, DiskReport, CPUReport, NICReport, MachineReport, NetworkReport, CASLoadReport, IssuesReport, ConfigReport, SiteReport;
    var __moduleName = context_34 && context_34.id;
    return {
        setters: [
            function (collectortype_enum_1_1) {
                collectortype_enum_1 = collectortype_enum_1_1;
            },
            function (machine_1_1) {
                machine_1 = machine_1_1;
            },
            function (utilities_1_1) {
                utilities_1 = utilities_1_1;
            },
            function (network_1_1) {
                network_1 = network_1_1;
            },
            function (applications_2_1) {
                applications_2 = applications_2_1;
            },
            function (services_2_1) {
                services_2 = services_2_1;
            },
            function (systemconfiguration_2_1) {
                systemconfiguration_2 = systemconfiguration_2_1;
            }
        ],
        execute: function () {
            (function (EReportTypes) {
                EReportTypes[EReportTypes["Server"] = 0] = "Server";
                EReportTypes[EReportTypes["Workstation"] = 1] = "Workstation";
                EReportTypes[EReportTypes["Network"] = 2] = "Network";
                EReportTypes[EReportTypes["CASLoad"] = 3] = "CASLoad";
                EReportTypes[EReportTypes["Issues"] = 4] = "Issues";
                EReportTypes[EReportTypes["SiteConfiguration"] = 5] = "SiteConfiguration";
                EReportTypes[EReportTypes["Site"] = 6] = "Site";
            })(EReportTypes || (EReportTypes = {}));
            exports_34("EReportTypes", EReportTypes);
            (function (EReportSubTypes) {
                EReportSubTypes[EReportSubTypes["Memory"] = 0] = "Memory";
                EReportSubTypes[EReportSubTypes["Disk"] = 1] = "Disk";
                EReportSubTypes[EReportSubTypes["CPU"] = 2] = "CPU";
                EReportSubTypes[EReportSubTypes["NIC"] = 3] = "NIC";
            })(EReportSubTypes || (EReportSubTypes = {}));
            exports_34("EReportSubTypes", EReportSubTypes);
            Report = /** @class */ (function () {
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
            exports_34("Report", Report);
            CurrentPeakReportBase = /** @class */ (function () {
                function CurrentPeakReportBase(report) {
                    this.currentPercentUsed = report.currentPercentUsed;
                    this.peakPercentUsed = report.peakPercentUsed;
                    this.peakTimestamp = new Date(report.peakTimestamp);
                }
                return CurrentPeakReportBase;
            }());
            MemoryReport = /** @class */ (function (_super) {
                __extends(MemoryReport, _super);
                function MemoryReport(report) {
                    return _super.call(this, report) || this;
                }
                return MemoryReport;
            }(CurrentPeakReportBase));
            DiskInfo = /** @class */ (function (_super) {
                __extends(DiskInfo, _super);
                function DiskInfo(info) {
                    var _this = _super.call(this, info) || this;
                    _this.name = info.name;
                    return _this;
                }
                return DiskInfo;
            }(CurrentPeakReportBase));
            DiskReport = /** @class */ (function () {
                function DiskReport(report) {
                    this.disks = [];
                    for (var i = 0; i < report.disks.length; ++i)
                        this.disks.push(new DiskInfo(report.disks[i]));
                }
                return DiskReport;
            }());
            CPUReport = /** @class */ (function (_super) {
                __extends(CPUReport, _super);
                function CPUReport(report) {
                    return _super.call(this, report) || this;
                }
                return CPUReport;
            }(CurrentPeakReportBase));
            NICReport = /** @class */ (function (_super) {
                __extends(NICReport, _super);
                function NICReport(report) {
                    var _this = _super.call(this, report) || this;
                    _this.bps = report.bps;
                    _this.peakBps = report.peakBps;
                    return _this;
                }
                return NICReport;
            }(CurrentPeakReportBase));
            MachineReport = /** @class */ (function (_super) {
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
                        t.services = new services_2.Services(data);
                        if (t.services.services)
                            t.splitServices = utilities_1.Utilities.chunkToTupleOf2(t.services.services);
                    });
                };
                MachineReport.prototype.getAppHistory = function () {
                    var t = this;
                    this.appHistory = null;
                    this.dataService.getAppChanges(this.settings.device.id, this.settings.startDate, this.settings.endDate)
                        .then(function (data) {
                        t.appHistory = new applications_2.AllApplicationsHistory(data);
                    });
                };
                return MachineReport;
            }(Report));
            exports_34("MachineReport", MachineReport);
            NetworkReport = /** @class */ (function (_super) {
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
            exports_34("NetworkReport", NetworkReport);
            CASLoadReport = /** @class */ (function (_super) {
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
            exports_34("CASLoadReport", CASLoadReport);
            IssuesReport = /** @class */ (function (_super) {
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
            exports_34("IssuesReport", IssuesReport);
            ConfigReport = /** @class */ (function (_super) {
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
                        t.configData = new systemconfiguration_2.SystemConfiguration(data);
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
            exports_34("ConfigReport", ConfigReport);
            SiteReport = /** @class */ (function (_super) {
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
            exports_34("SiteReport", SiteReport);
        }
    };
});
System.register("services/data.service", ["moment"], function (exports_35, context_35) {
    "use strict";
    var moment_1, URL, URLQueue, DataService;
    var __moduleName = context_35 && context_35.id;
    return {
        setters: [
            function (moment_1_1) {
                moment_1 = moment_1_1;
            }
        ],
        execute: function () {
            URL = /** @class */ (function () {
                function URL(url, d, http) {
                    this.url = url;
                    this.d = d;
                    this.http = http;
                    this.urlID = URL.nextID++;
                    this.func = this.get;
                }
                URL.prototype.get = function () {
                    var t = this;
                    //console.log("Executing: " + t.toString());
                    this.http.get(t.url)
                        .then(function (response) {
                        t.d.resolve(response.data);
                        t.urlQueue.remove(t);
                    })
                        .catch(function (reason) {
                        console.log("Rejected " + t.toString() + ": " + reason.toString());
                        t.d.reject(reason);
                        t.urlQueue.remove(t);
                    });
                };
                URL.prototype.toString = function () {
                    return "(" + this.urlID.toString() + ") " + this.url;
                };
                URL.nextID = 1;
                return URL;
            }());
            URLQueue = /** @class */ (function () {
                function URLQueue(maxLength, timeoutInS) {
                    this.maxLength = maxLength;
                    this.held = [];
                    this.executing = [];
                    if (timeoutInS)
                        this.timeoutInMS = timeoutInS * 1000;
                }
                URLQueue.prototype.push = function (url) {
                    url.urlQueue = this;
                    url.requestedAt = new Date();
                    this.held.push(url);
                    this.executeHeldURLs();
                };
                URLQueue.prototype.executeHeldURLs = function () {
                    this.clearTimedOut();
                    while (this.held.length > 0 && this.executing.length < this.maxLength) {
                        var url = this.held.shift();
                        url.transmittedAt = new Date();
                        //console.log("executing " + url.toString());
                        this.executing.push(url);
                        url.get();
                    }
                };
                URLQueue.prototype.remove = function (url) {
                    for (var i = 0; i < this.executing.length; ++i) {
                        if (this.executing[i].urlID == url.urlID) {
                            var url_1 = this.executing[i];
                            //let ms = Math.abs(new Date().getTime() - url.transmittedAt.getTime());
                            //console.log("completed " + url.toString() + " after " + ms.toString() + " ms");
                            this.executing.splice(i, 1);
                            break;
                        }
                    }
                };
                URLQueue.prototype.clearTimedOut = function () {
                    if (!this.timeoutInMS)
                        return;
                    var now = new Date().getTime();
                    for (var i = 0; i < this.executing.length; ++i) {
                        var doomed = this.executing[i];
                        var ms = Math.abs(now - doomed.transmittedAt.getTime());
                        if (ms >= this.timeoutInMS) {
                            console.log("doomed " + doomed.toString() + " after " + ms.toString() + " ms");
                            this.executing.splice(i--, 1);
                        }
                    }
                };
                return URLQueue;
            }());
            DataService = /** @class */ (function () {
                function DataService($http, $q) {
                    var _this = this;
                    this.$http = $http;
                    this.$q = $q;
                    this.getConfiguration = function () { return _this.get('configurationdata'); };
                    this.getDeviceStatus = function () { return _this.get('devicestatus'); };
                    this.getAllCollectors = function () { return _this.get('allcollectors'); };
                    this.getNetworkStatus = function (starting, ending) { return _this.get('networkstatus', DataService.getDatesTuple(starting, ending)); };
                    this.collectNow = function (collectorID) { return _this.post('collectnow', [collectorID]); };
                    this.getProcessHistory = function (id, processName) { return _this.get('processhistory', [id, processName]); };
                    this.getAllProcesses = function (id, starting, ending) { return _this.get('allprocesses', DataService.getIDAndDatesTuple(id, starting, ending)); };
                    this.getAppHistory = function (id, app) { return _this.get('apphistory', [id, app]); };
                    this.getAppChanges = function (deviceID, starting, ending) { return _this.get('xyz', DataService.getIDAndDatesTuple(deviceID, starting, ending)); };
                    // The database name might have some invalid characters, so let's encode it and let the server decode it
                    this.getDatabaseHistory = function (id, databaseName) { return _this.get('databasehistory', [id, btoa(databaseName)]); };
                    this.getServicesData = function (id) { return _this.get('servicesdata', [id]); };
                    this.getDeviceDetails = function (id) { return _this.get('devicedetails', [id]); };
                    this.getMachineData = function (id, parts, starting, ending) { return _this.get('machinedata', DataService.getIDAndDatesAndMachinePartsTuple(id, parts, starting, ending)); };
                    this.getSubReport = function (id, types, starting, ending) { return _this.get('machinesubreport', DataService.getIDAndDAtesAndReportPartsTuple(id, types, starting, ending)); };
                    //console.log("NewDataService.constructor");
                    this.urlQueue = new URLQueue(8, 120);
                }
                // The problem is that Date.toIsoString() always formats the date to UTC, or like this:
                // 2019-02-03T12:23:34.123Z
                // The Z means UTC (zulu)
                // But the dates in the SQLite database are stored as strings, not actual dates, so if we do a query
                // using the zulu time it doesn't quite match up:
                // 2019-02-03T06:23:34.123-06:00 != 2019-02-03T12:23:34.123Z
                // even though those are the same times. The left one is the local
                // time with the offset to GMT, the other is in GMT.
                // The time used in the DB is the C# DateTimeOffset.ToString("o"), which puts the
                // local time with the offset to GMT.
                // So, for consistency, we need to use the same format that DateTimeOffset.ToString("o")
                // uses, but there isn't one built in to JavaScript.
                //
                // Fortunately, moment.format() does exactly what we want. Well, almost. format() doesn't
                // put the milliseconds in there, so we'll have to
                DataService.getIDAndDatesTuple = function (id, starting, ending) {
                    var s = starting == null ? null : moment_1.default(starting).format(DataService.dateFormat);
                    var e = ending == null ? null : moment_1.default(ending).format(DataService.dateFormat);
                    return [id, s, e];
                };
                DataService.getDatesTuple = function (starting, ending) {
                    var s = starting == null ? null : moment_1.default(starting).format(DataService.dateFormat);
                    var e = ending == null ? null : moment_1.default(ending).format(DataService.dateFormat);
                    return [s, e];
                };
                DataService.getIDAndDatesAndMachinePartsTuple = function (id, parts, starting, ending) {
                    var mp = {
                        machineParts: parts
                    };
                    var json = JSON.stringify(mp.machineParts);
                    var t1 = DataService.getIDAndDatesTuple(id, starting, ending);
                    return [t1[0], json, t1[1], t1[2]];
                };
                DataService.getIDAndDAtesAndReportPartsTuple = function (id, types, starting, ending) {
                    var mp = {
                        reportTypes: types
                    };
                    var json = JSON.stringify(mp.reportTypes);
                    var t1 = DataService.getIDAndDatesTuple(id, starting, ending);
                    return [t1[0], json, t1[1], t1[2]];
                };
                DataService.prototype.get = function (method, data) {
                    var url = '/' + method;
                    if (data) {
                        for (var i = 0; i < data.length; ++i)
                            url += '/' + data[i];
                    }
                    // Push the URL into the queue, and it will automatically attempt
                    // to execute it if there aren't too many already executing.
                    var d = this.$q.defer();
                    var u = new URL(url, d, this.$http);
                    this.urlQueue.push(u);
                    return d.promise;
                };
                DataService.prototype.post = function (method, data) {
                    var url = '/' + method + '/' + data;
                    // console.log(url);
                    var d = this.$q.defer();
                    this.$http.post(encodeURI(url), data)
                        .then(function (response) {
                        d.resolve(response.data);
                    })
                        .catch(function (reason) {
                        d.reject(reason);
                    });
                    return d.promise;
                };
                DataService.Factory = function () {
                    var factory = function ($http, $q) {
                        return new DataService($http, $q);
                    };
                    factory.$inject = ['$http', '$q'];
                    return factory;
                };
                // kk is 00-23 hours
                DataService.dateFormat = "YYYY-MM-DDTkk:mm:ss.SSSZ";
                return DataService;
            }());
            exports_35("DataService", DataService);
        }
    };
});
/// <reference types="angular" />
System.register("services/network.service", ["classes/network", "classes/promisekeeper"], function (exports_36, context_36) {
    "use strict";
    var network_2, promisekeeper_3, NetworkService;
    var __moduleName = context_36 && context_36.id;
    return {
        setters: [
            function (network_2_1) {
                network_2 = network_2_1;
            },
            function (promisekeeper_3_1) {
                promisekeeper_3 = promisekeeper_3_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            NetworkService = /** @class */ (function () {
                function NetworkService(ds, devicemanagerService, $interval, $q, config) {
                    this.ds = ds;
                    this.devicemanagerService = devicemanagerService;
                    this.$interval = $interval;
                    this.$q = $q;
                    this.config = config;
                    this.promiseKeeper = new promisekeeper_3.PromiseKeeper($q);
                    var t = this;
                    config.get()
                        .then(function (c) {
                        NetworkService.network = new network_2.Network(t.ds, t.devicemanagerService, t.$interval, c);
                        t.promiseKeeper.resolve(NetworkService.network);
                    });
                    // console.log("NetworkService.constructor");
                }
                NetworkService.prototype.get = function () {
                    var d = this.$q.defer();
                    if (NetworkService.network)
                        d.resolve(NetworkService.network);
                    else
                        this.promiseKeeper.push(d);
                    return d.promise;
                };
                NetworkService.Factory = function () {
                    var factory = function (ds, devicemanagerService, $interval, $q, config) {
                        return new NetworkService(ds, devicemanagerService, $interval, $q, config);
                    };
                    factory.$inject = ['dataService', 'devicemanagerService', '$interval', '$q', 'configurationService'];
                    return factory;
                };
                return NetworkService;
            }());
            exports_36("NetworkService", NetworkService);
        }
    };
});
/// <reference types="angular-cookies" />
/// <reference types="angular-translate" />
System.register("classes/languages", [], function (exports_37, context_37) {
    "use strict";
    var Language, Languages;
    var __moduleName = context_37 && context_37.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular-cookies" />
            /// <reference types="angular-translate" />
            Language = /** @class */ (function () {
                function Language(code, language) {
                    this.code = code.toLowerCase();
                    this.language = language;
                }
                return Language;
            }());
            exports_37("Language", Language);
            Languages = /** @class */ (function () {
                function Languages($translate, $cookies) {
                    this.$translate = $translate;
                    this.$cookies = $cookies;
                    this.languages = [new Language('en', 'English')];
                    var language = "en";
                    var language_cookie = $cookies.get("language");
                    if (language_cookie !== undefined && language_cookie !== null)
                        language = language_cookie;
                    this.selectedLanguage = language;
                    $translate.use(language);
                    //moment.locale(language);
                }
                Languages.prototype.updateLanguages = function (data) {
                    if (!data || !data.languages)
                        return;
                    for (var i = 0; i < data.languages.length; ++i) {
                        if (data.languages[i].isEnabled === true) {
                            var language = new Language(data.languages[i].languageCode, data.languages[i].language);
                            var exists = false;
                            for (var j = 0; exists === false && j < this.languages.length; ++j) {
                                exists = this.languages[j].code === language.code;
                            }
                            if (exists === false)
                                this.languages.push(language);
                        }
                    }
                };
                Languages.prototype.use = function (language) {
                    this.selectedLanguage = language;
                    var now = new Date();
                    var expiration = new Date(now.getFullYear(), now.getMonth() + 1, now.getDate());
                    this.$cookies.put("language", language, { expires: expiration });
                    this.$translate.use(language);
                    //moment.locale(language);
                };
                return Languages;
            }());
            exports_37("Languages", Languages);
        }
    };
});
/// <reference types="angular-cookies" />
/// <reference types="angular-translate" />
System.register("services/languages.service", ["classes/languages"], function (exports_38, context_38) {
    "use strict";
    var languages_1, LanguagesService;
    var __moduleName = context_38 && context_38.id;
    return {
        setters: [
            function (languages_1_1) {
                languages_1 = languages_1_1;
            }
        ],
        execute: function () {/// <reference types="angular-cookies" />
            /// <reference types="angular-translate" />
            LanguagesService = /** @class */ (function () {
                function LanguagesService($translate, $cookies, $q) {
                    this.$translate = $translate;
                    this.$cookies = $cookies;
                    this.$q = $q;
                    LanguagesService.languages = new languages_1.Languages($translate, $cookies);
                    // console.log("LanguagesService.constructor");
                }
                LanguagesService.prototype.get = function () {
                    var d = this.$q.defer();
                    d.resolve(LanguagesService.languages);
                    return d.promise;
                };
                LanguagesService.Factory = function () {
                    var factory = function ($translate, $cookies, $q) {
                        return new LanguagesService($translate, $cookies, $q);
                    };
                    factory.$inject = ['$translate', '$cookies', '$q'];
                    return factory;
                };
                return LanguagesService;
            }());
            exports_38("LanguagesService", LanguagesService);
        }
    };
});
/// <reference types="angular" />
System.register("controllers/overview.controller", ["charts/chartjs"], function (exports_39, context_39) {
    "use strict";
    var chartjs_10, OverviewController;
    var __moduleName = context_39 && context_39.id;
    return {
        setters: [
            function (chartjs_10_1) {
                chartjs_10 = chartjs_10_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            OverviewController = /** @class */ (function () {
                function OverviewController(devicemanagerService) {
                    this.devicemanagerService = devicemanagerService;
                    var vm = this;
                    this.networkChartSettings = new chartjs_10.ChartJSSettings("Response time in ms", 100);
                    this.networkChartSettings.displayLegend = false;
                    devicemanagerService.get()
                        .then(function (deviceManager) {
                        vm.deviceManager = deviceManager;
                        vm.devices = vm.deviceManager.devices;
                        vm.groups = vm.deviceManager.groups;
                        //deviceManager.closeAllPanels();
                    });
                }
                OverviewController.Factory = function () {
                    var factory = function (devicemanagerService) {
                        return new OverviewController(devicemanagerService);
                    };
                    factory.$inject = ['devicemanagerService'];
                    return factory;
                };
                return OverviewController;
            }());
            exports_39("OverviewController", OverviewController);
        }
    };
});
/// <reference types="angular" />
System.register("controllers/header.controller", [], function (exports_40, context_40) {
    "use strict";
    var HeaderController;
    var __moduleName = context_40 && context_40.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            HeaderController = /** @class */ (function () {
                function HeaderController($route, devicemanagerService, languagesService, configurationService) {
                    this.$route = $route;
                    this.devicemanagerService = devicemanagerService;
                    this.languagesService = languagesService;
                    this.configurationService = configurationService;
                    this.onToggleDevicesSidebar = function () {
                        this.devicesSidebar.isOpen = !this.devicesSidebar.isOpen;
                    };
                    this.devices = [];
                    this.groups = [];
                    this.languages = [];
                    this.windowsDevices = [];
                    this.devicesSidebar = {
                        isOpen: false
                    };
                    var t = this;
                    devicemanagerService.get()
                        .then(function (deviceManager) {
                        t.devices = deviceManager.devices;
                        t.groups = deviceManager.groups;
                        for (var i = 0; i < t.devices.length; ++i) {
                            var d = t.devices[i];
                            if (d.isWindowsDevice())
                                t.windowsDevices.push(d);
                        }
                        for (var i = 0; i < t.groups.length; ++i) {
                            var g = t.groups[i];
                            for (var j = 0; j < g.devices.length; ++j) {
                                var d = g.devices[j];
                                if (d.isWindowsDevice())
                                    t.windowsDevices.push(d);
                            }
                        }
                        t.windowsDevices.sort(function (a, b) {
                            return a.name.localeCompare(b.name);
                        });
                    });
                    languagesService.get()
                        .then(function (languages) {
                        t.languages = languages.languages;
                    });
                    configurationService.get()
                        .then(function (config) {
                        t.siteName = config.siteName;
                    });
                }
                HeaderController.prototype.useLanguage = function (lang) {
                    this.languagesService.get()
                        .then(function (ls) {
                        ls.use(lang);
                    });
                };
                HeaderController.Factory = function () {
                    var factory = function ($route, devicemanagerService, languagesService, configurationService) {
                        return new HeaderController($route, devicemanagerService, languagesService, configurationService);
                    };
                    factory.$inject = ['$route', 'devicemanagerService', 'languagesService', 'configurationService'];
                    return factory;
                };
                return HeaderController;
            }());
            exports_40("HeaderController", HeaderController);
        }
    };
});
System.register("charts/chartbridgefactory", ["charts/chartjs", "classes/cpu", "classes/database", "classes/memory", "classes/network", "classes/nic", "classes/processes", "disk/disk", "classes/ups"], function (exports_41, context_41) {
    "use strict";
    var chartjs_11, cpu_2, database_2, memory_2, network_3, nic_2, processes_2, disk_2, ups_2, ChartJSChartBridgeFactory;
    var __moduleName = context_41 && context_41.id;
    return {
        setters: [
            function (chartjs_11_1) {
                chartjs_11 = chartjs_11_1;
            },
            function (cpu_2_1) {
                cpu_2 = cpu_2_1;
            },
            function (database_2_1) {
                database_2 = database_2_1;
            },
            function (memory_2_1) {
                memory_2 = memory_2_1;
            },
            function (network_3_1) {
                network_3 = network_3_1;
            },
            function (nic_2_1) {
                nic_2 = nic_2_1;
            },
            function (processes_2_1) {
                processes_2 = processes_2_1;
            },
            function (disk_2_1) {
                disk_2 = disk_2_1;
            },
            function (ups_2_1) {
                ups_2 = ups_2_1;
            }
        ],
        execute: function () {
            ChartJSChartBridgeFactory = /** @class */ (function () {
                function ChartJSChartBridgeFactory() {
                    this.factory = new chartjs_11.ChartJSChartFactory();
                }
                ChartJSChartBridgeFactory.prototype.createChartBridge = function (chart, settings) {
                    var b = null;
                    if (chart instanceof cpu_2.CPUData)
                        b = new cpu_2.CPUChartBridge(chart, settings, this.factory);
                    if (chart instanceof database_2.DatabaseHistory)
                        b = new database_2.DatabaseHistoryChartBridge(chart, this.factory);
                    if (chart instanceof memory_2.Memory)
                        b = new memory_2.MemoryChartBridge(chart, settings, this.factory);
                    if (chart instanceof network_3.NetworkStatus)
                        b = new network_3.NetworkChartBridge(chart, settings, this.factory);
                    if (chart instanceof nic_2.NICData)
                        b = new nic_2.NICChartBridge(chart, settings, this.factory);
                    if (chart instanceof processes_2.ProcessHistory)
                        b = new processes_2.ProcessHistoryChartBridge(chart, this.factory);
                    if (chart instanceof disk_2.DiskUsage)
                        b = new disk_2.DiskUsageChartBridge(chart, settings, this.factory);
                    if (chart instanceof disk_2.DiskPerformance)
                        b = new disk_2.DiskPerformanceChartBridge(chart, this.factory);
                    if (chart instanceof ups_2.UPSStatus)
                        b = new ups_2.UPSChartBridge(chart, this.factory);
                    return b;
                };
                ChartJSChartBridgeFactory.prototype.createChartSettings = function (yaxis, height) {
                    if (height === void 0) { height = 125; }
                    return new chartjs_11.ChartJSSettings(yaxis, height);
                };
                return ChartJSChartBridgeFactory;
            }());
            exports_41("ChartJSChartBridgeFactory", ChartJSChartBridgeFactory);
        }
    };
});
/// <reference types="angular" />
System.register("charts/chartbridgefactory.service", ["charts/chartbridgefactory"], function (exports_42, context_42) {
    "use strict";
    var chartbridgefactory_1, ChartBridgeFactoryService;
    var __moduleName = context_42 && context_42.id;
    return {
        setters: [
            function (chartbridgefactory_1_1) {
                chartbridgefactory_1 = chartbridgefactory_1_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            ChartBridgeFactoryService = /** @class */ (function () {
                function ChartBridgeFactoryService() {
                }
                ChartBridgeFactoryService.prototype.$get = function () {
                    return new chartbridgefactory_1.ChartJSChartBridgeFactory();
                };
                ChartBridgeFactoryService.Factory = function () {
                    var factory = function () {
                        return new ChartBridgeFactoryService();
                    };
                    return factory;
                };
                return ChartBridgeFactoryService;
            }());
            exports_42("ChartBridgeFactoryService", ChartBridgeFactoryService);
        }
    };
});
/// <reference types="angular" />
/// <reference types="angular-ui-bootstrap" />
/// <reference types="angular-route" />
System.register("controllers/windowsMachine.controller", ["classes/machine"], function (exports_43, context_43) {
    "use strict";
    var machine_2, WindowsMachineController;
    var __moduleName = context_43 && context_43.id;
    return {
        setters: [
            function (machine_2_1) {
                machine_2 = machine_2_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            /// <reference types="angular-ui-bootstrap" />
            /// <reference types="angular-route" />
            WindowsMachineController = /** @class */ (function () {
                function WindowsMachineController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) {
                    this.dataService = dataService;
                    this.$routeParams = $routeParams;
                    this.$scope = $scope;
                    this.devicemanagerService = devicemanagerService;
                    this.chartBridgeFactoryService = chartBridgeFactoryService;
                    this.$uibModal = $uibModal;
                    this.config = config;
                    this.windowsMachineScope = this.$scope;
                    this.windowsMachineScope.smartDisk = null;
                    this.id = $routeParams.id;
                    this.daysToRetrieveChoices = [15, 30, 60, 90, 120, 150, 180];
                    var fs = this.chartBridgeFactoryService.$get();
                    this.chartSettings = {
                        cpu: fs.createChartSettings("% Used"),
                        database: fs.createChartSettings("MB"),
                        memory: fs.createChartSettings("GB Used"),
                        nic: fs.createChartSettings("% Used"),
                        processes: fs.createChartSettings("CPU %", 250),
                        ups: fs.createChartSettings("Running on battery"),
                        diskUsage: fs.createChartSettings("GB Used"),
                        diskPerformance: fs.createChartSettings("% Time reading/writing"),
                    };
                    this.chartSettings.database.chartSizeInGB = false;
                    this.chartSettings.processes.yaxis2 = "Memory in MB";
                    this.chartSettings.processes.valueRange = [0, 100];
                    this.EMachineParts = machine_2.EMachineParts;
                    this.changeDaysToRetrieve(15);
                }
                WindowsMachineController.prototype.changeDaysToRetrieve = function (days) {
                    var _this = this;
                    this.daysToRetrieve = days;
                    var t = this;
                    this.config.get()
                        .then(function (c) {
                        _this.devicemanagerService.get()
                            .then(function (dm) {
                            t.device = dm.findDeviceFromID(t.id);
                            if (t.device) {
                                var now = new Date();
                                var startDate = new Date(c.mostRecentData.getTime() - (days * 24 * 60 * 60 * 1000));
                                t.machine = new machine_2.Machine(t.device, t.dataService, startDate);
                            }
                        });
                    });
                };
                WindowsMachineController.prototype.getMoreCPU = function () {
                    this.getMore(machine_2.EMachineParts.CPU);
                };
                WindowsMachineController.prototype.getMoreDiskUsage = function () {
                    this.getMore(machine_2.EMachineParts.DiskUsage);
                };
                WindowsMachineController.prototype.getMoreDiskPerformance = function () {
                    this.getMore(machine_2.EMachineParts.DiskPerformance);
                };
                WindowsMachineController.prototype.getMoreApplicationErrors = function () {
                    this.getMore(machine_2.EMachineParts.ApplicationErrors);
                };
                WindowsMachineController.prototype.getMoreSystemErrors = function () {
                    this.getMore(machine_2.EMachineParts.SystemErrors);
                };
                WindowsMachineController.prototype.getMoreMemory = function () {
                    this.getMore(machine_2.EMachineParts.Memory);
                };
                WindowsMachineController.prototype.getMoreNIC = function () {
                    this.getMore(machine_2.EMachineParts.NIC);
                };
                WindowsMachineController.prototype.getMoreUPS = function () {
                    this.getMore(machine_2.EMachineParts.UPS);
                };
                WindowsMachineController.prototype.getMore = function (part) {
                    if (this.machine) {
                        this.machine.loading[part] = true;
                        this.machine.getMoreDays(this.daysToRetrieve, part);
                    }
                };
                WindowsMachineController.prototype.showSMART = function (disk) {
                    if (!disk)
                        return;
                    var t = this;
                    t.windowsMachineScope.smartDisk = disk.smart;
                    t.smartModal = this.$uibModal.open({
                        templateUrl: 'app/disk/smart.modal.html',
                        //controller: t.factory(),
                        controllerAs: 'vm',
                        scope: t.windowsMachineScope,
                    });
                };
                return WindowsMachineController;
            }());
            exports_43("WindowsMachineController", WindowsMachineController);
        }
    };
});
System.register("controllers/server.controller", ["controllers/windowsMachine.controller"], function (exports_44, context_44) {
    "use strict";
    var windowsMachine_controller_1, ServerController;
    var __moduleName = context_44 && context_44.id;
    return {
        setters: [
            function (windowsMachine_controller_1_1) {
                windowsMachine_controller_1 = windowsMachine_controller_1_1;
            }
        ],
        execute: function () {
            ServerController = /** @class */ (function (_super) {
                __extends(ServerController, _super);
                function ServerController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) {
                    var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) || this;
                    _this.dataService = dataService;
                    _this.$routeParams = $routeParams;
                    _this.$scope = $scope;
                    _this.devicemanagerService = devicemanagerService;
                    _this.chartBridgeFactoryService = chartBridgeFactoryService;
                    _this.$uibModal = $uibModal;
                    _this.config = config;
                    // console.log("ServerController constructor");
                    _this.factory = ServerController.Factory;
                    return _this;
                }
                ServerController.Factory = function () {
                    var factory = function (dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) {
                        return new ServerController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config);
                    };
                    factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'chartBridgeFactoryService', '$uibModal', 'configurationService'];
                    return factory;
                };
                return ServerController;
            }(windowsMachine_controller_1.WindowsMachineController));
            exports_44("ServerController", ServerController);
        }
    };
});
System.register("controllers/workstation.controller", ["controllers/windowsMachine.controller", "charts/chartjs"], function (exports_45, context_45) {
    "use strict";
    var windowsMachine_controller_2, chartjs_12, WorkstationController;
    var __moduleName = context_45 && context_45.id;
    return {
        setters: [
            function (windowsMachine_controller_2_1) {
                windowsMachine_controller_2 = windowsMachine_controller_2_1;
            },
            function (chartjs_12_1) {
                chartjs_12 = chartjs_12_1;
            }
        ],
        execute: function () {
            WorkstationController = /** @class */ (function (_super) {
                __extends(WorkstationController, _super);
                function WorkstationController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, network, $uibModal, config) {
                    var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config) || this;
                    _this.dataService = dataService;
                    _this.$routeParams = $routeParams;
                    _this.$scope = $scope;
                    _this.devicemanagerService = devicemanagerService;
                    _this.chartBridgeFactoryService = chartBridgeFactoryService;
                    _this.network = network;
                    _this.$uibModal = $uibModal;
                    _this.config = config;
                    //console.log("WorkstationController constructor");
                    _this.networkChartSettings = new chartjs_12.ChartJSSettings("Response time in ms", 125);
                    var t = _this;
                    if (_this.device) {
                        _this.network.get()
                            .then(function (n) {
                            var ns = n.getNetworkStatusFromID(_this.device.id);
                            if (ns)
                                t.device.networkStatus = ns;
                        });
                    }
                    _this.factory = WorkstationController.Factory;
                    return _this;
                }
                WorkstationController.Factory = function () {
                    var factory = function (dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, network, $uibModal, config) {
                        return new WorkstationController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, network, $uibModal, config);
                    };
                    factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'chartBridgeFactoryService', 'networkService', '$uibModal', 'configurationService'];
                    return factory;
                };
                return WorkstationController;
            }(windowsMachine_controller_2.WindowsMachineController));
            exports_45("WorkstationController", WorkstationController);
        }
    };
});
System.register("controllers/datacollection.controller", [], function (exports_46, context_46) {
    "use strict";
    var DataCollectionController;
    var __moduleName = context_46 && context_46.id;
    return {
        setters: [],
        execute: function () {
            DataCollectionController = /** @class */ (function () {
                function DataCollectionController(dataService, $rootScope, $scope, devicemanagerService) {
                    this.dataService = dataService;
                    this.$rootScope = $rootScope;
                    this.$scope = $scope;
                    this.devicemanagerService = devicemanagerService;
                    var t = this;
                    this.devices = [];
                    devicemanagerService.get()
                        .then(function (deviceManager) {
                        t.devices = deviceManager.getDevicesForDataCollection();
                        t.deviceManager = deviceManager;
                    });
                }
                DataCollectionController.prototype.onCollectNow = function (collectorID) {
                    this.devicemanagerService.get()
                        .then(function (deviceManager) {
                        deviceManager.collectNow(collectorID);
                    });
                };
                DataCollectionController.prototype.onCollectAll = function (deviceID) {
                    this.devicemanagerService.get()
                        .then(function (deviceManager) {
                        deviceManager.collectAll(deviceID);
                    });
                };
                DataCollectionController.Factory = function () {
                    var factory = function (dataService, $rootScope, $scope, devicemanagerService) {
                        return new DataCollectionController(dataService, $rootScope, $scope, devicemanagerService);
                    };
                    factory.$inject = ['dataService', '$rootScope', '$scope', 'devicemanagerService'];
                    return factory;
                };
                return DataCollectionController;
            }());
            exports_46("DataCollectionController", DataCollectionController);
        }
    };
});
/// <reference types="angular" />
System.register("controllers/about.controller", [], function (exports_47, context_47) {
    "use strict";
    var AboutController;
    var __moduleName = context_47 && context_47.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            AboutController = /** @class */ (function () {
                function AboutController(configurationService) {
                    var _this = this;
                    this.configurationService = configurationService;
                    this.attributions = [
                        {
                            name: "JSON.net",
                            nameURL: "http://www.newtonsoft.com/json",
                            license: "MIT",
                        },
                        {
                            name: "AngularJS",
                            nameURL: "https://angularjs.org/",
                            license: "MIT",
                        },
                        {
                            name: "dirPagination.js",
                            nameURL: "https://github.com/michaelbromley/angularUtils",
                            license: "MIT",
                        },
                        {
                            name: "jQuery",
                            nameURL: "https://jquery.com/",
                            license: "MIT",
                        },
                        {
                            name: "AngularUI",
                            nameURL: "https://github.com/angular-ui",
                            license: "MIT",
                        },
                        {
                            name: "angular-translate",
                            nameURL: "https://github.com/angular-translate/angular-translate",
                            license: "MIT",
                        },
                        {
                            name: "angular-chart.js",
                            nameURL: "http://jtblin.github.io/angular-chart.js/",
                            license: "BSD",
                        },
                        {
                            name: "Bootstrap",
                            nameURL: "https://github.com/twbs/bootstrap",
                            license: "MIT",
                        },
                        {
                            name: "Chart.js",
                            nameURL: "https://github.com/chartjs/Chart.js",
                            license: "MIT",
                        },
                        {
                            name: "RequireJS",
                            nameURL: "https://github.com/requirejs/requirejs",
                            license: "MIT",
                        },
                        {
                            name: "Moment.js",
                            nameURL: "https://momentjs.com/",
                            license: "MIT",
                        },
                    ];
                    this.attributions.sort(function (a, b) {
                        return a.name.localeCompare(b.name);
                    });
                    var t = this;
                    configurationService.get()
                        .then(function (config) {
                        _this.softwareVersion = config.softwareVersion;
                        _this.softwareVersionShort = config.softwareVersionShort;
                    });
                    this.date = new Date();
                }
                AboutController.Factory = function () {
                    var factory = function (configurationService) {
                        return new AboutController(configurationService);
                    };
                    factory.$inject = ['configurationService'];
                    return factory;
                };
                return AboutController;
            }());
            exports_47("AboutController", AboutController);
        }
    };
});
/// <reference types="angular" />
System.register("controllers/admin.controller", [], function (exports_48, context_48) {
    "use strict";
    var AdminController;
    var __moduleName = context_48 && context_48.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            AdminController = /** @class */ (function () {
                function AdminController() {
                }
                AdminController.Factory = function () {
                    var factory = function () {
                        return new AdminController();
                    };
                    factory.$inject = [];
                    return factory;
                };
                return AdminController;
            }());
            exports_48("AdminController", AdminController);
        }
    };
});
/// <reference types="angular" />
System.register("controllers/help.controller", [], function (exports_49, context_49) {
    "use strict";
    var HelpController;
    var __moduleName = context_49 && context_49.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            HelpController = /** @class */ (function () {
                function HelpController() {
                }
                HelpController.Factory = function () {
                    var factory = function () {
                        return new HelpController();
                    };
                    factory.$inject = [];
                    return factory;
                };
                return HelpController;
            }());
            exports_49("HelpController", HelpController);
        }
    };
});
/// <reference types="angular" />
/// <reference types="angular-route" />
System.register("controllers/group.controller", ["charts/chartjs"], function (exports_50, context_50) {
    "use strict";
    var chartjs_13, GroupController;
    var __moduleName = context_50 && context_50.id;
    return {
        setters: [
            function (chartjs_13_1) {
                chartjs_13 = chartjs_13_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            /// <reference types="angular-route" />
            GroupController = /** @class */ (function () {
                function GroupController($routeParams, devicemanagerService) {
                    this.$routeParams = $routeParams;
                    this.devicemanagerService = devicemanagerService;
                    this.id = parseInt($routeParams.id);
                    this.chartSettings = new chartjs_13.ChartJSSettings("Response time in ms", 125);
                    var t = this;
                    devicemanagerService.get()
                        .then(function (dm) {
                        t.group = dm.findGroup(t.id);
                    });
                }
                GroupController.Factory = function () {
                    var factory = function ($routeParams, devicemanagerService) {
                        return new GroupController($routeParams, devicemanagerService);
                    };
                    factory.$inject = ['$routeParams', 'devicemanagerService'];
                    return factory;
                };
                return GroupController;
            }());
            exports_50("GroupController", GroupController);
        }
    };
});
/// <reference types="angular" />
/// <reference types="angular-route" />
/// <reference types="angular-ui-bootstrap" />
System.register("reports/reports.controller", ["reports/report", "classes/utilities", "enums/devicetypes.enum", "charts/chartjs"], function (exports_51, context_51) {
    "use strict";
    var report_1, utilities_2, devicetypes_enum_2, chartjs_14, download, ReportsController, SiteReportController, MachineReportController, NetworkReportController, CASLoadReportController, IssuesReportController, SiteConfigurationReportController;
    var __moduleName = context_51 && context_51.id;
    return {
        setters: [
            function (report_1_1) {
                report_1 = report_1_1;
            },
            function (utilities_2_1) {
                utilities_2 = utilities_2_1;
            },
            function (devicetypes_enum_2_1) {
                devicetypes_enum_2 = devicetypes_enum_2_1;
            },
            function (chartjs_14_1) {
                chartjs_14 = chartjs_14_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            /// <reference types="angular-route" />
            /// <reference types="angular-ui-bootstrap" />
            ReportsController = /** @class */ (function () {
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
            SiteReportController = /** @class */ (function (_super) {
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
                            if (device.type == devicetypes_enum_2.EDeviceTypes.Server || device.type == devicetypes_enum_2.EDeviceTypes.Workstation) {
                                var s = {
                                    deviceID: device.id,
                                    device: device,
                                    type: device.type == devicetypes_enum_2.EDeviceTypes.Server ? report_1.EReportTypes.Server : report_1.EReportTypes.Workstation,
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
                    this.startDate = utilities_2.Utilities.toMidnight(this.popupStartDate);
                    this.endDateOptions.minDate = this.popupStartDate;
                    // We want the end date to encompass the entire day, so that means
                    // we want to add 1 day to the date to get it to midnight of the next day.
                    this.endDate = new Date(utilities_2.Utilities.toMidnight(this.popupEndDate).getTime() + (24 * 60 * 60 * 1000));
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
            exports_51("SiteReportController", SiteReportController);
            MachineReportController = /** @class */ (function (_super) {
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
            exports_51("MachineReportController", MachineReportController);
            NetworkReportController = /** @class */ (function (_super) {
                __extends(NetworkReportController, _super);
                function NetworkReportController(dataService, $routeParams, $scope, devicemanagerService, network, configurationService) {
                    var _this = _super.call(this, dataService, $routeParams, $scope, devicemanagerService, network, configurationService, report_1.EReportTypes.Network, "Network Report") || this;
                    _this.dataService = dataService;
                    _this.$routeParams = $routeParams;
                    _this.$scope = $scope;
                    _this.devicemanagerService = devicemanagerService;
                    _this.network = network;
                    _this.configurationService = configurationService;
                    _this.networkChartSettings = new chartjs_14.ChartJSSettings("", 75);
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
            exports_51("NetworkReportController", NetworkReportController);
            CASLoadReportController = /** @class */ (function (_super) {
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
            exports_51("CASLoadReportController", CASLoadReportController);
            IssuesReportController = /** @class */ (function (_super) {
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
            exports_51("IssuesReportController", IssuesReportController);
            SiteConfigurationReportController = /** @class */ (function (_super) {
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
            exports_51("SiteConfigurationReportController", SiteConfigurationReportController);
        }
    };
});
/// <reference types="angular" />
/// <reference types="angular-route" />
System.register("controllers/device.controller", ["charts/chartjs"], function (exports_52, context_52) {
    "use strict";
    var chartjs_15, DeviceController;
    var __moduleName = context_52 && context_52.id;
    return {
        setters: [
            function (chartjs_15_1) {
                chartjs_15 = chartjs_15_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            /// <reference types="angular-route" />
            DeviceController = /** @class */ (function () {
                function DeviceController($routeParams, deviceManager) {
                    this.$routeParams = $routeParams;
                    this.deviceManager = deviceManager;
                    this.id = parseInt($routeParams.id);
                    this.chartSettings = new chartjs_15.ChartJSSettings("Response time in ms", 125);
                    var t = this;
                    deviceManager.get()
                        .then(function (dm) {
                        t.device = dm.findDeviceFromID(t.id);
                    });
                }
                DeviceController.Factory = function () {
                    var factory = function ($routeParams, deviceManager) {
                        return new DeviceController($routeParams, deviceManager);
                    };
                    factory.$inject = ['$routeParams', 'devicemanagerService'];
                    return factory;
                };
                return DeviceController;
            }());
            exports_52("DeviceController", DeviceController);
        }
    };
});
/// <reference types="angular" />
System.register("controllers/networkhistory.controller", ["charts/chartjs"], function (exports_53, context_53) {
    "use strict";
    var chartjs_16, NetworkHistoryController;
    var __moduleName = context_53 && context_53.id;
    return {
        setters: [
            function (chartjs_16_1) {
                chartjs_16 = chartjs_16_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            NetworkHistoryController = /** @class */ (function () {
                function NetworkHistoryController(devicemanagerService, networkService) {
                    this.devicemanagerService = devicemanagerService;
                    this.networkService = networkService;
                    this.devices = [];
                    this.groups = [];
                    this.chartSettings = new chartjs_16.ChartJSSettings("Response time in ms", 100);
                    this.chartSettings.displayLegend = false;
                    var t = this;
                    devicemanagerService.get()
                        .then(function (dm) {
                        t.devices = dm.devices;
                        t.groups = dm.groups;
                    });
                }
                NetworkHistoryController.prototype.getEarlierRange = function () {
                    this.networkService.get()
                        .then(function (n) {
                        n.getEarlierRange();
                    });
                };
                NetworkHistoryController.Factory = function () {
                    var factory = function (devicemanagerService, networkService) {
                        return new NetworkHistoryController(devicemanagerService, networkService);
                    };
                    factory.$inject = ['devicemanagerService', 'networkService'];
                    return factory;
                };
                return NetworkHistoryController;
            }());
            exports_53("NetworkHistoryController", NetworkHistoryController);
        }
    };
});
System.register("charts/graph.directive", [], function (exports_54, context_54) {
    "use strict";
    var GraphDirective;
    var __moduleName = context_54 && context_54.id;
    return {
        setters: [],
        execute: function () {
            GraphDirective = /** @class */ (function () {
                function GraphDirective($window, $rootScope, chartBridgeFactoryService) {
                    var _this = this;
                    this.$window = $window;
                    this.$rootScope = $rootScope;
                    this.chartBridgeFactoryService = chartBridgeFactoryService;
                    this.restrict = 'E';
                    this.scope = {
                        data: '<',
                        settings: '<'
                    };
                    this.template = '<canvas></canvas>';
                    this.link = function (scope, element, attrs) {
                        var context = {
                            element: element,
                            rootScope: _this.$rootScope,
                            scope: scope,
                            window: _this.$window
                        };
                        //let start: number = new Date().getTime();
                        var f = _this.chartBridgeFactoryService.$get();
                        var cb = f.createChartBridge(scope.data, scope.settings);
                        if (cb)
                            cb.makeChart(context);
                        //let duration = new Date().getTime() - start;
                        //console.log("makeChart: " + duration.toString() + " ms");
                    };
                }
                GraphDirective.Factory = function () {
                    var factory = function ($window, $rootScope, chartBridgeService) {
                        return new GraphDirective($window, $rootScope, chartBridgeService);
                    };
                    factory.$inject = ['$window', '$rootScope', 'chartBridgeFactoryService'];
                    return factory;
                };
                return GraphDirective;
            }());
            exports_54("GraphDirective", GraphDirective);
        }
    };
});
/// <reference types="angular" />
System.register("reports/networktable.directive", [], function (exports_55, context_55) {
    "use strict";
    var NetworkTableDirective;
    var __moduleName = context_55 && context_55.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            // Pass a NetworkStatus[] to the directive in the 'data' attribute.
            // Pass an optional IChartSettings to the chart-settings attribute,
            // if you want a chart displayed.
            NetworkTableDirective = /** @class */ (function () {
                function NetworkTableDirective() {
                    this.restrict = 'E';
                    this.scope = {
                        data: '=',
                        chartSettings: '='
                    };
                    this.templateUrl = 'app/reports/networktable.partial.html';
                    this.link = function (scope, element, attrs) {
                    };
                }
                NetworkTableDirective.Factory = function () {
                    var factory = function () {
                        return new NetworkTableDirective();
                    };
                    return factory;
                };
                return NetworkTableDirective;
            }());
            exports_55("NetworkTableDirective", NetworkTableDirective);
        }
    };
});
/// <reference types="angular" />
System.register("reports/machine.snapshot.directive", [], function (exports_56, context_56) {
    "use strict";
    var MachineSnapshotDirective;
    var __moduleName = context_56 && context_56.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            // Pass a MachineReport to the directive in the 'data' attribute.
            MachineSnapshotDirective = /** @class */ (function () {
                function MachineSnapshotDirective() {
                    this.restrict = 'E';
                    this.scope = {
                        data: '=',
                    };
                    this.templateUrl = 'app/reports/machine.snapshot.partial.html';
                    this.link = function (scope, element, attrs) {
                    };
                    // console.log("MachineSnapshotDirective.constructor");
                }
                MachineSnapshotDirective.Factory = function () {
                    var factory = function () {
                        return new MachineSnapshotDirective();
                    };
                    return factory;
                };
                return MachineSnapshotDirective;
            }());
            exports_56("MachineSnapshotDirective", MachineSnapshotDirective);
        }
    };
});
/// <reference types="angular" />
System.register("reports/datablock.directive", [], function (exports_57, context_57) {
    "use strict";
    var DataBlockDirective;
    var __moduleName = context_57 && context_57.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            DataBlockDirective = /** @class */ (function () {
                function DataBlockDirective() {
                    this.restrict = 'E';
                    this.scope = {
                        heading: '@',
                        line1: '@',
                        line2: '@?'
                    };
                    this.templateUrl = 'app/reports/datablock.partial.html';
                    this.link = function (scope, element, attrs) {
                    };
                    this.replace = false;
                    // console.log("DataBlockDirective");
                }
                DataBlockDirective.Factory = function () {
                    var factory = function () {
                        return new DataBlockDirective();
                    };
                    return factory;
                };
                return DataBlockDirective;
            }());
            exports_57("DataBlockDirective", DataBlockDirective);
        }
    };
});
System.register("reports/print.directive", [], function (exports_58, context_58) {
    "use strict";
    var PrintDirective;
    var __moduleName = context_58 && context_58.id;
    return {
        setters: [],
        execute: function () {
            PrintDirective = /** @class */ (function () {
                function PrintDirective() {
                    var _this = this;
                    this.restrict = 'A';
                    this.scope = {
                        printElementId: '@'
                    };
                    this.link = function (scope, button, attrs) {
                        var t = _this;
                        button.on('click', function () {
                            var d = document.getElementById(scope.printElementId);
                            if (d)
                                t.printElement(d);
                        });
                        window.onafterprint = function () {
                            t.parentElement.appendChild(t.originalElement);
                        };
                    };
                    // If there's a 'printSection' div defined in the document, use it.
                    // Otherwise, create one and append it to the document.
                    this.printSection = document.getElementById('printSection');
                    if (!this.printSection) {
                        this.printSection = document.createElement('div');
                        this.printSection.id = 'printSection';
                        document.body.appendChild(this.printSection);
                    }
                }
                PrintDirective.prototype.printElement = function (element) {
                    // Move the element to the printSection so the @media CSS things
                    // will work right.
                    // We can't clone the original element like the original code does
                    // because the graphs don't show up right. Instead, let's just
                    // move the element to the printSection element, then when the
                    // printing has completed it will be moved back.
                    // We just have to keep track of the original parent so it can be
                    // moved back properly.
                    this.originalElement = element;
                    this.parentElement = element.parentElement;
                    this.printSection.appendChild(element);
                    window.print();
                };
                PrintDirective.Factory = function () {
                    var factory = function () {
                        return new PrintDirective();
                    };
                    return factory;
                };
                return PrintDirective;
            }());
            exports_58("PrintDirective", PrintDirective);
            // Found at: http://blogs.microsoft.co.il/gilf/2014/08/09/building-a-simple-angularjs-print-directive/
            //function printDirective() {
            //    var printSection = document.getElementById('printSection');
            //    // if there is no printing section, create one
            //    if (!printSection) {
            //        printSection = document.createElement('div');
            //        printSection.id = 'printSection';
            //        document.body.appendChild(printSection);
            //    }
            //    function link(scope, element, attrs) {
            //        element.on('click', function () {
            //            var elemToPrint = document.getElementById(attrs.printElementId);
            //            if (elemToPrint) {
            //                printElement(elemToPrint);
            //            }
            //        });
            //        window.onafterprint = function () {
            //            // clean the print section before adding new content
            //            printSection.innerHTML = '';
            //        }
            //    }
            //    function printElement(elem) {
            //        // clones the element you want to print
            //        var domClone = elem.cloneNode(true);
            //        printSection.appendChild(domClone);
            //        window.print();
            //    }
            //    return {
            //        link: link,
            //        restrict: 'A'
            //    };
            //}
        }
    };
});
/// <reference types="angular" />
System.register("directives/deviceinfo.directive", [], function (exports_59, context_59) {
    "use strict";
    var DeviceInfoDirective;
    var __moduleName = context_59 && context_59.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            DeviceInfoDirective = /** @class */ (function () {
                function DeviceInfoDirective() {
                    this.restrict = 'E';
                    this.scope = {
                        device: '<',
                        networkChartSettings: '<',
                        //isOpen: '=?',
                    };
                    this.templateUrl = 'app/views/partials/deviceinfo.partial.html';
                    this.link = function (scope, element, attrs) {
                    };
                    console.log("DeviceInfoDirective.constructor");
                }
                DeviceInfoDirective.Factory = function () {
                    var factory = function () {
                        return new DeviceInfoDirective();
                    };
                    return factory;
                };
                return DeviceInfoDirective;
            }());
            exports_59("DeviceInfoDirective", DeviceInfoDirective);
        }
    };
});
/// <reference types="angular" />
System.register("directives/overview.devicealert.directive", [], function (exports_60, context_60) {
    "use strict";
    var OverviewDeviceAlertDirective;
    var __moduleName = context_60 && context_60.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            OverviewDeviceAlertDirective = /** @class */ (function () {
                function OverviewDeviceAlertDirective() {
                    this.restrict = 'E';
                    this.scope = {
                        device: '<',
                    };
                    this.templateUrl = 'app/views/partials/overview.devicealert.partial.html';
                    this.link = function (scope, element, attrs) {
                    };
                    //console.log("OverviewDeviceAlertDirective.constructor");
                }
                OverviewDeviceAlertDirective.Factory = function () {
                    var factory = function () {
                        return new OverviewDeviceAlertDirective();
                    };
                    return factory;
                };
                return OverviewDeviceAlertDirective;
            }());
            exports_60("OverviewDeviceAlertDirective", OverviewDeviceAlertDirective);
        }
    };
});
/// <reference types="angular" />
System.register("directives/collector.brick.directive", [], function (exports_61, context_61) {
    "use strict";
    var CollectorBrickDirective, CPUBrickDirective, MemoryBrickDirective, DiskBrickDirective, DiskSpeedBrickDirective, NICBrickDirective, ServicesBrickDirective, DatabaseBrickDirective, ProcessesBrickDirective, ApplicationsBrickDirective, UPSBrickDirective, SystemErrorsBrickDirective, ApplicationErrorsBrickDirective;
    var __moduleName = context_61 && context_61.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            CollectorBrickDirective = /** @class */ (function () {
                function CollectorBrickDirective(templateUrl) {
                    this.templateUrl = templateUrl;
                    this.restrict = 'E';
                    this.scope = {
                        vm: '<'
                    };
                    //templateUrl = 'app/views/partials/cpu.partial.html';
                    this.link = function (scope, element, attrs) {
                    };
                }
                return CollectorBrickDirective;
            }());
            exports_61("CollectorBrickDirective", CollectorBrickDirective);
            CPUBrickDirective = /** @class */ (function (_super) {
                __extends(CPUBrickDirective, _super);
                function CPUBrickDirective() {
                    return _super.call(this, 'app/views/partials/cpu.partial.html') || this;
                }
                CPUBrickDirective.Factory = function () {
                    var factory = function () {
                        return new CPUBrickDirective();
                    };
                    return factory;
                };
                return CPUBrickDirective;
            }(CollectorBrickDirective));
            exports_61("CPUBrickDirective", CPUBrickDirective);
            MemoryBrickDirective = /** @class */ (function (_super) {
                __extends(MemoryBrickDirective, _super);
                function MemoryBrickDirective() {
                    return _super.call(this, 'app/views/partials/memory.partial.html') || this;
                }
                MemoryBrickDirective.Factory = function () {
                    var factory = function () {
                        return new MemoryBrickDirective();
                    };
                    return factory;
                };
                return MemoryBrickDirective;
            }(CollectorBrickDirective));
            exports_61("MemoryBrickDirective", MemoryBrickDirective);
            DiskBrickDirective = /** @class */ (function (_super) {
                __extends(DiskBrickDirective, _super);
                function DiskBrickDirective() {
                    return _super.call(this, 'app/disk/disk.partial.html') || this;
                }
                DiskBrickDirective.Factory = function () {
                    var factory = function () {
                        return new DiskBrickDirective();
                    };
                    return factory;
                };
                return DiskBrickDirective;
            }(CollectorBrickDirective));
            exports_61("DiskBrickDirective", DiskBrickDirective);
            DiskSpeedBrickDirective = /** @class */ (function (_super) {
                __extends(DiskSpeedBrickDirective, _super);
                function DiskSpeedBrickDirective() {
                    return _super.call(this, 'app/disk/diskspeed.partial.html') || this;
                }
                DiskSpeedBrickDirective.Factory = function () {
                    var factory = function () {
                        return new DiskSpeedBrickDirective();
                    };
                    return factory;
                };
                return DiskSpeedBrickDirective;
            }(CollectorBrickDirective));
            exports_61("DiskSpeedBrickDirective", DiskSpeedBrickDirective);
            NICBrickDirective = /** @class */ (function (_super) {
                __extends(NICBrickDirective, _super);
                function NICBrickDirective() {
                    return _super.call(this, 'app/views/partials/nic.partial.html') || this;
                }
                NICBrickDirective.Factory = function () {
                    var factory = function () {
                        return new NICBrickDirective();
                    };
                    return factory;
                };
                return NICBrickDirective;
            }(CollectorBrickDirective));
            exports_61("NICBrickDirective", NICBrickDirective);
            ServicesBrickDirective = /** @class */ (function (_super) {
                __extends(ServicesBrickDirective, _super);
                function ServicesBrickDirective() {
                    return _super.call(this, 'app/views/partials/services.partial.html') || this;
                }
                ServicesBrickDirective.Factory = function () {
                    var factory = function () {
                        return new ServicesBrickDirective();
                    };
                    return factory;
                };
                return ServicesBrickDirective;
            }(CollectorBrickDirective));
            exports_61("ServicesBrickDirective", ServicesBrickDirective);
            DatabaseBrickDirective = /** @class */ (function (_super) {
                __extends(DatabaseBrickDirective, _super);
                function DatabaseBrickDirective() {
                    return _super.call(this, 'app/views/partials/database.partial.html') || this;
                }
                DatabaseBrickDirective.Factory = function () {
                    var factory = function () {
                        return new DatabaseBrickDirective();
                    };
                    return factory;
                };
                return DatabaseBrickDirective;
            }(CollectorBrickDirective));
            exports_61("DatabaseBrickDirective", DatabaseBrickDirective);
            ProcessesBrickDirective = /** @class */ (function (_super) {
                __extends(ProcessesBrickDirective, _super);
                function ProcessesBrickDirective() {
                    return _super.call(this, 'app/views/partials/processes.partial.html') || this;
                }
                ProcessesBrickDirective.Factory = function () {
                    var factory = function () {
                        return new ProcessesBrickDirective();
                    };
                    return factory;
                };
                return ProcessesBrickDirective;
            }(CollectorBrickDirective));
            exports_61("ProcessesBrickDirective", ProcessesBrickDirective);
            ApplicationsBrickDirective = /** @class */ (function (_super) {
                __extends(ApplicationsBrickDirective, _super);
                function ApplicationsBrickDirective() {
                    return _super.call(this, 'app/views/partials/applications.partial.html') || this;
                }
                ApplicationsBrickDirective.Factory = function () {
                    var factory = function () {
                        return new ApplicationsBrickDirective();
                    };
                    return factory;
                };
                return ApplicationsBrickDirective;
            }(CollectorBrickDirective));
            exports_61("ApplicationsBrickDirective", ApplicationsBrickDirective);
            UPSBrickDirective = /** @class */ (function (_super) {
                __extends(UPSBrickDirective, _super);
                function UPSBrickDirective() {
                    return _super.call(this, 'app/views/partials/ups.partial.html') || this;
                }
                UPSBrickDirective.Factory = function () {
                    var factory = function () {
                        return new UPSBrickDirective();
                    };
                    return factory;
                };
                return UPSBrickDirective;
            }(CollectorBrickDirective));
            exports_61("UPSBrickDirective", UPSBrickDirective);
            SystemErrorsBrickDirective = /** @class */ (function (_super) {
                __extends(SystemErrorsBrickDirective, _super);
                function SystemErrorsBrickDirective() {
                    return _super.call(this, 'app/views/partials/systemerrors.partial.html') || this;
                }
                SystemErrorsBrickDirective.Factory = function () {
                    var factory = function () {
                        return new SystemErrorsBrickDirective();
                    };
                    return factory;
                };
                return SystemErrorsBrickDirective;
            }(CollectorBrickDirective));
            exports_61("SystemErrorsBrickDirective", SystemErrorsBrickDirective);
            ApplicationErrorsBrickDirective = /** @class */ (function (_super) {
                __extends(ApplicationErrorsBrickDirective, _super);
                function ApplicationErrorsBrickDirective() {
                    return _super.call(this, 'app/views/partials/applicationerrors.partial.html') || this;
                }
                ApplicationErrorsBrickDirective.Factory = function () {
                    var factory = function () {
                        return new ApplicationErrorsBrickDirective();
                    };
                    return factory;
                };
                return ApplicationErrorsBrickDirective;
            }(CollectorBrickDirective));
            exports_61("ApplicationErrorsBrickDirective", ApplicationErrorsBrickDirective);
        }
    };
});
/// <reference types="angular" />
System.register("directives/daystoretrieve.directive", [], function (exports_62, context_62) {
    "use strict";
    var DaysToRetrieveDirective;
    var __moduleName = context_62 && context_62.id;
    return {
        setters: [],
        execute: function () {/// <reference types="angular" />
            DaysToRetrieveDirective = /** @class */ (function () {
                function DaysToRetrieveDirective() {
                    this.restrict = 'E';
                    this.scope = {
                        vm: '<'
                    };
                    this.templateUrl = 'app/views/partials/daystoretrieve.partial.html';
                    this.link = function (scope, element, attrs) {
                    };
                }
                DaysToRetrieveDirective.Factory = function () {
                    var factory = function () {
                        return new DaysToRetrieveDirective();
                    };
                    return factory;
                };
                return DaysToRetrieveDirective;
            }());
            exports_62("DaysToRetrieveDirective", DaysToRetrieveDirective);
        }
    };
});
/// <reference types="angular" />
System.register("app.module", ["services/data.service", "services/devicemanager.service", "services/network.service", "services/languages.service", "services/configuration.service", "app.config", "enums/collectortype.enum", "enums/devicetypes.enum", "controllers/overview.controller", "controllers/header.controller", "controllers/server.controller", "controllers/workstation.controller", "controllers/datacollection.controller", "controllers/about.controller", "controllers/admin.controller", "controllers/help.controller", "controllers/group.controller", "reports/reports.controller", "controllers/device.controller", "controllers/networkhistory.controller", "charts/graph.directive", "charts/chartbridgefactory.service", "reports/networktable.directive", "reports/machine.snapshot.directive", "reports/datablock.directive", "reports/print.directive", "directives/deviceinfo.directive", "classes/machine", "directives/overview.devicealert.directive", "directives/collector.brick.directive", "directives/daystoretrieve.directive"], function (exports_63, context_63) {
    "use strict";
    var data_service_1, devicemanager_service_1, network_service_1, languages_service_1, configuration_service_1, app_config_1, collectortype_enum_2, devicetypes_enum_3, overview_controller_1, header_controller_1, server_controller_1, workstation_controller_1, datacollection_controller_1, about_controller_1, admin_controller_1, help_controller_1, group_controller_1, reports_controller_1, device_controller_1, networkhistory_controller_1, graph_directive_1, chartbridgefactory_service_1, networktable_directive_1, machine_snapshot_directive_1, datablock_directive_1, print_directive_1, deviceinfo_directive_1, machine_3, overview_devicealert_directive_1, collector_brick_directive_1, daystoretrieve_directive_1, App;
    var __moduleName = context_63 && context_63.id;
    return {
        setters: [
            function (data_service_1_1) {
                data_service_1 = data_service_1_1;
            },
            function (devicemanager_service_1_1) {
                devicemanager_service_1 = devicemanager_service_1_1;
            },
            function (network_service_1_1) {
                network_service_1 = network_service_1_1;
            },
            function (languages_service_1_1) {
                languages_service_1 = languages_service_1_1;
            },
            function (configuration_service_1_1) {
                configuration_service_1 = configuration_service_1_1;
            },
            function (app_config_1_1) {
                app_config_1 = app_config_1_1;
            },
            function (collectortype_enum_2_1) {
                collectortype_enum_2 = collectortype_enum_2_1;
            },
            function (devicetypes_enum_3_1) {
                devicetypes_enum_3 = devicetypes_enum_3_1;
            },
            function (overview_controller_1_1) {
                overview_controller_1 = overview_controller_1_1;
            },
            function (header_controller_1_1) {
                header_controller_1 = header_controller_1_1;
            },
            function (server_controller_1_1) {
                server_controller_1 = server_controller_1_1;
            },
            function (workstation_controller_1_1) {
                workstation_controller_1 = workstation_controller_1_1;
            },
            function (datacollection_controller_1_1) {
                datacollection_controller_1 = datacollection_controller_1_1;
            },
            function (about_controller_1_1) {
                about_controller_1 = about_controller_1_1;
            },
            function (admin_controller_1_1) {
                admin_controller_1 = admin_controller_1_1;
            },
            function (help_controller_1_1) {
                help_controller_1 = help_controller_1_1;
            },
            function (group_controller_1_1) {
                group_controller_1 = group_controller_1_1;
            },
            function (reports_controller_1_1) {
                reports_controller_1 = reports_controller_1_1;
            },
            function (device_controller_1_1) {
                device_controller_1 = device_controller_1_1;
            },
            function (networkhistory_controller_1_1) {
                networkhistory_controller_1 = networkhistory_controller_1_1;
            },
            function (graph_directive_1_1) {
                graph_directive_1 = graph_directive_1_1;
            },
            function (chartbridgefactory_service_1_1) {
                chartbridgefactory_service_1 = chartbridgefactory_service_1_1;
            },
            function (networktable_directive_1_1) {
                networktable_directive_1 = networktable_directive_1_1;
            },
            function (machine_snapshot_directive_1_1) {
                machine_snapshot_directive_1 = machine_snapshot_directive_1_1;
            },
            function (datablock_directive_1_1) {
                datablock_directive_1 = datablock_directive_1_1;
            },
            function (print_directive_1_1) {
                print_directive_1 = print_directive_1_1;
            },
            function (deviceinfo_directive_1_1) {
                deviceinfo_directive_1 = deviceinfo_directive_1_1;
            },
            function (machine_3_1) {
                machine_3 = machine_3_1;
            },
            function (overview_devicealert_directive_1_1) {
                overview_devicealert_directive_1 = overview_devicealert_directive_1_1;
            },
            function (collector_brick_directive_1_1) {
                collector_brick_directive_1 = collector_brick_directive_1_1;
            },
            function (daystoretrieve_directive_1_1) {
                daystoretrieve_directive_1 = daystoretrieve_directive_1_1;
            }
        ],
        execute: function () {/// <reference types="angular" />
            App = /** @class */ (function () {
                function App() {
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
                    this.app.config(app_config_1.Configuration.Factory());
                }
                App.prototype.Run = function () {
                    var _this = this;
                    this.app.run([
                        '$rootScope',
                        '$filter',
                        'devicemanagerService',
                        'networkService',
                        'languagesService',
                        'configurationService',
                        function ($rootScope, $filter, devicemanagerService, networkService, languagesService, configurationService) {
                            var t = _this;
                            $rootScope.ECollectorType = collectortype_enum_2.ECollectorType;
                            $rootScope.EDeviceTypes = devicetypes_enum_3.EDeviceTypes;
                            $rootScope.EMachineParts = machine_3.EMachineParts;
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
                                    //networkService.get()
                                    //    .then((n: Network) => {
                                    //        n.startAutomaticUpdate();
                                    //    });
                                    devicemanagerService.setConfigured();
                                });
                            });
                            t.dateFilter = $filter('date');
                        }
                    ]);
                    this.app.factory('dataService', data_service_1.DataService.Factory());
                    this.app.factory('devicemanagerService', devicemanager_service_1.DeviceManagerService.Factory());
                    this.app.factory('networkService', network_service_1.NetworkService.Factory());
                    this.app.factory('languagesService', languages_service_1.LanguagesService.Factory());
                    this.app.factory('configurationService', configuration_service_1.ConfigurationService.Factory());
                    this.app.factory('chartBridgeFactoryService', chartbridgefactory_service_1.ChartBridgeFactoryService.Factory());
                    this.app.controller('OverviewController', overview_controller_1.OverviewController.Factory());
                    this.app.controller('HeaderController', header_controller_1.HeaderController.Factory());
                    this.app.controller('ServerController', server_controller_1.ServerController.Factory());
                    this.app.controller('WorkstationController', workstation_controller_1.WorkstationController.Factory());
                    this.app.controller('DataCollectionController', datacollection_controller_1.DataCollectionController.Factory());
                    this.app.controller('AboutController', about_controller_1.AboutController.Factory());
                    this.app.controller('AdminController', admin_controller_1.AdminController.Factory());
                    this.app.controller('DeviceController', device_controller_1.DeviceController.Factory());
                    this.app.controller('HelpController', help_controller_1.HelpController.Factory());
                    this.app.controller('GroupController', group_controller_1.GroupController.Factory());
                    this.app.controller('SiteReportController', reports_controller_1.SiteReportController.Factory());
                    this.app.controller('NetworkHistoryController', networkhistory_controller_1.NetworkHistoryController.Factory());
                    this.app.controller('MachineReportController', reports_controller_1.MachineReportController.Factory());
                    this.app.controller('NetworkReportController', reports_controller_1.NetworkReportController.Factory());
                    this.app.controller('CASLoadReportController', reports_controller_1.CASLoadReportController.Factory());
                    this.app.controller('IssuesReportController', reports_controller_1.IssuesReportController.Factory());
                    this.app.controller('SiteConfigurationReportController', reports_controller_1.SiteConfigurationReportController.Factory());
                    this.app.directive('cmGraph', graph_directive_1.GraphDirective.Factory());
                    this.app.directive('cmNetworkTable', networktable_directive_1.NetworkTableDirective.Factory());
                    this.app.directive('cmMachineSnapshot', machine_snapshot_directive_1.MachineSnapshotDirective.Factory());
                    this.app.directive('cmDataBlock', datablock_directive_1.DataBlockDirective.Factory());
                    this.app.directive('cmPrint', print_directive_1.PrintDirective.Factory());
                    this.app.directive('cmDeviceInfo', deviceinfo_directive_1.DeviceInfoDirective.Factory());
                    this.app.directive('cmOverviewDeviceAlert', overview_devicealert_directive_1.OverviewDeviceAlertDirective.Factory());
                    this.app.directive('cmCpuBrick', collector_brick_directive_1.CPUBrickDirective.Factory());
                    this.app.directive('cmMemoryBrick', collector_brick_directive_1.MemoryBrickDirective.Factory());
                    this.app.directive('cmDiskBrick', collector_brick_directive_1.DiskBrickDirective.Factory());
                    this.app.directive('cmDiskSpeedBrick', collector_brick_directive_1.DiskSpeedBrickDirective.Factory());
                    this.app.directive('cmNicBrick', collector_brick_directive_1.NICBrickDirective.Factory());
                    this.app.directive('cmServicesBrick', collector_brick_directive_1.ServicesBrickDirective.Factory());
                    this.app.directive('cmDatabaseBrick', collector_brick_directive_1.DatabaseBrickDirective.Factory());
                    this.app.directive('cmProcessesBrick', collector_brick_directive_1.ProcessesBrickDirective.Factory());
                    this.app.directive('cmApplicationsBrick', collector_brick_directive_1.ApplicationsBrickDirective.Factory());
                    this.app.directive('cmUpsBrick', collector_brick_directive_1.UPSBrickDirective.Factory());
                    this.app.directive('cmSystemErrorsBrick', collector_brick_directive_1.SystemErrorsBrickDirective.Factory());
                    this.app.directive('cmApplicationErrorsBrick', collector_brick_directive_1.ApplicationErrorsBrickDirective.Factory());
                    this.app.directive('cmDaysToRetrieve', daystoretrieve_directive_1.DaysToRetrieveDirective.Factory());
                    this.app.filter('commonDate', function () {
                        var t = _this;
                        return function (theDate) {
                            return t.dateFilter(theDate, 'yyyy-MM-dd HH:mm:ss');
                        };
                    });
                    this.app.filter('commonDay', function () {
                        var t = _this;
                        return function (theDate) {
                            return t.dateFilter(theDate, 'yyyy-MM-dd');
                        };
                    });
                };
                return App;
            }());
            exports_63("App", App);
        }
    };
});
System.register("classes/devicedata", [], function (exports_64, context_64) {
    "use strict";
    var DeviceData;
    var __moduleName = context_64 && context_64.id;
    return {
        setters: [],
        execute: function () {
            // See Models.cs/DeviceData
            DeviceData = /** @class */ (function () {
                function DeviceData() {
                }
                return DeviceData;
            }());
        }
    };
});
/// <reference types="jasmine" />
System.register("classes/utilities.spec", ["classes/utilities"], function (exports_65, context_65) {
    "use strict";
    var utilities_3;
    var __moduleName = context_65 && context_65.id;
    return {
        setters: [
            function (utilities_3_1) {
                utilities_3 = utilities_3_1;
            }
        ],
        execute: function () {/// <reference types="jasmine" />
            describe("Utilities", function () {
                describe("chunk", function () {
                    it("Handle chunk of 1 of 1", function () {
                        var array = [1];
                        var result = utilities_3.Utilities.chunk(array, 1);
                        expect(result).toEqual([[1]]);
                    });
                    it("Handle chunk of 2 of 1", function () {
                        var array = [1];
                        var result = utilities_3.Utilities.chunk(array, 2);
                        expect(result).toEqual([[1]]);
                    });
                    it("Handle chunk of 10007 of 1", function () {
                        var array = [1];
                        var result = utilities_3.Utilities.chunk(array, 10007);
                        expect(result).toEqual([[1]]);
                    });
                    it("Handle chunk of 0 of 1", function () {
                        var array = [1];
                        var result = utilities_3.Utilities.chunk(array, 0);
                        expect(result).toEqual([]);
                    });
                    it("Handle chunk of 0 of 0", function () {
                        var array = [];
                        var result = utilities_3.Utilities.chunk(array, 0);
                        expect(result).toEqual([]);
                    });
                    it("Handle chunk of 1 of 0", function () {
                        var array = [];
                        var result = utilities_3.Utilities.chunk(array, 1);
                        expect(result).toEqual([]);
                    });
                    it("Handle chunk of 1 of 3", function () {
                        var array = [1, 2, 3];
                        var result = utilities_3.Utilities.chunk(array, 1);
                        expect(result).toEqual([[1], [2], [3]]);
                    });
                    it("Handle chunk of 1 of 3", function () {
                        var array = [1, 2, 3];
                        var result = utilities_3.Utilities.chunk(array, 1);
                        expect(result).toEqual([[1], [2], [3]]);
                    });
                    it("Handle chunk of 2 of 3", function () {
                        var array = [1, 2, 3];
                        var result = utilities_3.Utilities.chunk(array, 2);
                        expect(result).toEqual([[1, 2], [3]]);
                    });
                    it("Handle chunk of 2 of 4", function () {
                        var array = [1, 2, 3, 4];
                        var result = utilities_3.Utilities.chunk(array, 2);
                        expect(result).toEqual([[1, 2], [3, 4]]);
                    });
                    it("Handle chunk of 2 of 5", function () {
                        var array = [1, 2, 3, 4, 5];
                        var result = utilities_3.Utilities.chunk(array, 2);
                        expect(result).toEqual([[1, 2], [3, 4], [5]]);
                    });
                    it("Handle chunk of 3 of 5", function () {
                        var array = [1, 2, 3, 4, 5];
                        var result = utilities_3.Utilities.chunk(array, 3);
                        expect(result).toEqual([[1, 2, 3], [4, 5]]);
                    });
                    it("Handle chunk of 7 of 5", function () {
                        var array = [1, 2, 3, 4, 5];
                        var result = utilities_3.Utilities.chunk(array, 7);
                        expect(result).toEqual([[1, 2, 3, 4, 5]]);
                    });
                    it("Handle string chunk of 3 of 5", function () {
                        var array = ["1", "2", "3", "4", "5"];
                        var result = utilities_3.Utilities.chunk(array, 3);
                        expect(result).toEqual([["1", "2", "3"], ["4", "5"]]);
                    });
                });
            });
        }
    };
});
//# sourceMappingURL=everything.js.map