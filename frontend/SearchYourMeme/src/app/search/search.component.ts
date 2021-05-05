import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { QueryParams } from '../models/query-params.interface';
import { InformationService } from '../services/information.service';
import { SearchService } from '../services/search.service';
import { Output, EventEmitter } from '@angular/core';
import { Meme } from '../models/meme';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {

    searchBox: string = '';
    @Input() queryParams!: QueryParams;

    @Output() searching = new EventEmitter<boolean>();

    @Output() memesEvent = new EventEmitter<Meme[]>();
    constructor(private searchService: SearchService) {

    }
    ngOnInit(): void {
    }

    async Search() {
        this.searching.emit(true);
        let result = await this.searchService.Search(this.searchBox);
        this.memesEvent.emit(result);
    }
    
}
