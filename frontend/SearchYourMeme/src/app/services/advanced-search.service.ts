import { Injectable, EventEmitter, Output } from '@angular/core';
import { QueryParams } from '../models/query-params.interface';

@Injectable({
    providedIn: 'root'
})
export class AdvancedSearchService {

    @Output() advancedSearchEvent = new EventEmitter<{params: QueryParams, first: boolean}>();

    Search(params: QueryParams, first = false) {
        this.advancedSearchEvent.emit({params, first});
    }
}
