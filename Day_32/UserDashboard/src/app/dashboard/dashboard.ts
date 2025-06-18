import { Component } from '@angular/core';
import { NgChartsModule } from 'ng2-charts';
import { UserService } from '../services/user.service';
import { ChartData, ChartOptions, ChartType} from 'chart.js';
import { CommonModule } from '@angular/common';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { Chart } from 'chart.js';
// Register plugin
Chart.register(ChartDataLabels);


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule,NgChartsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard {
  
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
      title: { display: true, text: 'User Gender Distribution' },
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
    labels: ['Male', 'Female'],
    datasets: [
      {
        data: [0,0],
        backgroundColor: ['#36A2EB', '#FF6384']
      }
    ]
  };


    constructor(private userService:UserService){}

    ngOnInit():void{
      this.loadGenderData();
    }

    loadGenderData(): void {
      let maleCount = 0;
      let femaleCount = 0;

      this.userService.filterUsers('gender', 'male').subscribe({
        next: (maleData: any) => {
          maleCount = maleData.total;
          this.userService.filterUsers('gender', 'female').subscribe({
            next: (femaleData: any) => {
              femaleCount = femaleData.total;
              this.pieChartData.datasets[0].data = [maleCount, femaleCount];
            }
          });
        }
      });
  }
}
