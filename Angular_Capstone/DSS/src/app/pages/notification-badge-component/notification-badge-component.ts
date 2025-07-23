import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { SignalRService } from '../../services/signalr.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-notification-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notification-badge" (click)="navigateToNotifications()">
      <div class="bell-container">
        <i class="bi bi-bell"></i>
        <span *ngIf="unreadCount > 0" class="badge">
          {{ unreadCount > 99 ? '99+' : unreadCount }}
        </span>
      </div>
    </div>
  `,
  styles: [`
    .notification-badge {
      position: relative;
      cursor: pointer;
      padding: 8px;
      transition: all 0.2s ease;
    }

    .notification-badge:hover {
      transform: scale(1.05);
    }

    .bell-container {
      position: relative;
      display: inline-block;
    }

    .bell-container i {
      font-size: 20px;
      color: #666;
      transition: color 0.2s ease;
    }

    .notification-badge:hover i {
      color: #333;
    }

    .badge {
      position: absolute;
      top: -8px;
      right: -8px;
      background-color: #ff4444;
      color: white;
      border-radius: 50%;
      padding: 2px 6px;
      font-size: 10px;
      font-weight: bold;
      min-width: 16px;
      height: 16px;
      text-align: center;
      line-height: 12px;
      border: 2px solid white;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
      animation: pulse 2s infinite;
    }

    @keyframes pulse {
      0% {
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
      }
      50% {
        box-shadow: 0 2px 8px rgba(255, 68, 68, 0.4);
      }
      100% {
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
      }
    }

    /* Responsive design */
    @media (max-width: 768px) {
      .bell-container i {
        font-size: 18px;
      }
      
      .badge {
        font-size: 9px;
        min-width: 14px;
        height: 14px;
        line-height: 10px;
      }
    }
  `]
})
export class NotificationBadgeComponent implements OnInit, OnDestroy {
  unreadCount = 0;
  private signalRSubscription?: Subscription;

  constructor(
    private notificationService: NotificationService,
    private signalRService: SignalRService,
    private router: Router
  ) {}

  async ngOnInit() {
    // Load initial unread count
    await this.loadUnreadCount();
    
    // Start SignalR connection and subscribe to real-time updates
    try {
      await this.signalRService.startConnection();
      this.setupSignalRSubscription();
    } catch (error) {
      console.error('Failed to connect to SignalR, falling back to polling:', error);
      // Fallback to polling if SignalR fails
      this.setupFallbackPolling();
    }
  }

  ngOnDestroy() {
    if (this.signalRSubscription) {
      this.signalRSubscription.unsubscribe();
    }
    this.signalRService.stopConnection();
  }

  private setupSignalRSubscription() {
    this.signalRSubscription = this.signalRService.unreadCount$.subscribe(
      (count: number) => {
        this.unreadCount = count;
      }
    );
  }

  private setupFallbackPolling() {
    // Fallback to polling every 30 seconds if SignalR fails
    setInterval(() => {
      this.loadUnreadCount();
    }, 30000);
  }

  private async loadUnreadCount() {
    try {
      const result = await this.notificationService.getUnreadCount().toPromise();
      this.unreadCount = result?.count || 0;
    } catch (error) {
      console.error('Error loading unread count:', error);
    }
  }

  navigateToNotifications() {
    this.router.navigate(['/main/notifications']);
  }
} 