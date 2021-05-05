import { Component, Input, OnInit } from '@angular/core';
import { Meme } from '../models/meme';

@Component({
    selector: 'app-meme-list',
    templateUrl: './meme-list.component.html',
    styleUrls: ['./meme-list.component.scss']
})
export class MemeListComponent implements OnInit {

    constructor() { }

    @Input() memes!: Meme[];
    ngOnInit(): void {
    }

}
