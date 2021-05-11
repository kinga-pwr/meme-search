import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-image-search-dialog',
  templateUrl: './image-search-dialog.component.html',
  styleUrls: ['./image-search-dialog.component.scss']
})
export class ImageSearchDialogComponent implements OnInit {

  url = new FormControl('', Validators.required);
  imageVisible = false;

  constructor() { }

  ngOnInit(): void {
  }

  Clear()
  {
    this.url.setValue('');
    this.imageVisible = false;
  }

}
