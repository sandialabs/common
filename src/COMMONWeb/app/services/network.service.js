/// <reference types="angular" />
define(["require", "exports", "../classes/network", "../classes/promisekeeper"], function (require, exports, network_1, promisekeeper_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var NetworkService = /** @class */ (function () {
        function NetworkService(ds, devicemanagerService, $interval, $q, config) {
            this.ds = ds;
            this.devicemanagerService = devicemanagerService;
            this.$interval = $interval;
            this.$q = $q;
            this.config = config;
            this.promiseKeeper = new promisekeeper_1.PromiseKeeper($q);
            var t = this;
            config.get()
                .then(function (c) {
                NetworkService.network = new network_1.Network(t.ds, t.devicemanagerService, t.$interval, c);
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
    exports.NetworkService = NetworkService;
});
//# sourceMappingURL=network.service.js.map