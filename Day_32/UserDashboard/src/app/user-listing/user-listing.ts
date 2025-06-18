import { Component } from '@angular/core';
import { UserModel } from '../models/user';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-user-listing',
  imports: [],
  templateUrl: './user-listing.html',
  styleUrl: './user-listing.css'
})
export class UserListing {
  users: UserModel[] | null = null;

  constructor(private userService:UserService){}

  ngOnInit():void{
    this.userService.getAllUsers().subscribe({
      next:(data:any)=>{
        console.log(data);
        this.users = data.users;
        console.log(this.users);
      }
    })
  }
}
