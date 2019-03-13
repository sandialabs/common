/// <reference types="angular" />

export class AutoUpdater<T> {
    private autoUpdateTimer: ng.IPromise<any>;

    // frequency: in milliseconds
    // callback: the function to call (a static-type function, not an object method), that takes
    // one parameter of type T
    // t: something of type T to pass to the parameter of callback
    // $interval: the angular interval service
    constructor(private frequency: number, private callback: Function, private t: T, private $interval: ng.IIntervalService) {
        this.autoUpdateTimer = null;
    }

    public start() {
        if (this.autoUpdateTimer !== null)
            return;

        this.autoUpdateTimer = this.$interval(this.callback, this.frequency, 0, true, this.t);

        // Call it once so it get's called immediately upon starting. Setting up the timer doesn't
        // call it until the interval has expired.
        this.callback(this.t);
    }

    public stop() {
        if (this.autoUpdateTimer === null)
            return;

        this.$interval.cancel(this.autoUpdateTimer);
        this.autoUpdateTimer = null;
    }

    public changeFrequency(frequency: number) {
        this.stop();
        this.frequency = frequency;
        this.start();
    }
}

export interface IAutoUpdatable<T> {
    autoUpdater: AutoUpdater<T>;

    startAutomaticUpdate();
    stopAutomaticUpdate();
}
