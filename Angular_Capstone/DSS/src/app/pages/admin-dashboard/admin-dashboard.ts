import { Component,OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ChartData, ChartOptions, ChartType} from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { Chart } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';
import { ApiResponse, DashBoardResponseDto, DocumentDateCountDto } from '../../models/dashboardResponseDto';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { DocumentService } from '../../services/document.service';

Chart.register(ChartDataLabels);

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule, NgChartsModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit{
  data: DashBoardResponseDto | null = null;
  barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      title: { display: true, text: 'Document Uploads in Last 7 Days' },
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context) => `Uploads: ${context.raw}`
        }
      }
    },
    scales: {
      x: { ticks: { maxRotation: 45, minRotation: 45 } },
      y: { beginAtZero: true }
    }
  };

  barChartType: 'bar' = 'bar';
  barChartData: ChartData<'bar', number[], string> = {
    labels: [],
    datasets: [
      {
        label: 'Uploads',
        data: [],
        backgroundColor: '#36A2EB'
      }
    ]
  };

  pieChartType: ChartType = 'pie';
  pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
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
      title: { display: true, text: 'Active vs Inactive Users' },
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
    labels: ['Active', 'Inactive'],
    datasets: [
      {
        data: [0,0],
        backgroundColor: ['#36A2EB', '#FF6384']
      }
    ]
  };

  sharedDocChartData: ChartData<'bar', number[], string> = {
    labels: [],
    datasets: [
      {
        label: 'Share Count',
        data: [],
        backgroundColor: '#36A2EB'
      }
    ]
  };

  sharedDocChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    indexAxis: 'y',
    plugins: {
      legend: {
        display: false
      },
      title: {
        display: true,
        text: 'Top 5 Shared Documents'
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            return `Shares: ${context.raw}`;
          }
        }
      }
    }
  };

  docTypeChartData: ChartData<'bar', number[], string> = {
    labels: [],
    datasets: [
      {
        label: 'Document Count',
        data: [],
        backgroundColor: '#FF9800'
      }
    ]
  };

  docTypeChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      title: { display: true, text: 'Document Type Distribution' },
      tooltip: {
        callbacks: {
          label: (context) => `Count: ${context.raw}`
        }
      }
    }
  };

  constructor(private http: HttpClient,
    private documentAccessService:DocumentAccessService,
    private documentService: DocumentService) {}

  ngOnInit(): void {
    this.documentAccessService.GetDashBoardData().subscribe();
    this.documentService.GetDashboardData().subscribe();
    this.documentService.GetUserRatio().subscribe();
    this.documentAccessService.getTopSharedDocuments().subscribe();
    this.documentService.getDocumentTypeDistribution().subscribe();


    this.loadUploadData();
  }

  loadUploadData() {
    this.documentService.dashBoardData$.subscribe({
      next: (res) => {
        if(res){
          console.log(res);
          const data = res.data.$values;
          const labels = data.map(d => this.formatDate(d.date));
          const counts = data.map(d => d.count);

          this.barChartData = {
            labels: labels,
            datasets: [
              {
                label: 'Uploads',
                data: counts,
                backgroundColor: '#36A2EB'
              }
            ]
          }
        }
      },
      error: (err) => {
        console.error('Failed to load document trend:', err);
      }
    });

    this.documentService.userRatioCountData$.subscribe({
      next:(data) =>{
        if(data){
          console.log(data);
          this.pieChartData = {
            labels: ['Active', 'Inactive'],
            datasets: [
              {
                data: [data.activeCount,data.inactiveCount],
                backgroundColor: ['#36A2EB', '#FF6384']
              }
            ]
          };
        }
      }
    })

    this.documentAccessService.topShared$.subscribe({
      next:(data)=>{
        if (data) {
          console.log(data);
          this.sharedDocChartData = {
            labels: data.map(d => d.fileName),
            datasets: [
              {
                label: 'Share Count',
                data: data.map(d => d.shareCount),
                backgroundColor: '#36A2EB'
              }
            ]
          };
        }
      }
    });

    this.documentService.docTypeCount$.subscribe({
      next:(data)=>{
        console.log(`Type : ${data}`)
        if(data){
          this.docTypeChartData = {
            labels: data.map(d => d.contentType),
            datasets: [
              {
                label: 'Document Count',
                data: data.map(d => d.count),
                backgroundColor: '#FF9800'
              }
            ]
          };
        }
      }
    })

    this.documentAccessService.dashboard$.subscribe({
      next:(res:any)=>{
        this.data = res;
        console.log(this.data);
      }
    })
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return `${date.getDate()}/${date.getMonth() + 1}`;
  }
}
