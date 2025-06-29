import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { UserResponseDto } from '../../models/userResponseDto';
import { RouterLink, RouterModule } from '@angular/router';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-home-component',
  imports: [RouterModule,RouterLink],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css'
})
export class HomeComponent {
  User:UserResponseDto|null =null;

  constructor(private userService:UserService){}

  ngOnInit():void{
    const authData = localStorage.getItem('authData');
    if(authData){
      this.userService.CurrentUser$.subscribe((user:any) =>{
        this.User = user;
      })
      this.userService.GetUser(JSON.parse(authData).email).subscribe();
    }
  }
}

