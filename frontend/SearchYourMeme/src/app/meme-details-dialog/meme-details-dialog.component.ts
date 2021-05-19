import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Meme } from '../models/meme';

@Component({
    selector: 'app-meme-details-dialog',
    templateUrl: './meme-details-dialog.component.html',
    styleUrls: ['./meme-details-dialog.component.scss']
})
export class MemeDetailsDialogComponent implements OnInit {

    meme: Meme;
    memes: Meme[];
    index: number;
    constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
        this.meme = data.meme;
        this.memes = data.memes;
        this.index = data.index;
    }

    ngOnInit(): void {
    }

    loadPrev() {
        if (this.index > 0) {
            this.meme = this.memes[this.index - 1];
            this.index = this.index - 1;
        }
    }

    loadNext() {
        if (this.index < this.memes.length - 1) {
            this.meme = this.memes[this.index + 1];
            this.index = this.index + 1;
        }
    }
}
