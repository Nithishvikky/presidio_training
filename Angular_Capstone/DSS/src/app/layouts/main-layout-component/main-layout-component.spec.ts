import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MainLayoutComponent } from './main-layout-component';
import { ActivatedRoute, RouterOutlet } from '@angular/router';
import { DocumentService } from '../../services/document.service';
import { of } from 'rxjs';
import { UserService } from '../../services/user.service';
import { MenuComponent } from '../../pages/menu-component/menu-component';
import { NotificationService } from '../../services/notification.service';

describe('MainLayoutComponent', () => {
  let component: MainLayoutComponent;
  let fixture: ComponentFixture<MainLayoutComponent>;

  const mockUserService = {
    CurrentUser$: of({ username: 'Test', role: 'Admin' }),
    GetUser: jasmine.createSpy('GetUser').and.returnValue(of({}))
  };

  const mockNotificationService = {
    notification$: of([]),
    addNotification: jasmine.createSpy('addNotification'),
    addUserNotification: jasmine.createSpy('addUserNotification')
  };

  const mockDocumentService = {
    clearDocumentCaches: jasmine.createSpy('clearDocumentCaches')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MainLayoutComponent, RouterOutlet, MenuComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: DocumentService, useValue: mockDocumentService },
        { provide: ActivatedRoute, useValue: { snapshot: { params: {} } } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MainLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
