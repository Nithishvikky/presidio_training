import { Component } from '@angular/core';
import { Toast } from 'bootstrap';
import { NotificationService } from '../../services/notification.service';
import { NotificationResponseDto } from '../../models/notificationResponseDto';
import { CommonModule } from '@angular/common';
import { NotificationSharedResponseDto } from '../../models/notificationSharedResponse';

@Component({
  selector: 'app-notification-modal-component',
  imports: [CommonModule],
  templateUrl: './notification-modal-component.html',
  styleUrl: './notification-modal-component.css'
})
export class NotificationModalComponent {
  notificationMessage:NotificationResponseDto[]=[];
  notificationSharedMessage:NotificationSharedResponseDto[]=[];
  role:string = "";

  constructor(public notifyService:NotificationService){}

  ngOnInit():void{
    this.notifyService.notification$.subscribe(msgList => {
    if (!msgList) return;
     const msg = msgList[0];
        if (msg.type === 'view') {
          this.notificationMessage.unshift(msg);
        } else if (msg.type === 'shared') {
          this.notificationSharedMessage.unshift(msg);
        }
    });
  }

  clear(){
    this.notifyService.clearCurrentNotification();
  }
}
