import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProfileComponent } from './profile-component';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of } from 'rxjs';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../services/notification.service';
import { DocumentService } from '../../services/document.service';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { Router } from '@angular/router';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { UserResponseDto } from '../../models/userResponseDto';

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;

  let mockUserService: jasmine.SpyObj<UserService>;
  let mockDocumentService: jasmine.SpyObj<DocumentService>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;
  let mockAccessService: jasmine.SpyObj<DocumentAccessService>;
  let mockRouter: jasmine.SpyObj<Router>;

  const mockUser: UserResponseDto = {
    userId: '1',
    email: 'user@example.com',
    username: 'TestUser',
    role: 'User',
    registeredAt: new Date(),
    updatedAt: new Date(),
    documentCount: 5
  };

  beforeEach(async () => {
    mockUserService = jasmine.createSpyObj('UserService', ['GetUser', 'LogoutUser', 'ChangePassword', 'clearUserCache'], {
      CurrentUser$: of(mockUser)
    });

    // ✅ Fix: mock GetUser to return observable
    mockUserService.GetUser.and.returnValue(of(mockUser));

    mockDocumentService = jasmine.createSpyObj('DocumentService', ['clearDocumentCaches']);
    mockNotificationService = jasmine.createSpyObj('NotificationService', ['clearNotification']);

    mockAccessService = jasmine.createSpyObj('DocumentAccessService', ['GetDocumentShared'], {
      sharedFiles$: of([{ id: 1 }, { id: 2 }])
    });

    mockAccessService.GetDocumentShared.and.returnValue(of([])); // ✅ mock subscribe

    mockRouter = jasmine.createSpyObj('Router', ['navigateByUrl']);

    await TestBed.configureTestingModule({
      imports: [CommonModule, ReactiveFormsModule, ProfileComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: DocumentService, useValue: mockDocumentService },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: DocumentAccessService, useValue: mockAccessService },
        { provide: Router, useValue: mockRouter }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    localStorage.setItem('authData', JSON.stringify({
      email: mockUser.email,
      role: 'User',
      refreshToken: 'mockRefreshToken'
    }));

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });


  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call logout and navigate on sign out', () => {
    mockUserService.LogoutUser.and.returnValue(of({ data: 'Logged out' }));
    spyOn(component, 'showToast');

    component.OnSignOut();

    expect(mockUserService.LogoutUser).toHaveBeenCalledWith('mockRefreshToken');
    expect(mockUserService.clearUserCache).toHaveBeenCalled();
    expect(mockNotificationService.clearNotification).toHaveBeenCalled();
    expect(mockDocumentService.clearDocumentCaches).toHaveBeenCalled();
    expect(component.showToast).toHaveBeenCalledWith('Logged out', 'success');
  });
});
