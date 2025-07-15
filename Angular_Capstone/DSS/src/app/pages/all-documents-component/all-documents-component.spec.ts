import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AllDocumentsComponent } from './all-documents-component';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of } from 'rxjs';
import { DocumentService } from '../../services/document.service';
import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';

describe('AllDocumentsComponent', () => {
  let component: AllDocumentsComponent;
  let fixture: ComponentFixture<AllDocumentsComponent>;
  let mockDocumentService: jasmine.SpyObj<DocumentService>;

  beforeEach(async () => {
    const documentServiceSpy = jasmine.createSpyObj('DocumentService', ['GetAllDocumentDetails'], {
      allDocuments$: of([]), // Mock observable here
    });

    await TestBed.configureTestingModule({
      imports: [AllDocumentsComponent, CommonModule, ReactiveFormsModule],
      providers: [
        { provide: DocumentService, useValue: documentServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AllDocumentsComponent);
    component = fixture.componentInstance;
    mockDocumentService = TestBed.inject(DocumentService) as jasmine.SpyObj<DocumentService>;

    // default mock return value
    mockDocumentService.GetAllDocumentDetails.and.returnValue(of([]));

    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with default values', () => {
    expect(component.filterForm.value).toEqual({
      userEmail: '',
      filename: '',
      sortBy: '',
      ascending: true
    });
  });

  it('should clear filters and reset form values', () => {
    component.filterForm.patchValue({
      userEmail: 'test@example.com',
      filename: 'doc.pdf',
      sortBy: 'email',
      ascending: false
    });

    component.clearFilters();

    expect(component.filterForm.value).toEqual({
      userEmail: '',
      filename: '',
      sortBy: '',
      ascending: true
    });
    expect(component.ascending).toBeTrue();
    expect(component.sortBy).toBe('');
  });

  it('should toggle sort direction', () => {
    expect(component.ascending).toBeTrue();
    component.toggleSortDirection();
    expect(component.ascending).toBeFalse();
    expect(component.filterForm.get('ascending')?.value).toBeFalse();
  });

  it('should toggle sidebar visibility', () => {
    expect(component.showFilterSidebar).toBeFalse();
    component.toggleSidebar();
    expect(component.showFilterSidebar).toBeTrue();
  });

  it('should clear individual input control', () => {
    component.filterForm.get('userEmail')?.setValue('someone@example.com');
    component.clearControl('userEmail');
    expect(component.filterForm.get('userEmail')?.value).toBe('');
  });
});
