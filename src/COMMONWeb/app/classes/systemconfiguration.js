define(["require", "exports", "./devices", "./group"], function (require, exports, devices_1, group_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ConfigurationData = /** @class */ (function () {
        function ConfigurationData(config) {
            this.configID = config.configID;
            this.path = config.path;
            this.value = config.value;
            this.deleted = config.deleted;
        }
        return ConfigurationData;
    }());
    var LanguageConfiguration = /** @class */ (function () {
        function LanguageConfiguration(config) {
            this.languageCode = config.languageCode;
            this.language = config.language;
            this.isEnabled = config.isEnabled;
        }
        return LanguageConfiguration;
    }());
    var SystemConfiguration = /** @class */ (function () {
        function SystemConfiguration(config) {
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
                this.groups.push(new group_1.Group(config.groups[i]));
            }
            this.devices = [];
            for (var i = 0; i < config.devices.length; ++i)
                this.devices.push(new devices_1.DeviceInfo(config.devices[i]));
            this.devices.sort(function (a, b) {
                return a.name.toLowerCase().localeCompare(b.name.toLowerCase());
            });
            this.languages = [];
            for (var i = 0; i < config.languages.length; ++i) {
                this.languages.push(new LanguageConfiguration(config.languages[i]));
            }
            this.siteName = this.configuration['site.name'].value;
            this.softwareVersion = config.softwareVersion;
            // Instead of 1.5.0.23, for example, make it 1.5.0
            var values = this.softwareVersion.split('.');
            if (values.length >= 3)
                this.softwareVersionShort = values[0] + '.' + values[1] + '.' + values[2];
            if (config.mostRecentData)
                this.mostRecentData = new Date(config.mostRecentData);
        }
        SystemConfiguration.prototype.getGroup = function (id) {
            var g = null;
            for (var i = 0; g === null && i < this.groups.length; ++i) {
                var group = this.groups[i];
                if (group.id === id)
                    g = group;
            }
            return g;
        };
        return SystemConfiguration;
    }());
    exports.SystemConfiguration = SystemConfiguration;
});
//# sourceMappingURL=systemconfiguration.js.map