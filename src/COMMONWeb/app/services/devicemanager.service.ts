/// <reference types="angular" />

import { DataService } from "./data.service";
import { DeviceManager } from "../classes/devices";
import { PromiseKeeper } from "../classes/promisekeeper";


export class DeviceManagerService {
    private keeper: PromiseKeeper<DeviceManager>;
    private configured: boolean;

    // Singleton
    private static deviceManager: DeviceManager;

    constructor(private $q: ng.IQService, private dataService: DataService, private $interval: ng.IIntervalService) {
        if (!DeviceManagerService.deviceManager)
            DeviceManagerService.deviceManager = new DeviceManager(this.dataService, this.$interval);
        this.keeper = new PromiseKeeper<DeviceManager>($q);
        this.configured = false;

        // console.log("NewDeviceManagerService.constructor");
    }

    get(alwaysReturn: boolean = false): ng.IPromise<DeviceManager> {
        let d: ng.IDeferred<DeviceManager> = this.$q.defer<DeviceManager>();
        if (this.configured || alwaysReturn)
            d.resolve(DeviceManagerService.deviceManager);
        else
            this.keeper.push(d);
        return d.promise;
    }

    setConfigured() {
        this.configured = true;
        this.keeper.resolve(DeviceManagerService.deviceManager);
    }

    public static Factory(): Function {
        let factory = ($q: ng.IQService, dataService: DataService, $interval: ng.IIntervalService) => {
            return new DeviceManagerService($q, dataService, $interval);
        }
        factory.$inject = ['$q', 'dataService', '$interval'];
        return factory;
    }
}
