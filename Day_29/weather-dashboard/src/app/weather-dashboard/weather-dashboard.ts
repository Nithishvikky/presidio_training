import { Component, OnInit } from '@angular/core';
import { WeatherService } from '../Services/Weather.service';
import { Observable } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CitySearchComponent } from '../city-search-component/city-search-component';
import { WeatherCardComponent } from '../weather-card-component/weather-card-component';

@Component({
  selector: 'app-weather-dashboard',
  imports: [CommonModule,CitySearchComponent,WeatherCardComponent],
  templateUrl: './weather-dashboard.html',
  styleUrl: './weather-dashboard.css'
})
export class WeatherDashboard implements OnInit{
weather:any = null;

  constructor(private weatherService: WeatherService) {}

  ngOnInit(): void {
    this.weatherService.weather$.subscribe(data => {
      this.weather = data;
      console.log('Weather data received:', this.weather);
    });
  }
}
