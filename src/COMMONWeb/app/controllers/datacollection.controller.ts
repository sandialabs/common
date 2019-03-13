import { DataService } from "../services/data.service";
import { DeviceManagerService } from "../services/devicemanager.service";
import { DeviceManager, DeviceInfo } from "../classes/devices";

export class DataCollectionController {
    devices: DeviceInfo[];
    deviceManager: DeviceManager;

    constructor(private dataService: DataService, private $rootScope: ng.IRootScopeService, private $scope: ng.IScope, private devicemanagerService: DeviceManagerService) {
        let t = this;

        this.devices = [];

        devicemanagerService.get()
            .then((deviceManager: DeviceManager) => {
                t.devices = deviceManager.getDevicesForDataCollection();
                t.deviceManager = deviceManager;
            });
    }

    public onCollectNow(collectorID: number) {
        this.devicemanagerService.get()
            .then((deviceManager: DeviceManager) => {
                deviceManager.collectNow(collectorID);
            });
    }

    public onCollectAll(deviceID: number) {
        this.devicemanagerService.get()
            .then((deviceManager: DeviceManager) => {
                deviceManager.collectAll(deviceID);
            });
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $rootScope: ng.IRootScopeService, $scope: ng.IScope, devicemanagerService: DeviceManagerService): ng.IController => {
            return new DataCollectionController(dataService, $rootScope, $scope, devicemanagerService);
        }
        factory.$inject = ['dataService', '$rootScope', '$scope', 'devicemanagerService'];
        return factory;
    }
}
