import { Component, HostListener } from '@angular/core';
import { ScrollService } from './services/scroll.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    title = 'SearchYourMeme';
    lastScrollTop = 0;
    constructor(private scrollService: ScrollService) {


    }
   
}
