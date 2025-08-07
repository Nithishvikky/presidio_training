import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserActivityLogDto } from '../../models/userActivityLogDto';
import { UserActivityLogService } from '../../services/user-activity-log.service';

@Component({
  selector: 'app-activity-log',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="activity-log-container">
      <div class="activity-header">
        <h3>Recent Activities</h3>
        <div class="activity-stats">
          <span class="stat-item">
            <i class="bi bi-clock"></i>
            {{ activities.length }} activities
          </span>
        </div>
      </div>

      <!-- Loading State -->
      <div class="loading-state" *ngIf="loading">
        <div class="spinner-border spinner-border-sm text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <span class="ms-2">Loading activities...</span>
      </div>

      <div class="activity-list" *ngIf="!loading">
        <div *ngFor="let activity of activities; trackBy: trackByActivity"
             class="activity-item"
             [class]="getActivityClass(activity.activityType)">

          <div class="activity-icon">
            <i [class]="getActivityIcon(activity.activityType)"></i>
          </div>

          <div class="activity-content">
            <div class="activity-description">
              {{ activity.description }}
            </div>
            <div class="activity-meta">
              <span class="activity-type">{{ activity.activityType }}</span>
              <span class="activity-time">{{ getFormattedTime(activity.timestamp) }}</span>
            </div>
          </div>

          <!-- <div class="activity-actions">
            <button class="action-btn" (click)="onActivityClick(activity)">
              <i class="bi bi-eye"></i>
            </button>
          </div> -->
        </div>

        <div *ngIf="activities.length === 0" class="no-activities">
          <div class="empty-state">
            <i class="bi bi-activity"></i>
            <h4>No activities yet</h4>
            <p>Your recent activities will appear here</p>
          </div>
        </div>
      </div>

      <div class="activity-footer" *ngIf="showLoadMore && activities.length > 0">
        <button class="load-more-btn" (click)="loadMore.emit()">
          <i class="bi bi-arrow-down"></i>
          Load More Activities
        </button>
      </div>
    </div>
  `,
  styles: [`
    .activity-log-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow: hidden;
    }

    .activity-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 20px;
      border-bottom: 1px solid #e9ecef;
      background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
    }

    .activity-header h3 {
      margin: 0;
      color: #2c3e50;
      font-weight: 600;
      font-size: 18px;
    }

    .activity-stats {
      display: flex;
      gap: 15px;
    }

    .stat-item {
      display: flex;
      align-items: center;
      gap: 6px;
      color: #6c757d;
      font-size: 14px;
      font-weight: 500;
    }

    .stat-item i {
      font-size: 12px;
    }

    .loading-state {
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 40px 20px;
      color: #6c757d;
      font-size: 14px;
    }

    .activity-list {
      max-height: 400px;
      overflow-y: auto;
    }

    .activity-item {
      display: flex;
      align-items: flex-start;
      padding: 16px 20px;
      border-bottom: 1px solid #f8f9fa;
      transition: all 0.2s ease;
      cursor: pointer;
    }

    .activity-item:hover {
      background-color: #f8f9fa;
    }

    .activity-item:last-child {
      border-bottom: none;
    }

    .activity-icon {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-right: 12px;
      flex-shrink: 0;
    }

    .activity-icon i {
      font-size: 16px;
      color: white;
    }

    .activity-item.login .activity-icon {
      background: linear-gradient(135deg, #28a745, #20c997);
    }

    .activity-item.logout .activity-icon {
      background: linear-gradient(135deg, #6c757d, #495057);
    }

    .activity-item.documentview .activity-icon {
      background: linear-gradient(135deg, #007bff, #0056b3);
    }

    .activity-item.documentshare .activity-icon {
      background: linear-gradient(135deg, #ffc107, #e0a800);
    }

    .activity-item.documentupload .activity-icon {
      background: linear-gradient(135deg, #17a2b8, #138496);
    }

    .activity-item.default .activity-icon {
      background: linear-gradient(135deg, #6f42c1, #5a32a3);
    }

    .activity-content {
      flex: 1;
      min-width: 0;
    }

    .activity-description {
      color: #2c3e50;
      font-weight: 500;
      margin-bottom: 4px;
      line-height: 1.4;
    }

    .activity-meta {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .activity-type {
      background: #e9ecef;
      color: #495057;
      padding: 2px 8px;
      border-radius: 12px;
      font-size: 11px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .activity-time {
      color: #6c757d;
      font-size: 12px;
    }

    .activity-actions {
      margin-left: 12px;
    }

    .action-btn {
      background: none;
      border: none;
      color: #6c757d;
      cursor: pointer;
      padding: 6px;
      border-radius: 4px;
      transition: all 0.2s;
    }

    .action-btn:hover {
      background: #e9ecef;
      color: #495057;
    }

    .no-activities {
      padding: 40px 20px;
      text-align: center;
    }

    .empty-state {
      color: #6c757d;
    }

    .empty-state i {
      font-size: 48px;
      margin-bottom: 16px;
      opacity: 0.5;
    }

    .empty-state h4 {
      margin: 0 0 8px 0;
      font-weight: 500;
    }

    .empty-state p {
      margin: 0;
      font-size: 14px;
    }

    .activity-footer {
      padding: 16px 20px;
      border-top: 1px solid #e9ecef;
      text-align: center;
    }

    .load-more-btn {
      background: #007bff;
      color: white;
      border: none;
      padding: 10px 20px;
      border-radius: 6px;
      cursor: pointer;
      font-weight: 500;
      display: flex;
      align-items: center;
      gap: 8px;
      margin: 0 auto;
      transition: background-color 0.2s;
    }

    .load-more-btn:hover {
      background: #0056b3;
    }

    @media (max-width: 768px) {
      .activity-header {
        flex-direction: column;
        gap: 12px;
        align-items: stretch;
      }

      .activity-item {
        padding: 12px 16px;
      }

      .activity-meta {
        flex-direction: column;
        align-items: flex-start;
        gap: 6px;
      }
    }
  `]
})
export class ActivityLogComponent implements OnInit {
  @Input() activities: UserActivityLogDto[] = [];
  @Input() showLoadMore: boolean = false;
  @Output() loadMore = new EventEmitter<void>();
  @Output() activityClick = new EventEmitter<UserActivityLogDto>();

  loading = false;

  constructor(private userActivityLogService: UserActivityLogService) {}

  ngOnInit() {
    this.loadActivities();
  }

  private loadActivities() {
    this.loading = true;
    this.userActivityLogService.getUserActivityLogs(1, 10).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          // Handle the $values array structure from the API response
          this.activities = response.data.$values || response.data || [];
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading activities:', error);
        this.loading = false;
      }
    });
  }

  trackByActivity(index: number, activity: UserActivityLogDto): string {
    return activity.id;
  }

  getActivityClass(activityType: string): string {
    const type = activityType.toLowerCase();
    if (['login', 'logout', 'documentview', 'documentshare', 'documentupload'].includes(type)) {
      return type;
    }
    return 'default';
  }

  getActivityIcon(activityType: string): string {
    const type = activityType.toLowerCase();
    switch (type) {
      case 'login':
        return 'bi bi-box-arrow-in-right';
      case 'logout':
        return 'bi bi-box-arrow-left';
      case 'documentview':
        return 'bi bi-eye';
      case 'documentshare':
        return 'bi bi-share';
      case 'documentupload':
        return 'bi bi-cloud-upload';
      default:
        return 'bi bi-activity';
    }
  }

  getFormattedTime(timestamp: string): string {
    const date = new Date(timestamp);
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

  onActivityClick(activity: UserActivityLogDto) {
    this.activityClick.emit(activity);
  }
}
