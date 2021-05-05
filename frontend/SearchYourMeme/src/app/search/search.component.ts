import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { QueryParams } from '../models/query-params.interface';
import { InformationService } from '../services/information.service';
import { SearchService } from '../services/search.service';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {

    searchBox: string = '';
    @Input() queryParams!: QueryParams;
    @Input() inputDrawer!: MatDrawer;
    
    constructor(private informationService: InformationService, private searchService: SearchService) {

    }
    ngOnInit(): void {
    }

    async Search() {
        let result = await this.searchService.Search(this.searchBox);
        console.log(result);
    }

    OpenSideNav()
    {
        this.inputDrawer.toggle()
        console.log("filter")
    }

}
