import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DocumentComponent } from './document-component';
import { ActivatedRoute } from '@angular/router';
import { DocumentService } from '../../services/document.service';
import { DomSanitizer } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { DocumentViewerService } from '../../services/documentView.service';
import { SafeResourceUrl } from '@angular/platform-browser';

describe('DocumentComponent', () => {
  let component: DocumentComponent;
  let fixture: ComponentFixture<DocumentComponent>;
  let sanitizer: DomSanitizer;

  const fakeFileData = {
    fileData: btoa('mock content'),
    fileName: 'testfile.pdf',
    contentType: 'application/pdf',
    uploadedAt: new Date(),
    size: 123
  };

  const mockRoute = {
    snapshot: {
      params: { filename: 'testfile.pdf' }
    }
  };

  const mockDocumentService = {
    OwnerDocumentPreview: jasmine.createSpy().and.returnValue(of({ data: fakeFileData })),
    documentDetail$: of(fakeFileData)
  };

  const mockAccessService = {
    sharedUsers$: of([{ id: 1, email: 'a@b.com', userName: 'abc' }]),
    GetSharedUsers: jasmine.createSpy().and.returnValue(of([])),
    GrantPermissionToUser: jasmine.createSpy().and.returnValue(of({})),
    RevokePermissionToUser: jasmine.createSpy().and.returnValue(of({})),
    GrantPermissionToAll: jasmine.createSpy().and.returnValue(of({})),
    RevokePermissionToAll: jasmine.createSpy().and.returnValue(of({}))
  };

  const mockViewerService = {
    viewer$: of([{ docViewId: 1, viewerName: 'abc', viewedAt: new Date() }]),
    GetViewerofFile: jasmine.createSpy().and.returnValue(of([]))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DocumentComponent, CommonModule, FormsModule],
      providers: [
        { provide: ActivatedRoute, useValue: mockRoute },
        { provide: DocumentService, useValue: mockDocumentService },
        { provide: DocumentAccessService, useValue: mockAccessService },
        { provide: DocumentViewerService, useValue: mockViewerService }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(DocumentComponent);
    component = fixture.componentInstance;
    sanitizer = TestBed.inject(DomSanitizer);

    localStorage.setItem('authData', JSON.stringify({ role: 'Admin' }));
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load filename from route', () => {
    expect(component.filename).toBe('testfile.pdf');
  });

  it('should fetch preview and shared users for admin', () => {
    expect(mockDocumentService.OwnerDocumentPreview).toHaveBeenCalledWith('testfile.pdf');
    expect(component.fileData?.fileName).toBe('testfile.pdf');
    expect(component.fileSharedUsers?.length).toBe(1);
  });

  it('should load preview correctly', () => {
    const fileData = component.fileData!;
    expect(fileData.contentType).toBe('application/pdf');
    expect(component.file).toBeTruthy();
    expect(component.iframeSrc).toBeTruthy();
  });

  it('should call GrantPermissionToUser', () => {
    component.userEmailForGrant = 'abc@xyz.com';
    component.GrantPermissionToUser();
    expect(mockAccessService.GrantPermissionToUser).toHaveBeenCalledWith('testfile.pdf', 'abc@xyz.com');
  });

  it('should call RevokePermissionToUser', () => {
    component.RevokePermissionToUser('test@xyz.com');
    expect(mockAccessService.RevokePermissionToUser).toHaveBeenCalledWith('testfile.pdf', 'test@xyz.com');
  });

  it('should call GrantPermissionToAll', () => {
    component.GrantPermissionForAll();
    expect(mockAccessService.GrantPermissionToAll).toHaveBeenCalledWith('testfile.pdf');
  });

  it('should call RevokePermissionToAll', () => {
    component.RevokePermissionForAll();
    expect(mockAccessService.RevokePermissionToAll).toHaveBeenCalledWith('testfile.pdf');
  });
});
