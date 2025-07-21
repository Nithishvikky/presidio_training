import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TrainingVideo {
  id: number;
  title: string;
  description: string;
  uploadDate: string;
  blobUrl: string;
}

@Injectable({
  providedIn: 'root',
})
export class VideoService {
  private baseUrl = 'http://localhost:5175/api/videos';

  constructor(private http: HttpClient) {}

  getVideos(): Observable<TrainingVideo[]> {
    return this.http.get<TrainingVideo[]>(this.baseUrl);
  }

  uploadVideo(formData: FormData): Observable<any> {
    console.log(formData);
    return this.http.post(this.baseUrl + '/upload', formData);
  }
}
