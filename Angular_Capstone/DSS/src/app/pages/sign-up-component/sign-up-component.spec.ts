import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { SignUpComponent } from './sign-up-component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { UserService } from '../../services/user.service';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';

describe('SignUpComponent', () => {
  let component: SignUpComponent;
  let fixture: ComponentFixture<SignUpComponent>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let router: Router;

  beforeEach(async () => {
    const userServiceMock = jasmine.createSpyObj('UserService', ['RegisterUser']);

    await TestBed.configureTestingModule({
      imports: [
        SignUpComponent,
        ReactiveFormsModule,
        FormsModule,
        CommonModule,
        RouterTestingModule.withRoutes([])
      ],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: ActivatedRoute, useValue: { snapshot: { queryParams: {} } } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SignUpComponent);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should disable submit if form is invalid', () => {
    component.registerForm.setValue({
      username: '',
      email: '',
      password: '',
      confirmPassword: '',
      role: ''
    });
    expect(component.registerForm.invalid).toBeTrue();
  });

  it('should call UserService.RegisterUser and navigate on success', fakeAsync(() => {
    const mockResponse = {
      data: {
        role: 'User'
      }
    };
    userServiceSpy.RegisterUser.and.returnValue(of(mockResponse));
    spyOn(component, 'showToast');
    spyOn(router, 'navigateByUrl');

    component.registerForm.setValue({
      username: 'test',
      email: 'test@example.com',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      role: 'User'
    });

    component.handleSubmit();

    expect(userServiceSpy.RegisterUser).toHaveBeenCalled();
    expect(component.showToast).toHaveBeenCalledWith('Registered as User !!', 'success');
    tick(2000);
    expect(router.navigateByUrl).toHaveBeenCalledWith('/signin');
  }));

  it('should show toast on register error', () => {
    spyOn(component, 'showToast');
    userServiceSpy.RegisterUser.and.returnValue(
      throwError(() => ({ error: { error: { errorMessage: 'Registration failed' } } }))
    );

    component.registerForm.setValue({
      username: 'test',
      email: 'test@example.com',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      role: 'User'
    });

    component.handleSubmit();
    expect(component.showToast).toHaveBeenCalledWith('Registration failed', 'danger');
  });

  it('should toggle password visibility', () => {
    expect(component.showPassword).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.showPassword).toBeTrue();
  });
});
