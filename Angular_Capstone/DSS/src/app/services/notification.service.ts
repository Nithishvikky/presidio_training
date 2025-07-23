import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NotificationDto, NotificationResponse, UnreadCountDto, ApiResponseModel } from '../models/notificationDto';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private baseUrl = 'http://localhost:5015/api/v1/notifications';

  constructor(private http: HttpClient) { }

  getUserNotifications(pageNumber: number = 1, pageSize: number = 10): Observable<NotificationResponse> {
    return this.http.get<NotificationResponse>(`${this.baseUrl}/user?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  markAsRead(id: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}/read`, {});
  }

  markAllAsRead(): Observable<any> {
    return this.http.put(`${this.baseUrl}/user/read-all`, {});
  }

  deleteNotification(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  getUnreadCount(): Observable<UnreadCountDto> {
    return this.http.get<UnreadCountDto>(`${this.baseUrl}/user/unread-count`);
  }

  NotifyInactiveUsers(): Observable<any> {
    return this.http.post(`${this.baseUrl}/NotifyInactiveUsers`, {});
  }
}
