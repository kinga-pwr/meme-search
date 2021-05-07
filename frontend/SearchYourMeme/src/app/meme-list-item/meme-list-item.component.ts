import { HostListener, Input } from '@angular/core';
import { AfterViewInit } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { Meme } from '../models/meme';

@Component({
    selector: 'app-meme-list-item',
    templateUrl: './meme-list-item.component.html',
    styleUrls: ['./meme-list-item.component.scss']
})
export class MemeListItemComponent implements OnInit, AfterViewInit {

    @Input() meme!: Meme;
    constructor() {

    }

    ngOnInit(): void {
    }


    ngAfterViewInit() {

    }
    @HostListener('window:scroll', ['$event']) onScrollEvent($event: any) {
        console.log($event);
        console.log("scrolling");
    }
}
