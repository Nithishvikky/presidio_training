import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignalRService } from '../../services/signalr.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-connection-status',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="connection-status" [class]="connectionState.toLowerCase()">
      <div class="status-indicator">
        <i [class]="getStatusIcon()"></i>
        <span class="status-text">{{ connectionState }}</span>
      </div>
      <div *ngIf="showDetails" class="connection-details">
        <small>ID: {{ connectionId || 'N/A' }}</small>
      </div>
    </div>
  `,
  styles: [`
    .connection-status {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      padding: 4px 8px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
      transition: all 0.3s ease;
    }

    .status-indicator {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .status-text {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .connection-details {
      font-size: 10px;
      opacity: 0.7;
    }

    /* Connection States */
    .connected {
      background-color: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }

    .connected i {
      color: #28a745;
    }

    .disconnected {
      background-color: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    .disconnected i {
      color: #dc3545;
    }

    .reconnecting {
      background-color: #fff3cd;
      color: #856404;
      border: 1px solid #ffeaa7;
      animation: pulse 1.5s infinite;
    }

    .reconnecting i {
      color: #ffc107;
    }

    .failed {
      background-color: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    .failed i {
      color: #dc3545;
    }

    @keyframes pulse {
      0%, 100% {
        opacity: 1;
      }
      50% {
        opacity: 0.7;
      }
    }

    /* Hover effect */
    .connection-status:hover {
      transform: scale(1.05);
      cursor: pointer;
    }

    /* Responsive */
    @media (max-width: 768px) {
      .connection-status {
        padding: 2px 6px;
        font-size: 10px;
      }
      
      .status-text {
        font-size: 10px;
      }
      
      .connection-details {
        display: none;
      }
    }
  `]
})
export class ConnectionStatusComponent implements OnInit, OnDestroy {
  connectionState = 'Disconnected';
  connectionId?: string;
  showDetails = false;
  private connectionSubscription?: Subscription;

  constructor(private signalRService: SignalRService) {}

  ngOnInit() {
    this.connectionSubscription = this.signalRService.connectionState$.subscribe(
      (state: string) => {
        this.connectionState = state;
        this.updateConnectionInfo();
      }
    );
    
    // Initial connection info
    this.updateConnectionInfo();
  }

  ngOnDestroy() {
    if (this.connectionSubscription) {
      this.connectionSubscription.unsubscribe();
    }
  }

  private updateConnectionInfo() {
    const info = this.signalRService.getConnectionInfo();
    this.connectionId = info.connectionId;
  }

  getStatusIcon(): string {
    switch (this.connectionState.toLowerCase()) {
      case 'connected':
        return 'bi bi-wifi';
      case 'disconnected':
        return 'bi bi-wifi-off';
      case 'reconnecting':
        return 'bi bi-arrow-clockwise';
      case 'failed':
        return 'bi bi-exclamation-triangle';
      default:
        return 'bi bi-question-circle';
    }
  }
} 