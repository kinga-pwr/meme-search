import { Component, Input, OnInit } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { Meme } from '../models/meme';
import { Output, EventEmitter } from '@angular/core';
import { QueryParams } from '../models/query-params.interface';
import { SearchService } from '../services/search.service';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss']
})
export class SearchComponent {

    searchBox: string = '';
    @Input() inputDrawer!: MatDrawer;
    @Input() queryParams!: QueryParams;
    @Output() memesEvent = new EventEmitter<Meme[]>();
    @Output() searching = new EventEmitter<boolean>();

    constructor(private searchService: SearchService) {

    }

    async Search() {
        this.searching.emit(true);
        let result = await this.searchService.Search(this.searchBox);
        this.memesEvent.emit(result);
    }

    OpenDrawer() {
        this.inputDrawer.toggle()
        console.log("filter")
    }

}
