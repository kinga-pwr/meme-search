import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemeListItemComponent } from './meme-list-item.component';

describe('MemeListItemComponent', () => {
  let component: MemeListItemComponent;
  let fixture: ComponentFixture<MemeListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemeListItemComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemeListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
