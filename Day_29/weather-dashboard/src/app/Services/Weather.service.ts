import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable()
export class WeatherService{
  private http = inject(HttpClient);
  private apiKey:string = "3154f6bfebefea4aa36f34ddab048dcc";
  private webUrl:string = "https://api.openweathermap.org/data/2.5/weather";

  private weatherSubject = new BehaviorSubject<any|null>(null);

  weather$:Observable<any|null> = this.weatherSubject.asObservable();

  getWeatherByCity(city:string): void{
    this.http.get(`${this.webUrl}?q=${city}&appid=${this.apiKey}&units=metric`).subscribe({
      next:(data)=>{
        console.log("Raw data",data);
        this.weatherSubject.next(data);
      },
      error:(err)=>{
        console.error("Api error",err);
        this.weatherSubject.next({error : "City not found"});
      }
    });
  }

  clearWeather(): void{
    this.weatherSubject.next(null);
  }
}
