export class PromiseKeeper<T> {
    private held: ng.IDeferred<T>[];

    constructor(private $q: ng.IQService) {
        this.held = [];
    }

    push(d: ng.IDeferred<T>) {
        this.held.push(d);
    }

    resolve(t: T) {
        while (this.held.length > 0) {
            let d = this.held.shift();
            d.resolve(t);
        }
    }
}