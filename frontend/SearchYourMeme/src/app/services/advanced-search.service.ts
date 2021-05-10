import { Injectable, EventEmitter, Output } from '@angular/core';
import { QueryParams } from '../models/query-params.interface';

@Injectable({
    providedIn: 'root'
})
export class AdvancedSearchService {

    @Output() advancedSearchEvent = new EventEmitter<QueryParams>();

    Search(params: QueryParams) {
        this.advancedSearchEvent.emit(params);
    }
}
