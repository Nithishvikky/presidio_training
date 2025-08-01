import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { News, PaginatedNews } from '../Models/news.model';


@Injectable()
export class NewsManagementService {
  private baseUrl = 'http://localhost:5190/api/v1';

  constructor(private http: HttpClient) {}

  getAllNewsPage(page:number): Observable<PaginatedNews> {
    return this.http.get<PaginatedNews>(`${this.baseUrl}/News/List?page=${page}`);
  }

  getAllNews(): Observable<News[]> {
    return this.http.get<News[]>(`${this.baseUrl}/NewsManagement/List`);
  }

  getNewsById(id: number): Observable<News> {
    return this.http.get<News>(`${this.baseUrl}/NewsManagement/Detail/${id}`);
  }

  createNews(news: News): Observable<News> {
    // console.log(news);
    news.createdDate = news.createdDate+'Z';
    if(news.image)
      news.imageUrl = news.image;
    console.log(news);
    return this.http.post<News>(`${this.baseUrl}/NewsManagement`, news);
  }

  updateNews(news: News): Observable<News> {
    // console.log(news);
    news.createdDate = news.createdDate+'Z';
    if(news.image)
      news.imageUrl = news.image;
    console.log(news);
    return this.http.put<News>(`${this.baseUrl}/NewsManagement/Edit/${news.newsId}`, news);
  }

  deleteNews(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/NewsManagement/Delete/${id}`);
  }
}
