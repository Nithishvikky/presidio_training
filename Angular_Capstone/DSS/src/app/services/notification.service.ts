import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { NotificationResponseDto } from '../models/notificationResponseDto';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private hubConnection!: signalR.HubConnection;

  private notifications:NotificationResponseDto[] = [];
  private messageSubject = new BehaviorSubject<NotificationResponseDto[]|null>(null);
  notification$ = this.messageSubject.asObservable();

  seenNoitify:number = 0;
  public seenNotification = new BehaviorSubject<number|null>(0);
  SeenNotifi$ = this.seenNotification.asObservable();

  private accessToken:string = "";

  startConnection(): void {
    const authdata = localStorage.getItem("authData");
    if(authdata) this.accessToken = JSON.parse(authdata).accessToken;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5015/notificationhub', {
        accessTokenFactory: () => this.accessToken
      })  // 🔁 adjust your backend URL
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.log('SignalR error:', err));
  }

  addNotification(){
    this.startConnection();
    this.hubConnection.on('DocumentViewed', (messageResponse) => {
      this.notifications.unshift(messageResponse);
      if(this.notifications.length == 1) {
        this.seenNotification.next(0);
      }
      this.messageSubject.next(this.notifications);
    });
  }

  clearNotification(){
    this.notifications = [];
    this.messageSubject.next(this.notifications);
  }
}
