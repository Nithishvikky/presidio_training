import { HttpClient, HttpParams } from "@angular/common/http";
import { UserLoginDto } from "../models/userLoginDto";
import { Injectable } from "@angular/core";
import { UserRegisterDto } from "../models/userRegisterDto";
import { BehaviorSubject, catchError, Observable, of, tap } from "rxjs";
import { DocumentViewersDto } from "../models/documentViewersDto";

@Injectable()
export class DocumentViewerService{
  private viewerSubject = new BehaviorSubject<DocumentViewersDto[]|null>(null);
  viewer$ = this.viewerSubject.asObservable();

  constructor(private http:HttpClient){}

  GetViewerofFile(filename:string): Observable<any>{
    const params = new HttpParams().set('Filename',filename);
    return this.http.get(`http://localhost:5015/api/v1/DocumentView/MyFileViewers`,{params})
    .pipe(
      tap((res:any) =>{
        this.viewerSubject.next(res.data.$values);
      }),
      catchError((err) => {
        this.viewerSubject.next([]);
          return of(null);
        })
      )
  }

  GetViewerofFileAdmin(filename:string,email:string): Observable<any>{
    const params = new HttpParams().set('Filename',filename).set('UploaderEmail',email);
    return this.http.get(`http://localhost:5015/api/v1/DocumentView/FileViewersForAdmin`,{params})
    .pipe(
      tap((res:any) =>{
        this.viewerSubject.next(res.data.$values);
      }),
      catchError((err) => {
        this.viewerSubject.next([]);
          return of(null);
        })
      )
  }
}
