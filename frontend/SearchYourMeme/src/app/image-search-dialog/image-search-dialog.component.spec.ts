import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageSearchDialogComponent } from './image-search-dialog.component';

describe('ImageSearchDialogComponent', () => {
  let component: ImageSearchDialogComponent;
  let fixture: ComponentFixture<ImageSearchDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImageSearchDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImageSearchDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
