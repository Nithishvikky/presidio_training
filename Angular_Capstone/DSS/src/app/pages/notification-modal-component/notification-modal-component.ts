import { Component } from '@angular/core';
import { Toast } from 'bootstrap';
import { NotificationService } from '../../services/notification.service';
import { NotificationResponseDto } from '../../models/notificationResponseDto';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification-modal-component',
  imports: [CommonModule],
  templateUrl: './notification-modal-component.html',
  styleUrl: './notification-modal-component.css'
})
export class NotificationModalComponent {
  notificationMessage:NotificationResponseDto[]=[];

  constructor(public notifyService:NotificationService){}

  ngOnInit():void{
    this.notifyService.notification$.subscribe(msg =>{
      if(msg){
        this.notificationMessage = msg;
      }
    })
    this.notifyService.addNotification();
  }

  clear(){
    this.notifyService.clearNotification();
  }
}
