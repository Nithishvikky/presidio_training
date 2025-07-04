import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoaderComponent } from "./loader-component/loader-component";
import { NotificationService } from './services/notification.service';
import { Toast } from 'bootstrap';
import { NotificationResponseDto } from './models/notificationResponseDto';
import { NotificationSharedResponseDto } from './models/notificationSharedResponse';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'DSS';
  notificationMessage:NotificationResponseDto[]=[];
  notificationSharedMessage:NotificationSharedResponseDto[]=[];

  role:string = "";

  constructor(public notifyService:NotificationService){}

  ngOnInit():void{
    const authData = localStorage.getItem('authData');
    if(authData){
      this.role = JSON.parse(authData).role;
      if(this.role === 'Admin'){
      this.notifyService.addNotification();
        this.notifyService.notification$.subscribe(msg =>{
          if(msg){
            this.notificationMessage = msg;
            const notification = this.notificationMessage[0];
            this.showToast(`${notification.viewerName} viewed ${notification.fileName}`,"success");
          }
        })
     }
     else{
      this.notifyService.addUserNotification();
      this.notifyService.notification$.subscribe(msg =>{
        if(msg){
          this.notificationSharedMessage = msg;
          const notification = this.notificationSharedMessage[0];
          this.showToast(`${notification.userName} granted access for ${notification.fileName}`,"success");
        }
      })
     }
    }
  }

  showToast(message: string, type: 'success' | 'danger') {
    const toastEl = document.getElementById('liveToast');
    const toastBody = document.querySelector('.toast-body');

    toastBody!.textContent = message;
    toastEl!.classList.remove('bg-success', 'bg-danger');
    toastEl!.classList.add(type === 'success' ? 'bg-primary' : 'bg-danger');

    const toast = new Toast(toastEl!);
    toast.show();
  }
}
