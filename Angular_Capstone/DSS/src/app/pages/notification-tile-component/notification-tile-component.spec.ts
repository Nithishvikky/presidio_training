import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotificationTileComponent } from './notification-tile-component';
import { NotificationDto } from '../../models/notificationDto';

describe('NotificationTileComponent', () => {
  let component: NotificationTileComponent;
  let fixture: ComponentFixture<NotificationTileComponent>;

  const mockNotification: NotificationDto = {
    id: 'test-id',
    entityName: 'DocumentShare',
    entityId: 'test-entity-id',
    content: 'kishore has shared the document \'RobloxPlayerInstaller.exe\' with you.',
    createdAt: '2025-07-18T21:44:32.007795Z',
    isRead: false,
    readAt: null
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationTileComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotificationTileComponent);
    component = fixture.componentInstance;
    component.notification = mockNotification;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display notification content', () => {
    const contentElement = fixture.nativeElement.querySelector('.content-text');
    expect(contentElement.textContent).toContain(mockNotification.content);
  });

  it('should display entity name', () => {
    const entityElement = fixture.nativeElement.querySelector('.entity-name');
    expect(entityElement.textContent).toContain(mockNotification.entityName);
  });

  it('should show unread status for unread notifications', () => {
    component.notification.isRead = false;
    fixture.detectChanges();
    
    const tile = fixture.nativeElement.querySelector('.notification-tile');
    expect(tile.classList.contains('unread')).toBe(true);
  });

  it('should show read status for read notifications', () => {
    component.notification.isRead = true;
    fixture.detectChanges();
    
    const tile = fixture.nativeElement.querySelector('.notification-tile');
    expect(tile.classList.contains('read')).toBe(true);
  });

  it('should emit markAsRead event when mark as read button is clicked', () => {
    spyOn(component.markAsRead, 'emit');
    const markReadButton = fixture.nativeElement.querySelector('.action-btn.mark-read');
    
    markReadButton.click();
    
    expect(component.markAsRead.emit).toHaveBeenCalledWith(mockNotification.id);
  });

  it('should emit delete event when delete button is clicked', () => {
    spyOn(component.delete, 'emit');
    const deleteButton = fixture.nativeElement.querySelector('.action-btn.delete');
    
    deleteButton.click();
    
    expect(component.delete.emit).toHaveBeenCalledWith(mockNotification.id);
  });

  it('should emit tileClick event when tile is clicked', () => {
    spyOn(component.tileClick, 'emit');
    const tile = fixture.nativeElement.querySelector('.notification-tile');
    
    tile.click();
    
    expect(component.tileClick.emit).toHaveBeenCalledWith(mockNotification);
  });

  it('should format time correctly', () => {
    const now = new Date();
    const oneHourAgo = new Date(now.getTime() - 60 * 60 * 1000);
    component.notification.createdAt = oneHourAgo.toISOString();
    
    const formattedTime = component.getFormattedTime();
    expect(formattedTime).toBe('1h ago');
  });
}); 