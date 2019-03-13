/// <reference types="angular" />

import { MachineReport } from "./report";

export interface IMachineReportDataScope extends ng.IScope {
    data: MachineReport;
}

// Pass a MachineReport to the directive in the 'data' attribute.
export class MachineSnapshotDirective implements ng.IDirective {
    restrict: string = 'E';
    scope = {
        data: '=',
    };
    templateUrl = 'app/reports/machine.snapshot.partial.html';
    link: ng.IDirectiveLinkFn = (scope: IMachineReportDataScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
    };

    constructor() {
        // console.log("MachineSnapshotDirective.constructor");
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new MachineSnapshotDirective();
        }
        return factory;
    }
}
