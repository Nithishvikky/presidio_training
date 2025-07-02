import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LoaderService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSubject.asObservable();

  private requestCount = 0;

  show():void {
   this.requestCount++;
   this.loadingSubject.next(true);
  }

  hide():void {
     this.requestCount--;
     if(this.requestCount <= 0){
      this.requestCount = 0;
      this.loadingSubject.next(false);
     }
  }
}
