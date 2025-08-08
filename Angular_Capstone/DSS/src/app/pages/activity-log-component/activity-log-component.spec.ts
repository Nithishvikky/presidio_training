import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivityLogComponent } from './activity-log-component';
import { UserActivityLogDto } from '../../models/userActivityLogDto';

describe('ActivityLogComponent', () => {
  let component: ActivityLogComponent;
  let fixture: ComponentFixture<ActivityLogComponent>;

  const mockActivities: UserActivityLogDto[] = [
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
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActivityLogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActivityLogComponent);
    component = fixture.componentInstance;
    component.activities = mockActivities;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display activities', () => {
    const activityItems = fixture.nativeElement.querySelectorAll('.activity-item');
    expect(activityItems.length).toBe(2);
  });

  it('should display activity descriptions', () => {
    const descriptions = fixture.nativeElement.querySelectorAll('.activity-description');
    expect(descriptions[0].textContent).toContain('User logged in successfully');
    expect(descriptions[1].textContent).toContain('Viewed document "Sample.pdf"');
  });

  it('should display activity types', () => {
    const types = fixture.nativeElement.querySelectorAll('.activity-type');
    expect(types[0].textContent).toContain('LOGIN');
    expect(types[1].textContent).toContain('DOCUMENTVIEW');
  });

  it('should emit activityClick when activity is clicked', () => {
    spyOn(component.activityClick, 'emit');
    const activityItem = fixture.nativeElement.querySelector('.activity-item');
    
    activityItem.click();
    
    expect(component.activityClick.emit).toHaveBeenCalledWith(mockActivities[0]);
  });

  it('should show empty state when no activities', () => {
    component.activities = [];
    fixture.detectChanges();
    
    const emptyState = fixture.nativeElement.querySelector('.empty-state');
    expect(emptyState).toBeTruthy();
  });

  it('should format time correctly', () => {
    const now = new Date();
    const oneHourAgo = new Date(now.getTime() - 60 * 60 * 1000);
    const activity = { ...mockActivities[0], timestamp: oneHourAgo.toISOString() };
    
    const formattedTime = component.getFormattedTime(activity.timestamp);
    expect(formattedTime).toBe('1h ago');
  });

  it('should get correct activity class', () => {
    expect(component.getActivityClass('Login')).toBe('login');
    expect(component.getActivityClass('DocumentView')).toBe('documentview');
    expect(component.getActivityClass('Unknown')).toBe('default');
  });

  it('should get correct activity icon', () => {
    expect(component.getActivityIcon('Login')).toBe('bi bi-box-arrow-in-right');
    expect(component.getActivityIcon('DocumentView')).toBe('bi bi-eye');
    expect(component.getActivityIcon('Unknown')).toBe('bi bi-activity');
  });
}); 