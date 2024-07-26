import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PublicHistoryComponent } from './public-history.component';

describe('PublicHistoryComponent', () => {
  let component: PublicHistoryComponent;
  let fixture: ComponentFixture<PublicHistoryComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
