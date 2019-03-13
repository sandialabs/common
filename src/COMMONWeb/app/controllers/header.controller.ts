/// <reference types="angular" />

import { DeviceManagerService } from "../services/devicemanager.service";
import { DeviceManager, DeviceInfo } from "../classes/devices";
import { LanguagesService } from "../services/languages.service";
import { Languages, Language } from "../classes/languages";
import { SystemConfiguration } from "../classes/systemconfiguration";
import { ConfigurationService } from "../services/configuration.service";
import { Group } from "../classes/group";
import { EDeviceTypes } from "../enums/devicetypes.enum";

interface IDevicesSidebar {
    isOpen: boolean;
}

export class HeaderController implements ng.IController {
    public siteName: string;
    public devices: DeviceInfo[];
    public groups: Group[];
    public languages: Language[];
    public devicesSidebar: IDevicesSidebar;
    public windowsDevices: DeviceInfo[];

    constructor(public $route: ng.route.IRoute, private devicemanagerService: DeviceManagerService, private languagesService: LanguagesService, private configurationService: ConfigurationService) {
        this.devices = [];
        this.groups = [];
        this.languages = [];
        this.windowsDevices = [];
        this.devicesSidebar = {
            isOpen: false
        }

        let t = this;
        devicemanagerService.get()
            .then((deviceManager: DeviceManager) => {
                t.devices = deviceManager.devices;
                t.groups = deviceManager.groups;

                for (let i = 0; i < t.devices.length; ++i) {
                    let d = t.devices[i];
                    if (d.isWindowsDevice())
                        t.windowsDevices.push(d);
                }
                for (let i = 0; i < t.groups.length; ++i) {
                    let g = t.groups[i];
                    for (let j = 0; j < g.devices.length; ++j) {
                        let d = g.devices[j]
                        if (d.isWindowsDevice())
                            t.windowsDevices.push(d);
                    }
                }

                t.windowsDevices.sort((a: DeviceInfo, b: DeviceInfo) => {
                    return a.name.localeCompare(b.name);
                });
            });

        languagesService.get()
            .then((languages: Languages) => {
                t.languages = languages.languages;
            });

        configurationService.get()
            .then((config: SystemConfiguration) => {
                t.siteName = config.siteName;
            });
    }

    onToggleDevicesSidebar = function () {
        this.devicesSidebar.isOpen = !this.devicesSidebar.isOpen;
    }

    useLanguage(lang: string) {
        this.languagesService.get()
            .then((ls: Languages) => {
                ls.use(lang);
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = ($route: ng.route.IRoute, devicemanagerService: DeviceManagerService, languagesService: LanguagesService, configurationService: ConfigurationService): ng.IController => {
            return new HeaderController($route, devicemanagerService, languagesService, configurationService);
        }
        factory.$inject = ['$route', 'devicemanagerService', 'languagesService', 'configurationService'];
        return factory;
    }
}