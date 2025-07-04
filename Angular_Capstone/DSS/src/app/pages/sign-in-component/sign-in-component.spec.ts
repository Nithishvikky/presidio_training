import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { SignInComponent } from './sign-in-component';
import { UserService } from '../../services/user.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';

describe('SignInComponent', () => {
  let component: SignInComponent;
  let fixture: ComponentFixture<SignInComponent>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const userServiceMock = jasmine.createSpyObj('UserService', ['Loginuser']);

    await TestBed.configureTestingModule({
      imports: [
        SignInComponent,
        FormsModule,
        CommonModule,
        ReactiveFormsModule,
        RouterTestingModule.withRoutes([])
      ],
      providers:[
        { provide: UserService, useValue: userServiceMock },
        { provide: ActivatedRoute, useValue: { queryParams: of({}) } }
      ]
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

  it('should toggle password visibility', () => {
    expect(component.showPassword).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.showPassword).toBeTrue();
  });

  it('should show toast on login error', () => {
    userServiceSpy.Loginuser.and.returnValue(
      throwError(() => ({
        error: { error: { errorMessage: 'Invalid credentials' } }
      }))
    );

    component.loginForm.setValue({ email: 'wrong@example.com', password: 'wrongpass' });
    component.handleSubmit();

    expect(component.showToast).toHaveBeenCalledWith('Invalid credentials', 'danger');
  });
});
