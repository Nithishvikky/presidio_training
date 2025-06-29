import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoaderComponent } from "./loader-component/loader-component";
import { NotificationService } from './services/notification.service';
import { Toast } from 'bootstrap';
import { NotificationResponseDto } from './models/notificationResponseDto';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'DSS';
  notificationMessage:NotificationResponseDto[]=[];

  constructor(public notifyService:NotificationService){}

  ngOnInit():void{
    this.notifyService.notification$.subscribe(msg =>{
      if(msg){
        this.notificationMessage = msg;
        const notification = this.notificationMessage[0];
        this.showToast(`${notification.viewerName} viewed ${notification.fileName}`,"success");
      }
    })
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
