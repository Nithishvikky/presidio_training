import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentSharedComponent } from './document-shared-component';

describe('DocumentSharedComponent', () => {
  let component: DocumentSharedComponent;
  let fixture: ComponentFixture<DocumentSharedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DocumentSharedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DocumentSharedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
