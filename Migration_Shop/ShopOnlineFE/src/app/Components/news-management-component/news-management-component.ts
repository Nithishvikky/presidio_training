import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { News } from '../../Models/news.model';
import { NewsManagementService } from '../../Services/NewsManagement.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-news-management-component',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './news-management-component.html',
  styleUrl: './news-management-component.css'
})
export class NewsManagementComponent {
  newsForm!: FormGroup;
  newsList: News[] = [];
  selectedNews: News | null = null;
  isEdit = false;

  constructor(
    private fb: FormBuilder,
    private newsService: NewsManagementService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadNews();
  }

  initForm(): void {
    const isoDate = this.getUtcISOString();

    this.newsForm = this.fb.group({
      newsId: [0],
      userId: [1, Validators.required],
      title: ['', Validators.required],
      shortDescription: [''],
      image: [''],
      content: [''],
      createdDate: [isoDate],
      status: [1]
    });
  }

  loadNews(): void {
    this.newsService.getAllNews().subscribe(res => {
      this.newsList = res;
    });
  }

  onSubmit(): void {
    const news = this.newsForm.value;

    if (this.isEdit) {
      this.newsService.updateNews(news).subscribe(() => {
        this.loadNews();
        this.resetForm();
      });
    } else {
      this.newsService.createNews(news).subscribe(() => {
        this.loadNews();
        this.resetForm();
      });
    }
  }

  onEdit(news: News): void {
    console.log(news);
    this.newsForm.patchValue(news);
    this.isEdit = true;
  }

  onDelete(id: number): void {
    if (confirm('Are you sure to delete?')) {
      this.newsService.deleteNews(id).subscribe();
    }
    this.loadNews();
  }

  resetForm(): void {
    this.newsForm.reset();
    this.newsForm.get('createdDate')?.setValue(this.getUtcISOString());
    this.newsForm.get('status')?.setValue(1);
    this.isEdit = false;
  }

  getUtcISOString(): string {
    const iso = new Date().toISOString();
    return iso.endsWith('Z') ? iso : iso + 'Z';
  }
}
