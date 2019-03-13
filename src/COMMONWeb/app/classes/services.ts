import { ValueInfo } from "./valueinfo";

export interface IServices {
    services: string[];
    timestamp: string;
}

export class Services {
    services: string[];
    timestamp: Date;

    constructor(s: IServices) {
        this.services = s.services;
        this.timestamp = new Date(s.timestamp);
    }
}