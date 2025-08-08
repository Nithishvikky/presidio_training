import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection
  // private unreadCountSubject = new BehaviorSubject<number>(0);
  // private connectionStateSubject = new BehaviorSubject<string>('Disconnected');

  // public unreadCount$ = this.unreadCountSubject.asObservable();
  // public connectionState$ = this.connectionStateSubject.asObservable();

  constructor(private notificationService:NotificationService){}

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

      this.hubConnection.on('DocumentViewed', (messageResponse) => {
        console.log(messageResponse);
        this.notificationService.getUnreadCount().subscribe();
         (window as any).appComponentRef?.showToast(messageResponse,'success');
      });
      this.hubConnection.on('DocumentGiven', (messageResponse) => {
        console.log(messageResponse);
        this.notificationService.getUnreadCount().subscribe();
        (window as any).appComponentRef?.showToast(messageResponse,'success');
      });
      this.hubConnection.on('DocumentDeleted', (messageResponse) => {
        console.log(messageResponse);
        this.notificationService.getUnreadCount().subscribe();
        (window as any).appComponentRef?.showToast(messageResponse,'danger');
      });
    }

    stopConnection(): void {
      if (this.hubConnection) {
        this.hubConnection.stop()
          .then(() => {
            console.log('SignalR disconnected');
            // Optionally remove all handlers to prevent memory leaks
            this.hubConnection.off('DocumentViewed');
            this.hubConnection.off('DocumentGiven');
            this.hubConnection.off('DocumentDeleted');
            this.hubConnection = undefined!;
          })
          .catch(err => console.error('SignalR disconnection error:', err));
      }
    }
}
