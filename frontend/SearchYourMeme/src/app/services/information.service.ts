import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Dictionary } from '../utils/dictionary.interface';
import { HttpService } from './http.service';

@Injectable({
    providedIn: 'root'
})
export class InformationService extends HttpService {

    private statuses: string[] = [];

    Statuses(): Observable<string[]> {
        get: {
            return this.http.get<string[]>(`${this.BASE_URL}Information/Statuses`);
        }
    }

    Categories(): Observable<Map<string, number>> {
        get: {
            return this.http.get<Map<string, number>>(`${this.BASE_URL}Information/Categories`);
        }
    }
    
    Source(): Observable<Map<string, number>> {
        get: {
            return this.http.get<Map<string, number>>(`${this.BASE_URL}Information/Details`);
        }
    }
}
