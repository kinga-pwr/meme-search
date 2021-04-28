import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Meme } from '../models/meme';
import { QueryParams } from '../models/query-params.interface';
import { HttpService } from './http.service';

@Injectable({
    providedIn: 'root'
})
export class SearchService extends HttpService {

    async Search(text: string): Promise<Meme[]> {
        return await this.http.get<Meme[]>(this.BASE_URL + 'Search/' + text).toPromise();
    }

    async AdnvancedSearch(text: string, params: QueryParams): Promise<Meme[]> {
        return await this.http.post<Meme[]>(this.BASE_URL + 'AdvancedSearch/' + text, params).toPromise();
    }
}
