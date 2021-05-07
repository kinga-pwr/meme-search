import { Injectable, EventEmitter, Output } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ScrollService {

    @Output() scrollEvent = new EventEmitter<string>();

    Scrolled() {
        this.scrollEvent.emit();
    }
}
