import { Component } from '@angular/core';
import { WeatherService } from '../Services/Weather.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-city-search-component',
  imports: [FormsModule],
  templateUrl: './city-search-component.html',
  styleUrl: './city-search-component.css'
})
export class CitySearchComponent {
city:string = '';

  constructor(private weatherService: WeatherService) {}

  onSearch() {
    if (this.city.trim()) {
      this.weatherService.getWeatherByCity(this.city.trim());
      this.city = '';
    }
  }
}
