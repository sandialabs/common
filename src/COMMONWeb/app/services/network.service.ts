/// <reference types="angular" />

import { DataService } from "./data.service";
import { DeviceManagerService } from "./devicemanager.service";
import { Network } from "../classes/network";
import { ConfigurationService } from "./configuration.service";
import { SystemConfiguration } from "../classes/systemconfiguration";
import { PromiseKeeper } from "../classes/promisekeeper";

export class NetworkService {
    private static network: Network;
    private promiseKeeper: PromiseKeeper<Network>;

    constructor(private ds: DataService, private devicemanagerService: DeviceManagerService, private $interval: ng.IIntervalService, private $q: ng.IQService, private config: ConfigurationService) {
        this.promiseKeeper = new PromiseKeeper($q);

        let t = this;
        config.get()
            .then((c: SystemConfiguration) => {
                NetworkService.network = new Network(t.ds, t.devicemanagerService, t.$interval, c);

                t.promiseKeeper.resolve(NetworkService.network);
            });

        // console.log("NetworkService.constructor");
    }

    get(): ng.IPromise<Network> {
        let d: ng.IDeferred<Network> = this.$q.defer<Network>();
        if (NetworkService.network)
            d.resolve(NetworkService.network);
        else
            this.promiseKeeper.push(d);
        return d.promise;
    }

    public static Factory(): Function {
        let factory = (ds: DataService, devicemanagerService: DeviceManagerService, $interval: ng.IIntervalService, $q: ng.IQService, config: ConfigurationService) => {
            return new NetworkService(ds, devicemanagerService, $interval, $q, config);
        }
        factory.$inject = ['dataService', 'devicemanagerService', '$interval', '$q', 'configurationService'];
        return factory;
    }
}