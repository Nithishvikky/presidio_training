import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserActivityLogService {
  private baseUrl = 'http://localhost:5015/api/v1/user-activity-logs';

  constructor(private http: HttpClient) { }

  // Get user's own activity logs
  getUserActivityLogs(pageNumber: number = 1, pageSize: number = 20): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetMyActivityLogs?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  // Get user's activity summary
  getUserDashboardActivity(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetMyActivitySummary`);
  }

  // Get user's activity count
  getUserActivityCount(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetMyActivityCount`);
  }

  // Log a new activity
  logActivity(activityDto: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/LogActivity`, activityDto);
  }

  // Admin endpoints
  getAllActivityLogs(pageNumber: number = 1, pageSize: number = 50): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetAllActivityLogs?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getAllUsersActivitySummary(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetAllUsersActivitySummary`);
  }

  getActivityLogsByType(activityType: string, pageNumber: number = 1, pageSize: number = 50): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetActivityLogsByType/${activityType}?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getActivityLogsByDateRange(startDate: string, endDate: string, pageNumber: number = 1, pageSize: number = 50): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetActivityLogsByDateRange?startDate=${startDate}&endDate=${endDate}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
} 