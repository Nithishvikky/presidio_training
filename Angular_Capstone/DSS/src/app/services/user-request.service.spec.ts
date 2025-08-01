import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { UserRequestService } from './user-request.service';

describe('UserRequestService', () => {
  let service: UserRequestService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserRequestService]
    });
    service = TestBed.inject(UserRequestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should have userRequests$ observable', () => {
    expect(service.userRequests$).toBeDefined();
  });

  it('should have allRequests$ observable', () => {
    expect(service.allRequests$).toBeDefined();
  });
}); 