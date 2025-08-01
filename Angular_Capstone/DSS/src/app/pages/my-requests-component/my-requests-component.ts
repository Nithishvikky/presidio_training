import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserRequestService } from '../../services/user-request.service';
import { UserRequestDto } from '../../models/userRequestDto';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-my-requests',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-requests-component.html',
  styleUrls: ['./my-requests-component.css']
})
export class MyRequestsComponent implements OnInit, OnDestroy {
  requests: UserRequestDto[] = [];
  currentPage = 1;
  pageSize = 20;
  loading = false;
  
  private subscriptions = new Subscription();

  constructor(
    private userRequestService: UserRequestService
  ) {}

  ngOnInit(): void {
    this.loadRequests();
    
    this.subscriptions.add(
      this.userRequestService.userRequests$.subscribe(requests => {
        console.log('Received requests:', requests);
        if (requests) {
          this.requests = requests;
          console.log('Updated requests array:', this.requests);
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadRequests(): void {
    this.loading = true;
    console.log('Loading requests...');
    this.userRequestService.getUserRequests(
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (response) => {
        console.log('Load requests response:', response);
        this.loading = false;
        console.log('Current requests array:', this.requests);
      },
      error: (error) => {
        console.error('Error loading requests:', error);
        this.loading = false;
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadRequests();
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'status-pending';
      case 'approved':
        return 'status-approved';
      case 'rejected':
        return 'status-rejected';
      default:
        return '';
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  formatDateTime(dateString: string): string {
    return new Date(dateString).toLocaleString();
  }
} 