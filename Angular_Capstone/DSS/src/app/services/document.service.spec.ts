import { TestBed } from '@angular/core/testing';
import { DocumentService } from './document.service';
import { HttpClientTestingModule, HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { DocumentDetailsResponseDto } from '../models/documentDetailsResponseDto';
import { skip } from 'rxjs';
import { provideHttpClient } from '@angular/common/http';

describe('DocumentService', () => {
  let service: DocumentService;
  let httpMock: HttpTestingController;

  const dummyDocs = [
    { id: 1, filename: 'file1.pdf' },
    { id: 2, filename: 'file2.pdf' }
  ] as unknown as DocumentDetailsResponseDto[];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [],
      providers: [DocumentService,provideHttpClient(),provideHttpClientTesting()]
    });
    service = TestBed.inject(DocumentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all user documents and update documents$', (done) => {
      service.GetAllDocuments().subscribe();

      const req = httpMock.expectOne('http://localhost:5015/api/v1/UserDoc/GetAllMyDocumentDetails');
      req.flush({ data: { $values: dummyDocs } });

      service.documents$.subscribe(docs => {
        expect(docs?.length).toBe(2);
        done(); // ✅ only call once
      });
  });

  it('should post a document and call GetAllDocuments()', (done) => {
    const formData = new FormData();

    service.PostDocument(formData).subscribe(() => {
      done();
    });

    const postReq = httpMock.expectOne('http://localhost:5015/api/v1/UserDoc/UploadDocument');
    expect(postReq.request.method).toBe('POST');
    postReq.flush({});

    const getReq = httpMock.expectOne('http://localhost:5015/api/v1/UserDoc/GetAllMyDocumentDetails');
    getReq.flush({ data: { $values: dummyDocs } });
  });

  it('should preview owner document and call GetAllDocuments()', () => {
    service.OwnerDocumentPreview('file1.pdf').subscribe();

    const previewReq = httpMock.expectOne('http://localhost:5015/api/v1/UserDoc/GetMyDocument?filename=file1.pdf');
    expect(previewReq.request.method).toBe('GET');
    previewReq.flush({});

    const getReq = httpMock.expectOne('http://localhost:5015/api/v1/UserDoc/GetAllMyDocumentDetails');
    getReq.flush({ data: { $values: dummyDocs } });
  });

  it('should download shared document and update documentDetail$', (done) => {
    const response = { data: { id: 1, fileName: 'shared.pdf' } };

    service.DownloadSharedDocument('shared.pdf', 'user@example.com').subscribe();

    const req = httpMock.expectOne(
      'http://localhost:5015/api/v1/UserDoc/GetDocument?filename=shared.pdf&UploaderEmail=user@example.com'
    );
    req.flush(response);

    service.documentDetail$.subscribe(detail => {
        expect(detail?.fileName).toBe('shared.pdf');
        done();
    });
  });

  it('should delete a document and call GetAllDocuments()', () => {
    service.DeleteDocument('file2.pdf').subscribe();

    const delReq = httpMock.expectOne('http://localhost:5015/api/v1/UserDoc/DeleteMyDocument?filename=file2.pdf');
    expect(delReq.request.method).toBe('DELETE');
    delReq.flush({});

    const getReq = httpMock.expectOne('http://localhost:5015/api/v1/UserDoc/GetAllMyDocumentDetails');
    getReq.flush({ data: { $values: dummyDocs } });
  });

  it('should fetch all document details and update allDocuments$', (done) => {
    service.GetAllDocumentDetails('user@example.com').subscribe();

    const req = httpMock.expectOne(
      'http://localhost:5015/api/v1/UserDoc/GetAllDocumentDetails?userEmail=user@example.com&ascending=true&pageNumber=1&pageSize=10'
    );
    expect(req.request.method).toBe('GET');
    req.flush({ data: { $values: dummyDocs } });

    service.allDocuments$.subscribe(docs => {
      expect(docs?.length).toBe(2);
      done();
    });
  });

  it('should clear all caches', (done) => {
    service.clearDocumentCaches();

    service.documents$.subscribe(docs => {
      expect(docs).toBeNull();
    });

    service.allDocuments$.subscribe(all => {
      expect(all).toEqual([]);
    });

    service.documentDetail$.subscribe(detail => {
      expect(detail).toBeNull();
      done();
    });
  });
});
