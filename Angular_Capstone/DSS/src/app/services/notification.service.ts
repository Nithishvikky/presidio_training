import { inject, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { NotificationResponseDto } from '../models/notificationResponseDto';
import { UserService } from './user.service';
import { DocumentService } from './document.service';
import { DocumentAccessService } from './documentAccess.service';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private hubConnection!: signalR.HubConnection;
  private documentService = inject(DocumentService);

  private documentAccessService = inject(DocumentAccessService);

  private notifications:any[] = [];
  private messageSubject = new BehaviorSubject<any[]|null>(null);
  notification$ = this.messageSubject.asObservable();

  seenNoitify:number = 0;
  public seenNotification = new BehaviorSubject<number|null>(0);
  SeenNotifi$ = this.seenNotification.asObservable();


  startConnection(): void {
    const authdata = localStorage.getItem("authData");
    var accessToken:string = "";
    if(authdata) accessToken = JSON.parse(authdata).accessToken;
    console.log(accessToken);
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5015/notificationhub', {
        accessTokenFactory: () => accessToken
      })  // 🔁 adjust your backend URL
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.log('SignalR error:', err));
  }

  addNotification(){
    console.log("Notification trigger");
    // this.startConnection();
    this.hubConnection.on('DocumentViewed', (messageResponse) => {
      console.log('DocumentViewed received:', messageResponse);
      this.notifications.unshift(messageResponse);
      if(this.notifications.length == 1) {
        this.seenNotification.next(0);
      }
      this.messageSubject.next(this.notifications);
      this.documentService.GetAllDocuments().subscribe();
    });
  }

  addUserNotification(){
    console.log("Notification trigger");
    // this.startConnection();
    this.hubConnection.on('DocumentGiven',(messageResponse)=>{
      console.log(messageResponse);
      this.notifications.unshift(messageResponse);
      if(this.notifications.length == 1){
        this.seenNotification.next(0);
      }
      this.messageSubject.next(this.notifications);
      this.documentAccessService.GetDocumentShared().subscribe();
    });
  }

  disconnect(){
    if(this.hubConnection){
      this.hubConnection.stop();
      this.hubConnection.off('DocumentViewed');
      this.hubConnection.off('DocumentGiven');
    }
  }

  clearCurrentNotification(){
    this.notifications = [];
    this.messageSubject.next([]);
    this.seenNotification.next(0);
  }

  clearNotification(){
    this.clearCurrentNotification();
    this.disconnect();
  }
}
