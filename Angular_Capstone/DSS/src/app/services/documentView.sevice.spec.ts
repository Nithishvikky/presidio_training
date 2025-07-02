import { TestBed } from '@angular/core/testing';

import { HttpClientTestingModule, HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { skip } from 'rxjs';
import { DocumentViewerService } from './documentView.service';
import { provideHttpClient } from '@angular/common/http';

describe('DocumentViewerService', () => {
  let service: DocumentViewerService;
  let httpMock: HttpTestingController;

  const mockViewers = [
    { viewerName: 'user1', viewedAt: '2024-12-01T12:00:00Z' },
    { viewerName: 'user2', viewedAt: '2024-12-01T13:00:00Z' }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [],
      providers: [DocumentViewerService,provideHttpClient(),provideHttpClientTesting()]
    });

    service = TestBed.inject(DocumentViewerService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // ensure no pending requests
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch viewers for a file and update viewer$', (done) => {
    const filename = 'test.pdf';

    service.viewer$.pipe(skip(1)).subscribe((viewers) => {
      expect(viewers?.length).toBe(2);
      expect(viewers?.[0].viewerName).toBe('user1');
      done();
    });

    service.GetViewerofFile(filename).subscribe();

    const req = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentView/MyFileViewers?Filename=test.pdf'
    );
    expect(req.request.method).toBe('GET');

    req.flush({ data: { $values: mockViewers } });
  });

  it('should handle error and update viewer$ with empty array', (done) => {
    service.viewer$.pipe(skip(1)).subscribe((viewers) => {
      expect(viewers).toEqual([]);
      done();
    });

    service.GetViewerofFile('invalid.pdf').subscribe(result => {
      expect(result).toBeNull(); // because of `return of(null)`
    });

    const req = httpMock.expectOne(
      'http://localhost:5015/api/v1/DocumentView/MyFileViewers?Filename=invalid.pdf'
    );
    req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
  });
});
