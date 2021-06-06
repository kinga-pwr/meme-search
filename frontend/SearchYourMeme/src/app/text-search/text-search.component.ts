import { HostListener, Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { Meme } from '../models/meme';
import { ScrollService } from '../services/scroll.service';
import { SearchService } from '../services/search.service';

@Component({
    selector: 'app-text-search',
    templateUrl: './text-search.component.html',
    styleUrls: ['./text-search.component.scss']
})
export class TextSearchComponent implements OnInit {
    memes!: Meme[];
    searching: boolean = false;
    resultNumber: number;
    @Input() inputDrawer!: MatDrawer;
    searched: boolean;

    constructor() {
        this.resultNumber = 0;
        this.searched = false;
    }
    SearchingStatus(searching: boolean): void {
        this.searching = searching;
    }
    SetMemes(memes: { numberOfResults: number, memes: Meme[] }): void {
        memes.memes.forEach(m => {
            m.categories = m.category.split(' ')
            // ! usuwa categorie Undefined
            // const index = m.categories.indexOf('Undefined', 0);
            // if (index > -1) {
            //     m.categories.splice(index, 1);
            // }
        });
        this.memes = memes.memes;
        this.resultNumber = memes.numberOfResults;
        this.searching = false;
        this.searched = true;
    }
    ngOnInit(): void {
    }
    AppendMemes(memes: Meme[]): void {
        memes.forEach(m => {
            m.categories = m.category.split(' ')
            // ! usuwa categorie Undefined
            // const index = m.categories.indexOf('Undefined', 0);
            // if (index > -1) {
            //     m.categories.splice(index, 1);
            // }
        });
        this.memes = this.memes.concat(memes);
        this.searching = false;
    }

}
