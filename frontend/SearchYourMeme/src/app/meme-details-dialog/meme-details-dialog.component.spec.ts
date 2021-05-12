import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemeDetailsDialogComponent } from './meme-details-dialog.component';

describe('MemeDetailsDialogComponent', () => {
  let component: MemeDetailsDialogComponent;
  let fixture: ComponentFixture<MemeDetailsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemeDetailsDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemeDetailsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
