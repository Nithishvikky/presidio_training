import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationDto } from '../../models/notificationDto';

@Component({
  selector: 'app-notification-tile',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notification-tile" 
         [class.unread]="!notification.isRead"
         [class.read]="notification.isRead"
         (click)="onTileClick()">
      
      <div class="tile-header">
        <div class="entity-info">
          <span class="entity-name">{{ notification.entityName }}</span>
          <span class="entity-id">#{{ notification.entityId.slice(0, 8) }}</span>
        </div>
        <div class="status-indicator">
          <div class="status-dot" [class.unread]="!notification.isRead"></div>
        </div>
      </div>

      <div class="tile-content">
        <p class="content-text">{{ notification.content }}</p>
      </div>

      <div class="tile-footer">
        <div class="timestamp">
          <i class="bi bi-clock"></i>
          {{ getFormattedTime() }}
        </div>
        <div class="actions">
          <button class="action-btn mark-read" 
                  *ngIf="!notification.isRead"
                  (click)="onMarkAsRead($event)">
            <i class="bi bi-check-circle"></i>
            Mark Read
          </button>
          <button class="action-btn delete" 
                  (click)="onDelete($event)">
            <i class="bi bi-trash"></i>
            Delete
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .notification-tile {
      background: white;
      border-radius: 12px;
      padding: 16px;
      margin-bottom: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      border: 1px solid #e0e0e0;
      transition: all 0.3s ease;
      cursor: pointer;
    }

    .notification-tile:hover {
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
      transform: translateY(-2px);
    }

    .notification-tile.unread {
      border-left: 4px solid #007bff;
      background: linear-gradient(135deg, #f8f9ff 0%, #ffffff 100%);
    }

    .notification-tile.read {
      opacity: 0.8;
      background: #fafafa;
    }

    .tile-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 12px;
    }

    .entity-info {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .entity-name {
      font-weight: 600;
      color: #2c3e50;
      font-size: 14px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .entity-id {
      background: #e9ecef;
      color: #6c757d;
      padding: 2px 6px;
      border-radius: 4px;
      font-size: 11px;
      font-family: monospace;
    }

    .status-indicator {
      display: flex;
      align-items: center;
    }

    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: #28a745;
    }

    .status-dot.unread {
      background: #ffc107;
      animation: pulse 2s infinite;
    }

    @keyframes pulse {
      0% { opacity: 1; }
      50% { opacity: 0.5; }
      100% { opacity: 1; }
    }

    .tile-content {
      margin-bottom: 12px;
    }

    .content-text {
      color: #495057;
      line-height: 1.5;
      margin: 0;
      font-size: 14px;
    }

    .tile-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .timestamp {
      display: flex;
      align-items: center;
      gap: 4px;
      color: #6c757d;
      font-size: 12px;
    }

    .timestamp i {
      font-size: 10px;
    }

    .actions {
      display: flex;
      gap: 8px;
    }

    .action-btn {
      display: flex;
      align-items: center;
      gap: 4px;
      padding: 6px 12px;
      border: none;
      border-radius: 6px;
      font-size: 12px;
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .action-btn.mark-read {
      background: #28a745;
      color: white;
    }

    .action-btn.mark-read:hover {
      background: #218838;
    }

    .action-btn.delete {
      background: #dc3545;
      color: white;
    }

    .action-btn.delete:hover {
      background: #c82333;
    }

    .action-btn i {
      font-size: 10px;
    }

    @media (max-width: 768px) {
      .notification-tile {
        padding: 12px;
      }

      .tile-footer {
        flex-direction: column;
        align-items: flex-start;
        gap: 8px;
      }

      .actions {
        width: 100%;
        justify-content: flex-end;
      }
    }
  `]
})
export class NotificationTileComponent {
  @Input() notification!: NotificationDto;
  @Output() markAsRead = new EventEmitter<string>();
  @Output() delete = new EventEmitter<string>();
  @Output() tileClick = new EventEmitter<NotificationDto>();

  onMarkAsRead(event: Event) {
    event.stopPropagation();
    this.markAsRead.emit(this.notification.id);
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.notification.id);
  }

  onTileClick() {
    this.tileClick.emit(this.notification);
  }

  getFormattedTime(): string {
    const date = new Date(this.notification.createdAt);
    const now = new Date();
    const diffInMinutes = Math.floor((now.getTime() - date.getTime()) / (1000 * 60));

    if (diffInMinutes < 1) {
      return 'Just now';
    } else if (diffInMinutes < 60) {
      return `${diffInMinutes}m ago`;
    } else if (diffInMinutes < 1440) {
      const hours = Math.floor(diffInMinutes / 60);
      return `${hours}h ago`;
    } else {
      const days = Math.floor(diffInMinutes / 1440);
      return `${days}d ago`;
    }
  }
} 