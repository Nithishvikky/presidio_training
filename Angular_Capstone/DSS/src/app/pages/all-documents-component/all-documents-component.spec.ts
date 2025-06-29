import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllDocumentsComponent } from './all-documents-component';

describe('AllDocumentsComponent', () => {
  let component: AllDocumentsComponent;
  let fixture: ComponentFixture<AllDocumentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllDocumentsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllDocumentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
