import { Component } from '@angular/core';
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

Chart.register(ChartDataLabels);

@Component({
  selector: 'app-home-component',
  imports: [RouterModule,RouterLink,NgChartsModule],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css'
})
export class HomeComponent {
  User:UserResponseDto|null =null;
  data:DashBoardResponseDto|null = null;

  constructor(private userService:UserService,private documentAccessService:DocumentAccessService){}

  ngOnInit():void{
    const authData = localStorage.getItem('authData');
    if(authData){
      console.log(authData);
      this.userService.CurrentUser$.subscribe((user:any) =>{
        this.User = user;
      })
      this.userService.GetUser(JSON.parse(authData).email).subscribe();
    }
    this.documentAccessService.dashboard$.subscribe({
      next:(res:any)=>{
        this.data = res;
        this.updateChart();
        console.log(this.data);
      }
    })
    this.documentAccessService.GetDashBoardData().subscribe();
  }
  
  pieChartType: ChartType = 'pie';
  pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio:false,
    plugins: {
      datalabels: {
        formatter: (value: number, ctx: any) => {
          const data = ctx.chart.data.datasets[0].data as number[];
          const total = data.reduce((a: number, b: number) => a + b, 0);
          const percentage = ((value / total) * 100).toFixed(1);
          return `${percentage}%`;
        },
        color: '#fff',
        font: {
          weight: 'bold' as const,
          size: 14,
        }
      },
      legend: { position: 'top' },
      title: { display: true, text: 'Community stats' },
      tooltip: {
        callbacks: {
          label: function(context){
            const label = context.label || '';
            const value = context.raw;
            return `${label} ${value}`;
          }
        }
      }
    }
  };

  pieChartData: ChartData<'pie', number[], string | string[]> = {
    labels: ['Admin', 'User'],
    datasets: [
      {
        data: [0,0],
        backgroundColor: ['#36A2EB', '#FF6384']
      }
    ]
  };


  updateChart(){
    if(this.data){
      console.log(this.data.totalUserRole);
      this.pieChartData = {
        labels: ['Admin', 'User'],
        datasets: [
          {
            data: [this.data.totalUserRole,this.data.totalAdmin],
            backgroundColor: ['#36A2EB', '#FF6384']
          }
        ]
      };
    }
  }

}

