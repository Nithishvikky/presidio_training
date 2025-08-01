import { Component } from '@angular/core';
import { News } from '../../Models/news.model';
import { NewsManagementService } from '../../Services/NewsManagement.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-news-component',
  imports: [CommonModule],
  templateUrl: './news-component.html',
  styleUrl: './news-component.css'
})
export class NewsComponent {
  newsList: News[] = [];
  pageNumber = 1;
  totalPages = 1;

  constructor(private newsService: NewsManagementService) {}

  ngOnInit(): void {
    this.loadNews();
  }

  loadNews(): void {
    this.newsService.getAllNewsPage(this.pageNumber).subscribe(response => {
      this.newsList = response.items;
      this.pageNumber = response.pageNumber;
      this.totalPages = response.totalPages;
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.loadNews();
    }
  }
}
