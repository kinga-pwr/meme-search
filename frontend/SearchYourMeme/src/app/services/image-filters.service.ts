import { Injectable, EventEmitter, Output } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ImageFiltersService {

    @Output() imageSearchEvent = new EventEmitter();

    TurnOnImageFilters() {
        this.imageSearchEvent.emit();
    }
}
