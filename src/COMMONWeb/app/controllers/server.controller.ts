import { WindowsMachineController } from "./windowsMachine.controller";
import { DataService } from "../services/data.service";
import { DeviceManagerService } from "../services/devicemanager.service";
import { ChartBridgeFactoryService } from "../charts/chartbridgefactory.service";
import { ConfigurationService } from "../services/configuration.service";

export class ServerController extends WindowsMachineController {
    constructor(protected dataService: DataService, protected $routeParams: ng.route.IRouteParamsService, protected $scope: ng.IScope, protected devicemanagerService: DeviceManagerService, protected chartBridgeFactoryService: ChartBridgeFactoryService, protected $uibModal: ng.ui.bootstrap.IModalService, protected config: ConfigurationService) {
        super(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config);

        // console.log("ServerController constructor");

        this.factory = ServerController.Factory;
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $routeParams: ng.route.IRouteParamsService, $scope: ng.IScope, devicemanagerService: DeviceManagerService, chartBridgeFactoryService: ChartBridgeFactoryService, $uibModal: ng.ui.bootstrap.IModalService, config: ConfigurationService): ng.IController => {
            return new ServerController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config);
        }
        factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'chartBridgeFactoryService', '$uibModal', 'configurationService'];
        return factory;
    }
}
