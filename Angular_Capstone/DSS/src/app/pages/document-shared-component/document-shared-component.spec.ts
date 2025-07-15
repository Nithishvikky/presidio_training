import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DocumentSharedComponent } from './document-shared-component';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';

import { DocumentService } from '../../services/document.service';
import { DocumentAccessService } from '../../services/documentAccess.service';

describe('DocumentSharedComponent', () => {
  let component: DocumentSharedComponent;
  let fixture: ComponentFixture<DocumentSharedComponent>;
  let documentAccessServiceSpy: jasmine.SpyObj<DocumentAccessService>;
  let documentServiceSpy: jasmine.SpyObj<DocumentService>;
  let routerSpy: jasmine.SpyObj<Router>;

  const mockDocuments = [
    {
      docId: '1',
      uploaderUsername: 'John',
      uploaderEmail: 'john@example.com',
      fileName: 'file1.pdf',
      uploadedAt: new Date(),
      size: 1024,
      contentType: 'application/pdf',
      lastViewerName: 'Tester'
    }
  ];

  beforeEach(async () => {
    documentAccessServiceSpy = jasmine.createSpyObj('DocumentAccessService', ['GetDocumentShared'], {
      sharedFiles$: of(mockDocuments)
    });
    documentAccessServiceSpy.GetDocumentShared.and.returnValue(of(mockDocuments));

    documentServiceSpy = jasmine.createSpyObj('DocumentService', ['DownloadSharedDocument'], {
      documents$: of(mockDocuments)
    });
    documentServiceSpy.DownloadSharedDocument.and.returnValue(of({}));

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [DocumentSharedComponent, CommonModule, RouterTestingModule],
      providers: [
        { provide: DocumentAccessService, useValue: documentAccessServiceSpy },
        { provide: DocumentService, useValue: documentServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DocumentSharedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load shared documents on init', () => {
    expect(component.documents?.length).toBe(1);
    expect(component.documents?.[0].fileName).toBe('file1.pdf');
    expect(documentAccessServiceSpy.GetDocumentShared).toHaveBeenCalled();
  });

  it('should download and navigate on onDetails()', () => {
    const doc = mockDocuments[0];

    documentServiceSpy.DownloadSharedDocument.and.returnValue(of({}));

    component.onDetails(doc);

    expect(documentServiceSpy.DownloadSharedDocument).toHaveBeenCalledWith(doc.fileName, doc.uploaderEmail);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/main/document', doc.fileName]);
  });
});

