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
    const authData = localStorage.getItem('authData');
    if(authData){
      this.role = JSON.parse(authData).role;
     if(this.role === 'Admin'){
        this.notifyService.notification$.subscribe(msg =>{
          if(msg){
            this.notificationMessage = msg;
          }
        })
     }
     else{
      this.notifyService.notification$.subscribe(msg =>{
        if(msg){
          this.notificationSharedMessage = msg;
        }
      })
     }
    }
  }

  clear(){
    this.notifyService.clearCurrentNotification();
  }
}
