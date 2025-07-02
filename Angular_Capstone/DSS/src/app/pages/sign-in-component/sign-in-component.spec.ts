import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';

import { SignInComponent } from './sign-in-component';
import { UserService } from '../../services/user.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { of, throwError } from 'rxjs';

describe('SignInComponent', () => {
  let component: SignInComponent;
  let fixture: ComponentFixture<SignInComponent>;
  let userServiceSpy: jasmine.SpyObj<UserService>;



  beforeEach(async () => {
    const userServiceMock = jasmine.createSpyObj('UserService', ['Loginuser']);

    await TestBed.configureTestingModule({
      imports: [SignInComponent,FormsModule,CommonModule,ReactiveFormsModule,RouterModule],
      providers:[
        {provide: UserService, useValue:userServiceMock},
        { provide: ActivatedRoute, useValue: { queryParams: of({}) } }]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SignInComponent);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;

    spyOn(component, 'showToast').and.callFake(() => {});
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should disable submit if form is invalid', () => {
    component.loginForm.setValue({ email: '', password: '' });
    expect(component.loginForm.invalid).toBeTrue();
  });

  // it('should call UserService.Loginuser', fakeAsync(() => {
  //   const mockResponse = {
  //     data: {
  //       accessToken: 'access',
  //       refreshToken: 'refresh',
  //       user: { id: '1', email: 'test@example.com', role: 'User' }
  //     }
  //   };

  //   userServiceSpy.Loginuser.and.returnValue(of(mockResponse));

  //   component.loginForm.setValue({ email: 'test@example.com', password: 'Password123' });
  //   component.handleSubmit();

  //   expect(userServiceSpy.Loginuser).toHaveBeenCalledWith({ email: 'test@example.com', password: 'Password123' });
  //   tick(2000); // Simulate timeout

  // }));

  // it('should show toast on login error', () => {
  //     userServiceSpy.Loginuser.and.returnValue(
  //       throwError(() => ({
  //         error: { error: { errorMessage: 'Invalid credentials' } }
  //       }))
  //     );

  //     component.loginForm.setValue({ email: 'wrong@example.com', password: 'wrongpass' });
  //     component.handleSubmit();

  //     expect(component.showToast).toHaveBeenCalledWith('Invalid credentials', 'danger');
  // });

  it('should toggle password visibility', () => {
    expect(component.showPassword).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.showPassword).toBeTrue();
  });
});
