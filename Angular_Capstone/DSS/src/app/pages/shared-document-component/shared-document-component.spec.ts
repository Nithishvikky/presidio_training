import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SharedDocumentComponent } from './shared-document-component';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { DocumentService } from '../../services/document.service';

import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { DocumentViewerService } from '../../services/documentView.service';

describe('SharedDocumentComponent', () => {
  let component: SharedDocumentComponent;
  let fixture: ComponentFixture<SharedDocumentComponent>;
  let mockSanitizer: jasmine.SpyObj<DomSanitizer>;
  let mockDocumentService: jasmine.SpyObj<DocumentService>;

  const mockFileData = 'SGVsbG8gd29ybGQ='; // "Hello world" in base64
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
    mockDocumentService = jasmine.createSpyObj('DocumentService', [], {
      documentDetail$: of(mockDocument)
    });

    await TestBed.configureTestingModule({
      imports: [SharedDocumentComponent, CommonModule],
      providers: [
        { provide: ActivatedRoute, useValue: { snapshot: { params: { filename: 'test.txt' } } } },
        { provide: DocumentService, useValue: mockDocumentService },
        { provide: DocumentAccessService, useValue: {} },
        { provide: DocumentViewerService, useValue: {} },
        { provide: DomSanitizer, useValue: mockSanitizer }
      ]
    }).compileComponents();

    localStorage.setItem('authData', JSON.stringify({ role: 'User' }));

    fixture = TestBed.createComponent(SharedDocumentComponent);
    component = fixture.componentInstance;

    mockSanitizer.bypassSecurityTrustResourceUrl.and.returnValue('safeUrl');
    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should set filename from route', () => {
    expect(component.filename).toBe('test.txt');
  });

  it('should set roleFlag as true if role is User', () => {
    expect(component.roleFlag).toBeTrue();
  });
});
