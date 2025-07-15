import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, Observable, of, switchMap, tap } from "rxjs";
import { DocumentSharedUsersDto } from "../models/documentSharedUsersDto";
import { DocumentDetailsResponseDto } from "../models/documentDetailsResponseDto";
import { DashBoardResponseDto } from "../models/dashboardResponseDto";

@Injectable()
export class DocumentAccessService{
  private sharedUserSubject = new BehaviorSubject<DocumentSharedUsersDto[]|null>(null);
  sharedUsers$ = this.sharedUserSubject.asObservable();

  private sharedFileSubject = new BehaviorSubject<DocumentDetailsResponseDto[]|null>(null);
  sharedFiles$ = this.sharedFileSubject.asObservable();

  private dashboardSubject = new BehaviorSubject<DashBoardResponseDto|null>(null);
  dashboard$ = this.dashboardSubject.asObservable();

  constructor(private http:HttpClient){}

  GetDocumentShared():Observable<any>{
    return this.http.get(`http://localhost:5015/api/v1/DocumentShare/GetFilesShared`)
    .pipe(
        tap((res:any) =>{
          this.sharedFileSubject.next(res.data.$values);
        }),
        catchError((err) => {
          this.sharedFileSubject.next([]);
          return of(null);
        })
      )
  }
  GetSharedUsers(filename:string): Observable<any>{
    const params = new HttpParams().set('filename',filename);
    return this.http.get(`http://localhost:5015/api/v1/DocumentShare/GetSharedUsers`,{params})
      .pipe(
        tap((res:any) =>{
          this.sharedUserSubject.next(res.data.$values);
        }),
        catchError((err) => {
          this.sharedUserSubject.next([]);
          return of(null);
        })
      )
  }

  GetSharedUsersAdmin(filename:string,email:string): Observable<any>{
    const params = new HttpParams().set('filename',filename).set('UploaderEmail',email);
    return this.http.get(`http://localhost:5015/api/v1/DocumentShare/GetSharedUsersForAdmin`,{params})
      .pipe(
        tap((res:any) =>{
          this.sharedUserSubject.next(res.data.$values);
        }),
        catchError((err) => {
          this.sharedUserSubject.next([]);
          return of(null);
        })
      )
  }

  GrantPermissionToUser(filename:string,email:string): Observable<any>{
    const params = new HttpParams().set('fileName',filename).set('ShareUserEmail',email);
    return this.http.post(`http://localhost:5015/api/v1/DocumentShare/GrantPermission`,'',{params})
    .pipe(switchMap(()=> this.GetSharedUsers(filename)));
  }
  GrantPermissionToAll(filename:string): Observable<any>{
    const params = new HttpParams().set('fileName',filename);
    return this.http.post(`http://localhost:5015/api/v1/DocumentShare/GrantPermissionToUsers`,'',{params})
    .pipe(switchMap(()=> this.GetSharedUsers(filename)));
  }
  RevokePermissionToAll(filename:string): Observable<any>{
    const params = new HttpParams().set('fileName',filename);
    return this.http.delete(`http://localhost:5015/api/v1/DocumentShare/RevokePermissionToAll`,{params})
    .pipe(switchMap(()=> this.GetSharedUsers(filename)));
  }
  RevokePermissionToUser(filename:string,email:string): Observable<any>{
    const params = new HttpParams().set('fileName',filename).set('ShareUserEmail',email);
    return this.http.delete(`http://localhost:5015/api/v1/DocumentShare/RevokePermission`,{params})
    .pipe(switchMap(()=> this.GetSharedUsers(filename)));
  }

  GetDashBoardData():Observable<any>{
    return this.http.get(`http://localhost:5015/api/v1/DocumentShare/GetDashboard`)
      .pipe(
        tap((res:any)=>{
          console.log(res.data);
          this.dashboardSubject.next(res.data);
        }),
        catchError((err) => {
          this.dashboardSubject.next(null);
          return of(null);
        })
      )
  }
}
