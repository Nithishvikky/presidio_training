import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDocumentPreview } from './admin-document-preview';

describe('AdminDocumentPreview', () => {
  let component: AdminDocumentPreview;
  let fixture: ComponentFixture<AdminDocumentPreview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDocumentPreview]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminDocumentPreview);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
