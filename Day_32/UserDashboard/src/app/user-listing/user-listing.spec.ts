import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserListing } from './user-listing';

describe('UserListing', () => {
  let component: UserListing;
  let fixture: ComponentFixture<UserListing>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserListing]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserListing);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
