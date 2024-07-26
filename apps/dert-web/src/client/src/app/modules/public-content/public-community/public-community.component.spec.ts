import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PublicCommunityComponent } from './public-community.component';

describe('PublicCommunityComponent', () => {
  let component: PublicCommunityComponent;
  let fixture: ComponentFixture<PublicCommunityComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PublicCommunityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PublicCommunityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
