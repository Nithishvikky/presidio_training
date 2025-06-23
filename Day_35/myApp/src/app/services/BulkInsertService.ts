import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class BulkInsertService {
  constructor(private http: HttpClient) {}

  public processData(file: File): Observable<any> {
    const result$ = new Subject<any>();
    const worker = new Worker(new URL('../workers/file-parser.worker', import.meta.url));

    worker.onmessage = ({ data }) => {
      if (typeof data !== 'string') {
        console.error('Unexpected worker data:', data);
        result$.error('Invalid data from worker');
        return;
      }

      const body = { csvContent: data };

      this.http.post('http://localhost:5251/api/Sample/FromCsv', body).subscribe({
        next: res => {
          result$.next(res);
          result$.complete();
        },
        error: err => {
          console.error('API error:', err);
          result$.error(err);
        }
      });
    };

    worker.onerror = () => {
      result$.error('Worker failed to read file');
    };

    worker.postMessage({ file });

    return result$.asObservable();
  }
}
