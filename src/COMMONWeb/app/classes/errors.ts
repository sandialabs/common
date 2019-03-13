interface IErrorInfo {
    message: string;
    timestamp: string;
    count: number;
    firstTimestamp: string;
    lastTimestamp: string;
}

export interface IDeviceErrors {
    deviceID: number;
    errors: IErrorInfo[];
}

class ErrorInfo {
    message: string;
    timestamp: Date;
    count: number;
    firstTimestamp: Date;
    lastTimestamp: Date;

    constructor(data: IErrorInfo) {
        this.message = data.message;
        this.timestamp = new Date(data.timestamp);
        this.count = data.count;
        this.firstTimestamp = this.lastTimestamp = null;

        if (data.firstTimestamp !== null)
            this.firstTimestamp = new Date(data.firstTimestamp);
        if (data.lastTimestamp !== null)
            this.lastTimestamp = new Date(data.lastTimestamp);
    }
}

export class ErrorManager {
    errors: ErrorInfo[];

    constructor(data: IDeviceErrors) {
        this.errors = [];

        if (data === undefined || data === null || data.errors.length === 0)
            return;

        for (var i = 0; i < data.errors.length; ++i) {
            var errorInfo = new ErrorInfo(data.errors[i]);
            this.errors.push(errorInfo);
        }
    }
}
