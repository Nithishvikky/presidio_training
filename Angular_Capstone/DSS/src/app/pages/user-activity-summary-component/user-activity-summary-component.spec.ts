import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserActivitySummaryComponent } from './user-activity-summary-component';
import { UserActivitySummaryDto, UserActivityLogDto, ActivityTypeBreakdown } from '../../models/userActivityLogDto';

describe('UserActivitySummaryComponent', () => {
  let component: UserActivitySummaryComponent;
  let fixture: ComponentFixture<UserActivitySummaryComponent>;

  const mockActivitySummary: UserActivitySummaryDto = {
    userId: 'user1',
    userEmail: 'user@example.com',
    totalActivities: 25,
    lastActivityDate: '2025-07-18T21:44:32.007795Z',
    isActive: true,
    recentActivities: [
      {
        id: '1',
        userId: 'user1',
        userEmail: 'user@example.com',
        userUsername: 'testuser',
        activityType: 'Login',
        description: 'User logged in successfully',
        timestamp: '2025-07-18T21:44:32.007795Z'
      },
      {
        id: '2',
        userId: 'user1',
        userEmail: 'user@example.com',
        userUsername: 'testuser',
        activityType: 'DocumentView',
        description: 'Viewed document "Sample.pdf"',
        timestamp: '2025-07-18T20:30:15.123456Z'
      }
    ],
    activityTypeBreakdown: [
      { activityType: 'Login', count: 10 },
      { activityType: 'DocumentView', count: 8 },
      { activityType: 'DocumentShare', count: 5 },
      { activityType: 'Logout', count: 2 }
    ]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserActivitySummaryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserActivitySummaryComponent);
    component = fixture.componentInstance;
    component.activitySummary = mockActivitySummary;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display total activities', () => {
    const totalActivitiesElement = fixture.nativeElement.querySelector('.stat-value');
    expect(totalActivitiesElement.textContent).toContain('25');
  });

  it('should display activity status', () => {
    const statusElement = fixture.nativeElement.querySelector('.status-indicator');
    expect(statusElement.textContent).toContain('Active');
  });

  it('should format last activity date correctly', () => {
    const lastActivityElement = fixture.nativeElement.querySelectorAll('.stat-value')[1];
    expect(lastActivityElement.textContent).toContain('Today');
  });

  it('should display recent activities count', () => {
    const activitiesElement = fixture.nativeElement.querySelectorAll('.stat-value')[2];
    expect(activitiesElement.textContent).toContain('2');
  });

  it('should show activity type breakdown', () => {
    const breakdown = component.getActivityTypeBreakdown();
    expect(breakdown.length).toBe(4);
    expect(breakdown[0].activityType).toBe('Login');
    expect(breakdown[0].count).toBe(10);
  });

  it('should handle null activity summary', () => {
    component.activitySummary = null;
    fixture.detectChanges();
    
    const totalActivitiesElement = fixture.nativeElement.querySelector('.stat-value');
    expect(totalActivitiesElement.textContent).toContain('0');
  });

  it('should format last activity as "Never" when no date', () => {
    component.activitySummary = { ...mockActivitySummary, lastActivityDate: '' };
    const formattedDate = component.getFormattedLastActivity();
    expect(formattedDate).toBe('Never');
  });

  it('should get correct activity class', () => {
    expect(component.getActivityClass('Login')).toBe('login');
    expect(component.getActivityClass('DocumentView')).toBe('documentview');
    expect(component.getActivityClass('Unknown')).toBe('default');
  });

  it('should get correct activity icon', () => {
    expect(component.getActivityIcon('Login')).toBe('bi bi-box-arrow-in-right');
    expect(component.getActivityIcon('DocumentView')).toBe('bi bi-eye');
    expect(component.getActivityIcon('DocumentShare')).toBe('bi bi-share');
    expect(component.getActivityIcon('Unknown')).toBe('bi bi-activity');
  });
}); 