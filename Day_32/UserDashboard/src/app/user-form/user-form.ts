import { Component } from '@angular/core';
import { UserModel } from '../models/user';
import { UserService } from '../services/user.service';
import { EmailValidator, FormControl, FormGroup, FormsModule, MinValidator, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-user-form',
  imports: [FormsModule,ReactiveFormsModule],
  templateUrl: './user-form.html',
  styleUrl: './user-form.css'
})
export class UserForm {
  user:UserModel = new UserModel();
  registerForm:FormGroup

  constructor(private userService:UserService)
  {
    this.registerForm = new FormGroup({
      firstName:new FormControl(null,Validators.required),
      lastName:new FormControl(null,Validators.required),
      age:new FormControl(null,[Validators.min(1),Validators.required]),
      gender:new FormControl(null,Validators.required),
      email:new FormControl(null,Validators.required),
      password:new FormControl(null,Validators.required)
    })
  }

  handleSumbit(){

    if(this.registerForm.invalid) return;

    this.userService.postUser(this.registerForm.value).subscribe({
      next:(res)=>{
        console.log("User created sucessfully",res);
        this.registerForm.reset();
        
      },
      error:(err)=>{
        console.error("Erroe while creating user",err);
      }
    })
  }
}
