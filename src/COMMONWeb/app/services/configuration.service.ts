/// <reference types="angular" />

import { DataService } from "./data.service";
import { SystemConfiguration, ISystemConfiguration } from "../classes/systemconfiguration";
import { PromiseKeeper } from "../classes/promisekeeper";

export class ConfigurationService {
    private static config: SystemConfiguration;
    private keeper: PromiseKeeper<SystemConfiguration>;
    private requested: boolean;

    constructor(private ds: DataService, private $q: ng.IQService) {
        this.keeper = new PromiseKeeper<SystemConfiguration>($q);
        this.requested = false;
        // console.log("ConfigurationService.constructor");
    }

    get(): ng.IPromise<SystemConfiguration> {
        let deferred = this.$q.defer<SystemConfiguration>();
        if (ConfigurationService.config)
            deferred.resolve(ConfigurationService.config);
        else {
            this.keeper.push(deferred);

            if (this.requested == false) {
                this.requested = true;

                let t = this;
                this.ds.getConfiguration()
                    .then((result: ISystemConfiguration) => {
                        let start = new Date().getTime();
                        let config = new SystemConfiguration(result);
                        let ms = new Date().getTime() - start;
                        console.log("new SystemConfiguration took " + ms.toString() + " ms");
                        t.keeper.resolve(config);

                        ConfigurationService.config = config;
                    });
            }
        }
        return deferred.promise;
    }

    public static Factory(): Function {
        let factory = (ds: DataService, $q: ng.IQService) => {
            return new ConfigurationService(ds, $q);
        }
        factory.$inject = ['dataService', '$q'];
        return factory;
    }
}