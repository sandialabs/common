/// <reference types="angular" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var AboutController = /** @class */ (function () {
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
    exports.AboutController = AboutController;
});
//# sourceMappingURL=about.controller.js.map