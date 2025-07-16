import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AllDocumentsComponent } from './all-documents-component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of } from 'rxjs';

import { Router } from '@angular/router';
import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';
import { DocumentViewerService } from '../../services/documentView.service';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { DocumentService } from '../../services/document.service';

describe('AllDocumentsComponent', () => {
  let component: AllDocumentsComponent;
  let fixture: ComponentFixture<AllDocumentsComponent>;

  let mockDocumentService: jasmine.SpyObj<DocumentService>;
  let mockAccessService: jasmine.SpyObj<DocumentAccessService>;
  let mockViewerService: jasmine.SpyObj<DocumentViewerService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockDocumentService = jasmine.createSpyObj('DocumentService', ['GetAllDocumentDetails', 'AdminDeleteDocument', 'DownloadSharedDocument'], {
      allDocuments$: of([])
    });
    mockAccessService = jasmine.createSpyObj('DocumentAccessService', ['GetSharedUsersAdmin']);
    mockViewerService = jasmine.createSpyObj('DocumentViewerService', ['GetViewerofFileAdmin']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [AllDocumentsComponent, CommonModule, FormsModule, ReactiveFormsModule],
      providers: [
        { provide: DocumentService, useValue: mockDocumentService },
        { provide: DocumentAccessService, useValue: mockAccessService },
        { provide: DocumentViewerService, useValue: mockViewerService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AllDocumentsComponent);
    component = fixture.componentInstance;

    mockDocumentService.GetAllDocumentDetails.and.returnValue(of([]));

    fixture.detectChanges();
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
