/// <reference types="angular" />

export class AdminController implements ng.IController {
    constructor() {
    }

    public static Factory(): ng.IControllerConstructor {
        let factory = (): ng.IController => {
            return new AdminController();
        }
        factory.$inject = [];
        return factory;
    }
}
