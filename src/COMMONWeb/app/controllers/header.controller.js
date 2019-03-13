/// <reference types="angular" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var HeaderController = /** @class */ (function () {
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
    exports.HeaderController = HeaderController;
});
//# sourceMappingURL=header.controller.js.map