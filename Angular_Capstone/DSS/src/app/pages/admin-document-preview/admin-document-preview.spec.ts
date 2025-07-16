import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminDocumentPreview } from './admin-document-preview';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { DocumentService } from '../../services/document.service';
import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { DocumentViewerService } from '../../services/documentView.service';

describe('AdminDocumentPreview', () => {
  let component: AdminDocumentPreview;
  let fixture: ComponentFixture<AdminDocumentPreview>;

  let mockSanitizer: jasmine.SpyObj<DomSanitizer>;
  let mockDocumentService: jasmine.SpyObj<DocumentService>;
  let mockAccessService: jasmine.SpyObj<DocumentAccessService>;
  let mockViewerService: jasmine.SpyObj<DocumentViewerService>;
  let mockRouter: jasmine.SpyObj<Router>;

  const mockDocument: DocumentDetailsResponseDto = {
    fileName: 'test.txt',
    uploaderEmail: 'test@example.com',
    uploaderUsername: 'TestUser',
    uploadedAt: new Date(),
    contentType: 'text/plain',
    docId: '',
    size: 0,
    lastViewerName: ''
  };

  beforeEach(async () => {
    mockSanitizer = jasmine.createSpyObj('DomSanitizer', ['bypassSecurityTrustResourceUrl']);
    mockDocumentService = jasmine.createSpyObj('DocumentService', ['AdminDeleteDocument'], {
      documentDetail$: of(mockDocument)
    });
    mockAccessService = jasmine.createSpyObj('DocumentAccessService', [], {
      sharedUsers$: of([{ email: 'shared@example.com' }])
    });
    mockViewerService = jasmine.createSpyObj('DocumentViewerService', [], {
      viewer$: of([{ email: 'viewer@example.com' }])
    });
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [AdminDocumentPreview, CommonModule],
      providers: [
        { provide: ActivatedRoute, useValue: { snapshot: { params: { filename: 'test.txt' } } } },
        { provide: DomSanitizer, useValue: mockSanitizer },
        { provide: DocumentService, useValue: mockDocumentService },
        { provide: DocumentAccessService, useValue: mockAccessService },
        { provide: DocumentViewerService, useValue: mockViewerService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDocumentPreview);
    component = fixture.componentInstance;

    mockSanitizer.bypassSecurityTrustResourceUrl.and.returnValue('safeUrl');
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should set filename from route params', () => {
    expect(component.filename).toBe('test.txt');
  });

  it('should subscribe to shared users and viewers', () => {
    expect(component.fileSharedUsers?.length).toBeGreaterThan(0);
    expect(component.fileViewers?.length).toBeGreaterThan(0);
  });

  it('should call AdminDeleteDocument and navigate after deletion', () => {
    spyOn(component, 'showToast');
    mockDocumentService.AdminDeleteDocument.and.returnValue(of({ data: 'deleted' }));

    component.onDelete();

    expect(mockDocumentService.AdminDeleteDocument).toHaveBeenCalledWith(
      mockDocument.fileName,
      mockDocument.uploaderEmail
    );
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/main/alldocuments']);
    expect(component.showToast).toHaveBeenCalledWith('File Deleted Succefully', 'danger');
  });
});
