import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { skip } from 'rxjs';
import { DocumentAccessService } from './documentAccess.service';
import { provideHttpClient } from '@angular/common/http';

describe('DocumentAccessService', () => {
  let service: DocumentAccessService;
  let httpMock: HttpTestingController;

  const dummySharedUsers = [
    { email: 'user1@example.com' },
    { email: 'user2@example.com' }
  ];

  const dummySharedFiles = [
    { id: 1, fileName: 'file1.pdf' },
    { id: 2, fileName: 'file2.pdf' }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [],
      providers: [DocumentAccessService,provideHttpClient(),provideHttpClientTesting()]
    });

    service = TestBed.inject(DocumentAccessService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch shared files and update sharedFiles$', (done) => {
    service.sharedFiles$.pipe(skip(1)).subscribe(files => {
      expect(files?.length).toBe(2);
      expect(files?.[0].fileName).toBe('file1.pdf');
      done();
    });

    service.GetDocumentShared().subscribe();

    const req = httpMock.expectOne('http://localhost:5015/api/v1/DocumentShare/GetFilesShared');
    req.flush({ data: { $values: dummySharedFiles } });
  });

  it('should fetch shared users for a file and update sharedUsers$', (done) => {
    service.sharedUsers$.pipe(skip(1)).subscribe(users => {
      expect(users?.length).toBe(2);
      expect(users?.[1].email).toBe('user2@example.com');
      done();
    });

    service.GetSharedUsers('file1.pdf').subscribe();

    const req = httpMock.expectOne('http://localhost:5015/api/v1/DocumentShare/GetSharedUsers?filename=file1.pdf');
    expect(req.request.method).toBe('GET');
    req.flush({ data: { $values: dummySharedUsers } });
  });

  it('should grant permission to user and call GetSharedUsers()', () => {
    service.GrantPermissionToUser('file1.pdf', 'user1@example.com').subscribe();

    const postReq = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentShare/GrantPermission?fileName=file1.pdf&ShareUserEmail=user1@example.com'
    );
    expect(postReq.request.method).toBe('POST');
    postReq.flush({});

    const getReq = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentShare/GetSharedUsers?filename=file1.pdf'
    );
    expect(getReq.request.method).toBe('GET');
    getReq.flush({ data: { $values: dummySharedUsers } });
  });

  it('should grant permission to all and call GetSharedUsers()', () => {
    service.GrantPermissionToAll('file2.pdf').subscribe();

    const postReq = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentShare/GrantPermissionToUsers?fileName=file2.pdf'
    );
    expect(postReq.request.method).toBe('POST');
    postReq.flush({});

    const getReq = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentShare/GetSharedUsers?filename=file2.pdf'
    );
    expect(getReq.request.method).toBe('GET');
    getReq.flush({ data: { $values: dummySharedUsers } });
  });

  it('should revoke permission to user and call GetSharedUsers()', () => {
    service.RevokePermissionToUser('file1.pdf', 'user1@example.com').subscribe();

    const deleteReq = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentShare/RevokePermission?fileName=file1.pdf&ShareUserEmail=user1@example.com'
    );
    expect(deleteReq.request.method).toBe('DELETE');
    deleteReq.flush({});

    const getReq = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentShare/GetSharedUsers?filename=file1.pdf'
    );
    expect(getReq.request.method).toBe('GET');
    getReq.flush({ data: { $values: dummySharedUsers } });
  });

  it('should revoke permission to all and call GetSharedUsers()', () => {
    service.RevokePermissionToAll('file1.pdf').subscribe();

    const deleteReq = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentShare/RevokePermissionToAll?fileName=file1.pdf'
    );
    expect(deleteReq.request.method).toBe('DELETE');
    deleteReq.flush({});

    const getReq = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentShare/GetSharedUsers?filename=file1.pdf'
    );
    expect(getReq.request.method).toBe('GET');
    getReq.flush({ data: { $values: dummySharedUsers } });
  });
});
