import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, of, tap } from 'rxjs';
import { 
  CreateUserRequestDto, 
  UserRequestDto, 
  ApproveUserRequestDto, 
  RejectUserRequestDto 
} from '../models/userRequestDto';

@Injectable({
  providedIn: 'root'
})
export class UserRequestService {
  private userRequestsSubject = new BehaviorSubject<UserRequestDto[] | null>(null);
  userRequests$ = this.userRequestsSubject.asObservable();

  private allRequestsSubject = new BehaviorSubject<UserRequestDto[] | null>(null);
  allRequests$ = this.allRequestsSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Create a new user request
  createUserRequest(request: CreateUserRequestDto): Observable<UserRequestDto> {
    return this.http.post<UserRequestDto>('http://localhost:5015/api/v1/UserRequest/CreateRequest', request)
      .pipe(
        tap(() => this.refreshUserRequests()),
        catchError((error) => {
          console.error('Error creating user request:', error);
          return of(null as any);
        })
      );
  }

  // Get user's own requests
  getUserRequests(
    pageNumber: number = 1,
    pageSize: number = 20
  ): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get('http://localhost:5015/api/v1/UserRequest/GetMyRequests', { params })
      .pipe(
        tap((response: any) => {
          console.log('API Response:', response);
          // Extract data from the nested structure: response.data.$values
          const requests = response?.data?.$values || [];
          console.log('Extracted requests:', requests);
          this.userRequestsSubject.next(requests);
        }),
        catchError((error) => {
          console.error('Error fetching user requests:', error);
          this.userRequestsSubject.next([]);
          return of(null);
        })
      );
  }

  // Get all requests (Admin only)
  getAllRequests(
    pageNumber: number = 1,
    pageSize: number = 50
  ): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get('http://localhost:5015/api/v1/UserRequest/GetAllRequests', { params })
      .pipe(
        tap((response: any) => {
          // Extract data from the nested structure: response.data.$values
          const requests = response?.data?.$values || [];
          this.allRequestsSubject.next(requests);
        }),
        catchError((error) => {
          console.error('Error fetching all requests:', error);
          this.allRequestsSubject.next([]);
          return of(null);
        })
      );
  }

  // Get pending requests (Admin only)
  getPendingRequests(
    pageNumber: number = 1,
    pageSize: number = 50
  ): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get('http://localhost:5015/api/v1/UserRequest/GetPendingRequests', { params })
      .pipe(
        tap((response: any) => {
          // Extract data from the nested structure: response.data.$values
          const requests = response?.data?.$values || [];
          this.allRequestsSubject.next(requests);
        }),
        catchError((error) => {
          console.error('Error fetching pending requests:', error);
          this.allRequestsSubject.next([]);
          return of(null);
        })
      );
  }

  // Process request (Approve/Reject) - Admin only
  processRequest(processDto: any): Observable<UserRequestDto> {
    return this.http.put<UserRequestDto>('http://localhost:5015/api/v1/UserRequest/ProcessRequest', processDto)
      .pipe(
        tap(() => {
          this.refreshAllRequests();
          this.refreshUserRequests();
        }),
        catchError((error) => {
          console.error('Error processing request:', error);
          return of(null as any);
        })
      );
  }

  // Get request details by ID
  getRequestDetails(requestId: string): Observable<UserRequestDto> {
    return this.http.get<UserRequestDto>(`http://localhost:5015/api/v1/UserRequest/GetRequest/${requestId}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching request details:', error);
          return of(null as any);
        })
      );
  }

  // Get document access status
  getDocumentAccessStatus(documentId: string): Observable<any> {
    return this.http.get(`http://localhost:5015/api/v1/UserRequest/GetDocumentAccessStatus/${documentId}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching document access status:', error);
          return of(null);
        })
      );
  }

  // Get user's accessible documents
  getMyAccessibleDocuments(): Observable<any> {
    return this.http.get('http://localhost:5015/api/v1/UserRequest/GetMyAccessibleDocuments')
      .pipe(
        catchError((error) => {
          console.error('Error fetching accessible documents:', error);
          return of(null);
        })
      );
  }

  // Check if user has access to a document
  hasDocumentAccess(documentId: string): Observable<any> {
    return this.http.get(`http://localhost:5015/api/v1/UserRequest/HasDocumentAccess/${documentId}`)
      .pipe(
        catchError((error) => {
          console.error('Error checking document access:', error);
          return of(null);
        })
      );
  }

  // Request unarchive for a document
  requestUnarchive(filename: string, documentId?: string): Observable<any> {
    const request: CreateUserRequestDto = {
      documentId: documentId || '', // We need the document ID from the document service
      requestType: 'DocumentAccess',
      reason: `Request to unarchive document: ${filename}`,
      accessDurationHours: 24 // Default 24 hours access
    };

    return this.createUserRequest(request);
  }

  private refreshUserRequests(): void {
    this.getUserRequests().subscribe();
  }

  private refreshAllRequests(): void {
    this.getAllRequests().subscribe();
  }

  clearCaches(): void {
    this.userRequestsSubject.next(null);
    this.allRequestsSubject.next(null);
  }
}     