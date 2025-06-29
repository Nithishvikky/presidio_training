import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, Observable, of, switchMap, tap } from "rxjs";
import { DocumentSharedUsersDto } from "../models/documentSharedUsersDto";
import { DocumentDetailsResponseDto } from "../models/documentDetailsResponseDto";

@Injectable()
export class DocumentAccessService{
  private sharedUserSubject = new BehaviorSubject<DocumentSharedUsersDto[]|null>(null);
  sharedUsers$ = this.sharedUserSubject.asObservable();

  private sharedFileSubject = new BehaviorSubject<DocumentDetailsResponseDto[]|null>(null);
  sharedFiles$ = this.sharedFileSubject.asObservable();

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
    return this.http.get(`http://localhost:5015/api/v1/DocumentShare/GetSharedUsers?fileName=${filename}`)
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
    return this.http.post(`http://localhost:5015/api/v1/DocumentShare/GrantPermission?fileName=${filename}&ShareUserEmail=${email}`,'')
    .pipe(switchMap(()=> this.GetSharedUsers(filename)));
  }
  GrantPermissionToAll(filename:string): Observable<any>{
    return this.http.post(`http://localhost:5015/api/v1/DocumentShare/GrantPermissionToUsers?fileName=${filename}`,'')
    .pipe(switchMap(()=> this.GetSharedUsers(filename)));
  }
  RevokePermissionToAll(filename:string): Observable<any>{
    return this.http.delete(`http://localhost:5015/api/v1/DocumentShare/RevokePermissionToAll?fileName=${filename}`)
    .pipe(switchMap(()=> this.GetSharedUsers(filename)));
  }
  RevokePermissionToUser(filename:string,email:string): Observable<any>{
    return this.http.delete(`http://localhost:5015/api/v1/DocumentShare/RevokePermission?fileName=${filename}&ShareUserEmail=${email}`)
    .pipe(switchMap(()=> this.GetSharedUsers(filename)));
  }
}
