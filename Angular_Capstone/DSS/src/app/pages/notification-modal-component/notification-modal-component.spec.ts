import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotificationModalComponent } from './notification-modal-component';
import { NotificationService } from '../../services/notification.service';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';


interface NotificationResponseDto {
  viewerName: string;
  fileName: string;
  type: 'view' | 'shared';
}

interface NotificationSharedResponseDto {
  userName: string;
  fileName: string;
  type: 'view' | 'shared';
}

describe('NotificationModalComponent', () => {
  let component: NotificationModalComponent;
  let fixture: ComponentFixture<NotificationModalComponent>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;

  beforeEach(async () => {
    mockNotificationService = jasmine.createSpyObj('NotificationService', ['clearCurrentNotification'], {
      notification$: of([
        {
          viewerName: 'Alice',
          fileName: 'report.pdf',
          type: 'view'
        } as NotificationResponseDto
      ])
    });

    await TestBed.configureTestingModule({
      imports: [CommonModule, NotificationModalComponent],
      providers: [
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    // Set mock auth data for Admin
    localStorage.setItem('authData', JSON.stringify({ role: 'Admin' }));

    fixture = TestBed.createComponent(NotificationModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should assign notification messages for Admin role', () => {
    expect(component.notificationMessage.length).toBeGreaterThan(0);
    expect(component.notificationSharedMessage.length).toBe(0);
  });

  it('should call clear on clear()', () => {
    component.clear();
    expect(mockNotificationService.clearCurrentNotification).toHaveBeenCalled();
  });
});
