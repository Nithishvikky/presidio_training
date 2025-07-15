import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HomeComponent } from './home-component';
import { UserService } from '../../services/user.service';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { NgChartsModule } from 'ng2-charts';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { UserResponseDto } from '../../models/userResponseDto';
import { DashBoardResponseDto } from '../../models/dashboardResponseDto';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;

  const mockUser: UserResponseDto = {
    userId: '1',
    email: 'admin@example.com',
    username: 'AdminUser',
    role: 'Admin',
    registeredAt: new Date(),
    updatedAt: new Date(),
    documentCount: 5
  };

  const mockDashboardData: DashBoardResponseDto = {
    totalUsers: 10,
    totalDocuments: 50,
    totalUserRole: 6,
    totalAdmin: 4,
    totalShared: 20,
    totalViews: 100
  };

  const mockUserService = {
    CurrentUser$: of(mockUser),
    GetUser: jasmine.createSpy('GetUser').and.returnValue(of(mockUser))
  };

  const mockAccessService = {
    dashboard$: of(mockDashboardData),
    GetDashBoardData: jasmine.createSpy('GetDashBoardData').and.returnValue(of(mockDashboardData))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HomeComponent, // standalone component
        CommonModule,
        RouterTestingModule,
        NgChartsModule
      ],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: DocumentAccessService, useValue: mockAccessService }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    localStorage.setItem('authData', JSON.stringify({ email: mockUser.email }));

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // ngOnInit is called here for standalone components
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize user and dashboard data', () => {
    expect(component.User).toEqual(mockUser);
    expect(component.data).toEqual(mockDashboardData);
    expect(mockUserService.GetUser).toHaveBeenCalledWith(mockUser.email);
    expect(mockAccessService.GetDashBoardData).toHaveBeenCalled();
  });

  it('should update chart data correctly', () => {
    component.updateChart();
    expect(component.pieChartData.datasets[0].data).toEqual([
      mockDashboardData.totalUserRole,
      mockDashboardData.totalAdmin
    ]);
  });

  it('should display admin-specific buttons when role is Admin', () => {
    component.User = { ...mockUser, role: 'Admin' };
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Manage Users');
    expect(compiled.textContent).toContain('Go to My Documents');
  });

  it('should display user-specific button when role is User', () => {
    component.User = { ...mockUser, role: 'User' };
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Go to My Documents');
    expect(compiled.textContent).not.toContain('Manage Users');
  });
});
