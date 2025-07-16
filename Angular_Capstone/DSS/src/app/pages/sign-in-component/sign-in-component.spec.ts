import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SignInComponent } from './sign-in-component';
import { UserService } from '../../services/user.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { NotificationService } from '../../services/notification.service';
import { DocumentService } from '../../services/document.service';

describe('SignInComponent', () => {
  let component: SignInComponent;
  let fixture: ComponentFixture<SignInComponent>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const userServiceMock = jasmine.createSpyObj('UserService', ['Loginuser']);
    const notificationServiceMock = jasmine.createSpyObj('NotificationService', [
      'startConnection',
      'addUserNotification',
      'addNotification',
      'notification$'
    ], {
      notification$: of([])
    });

    const documentServiceMock = jasmine.createSpyObj('DocumentService', ['']);

    await TestBed.configureTestingModule({
      imports: [
        SignInComponent,
        FormsModule,
        ReactiveFormsModule,
        CommonModule,
        RouterTestingModule.withRoutes([])
      ],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: NotificationService, useValue: notificationServiceMock },
        { provide: DocumentService, useValue: documentServiceMock },
        { provide: ActivatedRoute, useValue: { queryParams: of({}) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SignInComponent);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should disable submit if form is invalid', () => {
    component.loginForm.setValue({ email: '', password: '' });
    expect(component.loginForm.invalid).toBeTrue();
  });

  it('should toggle password visibility', () => {
    expect(component.showPassword).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.showPassword).toBeTrue();
  });
});
