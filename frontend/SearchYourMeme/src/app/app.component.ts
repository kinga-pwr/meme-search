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

    @HostListener('window:scroll', ['$event']) onScrollEvent($event: any) {
        console.log($event);
        console.log("scrolling");
    }
    onScroll($event: any): void {
        let pos = (document.documentElement.scrollTop || document.body.scrollTop) + document.documentElement.offsetHeight;
        let max = document.documentElement.scrollHeight;
        // pos/max will give you the distance between scroll bottom and and bottom of screen in percentage.
        if (pos == max) {
            console.log("bottom1")
        }
        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
            console.log("bottom2")
            // you're at the bottom of the page
        }
        if ((window.innerHeight + window.pageYOffset) >= document.body.offsetHeight) {
            console.log("bottom3")
        }
        var st = window.pageYOffset || document.documentElement.scrollTop; // Credits: "https://github.com/qeremy/so/blob/master/so.dom.js#L426"
        if (st > this.lastScrollTop){
           console.log("down");
           
        } else {
            console.log("up");
        }
        this.lastScrollTop = st <= 0 ? 0 : st; // For Mobile or negative scrolling
    }
}
