import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MydocumentComponent } from './mydocument-component';
import { DocumentService } from '../../services/document.service';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { ElementRef } from '@angular/core';

describe('MydocumentComponent', () => {
  let component: MydocumentComponent;
  let fixture: ComponentFixture<MydocumentComponent>;
  let documentServiceSpy: jasmine.SpyObj<DocumentService>;

  beforeEach(async () => {
    const mockDocService = jasmine.createSpyObj('DocumentService', [
      'GetAllDocuments',
      'PostDocument',
      'DeleteDocument'
    ]);
    mockDocService.documents$ = of([
      {
        docId: '1',
        fileName: 'file1.pdf',
        uploadedAt: new Date(),
        lastViewerName: 'User A',
        uploaderEmail: 'a@example.com'
      }
    ]);
    mockDocService.GetAllDocuments.and.returnValue(of([]));


    await TestBed.configureTestingModule({
      imports: [MydocumentComponent, CommonModule, FormsModule, RouterTestingModule],
      providers: [
        { provide: DocumentService, useValue: mockDocService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MydocumentComponent);
    component = fixture.componentInstance;
    documentServiceSpy = TestBed.inject(DocumentService) as jasmine.SpyObj<DocumentService>;

    spyOn(component, 'showToast').and.callFake(() => {});
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load documents on init', () => {
    expect(component.documents?.length).toBe(1);
    expect(documentServiceSpy.GetAllDocuments).toHaveBeenCalled();
  });

  it('should handle file selection under 10MB', () => {
    const file = new File(['test'], 'test.txt', { type: 'text/plain' });
    Object.defineProperty(file, 'size', { value: 5 * 1024 * 1024 }); // 5MB

    const event = { target: { files: [file] } } as any;
    component.onFileSelected(event);

    expect(component.selectedFile).toBe(file);
    expect(component.fileSizeFlag).toBeFalse();
  });

  it('should reject file over 10MB', () => {
    const file = new File(['x'.repeat(11 * 1024 * 1024)], 'bigfile.txt');
    Object.defineProperty(file, 'size', { value: 11 * 1024 * 1024 }); // 11MB

    const event = { target: { files: [file] } } as any;
    component.onFileSelected(event);

    expect(component.selectedFile).toBeNull();
    expect(component.fileSizeFlag).toBeTrue();
  });

  it('should upload selected file', () => {
    const mockFile = new File(['hello'], 'test.txt');
    component.selectedFile = mockFile;

    documentServiceSpy.PostDocument.and.returnValue(of({ data: 'Uploaded' }));
    component.onUpload();

    expect(documentServiceSpy.PostDocument).toHaveBeenCalled();
    expect(component.showToast).toHaveBeenCalledWith('File Uploaded Succefully', 'success');
    expect(component.selectedFile).toBeNull();
  });

  it('should show error if upload fails', () => {
    const mockFile = new File(['hello'], 'test.txt');
    component.selectedFile = mockFile;

    documentServiceSpy.PostDocument.and.returnValue(
      throwError(() => ({ error: { error: { errorMessage: 'Upload failed' } } }))
    );
    component.onUpload();

    expect(component.showToast).toHaveBeenCalledWith('Upload failed', 'danger');
    expect(component.selectedFile).toBeNull();
  });

  it('should call DeleteDocument and show toast', () => {
    documentServiceSpy.DeleteDocument.and.returnValue(of({}));
    component.onFileDelete('file1.pdf');

    expect(documentServiceSpy.DeleteDocument).toHaveBeenCalledWith('file1.pdf');
    expect(component.showToast).toHaveBeenCalledWith('File Deleted Succefully', 'danger');
  });

  it('should show error toast if delete fails', () => {
    documentServiceSpy.DeleteDocument.and.returnValue(
      throwError(() => ({ error: { error: { errorMessage: 'Delete failed' } } }))
    );
    component.onFileDelete('file1.pdf');

    expect(component.showToast).toHaveBeenCalledWith('Delete failed', 'danger');
  });

  it('should cancel upload and reset file input', () => {
    const mockInput = document.createElement('input');
    component.fileInput = new ElementRef(mockInput);
    component.selectedFile = new File(['data'], 'somefile.txt');

    component.onUploadCancel();

    expect(component.selectedFile).toBeNull();
    expect(component.fileInput.nativeElement.value).toBe('');
  });
});
