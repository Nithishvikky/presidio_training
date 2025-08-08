import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserRequestService } from '../../services/user-request.service';
import { UserRequestDto } from '../../models/userRequestDto';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-requests',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-requests-component.html',
  styleUrls: ['./user-requests-component.css']
})
export class UserRequestsComponent implements OnInit, OnDestroy {
  requests: UserRequestDto[] = [];
  currentPage = 1;
  pageSize = 50;
  loading = false;
  showRejectModal = false;
  selectedRequest: UserRequestDto | null = null;
  rejectionReason = '';

  private subscriptions = new Subscription();

  constructor(private userRequestService: UserRequestService) {}

  ngOnInit(): void {
    this.loadRequests();
    this.subscriptions.add(
      this.userRequestService.allRequests$.subscribe(requests => {
        if (requests) {
          console.log(requests);
          this.requests = requests;
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadRequests(): void {
    this.loading = true;
    this.userRequestService.getAllRequests(
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (response) => {
        console.log(response);
        this.loading = false;
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

  approveRequest(request: UserRequestDto): void {
    this.loading = true;
    const processDto = {
      requestId: request.id,
      status: 'Approved',
      accessDurationHours: request.accessDurationHours || 24
    };

    this.userRequestService.processRequest(processDto).subscribe({
      next: (response) => {
        if (response) {
          console.log('Request approved successfully');
          this.loadRequests();
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error approving request:', error);
        this.loading = false;
      }
    });
  }

  openRejectModal(request: UserRequestDto): void {
    this.selectedRequest = request;
    this.rejectionReason = '';
    this.showRejectModal = true;
  }

  closeRejectModal(): void {
    this.showRejectModal = false;
    this.selectedRequest = null;
    this.rejectionReason = '';
  }

  rejectRequest(request:UserRequestDto): void {
    this.selectedRequest = request;
    console.log("called");
    if (!this.selectedRequest) {
      return;
    }

    this.loading = true;
    const processDto = {
      requestId: this.selectedRequest.id,
      status: 'Rejected',
    };

    this.userRequestService.processRequest(processDto).subscribe({
      next: (response) => {
        if (response) {
          console.log(`Data recieved from${response}`);
          console.log('Request rejected successfully');
          this.closeRejectModal();
          this.loadRequests();
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error rejecting request:', error);
        this.loading = false;
      }
    });
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
