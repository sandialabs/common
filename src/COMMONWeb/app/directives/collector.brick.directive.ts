/// <reference types="angular" />

import { WindowsMachineController } from "../controllers/windowsMachine.controller";

export interface ICollectorBrickScope extends ng.IScope {
    windowMachineController: WindowsMachineController
}

export class CollectorBrickDirective implements ng.IDirective {
    restrict: string = 'E';
    scope = {
        vm: '<'
    };
    //templateUrl = 'app/views/partials/cpu.partial.html';
    link: ng.IDirectiveLinkFn = (scope: ICollectorBrickScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
    };

    constructor(public templateUrl: string) {
    }
}

export class CPUBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/cpu.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new CPUBrickDirective();
        }
        return factory;
    }
}

export class MemoryBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/memory.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new MemoryBrickDirective();
        }
        return factory;
    }
}

export class DiskBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/disk/disk.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new DiskBrickDirective();
        }
        return factory;
    }
}

export class DiskSpeedBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/disk/diskspeed.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new DiskSpeedBrickDirective();
        }
        return factory;
    }
}

export class NICBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/nic.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new NICBrickDirective();
        }
        return factory;
    }
}

export class ServicesBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/services.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new ServicesBrickDirective();
        }
        return factory;
    }
}

export class DatabaseBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/database.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new DatabaseBrickDirective();
        }
        return factory;
    }
}

export class ProcessesBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/processes.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new ProcessesBrickDirective();
        }
        return factory;
    }
}

export class ApplicationsBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/applications.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new ApplicationsBrickDirective();
        }
        return factory;
    }
}

export class UPSBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/ups.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new UPSBrickDirective();
        }
        return factory;
    }
}

export class SystemErrorsBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/systemerrors.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new SystemErrorsBrickDirective();
        }
        return factory;
    }
}

export class ApplicationErrorsBrickDirective extends CollectorBrickDirective {
    constructor() {
        super('app/views/partials/applicationerrors.partial.html');
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new ApplicationErrorsBrickDirective();
        }
        return factory;
    }
}
