import { ComponentFixture, TestBed } from '@angular/core/testing';
import { App } from './app';

import { of } from 'rxjs';
import { Toast } from 'bootstrap';
import { RouterOutlet } from '@angular/router';
import { LoaderComponent } from './loader-component/loader-component';
import { NotificationResponseDto } from './models/notificationResponseDto';
import { NotificationService } from './services/notification.service';

describe('App Component', () => {
  let component: App;
  let fixture: ComponentFixture<App>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;

  beforeEach(async () => {
    mockNotificationService = jasmine.createSpyObj('NotificationService', ['notification$'], {
      notification$: of([
        { viewerName: 'Alice', fileName: 'report.pdf' } as NotificationResponseDto
      ])
    });

    await TestBed.configureTestingModule({
      imports: [App, LoaderComponent, RouterOutlet], // ✅ App goes here
      providers: [
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
    spyOn(component, 'showToast'); // Spy to prevent actual DOM manipulation
    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should subscribe to notifications and show toast', () => {
    expect(component.notificationMessage.length).toBeGreaterThan(0);
    expect(component.notificationMessage[0].viewerName).toBe('Alice');
    expect(component.showToast).toHaveBeenCalledWith('Alice viewed report.pdf', 'success');
  });
});
