import { Component } from '@angular/core';
import { UserLoginModel } from '../models/userLogin';
import { UserService } from '../services/user.service';
import { Route, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login-component',
  imports: [FormsModule],
  templateUrl: './login-component.html',
  styleUrl: './login-component.css'
})
export class LoginComponent {
  user:UserLoginModel = new UserLoginModel();
  
  constructor(private userService:UserService){}

  handleLogin(){
    this.userService.validateUserLogin(this.user);
  }
}
