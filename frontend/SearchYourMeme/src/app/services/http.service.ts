import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class HttpService {
    readonly BASE_URL: string = 'http://localhost:5000/'

    readonly http: HttpClient;
    constructor(http: HttpClient) {
        this.http = http;
    }
}
