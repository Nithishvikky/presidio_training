import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MenuComponent } from './menu-component';
import { RouterTestingModule } from '@angular/router/testing';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { of, Subject } from 'rxjs';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../services/notification.service';
import { DocumentService } from '../../services/document.service';
import { Router } from '@angular/router';

describe('MenuComponent', () => {
  let component: MenuComponent;
  let fixture: ComponentFixture<MenuComponent>;
  let router: Router;

  const mockNotification$ = new Subject<any>();
  const mockSeenNotifi$ = new Subject<number>();
  const mockUser$ = new Subject<any>();

  beforeEach(async () => {
    const userServiceMock = {
      GetUser: jasmine.createSpy('GetUser').and.returnValue(of({})),
      LogoutUser: jasmine.createSpy('LogoutUser').and.returnValue(of({ data: 'Logged out' })),
      clearUserCache: jasmine.createSpy('clearUserCache'),
      CurrentUser$: mockUser$.asObservable()
    };

    const notificationServiceMock = {
      notification$: mockNotification$.asObservable(),
      SeenNotifi$: mockSeenNotifi$.asObservable(),
      seenNotification: { next: jasmine.createSpy('next') },
      clearNotification: jasmine.createSpy('clearNotification')
    };

    const documentServiceMock = {
      clearDocumentCaches: jasmine.createSpy('clearDocumentCaches')
    };

    await TestBed.configureTestingModule({
      imports: [MenuComponent, RouterTestingModule],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: NotificationService, useValue: notificationServiceMock },
        { provide: DocumentService, useValue: documentServiceMock },
        {
          provide: BreakpointObserver,
          useValue: {
            observe: () => of({ matches: true })
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MenuComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    spyOn(component, 'showToast').and.callFake(() => {}); // mock toast
    localStorage.setItem('authData', JSON.stringify({
      role: 'Admin',
      refreshToken: 'dummyToken',
      email: 'admin@example.com'
    }));
    fixture.detectChanges();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize user and role from localStorage', () => {
    expect(component.role).toBe('Admin');
    expect(component.refreshToken).toBe('dummyToken');
  });

  it('should subscribe to current user and set username', () => {
    mockUser$.next({ username: 'testUser' });
    expect(component.username).toBe('testUser');
  });

  it('should collapse on small screens', () => {
    expect(component.collapsed).toBeTrue();
  });

  it('should calculate unseen notifications', () => {
    mockNotification$.next([{ msg: 'One' }, { msg: 'Two' }]);
    expect(component.notification.length).toBe(2);
    expect(component.UnseenNotification).toBe(2);

    mockSeenNotifi$.next(1);
    expect(component.UnseenNotification).toBe(1);
  });

  it('should call seenNotification on modal click', () => {
    component.notification = [
      {
        docViewId: '1',
        viewerName: 'Alice',
        viewerEmail: 'alice@example.com',
        fileName: 'file1.pdf',
        viewedAt: new Date()
      },
      {
        docViewId: '2',
        viewerName: 'Bob',
        viewerEmail: 'bob@example.com',
        fileName: 'file2.pdf',
        viewedAt: new Date()
      },
      {
        docViewId: '3',
        viewerName: 'Charlie',
        viewerEmail: 'charlie@example.com',
        fileName: 'file3.pdf',
        viewedAt: new Date()
      }
    ];

    component.OnModalNotification();
    expect(component['notifyService'].seenNotification.next).toHaveBeenCalledWith(3);
  });

  it('should collapse navbar if open', () => {
    const fakeElement = document.createElement('div');
    fakeElement.classList.add('show');
    fakeElement.id = 'navbarNavAltMarkup';
    document.body.appendChild(fakeElement);
    spyOn(document, 'getElementById').and.returnValue(fakeElement);
    component.collapseNavbar();
    expect(fakeElement.classList.contains('show')).toBeFalse();
    fakeElement.remove();
  });

  it('should call services and navigate on sign out', fakeAsync(() => {
    spyOn(router, 'navigateByUrl');

    component.OnSignOut();
    tick(2000); // for setTimeout
    expect(component['userService'].LogoutUser).toHaveBeenCalledWith('dummyToken');
    expect(component['notifyService'].clearNotification).toHaveBeenCalled();
    expect(component['userService'].clearUserCache).toHaveBeenCalled();
    expect(router.navigateByUrl).toHaveBeenCalledWith('/auth/signin');
    expect(component.showToast).toHaveBeenCalledWith('Logged out', 'success');
  }));

  it('should toggle dropdown state', () => {
    expect(component.isDropdownOpen).toBeFalse();
    component.toggleDropdown();
    expect(component.isDropdownOpen).toBeTrue();
    component.toggleDropdown();
    expect(component.isDropdownOpen).toBeFalse();
  });

  it('should close dropdown after short delay', fakeAsync(() => {
    component.isDropdownOpen = true;
    component.closeDropdown();
    tick(100);
    expect(component.isDropdownOpen).toBeFalse();
  }));
});
