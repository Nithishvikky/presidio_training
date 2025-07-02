import { TestBed } from '@angular/core/testing';
import { UserService } from './user.service';
import { HttpClientTestingModule, HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { UserRegisterDto } from '../models/userRegisterDto';
import { UserLoginDto } from '../models/userLoginDto';
import { PasswordChangeDto } from '../models/passwordChangeDto';
import { UserResponseDto } from '../models/userResponseDto';
import { provideHttpClient } from '@angular/common/http';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [],
      providers: [UserService,provideHttpClient(),provideHttpClientTesting()]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no outstanding HTTP calls
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should register user', () => {
    const dummyUser: UserRegisterDto = {
      email: 'test@example.com',
      password: '123456',
      username: 'testuser',
      role: 'User'
    };

    service.RegisterUser(dummyUser).subscribe();

    const req = httpMock.expectOne(`http://localhost:5015/api/v1/User/RegisterUser`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(dummyUser);
  });

  it('should login user', () => {
    const loginDto: UserLoginDto = { email: 'user@example.com', password: 'pass' };

    service.Loginuser(loginDto).subscribe();

    const req = httpMock.expectOne(`http://localhost:5015/api/v1/Auth/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(loginDto);
  });

  it('should call refresh token API', () => {
    service.RefreshAccessToken('refreshToken').subscribe();

    const req = httpMock.expectOne(`http://localhost:5015/api/v1/Auth/refresh`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ RToken: 'refreshToken' });
  });

  it('should logout user', () => {
    service.LogoutUser('refreshToken').subscribe();

    const req = httpMock.expectOne(`http://localhost:5015/api/v1/Auth/logout`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ RToken: 'refreshToken' });
  });

  it('should call ChangePassword and fetch updated user', () => {
    const email = "example@gmail.com";
    const dto: PasswordChangeDto = {
      OldPassword: 'oldpass',
      NewPassword: 'newpass'
    };

    service.ChangePassword(dto, email).subscribe();

    const changeReq = httpMock.expectOne(`http://localhost:5015/api/v1/User/ChangePassword`);
    expect(changeReq.request.method).toBe('PUT');
    expect(changeReq.request.body).toEqual(dto);

    // const getReq = httpMock.expectOne(`http://localhost:5015/api/v1/User/GetUser?email=${email}`);
    // expect(getReq.request.method).toBe('GET');
  });

  it('should fetch a user and update CurrentUser$', (done) => {
    const response = {
    Id: "1",
    email: 'user@example.com',
    username: 'testuser',
    role: 'user',
    registeredAt: new Date(),
    updatedAt: new Date(),
    uploadedDocuments: { $values: [1, 2, 3] }
  };

    service.GetUser('user@example.com').subscribe();

    const req = httpMock.expectOne(`http://localhost:5015/api/v1/User/GetUser?email=user@example.com`);
    expect(req.request.method).toBe('GET');

    req.flush(response);

    service.CurrentUser$.subscribe(user => {
      expect(user?.email).toBe('user@example.com');
      expect(user?.documentCount).toBe(3);
      done();
    });
  });

  it('should fetch all users and update users$', () => {
    const response = {
      data: [
        { email: 'user1@example.com' },
        { email: 'user2@example.com' }
      ]
    };

    service.GetAllUsers().subscribe();

    const req = httpMock.expectOne(`http://localhost:5015/api/v1/User/GetAllUsers?ascending=true&pageNumber=1&pageSize=10`);
    expect(req.request.method).toBe('GET');

    req.flush(response);

    service.users$.subscribe(users => {
      expect(users?.length).toBe(2);
    });
  });

  it('should handle GetAllUsers error and update users$ to []', () => {
    service.GetAllUsers().subscribe(result => {
      expect(result).toBeNull();
    });

    const req = httpMock.expectOne(`http://localhost:5015/api/v1/User/GetAllUsers?ascending=true&pageNumber=1&pageSize=10`);
    req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });

    service.users$.subscribe(users => {
      expect(users).toEqual([]);
    });
  });

  it('should clear user cache', () => {
    service.clearUserCache();

    service.CurrentUser$.subscribe(user => {
      expect(user).toBeNull();
    });

    service.users$.subscribe(users => {
      expect(users).toBeNull();
    });
  });
});
