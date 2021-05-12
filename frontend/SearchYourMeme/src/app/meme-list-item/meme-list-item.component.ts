import { HostListener, Input } from '@angular/core';
import { AfterViewInit } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MemeDetailsDialogComponent } from '../meme-details-dialog/meme-details-dialog.component';
import { Meme } from '../models/meme';

@Component({
    selector: 'app-meme-list-item',
    templateUrl: './meme-list-item.component.html',
    styleUrls: ['./meme-list-item.component.scss']
})
export class MemeListItemComponent implements OnInit, AfterViewInit {

    @Input() meme!: Meme;
    constructor(public dialog: MatDialog) {

    }

    ngOnInit(): void {
    }


    ngAfterViewInit() {

    }

    OpenDetails() {
        const dialogRef = this.dialog.open(MemeDetailsDialogComponent, {
            width: '80vw',
            data: this.meme
        });

        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
            console.log(result);
        });
    }
}
