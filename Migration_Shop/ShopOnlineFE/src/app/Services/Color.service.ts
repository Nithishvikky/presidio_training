import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { Color } from '../Models/color.model';

@Injectable()
export class ColorService {
  private baseUrl = 'http://localhost:5190/api/v1/Color';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Color[]> {
    return this.http.get<Color[]>(`${this.baseUrl}/List`);
  }

  getById(id: number): Observable<Color> {
    return this.http.get<Color>(`${this.baseUrl}/Detail/${id}`);
  }

  create(color: Color): Observable<Color> {
    console.log(color);
    return this.http.post<Color>(`${this.baseUrl}`, color);
  }

  update(color: Color): Observable<any> {
    return this.http.put(`${this.baseUrl}/Edit`, color);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Delete/${id}`);
  }
}
