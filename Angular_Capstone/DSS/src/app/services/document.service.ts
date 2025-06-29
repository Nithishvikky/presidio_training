import { HttpClient } from "@angular/common/http";
import { UserLoginDto } from "../models/userLoginDto";
import { Injectable } from "@angular/core";
import { UserRegisterDto } from "../models/userRegisterDto";
import { BehaviorSubject, catchError, Observable, of, switchMap, tap } from "rxjs";
import { DocumentDetailsResponseDto } from "../models/documentDetailsResponseDto";

@Injectable()
export class DocumentService{
  private documentsSubject = new BehaviorSubject<DocumentDetailsResponseDto[]|null>(null);
  documents$ = this.documentsSubject.asObservable();

  private allDocuments =new BehaviorSubject<DocumentDetailsResponseDto[]|null>(null);
  allDocuments$ = this.allDocuments.asObservable();

  private DocumentDetailSubject = new BehaviorSubject<DocumentDetailsResponseDto|null>(null);
  documentDetail$ = this.DocumentDetailSubject.asObservable();

  constructor(private http:HttpClient){}

  GetAllDocuments(): Observable<any>{
    return this.http.get(`http://localhost:5015/api/v1/UserDoc/GetAllMyDocumentDetails`)
    .pipe(
      tap((res:any) =>{
        console.log(res.data.$values);
        this.documentsSubject.next(res.data.$values);
      }),
      catchError((err) => {
        this.documentsSubject.next([]);
        return of(null);
      })
    )
  }

  PostDocument(file:FormData){
    return this.http.post(`http://localhost:5015/api/v1/UserDoc/UploadDocument`,file)
    .pipe(switchMap(()=> this.GetAllDocuments()));
  }

  OwnerDocumentPreview(filename:string){
    return this.http.get(`http://localhost:5015/api/v1/UserDoc/GetMyDocument?filename=${filename}`)
    .pipe(tap(()=> this.GetAllDocuments().subscribe()));
  }
  DownloadSharedDocument(filename:string,email:string){
    return this.http.get(`http://localhost:5015/api/v1/UserDoc/GetDocument?filename=${filename}&UploaderEmail=${email}`)
    .pipe(
      tap((res:any)=> {
        this.DocumentDetailSubject.next(res.data);
      }),
      catchError((err) => {
        console.error(err);
        return of(null);
      })
    );
  }
  DeleteDocument(filename:string){
    return this.http.delete(`http://localhost:5015/api/v1/UserDoc/DeleteMyDocument?fileName=${filename}`)
    .pipe(switchMap(()=> this.GetAllDocuments()));
  }

  GetAllDocumentDetails(
    userEmail?: string,
    filename?: string,
    sortBy?: string,
    ascending: boolean = true,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<any> {
    const params: any = {};

    if (userEmail) params.userEmail = userEmail;
    if (filename) params.Filename = filename;
    if (sortBy) params.sortBy = sortBy;
    params.ascending = ascending;
    params.pageNumber = pageNumber;
    params.pageSize = pageSize;

    return this.http.get(`http://localhost:5015/api/v1/UserDoc/GetAllDocumentDetails`, {
      params: params
    })
    .pipe(
      tap((res:any) =>{
        this.allDocuments.next(res.data.$values);
      }),
      catchError((err) => {
        this.allDocuments.next([]);
        return of(null);
      })
    );
  }

}
