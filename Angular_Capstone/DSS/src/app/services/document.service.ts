import { HttpClient, HttpParams } from "@angular/common/http";
import { UserLoginDto } from "../models/userLoginDto";
import { Injectable } from "@angular/core";
import { UserRegisterDto } from "../models/userRegisterDto";
import { BehaviorSubject, catchError, Observable, of, switchMap, tap } from "rxjs";
import { DocumentDetailsResponseDto } from "../models/documentDetailsResponseDto";
import { UserDocDetailDto } from "../models/userDocDetailDto";
import { PagedResponseDto } from "../models/pagedResponseDto";
import { ApiResponse, DocumentDateCountDto, DocumentTypeCountDto, TopSharedDocumentDto, UserCountDto } from "../models/dashboardResponseDto";

@Injectable()
export class DocumentService{
  private documentsSubject = new BehaviorSubject<DocumentDetailsResponseDto[]|null>(null);
  documents$ = this.documentsSubject.asObservable();

  private allDocuments =new BehaviorSubject<DocumentDetailsResponseDto[]|null>(null);
  allDocuments$ = this.allDocuments.asObservable();

  private DocumentDetailSubject = new BehaviorSubject<DocumentDetailsResponseDto|null>(null);
  documentDetail$ = this.DocumentDetailSubject.asObservable();

  private userDocsSubject = new BehaviorSubject<UserDocDetailDto[]|null>(null);
  userDocs$ = this.userDocsSubject.asObservable();

  private archivedDocsSubject = new BehaviorSubject<UserDocDetailDto[]|null>(null);
  archivedDocs$ = this.archivedDocsSubject.asObservable();

  private dashBoardData = new BehaviorSubject<ApiResponse<DocumentDateCountDto>|null>(null);
  dashBoardData$ = this.dashBoardData.asObservable();

  private userRatioCountData = new BehaviorSubject<UserCountDto|null>(null);
  userRatioCountData$ = this.userRatioCountData.asObservable();

  private docTypeSubject = new BehaviorSubject<DocumentTypeCountDto[]|null>(null);
  docTypeCount$ = this.docTypeSubject.asObservable();


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

  GetDashboardData():Observable<any>{
    return this.http.get<ApiResponse<DocumentDateCountDto>>('http://localhost:5015/api/v1/UserDoc/document-upload-trend')
    .pipe(
      tap((res) =>{
        console.log(res);
        this.dashBoardData.next(res);
      })
    )
  }

  GetUserRatio():Observable<any>{
    return this.http.get<UserCountDto>('http://localhost:5015/api/v1/User/GetUserRatio')
    .pipe(
      tap((res:any) =>{
        console.log(res.data);
        this.userRatioCountData.next(res.data);
      })
    )
  }

  getDocumentTypeDistribution(): Observable<DocumentTypeCountDto[]> {
    return this.http.get<DocumentTypeCountDto[]>('http://localhost:5015/api/v1/UserDoc/document-type-distribution')
      .pipe(
        tap((res: any) => {
          const data = res.data?.$values;
          this.docTypeSubject.next(data);
        }),
        catchError((err) => {
          console.error('Error loading document type distribution', err);
          this.docTypeSubject.next([]);
          return of([]);
        })
      );
  }


  PostDocument(file:FormData){
    return this.http.post(`http://localhost:5015/api/v1/UserDoc/UploadDocument`,file)
    .pipe(switchMap(()=> this.GetAllDocuments()));
  }

  OwnerDocumentPreview(filename:string){
    const params = new HttpParams().set('filename',filename);
    return this.http.get(`http://localhost:5015/api/v1/UserDoc/GetMyDocument`,{params})
    .pipe(tap(()=> this.GetAllDocuments().subscribe()));
  }

  DownloadSharedDocument(filename:string,email:string){
    const params = new HttpParams().set('filename',filename).set('UploaderEmail',email);
    return this.http.get(`http://localhost:5015/api/v1/UserDoc/GetDocument`,{params})
    .pipe(
      tap((res:any)=> {
        console.log(res);
        this.DocumentDetailSubject.next(res.data);
      }),
      catchError((err) => {
        console.error(err);
        return of(null);
      })
    );
  }

  DeleteDocument(filename:string){
    const params = new HttpParams().set('filename',filename);
    return this.http.delete(`http://localhost:5015/api/v1/UserDoc/DeleteMyDocument`,{params})
    .pipe(switchMap(()=> this.GetAllDocuments()));
  }

  AdminDeleteDocument(filename:string,email:string){
    const params = new HttpParams().set('filename',filename).set('uploaderEmail',email);
    return this.http.delete(`http://localhost:5015/api/v1/UserDoc/DeleteDocumentByAdmin`,{params})
    .pipe(switchMap(()=> this.GetAllDocumentDetails()));
  }

  GetAllDocumentDetails(
    userEmail?: string,
    filename?: string,
    sortBy?: string,
    ascending: boolean = true
  ): Observable<any> {
    const params: any = {};

    if (userEmail) params.userEmail = userEmail;
    if (filename) params.Filename = filename;
    if (sortBy) params.sortBy = sortBy;
    params.ascending = ascending;

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

  clearDocumentCaches(){
    this.allDocuments.next([]);
    this.DocumentDetailSubject.next(null);
    this.documentsSubject.next(null);
    this.userDocsSubject.next(null);
    this.archivedDocsSubject.next(null);
  }

  ArchiveUserFiles(userIds: string[]): Observable<any> {
    return this.http.post(`http://localhost:5015/api/v1/UserDoc/ArchiveUserFiles`, userIds);
  }

  ArchiveUserFilesById(userId: string): Observable<any> {
    return this.http.post(`http://localhost:5015/api/v1/UserDoc/ArchiveUserFiles/${userId}`, {});
  }


  getUserDocuments(
    pageNumber: number = 1,
    pageSize: number = 10,
    status?: string
  ): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (status) {
      params = params.set('status', status);
    }

    return this.http.get('http://localhost:5015/api/v1/UserDoc/GetAllMyDocumentDetails', { params })
      .pipe(
        tap((response: any) => {
          this.userDocsSubject.next(response.data || []);
        }),
        catchError((error) => {
          console.error('Error fetching user documents:', error);
          this.userDocsSubject.next([]);
          return of(null);
        })
      );
  }

  getArchivedDocuments(
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get('http://localhost:5015/api/v1/UserDoc/GetAllDocumentDetails', { params })
      .pipe(
        tap((response: any) => {
          const archivedDocs = response.data?.filter((doc: any) => doc.isArchived) || [];
          this.archivedDocsSubject.next(archivedDocs);
        }),
        catchError((error) => {
          console.error('Error fetching archived documents:', error);
          this.archivedDocsSubject.next([]);
          return of(null);
        })
      );
  }
}
