import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-weather-card-component',
  imports: [CommonModule],
  templateUrl: './weather-card-component.html',
  styleUrl: './weather-card-component.css'
})
export class WeatherCardComponent {
@Input() weather: any;
dateNow = new Date();
}
