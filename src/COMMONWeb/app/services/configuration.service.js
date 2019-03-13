/// <reference types="angular" />
define(["require", "exports", "../classes/systemconfiguration", "../classes/promisekeeper"], function (require, exports, systemconfiguration_1, promisekeeper_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ConfigurationService = /** @class */ (function () {
        function ConfigurationService(ds, $q) {
            this.ds = ds;
            this.$q = $q;
            this.keeper = new promisekeeper_1.PromiseKeeper($q);
            // console.log("ConfigurationService.constructor");
        }
        ConfigurationService.prototype.get = function () {
            var deferred = this.$q.defer();
            if (ConfigurationService.config)
                deferred.resolve(ConfigurationService.config);
            else {
                var t_1 = this;
                this.keeper.push(deferred);
                this.ds.getConfiguration()
                    .then(function (result) {
                    var config = new systemconfiguration_1.SystemConfiguration(result);
                    t_1.keeper.resolve(config);
                    ConfigurationService.config = config;
                });
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
    exports.ConfigurationService = ConfigurationService;
});
//# sourceMappingURL=configuration.service.js.map