import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ProfileComponent } from './profile-component';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../services/notification.service';
import { DocumentService } from '../../services/document.service';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let notificationServiceSpy: jasmine.SpyObj<NotificationService>;
  let documentServiceSpy: jasmine.SpyObj<DocumentService>;
  let documentAccessServiceSpy: jasmine.SpyObj<DocumentAccessService>;

  beforeEach(async () => {
    const userServiceMock = jasmine.createSpyObj('UserService', ['GetUser', 'LogoutUser', 'ChangePassword', 'clearUserCache'], { CurrentUser$: of({ username: 'testuser', email: 'test@example.com', role: 'User', registeredAt: new Date(), updatedAt: new Date(), documentCount: 3 }) });
    const notificationServiceMock = jasmine.createSpyObj('NotificationService', ['clearNotification']);
    const documentServiceMock = jasmine.createSpyObj('DocumentService', ['clearDocumentCaches']);
    const documentAccessServiceMock = jasmine.createSpyObj('DocumentAccessService', ['GetDocumentShared'], { sharedFiles$: of([{ id: 1 }, { id: 2 }]) });

    await TestBed.configureTestingModule({
      imports: [ProfileComponent, CommonModule, ReactiveFormsModule, RouterTestingModule],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: NotificationService, useValue: notificationServiceMock },
        { provide: DocumentService, useValue: documentServiceMock },
        { provide: DocumentAccessService, useValue: documentAccessServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    notificationServiceSpy = TestBed.inject(NotificationService) as jasmine.SpyObj<NotificationService>;
    documentServiceSpy = TestBed.inject(DocumentService) as jasmine.SpyObj<DocumentService>;
    documentAccessServiceSpy = TestBed.inject(DocumentAccessService) as jasmine.SpyObj<DocumentAccessService>;

    spyOn(component, 'showToast').and.callFake(() => {});
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should sign out and clear data', () => {
    const mockResponse = { data: 'Signed out successfully' };
    userServiceSpy.LogoutUser.and.returnValue(of(mockResponse));

    localStorage.setItem('authData', JSON.stringify({ refreshToken: 'token', email: 'test@example.com' }));

    component.OnSignOut();

    expect(userServiceSpy.LogoutUser).toHaveBeenCalledWith('token');
    expect(userServiceSpy.clearUserCache).toHaveBeenCalled();
    expect(notificationServiceSpy.clearNotification).toHaveBeenCalled();
    expect(documentServiceSpy.clearDocumentCaches).toHaveBeenCalled();
    expect(component.showToast).toHaveBeenCalledWith('Signed out successfully', 'success');
  });

  it('should call ChangePassword on submit and show success toast', fakeAsync(() => {
    userServiceSpy.ChangePassword.and.returnValue(of({}));
    localStorage.setItem('authData', JSON.stringify({ email: 'test@example.com' }));

    component.passwordChangeForm.setValue({ OldPassword: 'OldPass123!', NewPassword: 'NewPass@123' });
    component.userEmail = 'test@example.com';

    component.handleSubmit();
    tick(2000);

    expect(userServiceSpy.ChangePassword).toHaveBeenCalled();
    expect(component.showToast).toHaveBeenCalledWith('Password updated successfully', 'success');
  }));

  it('should handle ChangePassword error and show danger toast', () => {
    const error = { error: { error: { errorMessage: 'Invalid password' } } };
    userServiceSpy.ChangePassword.and.returnValue(throwError(() => error));

    component.passwordChangeForm.setValue({ OldPassword: 'wrong', NewPassword: 'new' });
    component.userEmail = 'test@example.com';
    component.handleSubmit();

    expect(component.showToast).toHaveBeenCalledWith('Invalid password', 'danger');
  });
});
