import { Component, Input, OnInit } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { Meme } from '../models/meme';
import { Output, EventEmitter } from '@angular/core';
import { QueryParams, ImageQueryParams } from '../models/query-params.interface';
import { SearchService } from '../services/search.service';
import { ScrollService } from '../services/scroll.service';
import { AdvancedSearchService } from '../services/advanced-search.service';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ImageSearchDialogComponent } from '../image-search-dialog/image-search-dialog.component';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {

    searchBox: string = '';
    lastSearch: string = '';
    @Input() inputDrawer!: MatDrawer;
    @Input() queryParams!: any;
    @Output() memesEvent = new EventEmitter<Meme[]>();
    @Output() appendMemesEvent = new EventEmitter<Meme[]>();
    @Output() searching = new EventEmitter<boolean>();

    public page: number;
    public resultsCount: number;
    public filterParams!: any;

    constructor(private searchService: SearchService, private scrollService: ScrollService,
        private advancedSearchService: AdvancedSearchService, public dialog: MatDialog) {
        this.page = 0;
        this.resultsCount = 20;
    }
    ngOnInit(): void {
        this.scrollService.scrollEvent.subscribe(() => {
            this.page += this.resultsCount;
            this.AdvancedSearchNext();
        });
        this.advancedSearchService.advancedSearchEvent.subscribe(
            (obj: {params: QueryParams, first: boolean}) => {
                if (this.IsImageSearch())
                {
                    var updatedFilters = {...obj.params, url: this.filterParams.url, searchSimilarities: true};
                    this.filterParams = updatedFilters;
                }
                else
                    this.filterParams = obj.params;
                if (!obj.first) {
                    this.AdvanceSearch();
                }
            }
        );
    }

    async AdvanceSearch() {
        this.page = 0;
        this.searching.emit(true);
        var result = null;
        if (this.IsImageSearch())
            result = await this.searchService.ImageSearch(this.searchBox, this.filterParams, this.page, this.resultsCount);
        else
            result = await this.searchService.AdnvancedSearch(this.searchBox, this.filterParams, this.page, this.resultsCount);
        this.memesEvent.emit(result);
    }

    IsImageSearch(): boolean {
        return this.filterParams && this.filterParams.url;
    }

    RemoveSearchImageChip()
    {
        delete this.filterParams.url; // a'la cast
        this.advancedSearchService.Search(this.filterParams);
    }

    // async Search() {
    //     this.page = 0;
    //     this.isAdvancedSearch = false;
    //     this.searching.emit(true);
    //     let result = await this.searchService.Search(this.searchBox, this.page, this.resultsCount);
    //     this.memesEvent.emit(result);
    // }


    // async SearchNext() {
    //     this.searching.emit(true);
    //     let result = await this.searchService.Search(this.searchBox, this.page, this.resultsCount);
    //     this.appendMemesEvent.emit(result);
    // }

    async AdvancedSearchNext() {
        this.searching.emit(true);
        var result = null;
        if (this.IsImageSearch())
            result = await this.searchService.ImageSearch(this.searchBox, this.filterParams, this.page, this.resultsCount);
        else
            result = await this.searchService.AdnvancedSearch(this.searchBox, this.filterParams, this.page, this.resultsCount);
        this.appendMemesEvent.emit(result);
    }

    OpenDrawer() {
        this.inputDrawer.toggle()
    }

    OpenDialogWithImage() {
        const dialogRef = this.dialog.open(ImageSearchDialogComponent, {
            width: '60vw'
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result)
            {
                var imageSearch: ImageQueryParams = {...this.filterParams, url: result, searchSimilarities: true};
                this.advancedSearchService.Search(imageSearch);
            }
        });
    }

}

