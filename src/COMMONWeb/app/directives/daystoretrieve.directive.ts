/// <reference types="angular" />

import { WindowsMachineController } from "../controllers/windowsMachine.controller";

export interface ICollectorBrickScope extends ng.IScope {
    windowMachineController: WindowsMachineController
}

export class DaysToRetrieveDirective implements ng.IDirective {
    restrict: string = 'E';
    scope = {
        vm: '<'
    };
    templateUrl = 'app/views/partials/daystoretrieve.partial.html';
    link: ng.IDirectiveLinkFn = (scope: ICollectorBrickScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
    };

    constructor() {
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new DaysToRetrieveDirective();
        }
        return factory;
    }
}
