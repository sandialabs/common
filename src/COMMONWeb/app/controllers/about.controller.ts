/// <reference types="angular" />

import { ConfigurationService } from "../services/configuration.service";
import { SystemConfiguration } from "../classes/systemconfiguration";

interface IAttributions {
    name: string;
    nameURL: string;
    license: string;
}

export class AboutController implements ng.IController {
    attributions: IAttributions[];
    softwareVersion: string;
    softwareVersionShort: string;
    date: Date;

    constructor(private configurationService: ConfigurationService) {
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
        this.attributions.sort((a, b) => {
            return a.name.localeCompare(b.name);
        })

        let t = this;
        configurationService.get()
            .then((config: SystemConfiguration) => {
                this.softwareVersion = config.softwareVersion;
                this.softwareVersionShort = config.softwareVersionShort;
            });
        this.date = new Date();
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (configurationService: ConfigurationService): ng.IController => {
            return new AboutController(configurationService);
        }
        factory.$inject = ['configurationService'];
        return factory;
    }
}

