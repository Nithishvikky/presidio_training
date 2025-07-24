import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection?: HubConnection;
  private unreadCountSubject = new BehaviorSubject<number>(0);
  private connectionStateSubject = new BehaviorSubject<string>('Disconnected');
  
  public unreadCount$ = this.unreadCountSubject.asObservable();
  public connectionState$ = this.connectionStateSubject.asObservable();

  constructor() {}

  public startConnection(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(environment.signalRUrl)
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      this.hubConnection.start()
        .then(() => {
          console.log('SignalR Connected!');
          this.connectionStateSubject.next('Connected');
          this.registerHandlers();
          resolve();
        })
        .catch(err => {
          console.error('Error while establishing connection: ', err);
          this.connectionStateSubject.next('Failed');
          reject(err);
        });
    });
  }

  private registerHandlers() {
    if (!this.hubConnection) return;

    // Handle unread count updates
    this.hubConnection.on('UpdateUnreadCount', (count: number) => {
      console.log('Received unread count update:', count);
      this.unreadCountSubject.next(count);
    });

    // Handle new notification
    this.hubConnection.on('NewNotification', (notification: any) => {
      console.log('Received new notification:', notification);
      // You can emit this to other components if needed
    });

    // Handle connection events
    this.hubConnection.onreconnecting((error) => {
      console.log('SignalR reconnecting...', error);
      this.connectionStateSubject.next('Reconnecting');
    });

    this.hubConnection.onreconnected((connectionId) => {
      console.log('SignalR reconnected. ConnectionId: ', connectionId);
      this.connectionStateSubject.next('Connected');
    });

    this.hubConnection.onclose((error) => {
      console.log('SignalR connection closed: ', error);
      this.connectionStateSubject.next('Disconnected');
    });
  }

  public stopConnection(): Promise<void> {
    if (this.hubConnection) {
      this.connectionStateSubject.next('Disconnected');
      return this.hubConnection.stop();
    }
    return Promise.resolve();
  }

  public getConnectionState(): string {
    return this.hubConnection?.state || 'Disconnected';
  }

  public isConnected(): boolean {
    return this.hubConnection?.state === 'Connected';
  }

  public getConnectionId(): string | undefined {
    return this.hubConnection?.connectionId || undefined;
  }

  public getConnectionInfo(): { state: string; connectionId?: string; isConnected: boolean } {
    return {
      state: this.hubConnection?.state || 'Disconnected',
      connectionId: this.hubConnection?.connectionId || undefined,
      isConnected: this.hubConnection?.state === 'Connected'
    };
  }
} 