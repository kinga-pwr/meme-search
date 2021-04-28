import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Dictionary } from '../utils/dictionary.interface';
import { HttpService } from './http.service';

@Injectable({
    providedIn: 'root'
})
export class InformationService extends HttpService {

    private statuses: string[] = [];
    private details!: Dictionary<number>;
    private categories!: Dictionary<number>;

    async Statuses(): Promise<string[]> {
        get: {
            if (this.statuses.length == 0) {
                this.statuses = await this.http.get<string[]>(`${this.BASE_URL}Information/Statuses`).toPromise();
            }
            return this.statuses;
        }
    }

    async Details(): Promise<Dictionary<number>> {
        get: {
            if (this.details == null) {
                this.details = await this.http.get<Dictionary<number>>(`${this.BASE_URL}Information/Details`).toPromise();
            }
            return this.details;
        }
    }

    async Categories(): Promise<Dictionary<number>> {
        get: {
            if (this.categories == null) {
                this.categories = await this.http.get<Dictionary<number>>(`${this.BASE_URL}Information/Categories`).toPromise();
            }
            return this.categories;
        }
    }
}
