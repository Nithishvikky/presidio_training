import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserActivitySummaryDto, ActivityTypeBreakdown, UserActivityLogDto } from '../../models/userActivityLogDto';
import { UserActivityLogService } from '../../services/user-activity-log.service';

@Component({
  selector: 'app-user-activity-summary',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="activity-summary-container">
      <div class="summary-header">
        <h3>Activity Summary</h3>
        <div class="status-indicator" [class.active]="activitySummary?.isActive">
          <span class="status-dot"></span>
          {{ activitySummary?.isActive ? 'Active' : 'Inactive' }}
        </div>
      </div>

      <div class="summary-stats">
        <div class="stat-card">
          <div class="stat-icon">
            <i class="bi bi-activity"></i>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ activitySummary?.totalActivities || 0 }}</div>
            <div class="stat-label">Total Activities</div>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon">
            <i class="bi bi-calendar-check"></i>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ getFormattedLastActivity() }}</div>
            <div class="stat-label">Last Activity</div>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon">
            <i class="bi bi-clock-history"></i>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ recentActivities.length }}</div>
            <div class="stat-label">Recent Activities</div>
          </div>
        </div>
      </div>

      <div class="activity-insights" *ngIf="getActivityTypeBreakdown().length > 0">
        <h4>Activity Types</h4>
        <div class="insights-grid">
          <div *ngFor="let breakdown of getActivityTypeBreakdown()" class="insight-item">
            <div class="insight-icon" [class]="getActivityClass(breakdown.activityType)">
              <i [class]="getActivityIcon(breakdown.activityType)"></i>
            </div>
            <div class="insight-content">
              <div class="insight-count">{{ breakdown.count }}</div>
              <div class="insight-type">{{ breakdown.activityType }}</div>
            </div>
          </div>
        </div>
      </div>

      

      <!-- Loading State -->
      <div class="loading-state" *ngIf="loading">
        <div class="spinner-border spinner-border-sm text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <span class="ms-2">Loading activities...</span>
      </div>
    </div>
  `,
  styles: [`
    .activity-summary-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      padding: 24px;
    }

    .summary-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
      padding-bottom: 16px;
      border-bottom: 1px solid #e9ecef;
    }

    .summary-header h3 {
      margin: 0;
      color: #2c3e50;
      font-weight: 600;
      font-size: 18px;
    }

    .status-indicator {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 14px;
      font-weight: 500;
    }

    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: #dc3545;
    }

    .status-indicator.active .status-dot {
      background: #28a745;
    }

    .status-indicator.active {
      color: #28a745;
    }

    .summary-stats {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 16px;
      margin-bottom: 24px;
    }

    .stat-card {
      display: flex;
      align-items: center;
      padding: 16px;
      background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
      border-radius: 8px;
      border: 1px solid #e9ecef;
      transition: all 0.2s ease;
    }

    .stat-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }

    .stat-icon {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: linear-gradient(135deg, #007bff, #0056b3);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-right: 12px;
      flex-shrink: 0;
    }

    .stat-icon i {
      color: white;
      font-size: 16px;
    }

    .stat-content {
      flex: 1;
    }

    .stat-value {
      font-size: 20px;
      font-weight: 700;
      color: #2c3e50;
      line-height: 1;
      margin-bottom: 4px;
    }

    .stat-label {
      font-size: 12px;
      color: #6c757d;
      font-weight: 500;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .activity-insights {
      border-top: 1px solid #e9ecef;
      padding-top: 20px;
      margin-bottom: 24px;
    }

    .activity-insights h4 {
      margin: 0 0 16px 0;
      color: #2c3e50;
      font-weight: 600;
      font-size: 16px;
    }

    .insights-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
      gap: 12px;
    }

    .insight-item {
      display: flex;
      align-items: center;
      padding: 12px;
      background: #f8f9fa;
      border-radius: 6px;
      transition: background-color 0.2s;
    }

    .insight-item:hover {
      background: #e9ecef;
    }

    .insight-icon {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-right: 8px;
      flex-shrink: 0;
    }

    .insight-icon i {
      color: white;
      font-size: 12px;
    }

    .insight-icon.login {
      background: linear-gradient(135deg, #28a745, #20c997);
    }

    .insight-icon.logout {
      background: linear-gradient(135deg, #6c757d, #495057);
    }

    .insight-icon.documentview {
      background: linear-gradient(135deg, #007bff, #0056b3);
    }

    .insight-icon.documentshare {
      background: linear-gradient(135deg, #ffc107, #e0a800);
    }

    .insight-icon.documentupload {
      background: linear-gradient(135deg, #17a2b8, #138496);
    }

    .insight-icon.default {
      background: linear-gradient(135deg, #6f42c1, #5a32a3);
    }

    .insight-content {
      flex: 1;
    }

    .insight-count {
      font-size: 16px;
      font-weight: 600;
      color: #2c3e50;
      line-height: 1;
    }

    .insight-type {
      font-size: 10px;
      color: #6c757d;
      font-weight: 500;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .recent-activities {
      border-top: 1px solid #e9ecef;
      padding-top: 20px;
    }

    .recent-activities h4 {
      margin: 0 0 16px 0;
      color: #2c3e50;
      font-weight: 600;
      font-size: 16px;
    }

    .activities-list {
      max-height: 300px;
      overflow-y: auto;
    }

    .activity-item {
      display: flex;
      align-items: flex-start;
      padding: 12px;
      border-bottom: 1px solid #f8f9fa;
      transition: background-color 0.2s;
    }

    .activity-item:hover {
      background-color: #f8f9fa;
    }

    .activity-item:last-child {
      border-bottom: none;
    }

    .activity-item .activity-icon {
      width: 28px;
      height: 28px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-right: 10px;
      flex-shrink: 0;
    }

    .activity-item .activity-icon i {
      color: white;
      font-size: 10px;
    }

    .activity-item .activity-icon.login {
      background: linear-gradient(135deg, #28a745, #20c997);
    }

    .activity-item .activity-icon.logout {
      background: linear-gradient(135deg, #6c757d, #495057);
    }

    .activity-item .activity-icon.documentview {
      background: linear-gradient(135deg, #007bff, #0056b3);
    }

    .activity-item .activity-icon.documentshare {
      background: linear-gradient(135deg, #ffc107, #e0a800);
    }

    .activity-item .activity-icon.documentupload {
      background: linear-gradient(135deg, #17a2b8, #138496);
    }

    .activity-item .activity-icon.default {
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
      font-size: 13px;
    }

    .activity-meta {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .activity-type {
      background: #e9ecef;
      color: #495057;
      padding: 1px 6px;
      border-radius: 10px;
      font-size: 9px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .activity-time {
      color: #6c757d;
      font-size: 11px;
    }

    .view-all-link {
      text-align: center;
      padding-top: 12px;
      border-top: 1px solid #f8f9fa;
      margin-top: 12px;
    }

    .view-all-link a {
      color: #007bff;
      text-decoration: none;
      font-size: 13px;
      font-weight: 500;
    }

    .view-all-link a:hover {
      text-decoration: underline;
    }

    .loading-state {
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 20px;
      color: #6c757d;
      font-size: 14px;
    }

    @media (max-width: 768px) {
      .activity-summary-container {
        padding: 16px;
      }

      .summary-header {
        flex-direction: column;
        gap: 12px;
        align-items: stretch;
      }

      .summary-stats {
        grid-template-columns: 1fr;
      }

      .insights-grid {
        grid-template-columns: repeat(2, 1fr);
      }

      .activity-meta {
        flex-direction: column;
        align-items: flex-start;
        gap: 4px;
      }
    }
  `]
})
export class UserActivitySummaryComponent implements OnInit {
  @Input() activitySummary: UserActivitySummaryDto | null = null;
  recentActivities: UserActivityLogDto[] = [];
  loading = false;

  constructor(private userActivityLogService: UserActivityLogService) {}

  ngOnInit() {
    this.loadRecentActivities();
  }

  private loadRecentActivities() {
    this.loading = true;
    this.userActivityLogService.getUserActivityLogs(1, 10).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          // Handle the $values array structure from the API response
          this.recentActivities = response.data.$values || response.data || [];
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading recent activities:', error);
        this.loading = false;
      }
    });
  }

  getFormattedLastActivity(): string {
    if (!this.activitySummary?.lastActivityDate) {
      return 'Never';
    }

    const lastActivity = new Date(this.activitySummary.lastActivityDate);
    const now = new Date();
    const diffInDays = Math.floor((now.getTime() - lastActivity.getTime()) / (1000 * 60 * 60 * 24));

    if (diffInDays === 0) {
      return 'Today';
    } else if (diffInDays === 1) {
      return 'Yesterday';
    } else if (diffInDays < 7) {
      return `${diffInDays} days ago`;
    } else {
      return lastActivity.toLocaleDateString();
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

  getActivityTypeBreakdown(): ActivityTypeBreakdown[] {
    if (!this.activitySummary?.activityTypeBreakdown) {
      return [];
    }

    return this.activitySummary.activityTypeBreakdown
      .sort((a, b) => b.count - a.count)
      .slice(0, 4); // Show top 4 activity types
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

  onViewAllActivities(event: Event) {
    event.preventDefault();
    // Navigate to full activity log page or show modal
    console.log('View all activities clicked');
  }
} 