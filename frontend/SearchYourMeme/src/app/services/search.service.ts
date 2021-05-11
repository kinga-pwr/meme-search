import { HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import { Injectable } from '@angular/core';
import { Meme } from '../models/meme';
import { QueryParams } from '../models/query-params.interface';
@Injectable({
    providedIn: 'root'
})
export class SearchService extends HttpService {

    async Search(text: string, page: number, resultsCount: number): Promise<Meme[]> {

        let params = new HttpParams().set("results", resultsCount.toString()).set("start", page.toString())

        return await this.http.get<Meme[]>(this.BASE_URL + 'Search/' + text, { params: params }).toPromise();
    }

    async AdnvancedSearch(text: string, queryParams: QueryParams, page: number, resultsCount: number): Promise<Meme[]> {

        let params = new HttpParams().set("results", resultsCount.toString()).set("start", page.toString())

        return await this.http.post<Meme[]>(this.BASE_URL + `AdvancedSearch${text === "" ? "" : "?query="}` + text, queryParams, { params: params }).toPromise();
    }


}
