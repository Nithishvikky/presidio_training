import { Component, OnInit } from '@angular/core';
import { LoginDto } from '../models/login';
import { FormsModule } from '@angular/forms';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login{
Crendential:LoginDto = new LoginDto();

  constructor(private loginService:LoginService){}

  OnSubmitClick(e: Event):void{
    e.preventDefault();
    console.log(this.Crendential);
    if(this.loginService.IsCredentialsValid(this.Crendential)){
      alert("Login successful");
    }
    else{
      alert("Invalid credentials");
    }
  }

}
