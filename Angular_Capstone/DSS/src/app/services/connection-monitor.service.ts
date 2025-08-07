import { Injectable } from '@angular/core';
import { SignalRService } from './signalr.service';
import { Observable, interval } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ConnectionMonitorService {
  // private checkInterval = 5000; // Check every 5 seconds

  // constructor(private signalRService: SignalRService) {}

  // /**
  //  * Get real-time connection status as an observable
  //  */
  // getConnectionStatus(): Observable<{
  //   isConnected: boolean;
  //   state: string;
  //   connectionId?: string;
  //   lastChecked: Date;
  // }> {
  //   return interval(this.checkInterval).pipe(
  //     startWith(0), // Start immediately
  //     map(() => {
  //       const info = this.signalRService.getConnectionInfo();
  //       return {
  //         isConnected: info.isConnected,
  //         state: info.state,
  //         connectionId: info.connectionId,
  //         lastChecked: new Date()
  //       };
  //     })
  //   );
  // }

  // /**
  //  * Check if SignalR is currently connected
  //  */
  // isConnected(): boolean {
  //   return this.signalRService.isConnected();
  // }

  // /**
  //  * Get current connection state
  //  */
  // getConnectionState(): string {
  //   return this.signalRService.getConnectionState();
  // }

  // /**
  //  * Get connection ID
  //  */
  // getConnectionId(): string | undefined {
  //   return this.signalRService.getConnectionId();
  // }

  // /**
  //  * Get detailed connection information
  //  */
  // getConnectionInfo() {
  //   return this.signalRService.getConnectionInfo();
  // }

  // /**
  //  * Manually test connection by attempting to reconnect
  //  */
  // async testConnection(): Promise<boolean> {
  //   try {
  //     await this.signalRService.startConnection();
  //     return this.signalRService.isConnected();
  //   } catch (error) {
  //     console.error('Connection test failed:', error);
  //     return false;
  //   }
  // }

  // /**
  //  * Get connection health status
  //  */
  // getConnectionHealth(): {
  //   status: 'healthy' | 'warning' | 'critical';
  //   message: string;
  //   details: any;
  // } {
  //   const info = this.signalRService.getConnectionInfo();

  //   if (info.isConnected) {
  //     return {
  //       status: 'healthy',
  //       message: 'SignalR connection is active and working properly',
  //       details: info
  //     };
  //   } else if (info.state === 'Reconnecting') {
  //     return {
  //       status: 'warning',
  //       message: 'SignalR is attempting to reconnect',
  //       details: info
  //     };
  //   } else {
  //     return {
  //       status: 'critical',
  //       message: 'SignalR connection is not available',
  //       details: info
  //     };
  //   }
  // }
}
