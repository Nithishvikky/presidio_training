import { ComponentFixture, TestBed } from '@angular/core/testing';
import { App } from './app';
import { RouterOutlet } from '@angular/router';
import { NotificationService } from './services/notification.service';
import { of } from 'rxjs';
import { NotificationResponseDto } from './models/notificationResponseDto';
import { LoaderComponent } from './loader-component/loader-component';


describe('App Component', () => {
  let component: App;
  let fixture: ComponentFixture<App>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;

  beforeEach(async () => {
    mockNotificationService = jasmine.createSpyObj('NotificationService', [
      'addNotification',
      'addUserNotification'
    ], {
      notification$: of([
        { viewerName: 'Alice', fileName: 'report.pdf' } as NotificationResponseDto
      ])
    });

    await TestBed.configureTestingModule({
      imports: [App, LoaderComponent, RouterOutlet],
      providers: [
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    // Simulate authData in localStorage
    localStorage.setItem('authData', JSON.stringify({ role: 'Admin' }));

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;

    spyOn(component, 'showToast'); // Avoid DOM changes during test

    fixture.detectChanges(); // Triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
