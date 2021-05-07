import { Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { fromEvent } from 'rxjs';
import { Meme } from '../models/meme';
import { ScrollService } from '../services/scroll.service';

@Component({
    selector: 'app-meme-list',
    templateUrl: './meme-list.component.html',
    styleUrls: ['./meme-list.component.scss']
})
export class MemeListComponent implements OnInit {

    constructor(private scrollService: ScrollService) { }

    @ViewChild('scrol', { static: true })
    _div!: ElementRef;
    lastScroll = 0;
    counter = 0;
    @Input() memes!: Meme[];
    ngOnInit(): void {
        fromEvent(this._div.nativeElement, 'scroll')
            .subscribe((e: any) => {
                if (e.target['scrollTop'] > this.lastScroll) {
                    if (e.target['scrollTop'] - document.body.offsetHeight >= -20) {
                        this.counter++;
                        if (this.counter % 4 == 0) {
                            this.scrollService.Scrolled();
                        }
                    }
                }
                this.lastScroll = e.target['scrollTop']
            });
    }


}
