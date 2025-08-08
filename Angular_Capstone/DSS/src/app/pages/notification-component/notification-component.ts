import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { SignalRService } from '../../services/signalr.service';
import { NotificationDto, NotificationResponse } from '../../models/notificationDto';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { NotificationTileComponent } from '../notification-tile-component/notification-tile-component';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule, NotificationTileComponent],
  template: `
    <div class="notification-container">
      <div class="notification-header">
        <h2>Notifications</h2>
        <button *ngIf="hasUnread" (click)="markAllAsRead()" class="mark-all-read">
          Mark all as read
        </button>
      </div>

      <div class="notifications-list">
        <app-notification-tile
          *ngFor="let notification of notifications"
          [notification]="notification"
          (markAsRead)="markAsRead($event)"
          (delete)="deleteNotification($event)"
          (tileClick)="onNotificationClick($event)">
        </app-notification-tile>

        <div *ngIf="notifications.length === 0" class="no-notifications">
          <div class="empty-state">
            <i class="bi bi-bell-slash"></i>
            <h3>No notifications</h3>
            <p>You're all caught up! New notifications will appear here.</p>
          </div>
        </div>
      </div>

      <div class="pagination" *ngIf="pagedResult && pagedResult.totalPages > 1">
        <button
          [disabled]="!pagedResult.hasPreviousPage"
          (click)="loadPage(pagedResult.pageNumber - 1)">
          Previous
        </button>
        <span>Page {{ pagedResult.pageNumber }} of {{ pagedResult.totalPages }}</span>
        <button
          [disabled]="!pagedResult.hasNextPage"
          (click)="loadPage(pagedResult.pageNumber + 1)">
          Next
        </button>
      </div>
    </div>
  `,
  styles: [`
    .notification-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
    }

    .notification-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
      padding-bottom: 15px;
      border-bottom: 2px solid #f0f0f0;
    }

    .notification-header h2 {
      color: #2c3e50;
      margin: 0;
      font-weight: 600;
    }

    .mark-all-read {
      padding: 10px 20px;
      background-color: #007bff;
      color: white;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .mark-all-read:hover {
      background-color: #0056b3;
    }

    .notifications-list {
      margin-bottom: 20px;
    }

    .no-notifications {
      text-align: center;
      padding: 40px 20px;
    }

    .empty-state {
      color: #6c757d;
    }

    .empty-state i {
      font-size: 48px;
      margin-bottom: 16px;
      opacity: 0.5;
    }

    .empty-state h3 {
      margin: 0 0 8px 0;
      font-weight: 500;
    }

    .empty-state p {
      margin: 0;
      font-size: 14px;
    }

    .pagination {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 15px;
      margin-top: 30px;
      padding-top: 20px;
      border-top: 1px solid #e9ecef;
    }

    .pagination button {
      padding: 10px 20px;
      border: 1px solid #dee2e6;
      background-color: white;
      color: #495057;
      border-radius: 6px;
      cursor: pointer;
      font-weight: 500;
      transition: all 0.2s;
    }

    .pagination button:hover:not(:disabled) {
      background-color: #f8f9fa;
      border-color: #adb5bd;
    }

    .pagination button:disabled {
      background-color: #f8f9fa;
      color: #adb5bd;
      cursor: not-allowed;
    }

    .pagination span {
      color: #6c757d;
      font-weight: 500;
    }

    @media (max-width: 768px) {
      .notification-container {
        padding: 15px;
      }

      .notification-header {
        flex-direction: column;
        gap: 15px;
        align-items: stretch;
      }

      .mark-all-read {
        width: 100%;
      }
    }
  `]
})
export class NotificationComponent implements OnInit{
  notifications: NotificationDto[] = [];
  pagedResult: any = null;
  hasUnread = false;
  private signalRSubscription?: Subscription;

  constructor(
    private notificationService: NotificationService,
    private signalRService: SignalRService
  ) {}

  ngOnInit() {
    this.loadNotifications();
    this.signalRService.startConnection();

    // Start SignalR connection for real-time updates
    // try {
    //   await this.signalRService.startConnection();
    //   this.setupSignalRSubscription();
    // } catch (error) {
    //   console.error('Failed to connect to SignalR, falling back to polling:', error);
    //   // Fallback to polling if SignalR fails
    //   this.setupFallbackPolling();
    // }
  }

  // ngOnDestroy() {
  //   if (this.signalRSubscription) {
  //     this.signalRSubscription.unsubscribe();
  //   }
  //   this.signalRService.stopConnection();
  // }

  // private setupSignalRSubscription() {
  //   // Subscribe to unread count updates and refresh notifications when count changes
  //   this.signalRSubscription = this.signalRService.unreadCount$.subscribe(
  //     (count: number) => {
  //       // Refresh notifications when unread count changes
  //       this.loadNotifications();
  //     }
  //   );
  // }

  // private setupFallbackPolling() {
  //   // Fallback to polling every 30 seconds if SignalR fails
  //   setInterval(() => {
  //     this.loadNotifications();
  //   }, 30000);
  // }

  loadNotifications(pageNumber: number = 1) {
    this.notificationService.getUserNotifications(pageNumber).subscribe((result: NotificationResponse) => {
      if (result.success && result.data) {
        this.notifications = result.data.$values || [];
        console.log(this.notifications);
        this.pagedResult = result.data;
        this.checkUnreadStatus();
      }
    });
  }

  loadPage(pageNumber: number) {
    this.loadNotifications(pageNumber);
  }

  markAsRead(notificationId: string) {
    this.notificationService.markAsRead(notificationId).subscribe(() => {
      const notification = this.notifications.find(n => n.id === notificationId);
      if (notification) {
        notification.isRead = true;
        this.checkUnreadStatus();
      }
    });
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications.forEach(n => n.isRead = true);
      this.hasUnread = false;
      this.checkUnreadStatus();
    });
  }

  deleteNotification(id: string) {
    this.notificationService.deleteNotification(id).subscribe(() => {
      this.notifications = this.notifications.filter(n => n.id !== id);
      this.checkUnreadStatus();
    });
  }

  onNotificationClick(notification: NotificationDto) {
    // Handle notification click - could navigate to related content
    console.log('Notification clicked:', notification);

    // Mark as read if not already read
    if (!notification.isRead) {
      this.markAsRead(notification.id);
    }
    this.notificationService.getUnreadCount().subscribe();
  }

  private checkUnreadStatus() {
    this.notificationService.getUnreadCount().subscribe();
    this.hasUnread = this.notifications.some(n => !n.isRead);
    console.log(this.hasUnread);
  }
}
