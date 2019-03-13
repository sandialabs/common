/// <reference types="angular" />

export class HelpController implements ng.IController {
    constructor() {
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (): ng.IController => {
            return new HelpController();
        }
        factory.$inject = [];
        return factory;
    }
}
