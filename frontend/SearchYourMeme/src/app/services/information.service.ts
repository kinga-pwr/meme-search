import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Category } from '../models/category';
import { Source } from '../models/source';
import { HttpService } from './http.service';

@Injectable({
    providedIn: 'root'
})
export class InformationService extends HttpService {
    Statuses(): Observable<string[]> {
        get: {
            return this.http.get<string[]>(`${this.BASE_URL}Information/Statuses`);
        }
    }

    Categories(): Observable<Category[]> {
        get: {
            return this.http.get<Category[]>(`${this.BASE_URL}Information/Categories`);
        }
    }
    
    Source(): Observable<Source[]> {
        get: {
            return this.http.get<Source[]>(`${this.BASE_URL}Information/Details`);
        }
    }
}
