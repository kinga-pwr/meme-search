import { HttpHeaders, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import { Injectable } from '@angular/core';
import { Meme } from '../models/meme';
import { ImageQueryParams, QueryParams } from '../models/query-params.interface';
import { ImageSearchResult } from '../models/image-search-result.interface';
import { TextSearchResult } from '../models/text-search-result.interface';
@Injectable({
    providedIn: 'root'
})
export class SearchService extends HttpService {
    // async Search(text: string, page: number, resultsCount: number): Promise<TextSearchResult> {

    //     let params = new HttpParams().set("results", resultsCount.toString()).set("start", page.toString())


    //     return await this.http.get<TextSearchResult>(this.BASE_URL + 'Search/' + text, { params: params }).toPromise();
    // }

    async AdnvancedSearch(text: string, queryParams: QueryParams, page: number, resultsCount: number): Promise<TextSearchResult> {

        let params = new HttpParams().set('query', text).set("results", resultsCount.toString()).set("start", page.toString());
        const headers = new HttpHeaders().set('Content-Type', 'application/json');

        console.log({ service: 'SearchService', method: 'AdvancedSearch', params: [text, page, resultsCount] });

        return await this.http.post<TextSearchResult>(`${this.BASE_URL}AdvancedSearch`, queryParams, { params: params, headers: headers }).toPromise();
    }

    async ImageSearch(text: string, queryParams: ImageQueryParams, page: number, resultsCount: number): Promise<ImageSearchResult> {
        // todo add query
        let params = new HttpParams().set("results", resultsCount.toString()).set("start", page.toString());
        const headers = new HttpHeaders().set('Content-Type', 'application/json');

        console.log({ service: 'SearchService', method: 'ImageSearch', params: [page, resultsCount] });

        return await this.http.post<ImageSearchResult>(`${this.BASE_URL}ImageSearch`, queryParams, { params: params, headers: headers }).toPromise();
    }

}
