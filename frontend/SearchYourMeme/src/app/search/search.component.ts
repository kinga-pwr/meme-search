import { Component, Input, OnInit } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { Meme } from '../models/meme';
import { Output, EventEmitter } from '@angular/core';
import { QueryParams } from '../models/query-params.interface';
import { SearchService } from '../services/search.service';
import { ScrollService } from '../services/scroll.service';
import { AdvancedSearchService } from '../services/advanced-search.service';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {

    searchBox: string = '';
    lastSearch: string = '';
    @Input() inputDrawer!: MatDrawer;
    @Input() queryParams!: QueryParams;
    @Output() memesEvent = new EventEmitter<Meme[]>();
    @Output() appendMemesEvent = new EventEmitter<Meme[]>();
    @Output() searching = new EventEmitter<boolean>();

    public page: number;
    public resultsCount: number;

    constructor(private searchService: SearchService, private scrollService: ScrollService,
        private advancedSearchService: AdvancedSearchService) {
        this.page = 0;
        this.resultsCount = 20;
    }
    ngOnInit(): void {
        this.scrollService.scrollEvent.subscribe(() => {
            this.page+=this.resultsCount;
            this.SearchNext();
        });
        this.advancedSearchService.advancedSearchEvent.subscribe(
            (params: QueryParams) => this.AdvanceSearch(params)
        );
    }

    async AdvanceSearch(params: QueryParams) {
        this.page = 0;
        this.searching.emit(true);
        let result = await this.searchService.AdnvancedSearch(this.searchBox, params, this.page, this.resultsCount);
        this.memesEvent.emit(result);
    }

    async Search() {
        this.page = 0;
        this.searching.emit(true);
        let result = await this.searchService.Search(this.searchBox, this.page, this.resultsCount);
        this.memesEvent.emit(result);
    }


    async SearchNext() {
        this.searching.emit(true);
        let result = await this.searchService.Search(this.searchBox, this.page, this.resultsCount);
        this.appendMemesEvent.emit(result);
    }

    OpenDrawer() {
        this.inputDrawer.toggle()
    }

}
