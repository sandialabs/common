import { IDeviceInfo, DeviceInfo } from "./devices";
import { IGroup, Group } from "./group";

interface IConfigurationData {
    configID: number;
    path: string;
    value: string;
    deleted: boolean;
}

interface IConfigurationMap {
    [configuration: string]: IConfigurationData;
}

class ConfigurationData implements IConfigurationData {
    public configID: number;
    public path: string;
    public value: string;
    public deleted: boolean;

    constructor(config: IConfigurationData) {
        this.configID = config.configID;
        this.path = config.path;
        this.value = config.value;
        this.deleted = config.deleted;
    }
}

interface ILanguageConfiguration {
    languageCode: string;
    language: string;
    isEnabled: boolean;
}

class LanguageConfiguration implements ILanguageConfiguration {
    languageCode: string;
    language: string;
    isEnabled: boolean;

    constructor(config: ILanguageConfiguration) {
        this.languageCode = config.languageCode;
        this.language = config.language;
        this.isEnabled = config.isEnabled;
    }
}

export interface ISystemConfiguration {
    configuration: IConfigurationMap;
    devices: IDeviceInfo[];
    groups: IGroup[];
    languages: ILanguageConfiguration[];
    softwareVersion: string;
    mostRecentData: string;
}

export class SystemConfiguration {
    configuration: IConfigurationMap;
    devices: DeviceInfo[];
    groups: IGroup[];
    languages: LanguageConfiguration[];

    siteName: string;
    softwareVersion: string;
    softwareVersionShort: string;

    mostRecentData: Date;

    constructor(config: ISystemConfiguration) {
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
            this.groups.push(new Group(config.groups[i]));
        }

        this.devices = [];
        for (var i = 0; i < config.devices.length; ++i)
            this.devices.push(new DeviceInfo(config.devices[i]));
        this.devices.sort((a, b) => {
            return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
        });

        this.languages = [];
        for (var i = 0; i < config.languages.length; ++i) {
            this.languages.push(new LanguageConfiguration(config.languages[i]));
        }

        this.siteName = this.configuration['site.name'].value;
        this.softwareVersion = config.softwareVersion;

        // Instead of 1.5.0.23, for example, make it 1.5.0
        let values = this.softwareVersion.split('.');
        if (values.length >= 3)
            this.softwareVersionShort = values[0] + '.' + values[1] + '.' + values[2];

        if (config.mostRecentData)
            this.mostRecentData = new Date(config.mostRecentData);
    }

    public getGroup(id: number): IGroup {
        let g: IGroup = null;
        for (var i = 0; g === null && i < this.groups.length; ++i) {
            let group = this.groups[i];
            if (group.id === id)
                g = group;
        }
        return g;
    }
}
