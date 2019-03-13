/// <reference types="angular" />

export class DataBlockDirective implements ng.IDirective {
    restrict: string = 'E';
    scope = {
        heading: '@',
        line1: '@',
        line2: '@?'
    };
    templateUrl = 'app/reports/datablock.partial.html';
    link: ng.IDirectiveLinkFn = (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
    };
    replace = false;

    constructor() {
        // console.log("DataBlockDirective");
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new DataBlockDirective();
        }
        return factory;
    }
}
