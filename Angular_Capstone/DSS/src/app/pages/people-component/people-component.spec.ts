import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { PeopleComponent } from './people-component';
import { UserService } from '../../services/user.service';
import { UserResponseDto } from '../../models/userResponseDto';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of } from 'rxjs';
import { UserModalComponent } from '../user-modal-component/user-modal-component';


describe('PeopleComponent', () => {
  let component: PeopleComponent;
  let fixture: ComponentFixture<PeopleComponent>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  const mockUsers: UserResponseDto[] = [
    new UserResponseDto('1', 'user1@example.com', 'user1', 'User', new Date(), new Date(), 0),
    new UserResponseDto('2', 'admin@example.com', 'admin', 'Admin', new Date(), new Date(), 3)
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserService', ['GetAllUsers', 'GetUserDetails']);

    await TestBed.configureTestingModule({
      imports: [PeopleComponent, ReactiveFormsModule, CommonModule, UserModalComponent],
      providers: [{ provide: UserService, useValue: spy }]
    }).compileComponents();

    fixture = TestBed.createComponent(PeopleComponent);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize filter form and fetch users', fakeAsync(() => {
    userServiceSpy.GetAllUsers.and.returnValue(of({
      data: {
        totalPages: 1,
        items: { $values: mockUsers }
      }
    }));

    fixture.detectChanges();
    tick(300); // debounce time

    expect(component.filterForm).toBeDefined();
    expect(userServiceSpy.GetAllUsers).toHaveBeenCalled();
    expect(component.users.length).toBe(2);
  }));

  it('should clear individual control', fakeAsync(() => {
    userServiceSpy.GetAllUsers.and.returnValue(of({
      data: {
        totalPages: 1,
        items: { $values: mockUsers }
      }
    }));

    fixture.detectChanges();
    tick(300);

    component.filterForm.patchValue({ userEmail: 'test@example.com' });
    component.clearControl('userEmail');
    expect(component.filterForm.get('userEmail')?.value).toBe('');
  }));


  it('should clear all filters', fakeAsync(() => {
    userServiceSpy.GetAllUsers.and.returnValue(of({
      data: {
        totalPages: 1,
        items: { $values: mockUsers }
      }
    }));

    fixture.detectChanges();
    tick(300); 

    component.filterForm.patchValue({
      userEmail: 'test@example.com',
      userName: 'testuser',
      filterBy: 'Admin',
      sortBy: 'email',
      ascending: 'false'
    });

    component.clearFilters();

    expect(component.filterForm.value).toEqual({
      userEmail: '',
      userName: '',
      filterBy: '',
      sortBy: '',
      ascending: 'true'
    });
  }));

});
