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
import { MatSnackBar } from '@angular/material/snack-bar';

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
        private advancedSearchService: AdvancedSearchService, public dialog: MatDialog,
        private _snackBar: MatSnackBar) {
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
        if (this.IsImageSearch())
        {
            let img_res = await this.searchService.ImageSearch(this.searchBox, this.filterParams, 
                this.page, this.resultsCount);
            if (!img_res.tags)
            {
                this.OpenSnackBar("Can not recognize the image", "OK");
            } else {
                this.searchBox = img_res.tags;
                this.memesEvent.emit(img_res.memes);
            }
            this.RemoveImageFromFilters();
        }
        else {
            let result = await this.searchService.AdnvancedSearch(this.searchBox, this.filterParams,
                this.page, this.resultsCount);
            this.memesEvent.emit(result);
        }
        this.OpenDrawerIfClose();
    }

    IsImageSearch(): boolean {
        return this.filterParams && this.filterParams.url;
    }

    RemoveImageFromFilters()
    {
        delete this.filterParams.url; // a'la cast
        delete this.filterParams.searchSimilarities; // a'la cast
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
        if (this.IsImageSearch())
        {
            let img_res = await this.searchService.ImageSearch(this.searchBox, this.filterParams, 
                this.page, this.resultsCount);
            if (!img_res.tags)
            {
                this.OpenSnackBar("Can not recognize the image", "OK");
            } else {
                this.searchBox = img_res.tags;
                this.appendMemesEvent.emit(img_res.memes);
            }
            this.RemoveImageFromFilters();
        }
        else {
            let result = await this.searchService.AdnvancedSearch(this.searchBox, this.filterParams,
                this.page, this.resultsCount);
            this.appendMemesEvent.emit(result);
        }
    }

    OpenDrawerIfClose() {
        if (!this.inputDrawer.opened)
            this.inputDrawer.open();
    }

    OpenSnackBar(message: string, action: string) {
        this._snackBar.open(message, action, {
            horizontalPosition: "end",
            verticalPosition: "top"
        });
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

