import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PublicNotationsComponent } from './public-notations.component';

describe('PublicCommunityComponent', () => {
  let component: PublicNotationsComponent;
  let fixture: ComponentFixture<PublicNotationsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicNotationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicNotationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
