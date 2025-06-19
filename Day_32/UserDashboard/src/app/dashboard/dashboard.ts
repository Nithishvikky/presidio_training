import { Component } from '@angular/core';
import { NgChartsModule } from 'ng2-charts';
import { UserService } from '../services/user.service';
import { ChartData, ChartOptions, ChartType} from 'chart.js';
import { CommonModule } from '@angular/common';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { Chart } from 'chart.js';
import {FormsModule } from '@angular/forms';
import { UserModel } from '../models/user';


// Register plugin
Chart.register(ChartDataLabels);


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule,NgChartsModule,FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard {

  selectedText:string = "";
  selectedGender:string = "";
  selectedRole:string = "";
  selectedState:string = "";
  allUsers:UserModel[] = [];
  availableStates:string[] = [];
  displayUsers:UserModel[] = [];

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

  barChartType: ChartType = 'bar';

  barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      datalabels: {
        anchor: 'start',
        align: 'end',
        formatter: (value: number) => value,
        font: {
          weight: 'bold' as const,
          size: 12
        }
      },
      legend: {
        display: false
      },
      title: {
        display: true,
        text: 'Role Distribution'
      }
    }
  };

  barChartData: ChartData<'bar', number[], string> = {
    labels: [],
    datasets: [
      {
        label: 'User Count',
        data: [],
        backgroundColor: []
      }
    ]
  };

  stateBarChartType: ChartType = 'bar';

  stateBarChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      datalabels: {
        anchor: 'start',
        align: 'end',
        formatter: (value: number) => value,
        font: {
          weight: 'bold' as const,
          size: 12
        }
      },
      legend: {
        display: false
      },
      title: {
        display: true,
        text: 'State Distribution'
      }
    }
  };

  stateBarChartData: ChartData<'bar', number[], string> = {
    labels: [],
    datasets: [
      {
        label: 'User Count',
        data: [],
        backgroundColor: []
      }
    ]
  };


  constructor(private userService:UserService){}

  ngOnInit():void{
    this.userService.getAllUsers().subscribe({
      next:(data:any)=>{
        console.log(data);
        this.allUsers = data.users.map((user:any)=>{
          return {
            firstName : user.firstName,
            role : user.role,
            state : user.address.state,
            gender: user.gender,
            age : user.age,
            email : user.email,
            password : user.password,
          } as UserModel;
        })
        this.displayUsers = this.allUsers;

        const stateSet = new Set<string>();

        this.allUsers.forEach(user => {
          if (user.state) {
            stateSet.add(user.state);
          }
        });
        this.availableStates = Array.from(stateSet).sort();
        this.updateChart();
      },
      error:(err)=>{
        console.error(err);
      }
    })
  }

  generateColor(index: number): string {
    const colors = ['#36A2EB', '#FF6384', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'];
    return colors[index % colors.length];
  }

  updateChart(){
    const users = this.filteredData;
    this.displayUsers = users;
    console.log(users);

    this.pieChartData = {
      labels: ['Male', 'Female'],
      datasets: [
        {
          data: [
            users.filter(u => u.gender === 'male').length,
            users.filter(u => u.gender === 'female').length
          ],
          backgroundColor: ['#36A2EB', '#FF6384']
        }
      ]
    };

    const roleCounts: { [key: string]: number } = {};
    this.allUsers.forEach(user => {
      roleCounts[user.role] = 0;
    });

    this.displayUsers.forEach(user => {
      roleCounts[user.role] = (roleCounts[user.role] || 0) + 1;
    });

    this.barChartData = {
      labels: Object.keys(roleCounts),
      datasets: [
        {
          data: Object.values(roleCounts),
          backgroundColor: Object.keys(roleCounts).map((_, index) =>
            this.selectedRole === Object.keys(roleCounts)[index] ? '#FF6384' : this.generateColor(index)
          )
        }
      ]
    };

    const stateCounts: { [key: string]: number } = {};
    this.availableStates.forEach(state => {
      stateCounts[state] = 0;
    });

    this.displayUsers.forEach(user => {
      stateCounts[user.state] = (stateCounts[user.state] || 0) + 1;
    });

    this.stateBarChartData = {
      labels: Object.keys(stateCounts),
      datasets: [
        {
          data: Object.values(stateCounts),
          backgroundColor: Object.keys(stateCounts).map((_, index) =>
            this.selectedRole === Object.keys(stateCounts)[index] ? '#FF6384' : this.generateColor(index)
          )
        }
      ]
    };
  }

  get filteredData():UserModel[]{
    return this.allUsers.filter(user =>{
      const matchesGender = this.selectedGender ? user.gender === this.selectedGender : true;
      const matchesRole = this.selectedRole ? user.role === this.selectedRole : true;
      const matchesState = this.selectedState ? user.state === this.selectedState : true;
      const matchesSearch = this.selectedText ? (`${user.firstName} ${user.lastName}`.toLowerCase().includes(this.selectedText.toLowerCase())) : true;

      return matchesGender && matchesRole && matchesSearch && matchesState;
    })
  }

}
