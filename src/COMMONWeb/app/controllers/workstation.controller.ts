import { WindowsMachineController } from "./windowsMachine.controller";
import { ChartBridgeFactoryService } from "../charts/chartbridgefactory.service";
import { DataService } from "../services/data.service";
import { DeviceManagerService } from "../services/devicemanager.service";
import { NetworkService } from "../services/network.service";
import { Network } from "../classes/network";
import { IChartJSSettings, ChartJSSettings } from "../charts/chartjs";
import { ConfigurationService } from "../services/configuration.service";

export class WorkstationController extends WindowsMachineController {
    networkChartSettings: IChartJSSettings;

    constructor(protected dataService: DataService, protected $routeParams: ng.route.IRouteParamsService, protected $scope: ng.IScope, protected devicemanagerService: DeviceManagerService, protected chartBridgeFactoryService: ChartBridgeFactoryService, protected network: NetworkService, protected $uibModal: ng.ui.bootstrap.IModalService, protected config: ConfigurationService) {
        super(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, $uibModal, config);

        //console.log("WorkstationController constructor");
        this.networkChartSettings = new ChartJSSettings("Response time in ms", 125);

        let t = this;
        if (this.device) {
            this.network.get()
                .then((n: Network) => {
                    let ns = n.getNetworkStatusFromID(this.device.id);
                    if (ns)
                        t.device.networkStatus = ns;
                });
        }

        this.factory = WorkstationController.Factory;
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (dataService: DataService, $routeParams: ng.route.IRouteParamsService, $scope: ng.IScope, devicemanagerService: DeviceManagerService, chartBridgeFactoryService: ChartBridgeFactoryService, network: NetworkService, $uibModal: ng.ui.bootstrap.IModalService, config: ConfigurationService): ng.IController => {
            return new WorkstationController(dataService, $routeParams, $scope, devicemanagerService, chartBridgeFactoryService, network, $uibModal, config);
        }
        factory.$inject = ['dataService', '$routeParams', '$scope', 'devicemanagerService', 'chartBridgeFactoryService', 'networkService', '$uibModal', 'configurationService'];
        return factory;
    }
}
