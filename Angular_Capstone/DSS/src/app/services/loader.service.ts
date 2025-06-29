import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LoaderService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSubject.asObservable();

  private timerRef: any;

  show():void {
    console.log('LOADER ON');
    this.timerRef = setTimeout(() => {
      this.loadingSubject.next(true);
    }, 200);
  }

  hide():void {
     console.log('LOADER OFF');
    clearTimeout(this.timerRef);
    this.loadingSubject.next(false);
  }
}
