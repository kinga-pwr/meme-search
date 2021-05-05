import { Component, OnInit } from '@angular/core';
import { Meme } from '../models/meme';
import { SearchService } from '../services/search.service';

@Component({
    selector: 'app-text-search',
    templateUrl: './text-search.component.html',
    styleUrls: ['./text-search.component.scss']
})
export class TextSearchComponent implements OnInit {

    memes!: Meme[];
    searching: boolean = false;
    constructor() { }
    SearchingStatus(searching: boolean): void {
        this.searching = searching;
    }
    SetMemes(memes: Meme[]): void {
        memes.forEach(m => {
            m.categories = m.category.split(' ')
            // ! usuwa categorie Undefined
            // const index = m.categories.indexOf('Undefined', 0);
            // if (index > -1) {
            //     m.categories.splice(index, 1);
            // }
        })
        this.memes = memes;
        this.searching = false;
    }
    ngOnInit(): void {
    }

}
