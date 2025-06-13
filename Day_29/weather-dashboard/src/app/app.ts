import { Component } from '@angular/core';
import { WeatherDashboard } from './weather-dashboard/weather-dashboard';


@Component({
  selector: 'app-root',
  imports: [WeatherDashboard],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'weather-dashboard';
}
