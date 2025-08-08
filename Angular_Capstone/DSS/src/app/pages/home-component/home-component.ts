import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { UserResponseDto } from '../../models/userResponseDto';
import { RouterLink, RouterModule } from '@angular/router';
import { NotificationService } from '../../services/notification.service';
import { ChartData, ChartOptions, ChartType} from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { Chart } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { DashBoardResponseDto } from '../../models/dashboardResponseDto';
import { UserActivityLogService } from '../../services/user-activity-log.service';
import { UserActivityLogDto, UserActivitySummaryDto } from '../../models/userActivityLogDto';
import { ActivityLogComponent } from '../activity-log-component/activity-log-component';
import { UserActivitySummaryComponent } from '../user-activity-summary-component/user-activity-summary-component';
import { CommonModule } from '@angular/common';
import { SignalRService } from '../../services/signalr.service';

Chart.register(ChartDataLabels);

@Component({
  selector: 'app-home-component',
  imports: [RouterModule, RouterLink, NgChartsModule, CommonModule, ActivityLogComponent, UserActivitySummaryComponent],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css'
})
export class HomeComponent implements OnInit{
  User: UserResponseDto | null = null;
  data: DashBoardResponseDto | null = null;
  activitySummary: UserActivitySummaryDto | null = null;
  role : string = "";
  loading = true;

  constructor(
    private userService: UserService,
    private documentAccessService: DocumentAccessService,
    private userActivityLogService: UserActivityLogService,
    private signalrService: SignalRService
  ){}

  ngOnInit(): void {
    const authData = localStorage.getItem('authData');
    if(authData){
      console.log(authData);
      this.role = JSON.parse(authData).role;
      this.userService.CurrentUser$.subscribe((user:any) =>{
        this.User = user;
      })
      this.userService.GetUser(JSON.parse(authData).email).subscribe();
    }

    this.documentAccessService.dashboard$.subscribe({
      next:(res:any)=>{
        this.data = res;
        // this.updateChart();
        console.log(this.data);
      }
    })
    // this.documentAccessService.GetDashBoardData().subscribe();
    this.signalrService.startConnection();
    // Load user activity data
    this.loadUserActivityData();
  }

  private loadUserActivityData() {
    this.loading = true;

    // Load activity summary
    this.userActivityLogService.getUserDashboardActivity().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.activitySummary = response.data;
          console.log(this.activitySummary);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading activity summary:', error);
        this.loading = false;
      }
    });
  }

  onActivityClick(activity: UserActivityLogDto) {
    console.log('Activity clicked:', activity);
    // Handle activity click - could navigate to related content or show details
  }

  onLoadMoreActivities() {
    // Load more activities if needed
    console.log('Loading more activities...');
  }

  // pieChartType: ChartType = 'pie';
  // pieChartOptions: ChartOptions = {
  //   responsive: true,
  //   maintainAspectRatio: false,
  //   plugins: {
  //     datalabels: {
  //       formatter: (value: number, ctx: any) => {
  //         const data = ctx.chart.data.datasets[0].data as number[];
  //         const total = data.reduce((a: number, b: number) => a + b, 0);
  //         const percentage = ((value / total) * 100).toFixed(1);
  //         return `${percentage}%`;
  //       },
  //       color: '#fff',
  //       font: {
  //         weight: 'bold' as const,
  //         size: 14,
  //       }
  //     },
  //     legend: { position: 'top' },
  //     title: { display: true, text: 'Community stats' },
  //     tooltip: {
  //       callbacks: {
  //         label: function(context){
  //           const label = context.label || '';
  //           const value = context.raw;
  //           return `${label} ${value}`;
  //         }
  //       }
  //     }
  //   }
  // };

  // pieChartData: ChartData<'pie', number[], string | string[]> = {
  //   labels: ['Admin', 'User'],
  //   datasets: [
  //     {
  //       data: [0,0],
  //       backgroundColor: ['#36A2EB', '#FF6384']
  //     }
  //   ]
  // };

  // updateChart(){
  //   if(this.data){
  //     console.log(this.data.totalUserRole);
  //     this.pieChartData = {
  //       labels: ['Admin', 'User'],
  //       datasets: [
  //         {
  //           data: [this.data.totalUserRole,this.data.totalAdmin],
  //           backgroundColor: ['#36A2EB', '#FF6384']
  //         }
  //       ]
  //     };
  //   }
  // }
}

