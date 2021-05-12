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
    constructor(@Inject(MAT_DIALOG_DATA) public data: Meme) {
        this.meme = data;
    }

    ngOnInit(): void {
    }

}
