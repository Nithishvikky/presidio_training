import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserModel } from '../models/user';
import { UserService } from '../services/user.service';
import { emailValidator } from '../Validation/EmailValidator';
import { passwordMissmatchValidator } from '../Validation/PasswordMismatchValidator';
import { userNameValidator } from '../Validation/UserNameValidator';
import { passwordValidator } from '../Validation/PasswordValidator';
import * as bootstrap from 'bootstrap';

@Component({
  selector: 'app-user-register',
  imports: [FormsModule,ReactiveFormsModule],
  templateUrl: './user-register.html',
  styleUrl: './user-register.css'
})
export class UserRegister {
  toastMessage:string = "Success!";
  user:UserModel = new UserModel();
  registerForm:FormGroup

  constructor(private userService:UserService)
  {
    this.registerForm = new FormGroup({
      username:new FormControl(null,[Validators.required,userNameValidator()]),
      age:new FormControl(null,[Validators.min(1),Validators.required]),
      gender:new FormControl(null,Validators.required),
      role:new FormControl(null,Validators.required),
      email:new FormControl(null,[Validators.required,Validators.email,emailValidator()]),
      password:new FormControl(null,[Validators.required,passwordValidator()]),
      confirmPassword: new FormControl('', [Validators.required])
    },{validators: passwordMissmatchValidator()})
  }

  handleSumbit(){
    if(this.registerForm.invalid) return;
    console.log(this.registerForm.value);
    this.userService.AddUser(this.registerForm.value).subscribe({
      next:(res)=>{
        this.showToast('User Created successfully!','success');
      },
      error:(err)=>{
        this.showToast('Failed to create user.','danger');
      }
    })
    this.registerForm.reset();
  }

  showToast(message: string, type: 'success' | 'danger') {
    const toastEl = document.getElementById('liveToast');
    const toastBody = document.querySelector('.toast-body');
    toastBody!.textContent = message;

    toastEl!.classList.remove('bg-success', 'bg-danger');
    toastEl!.classList.add(type === 'success' ? 'bg-success' : 'bg-danger');

    const toast = new bootstrap.Toast(toastEl!);
    toast.show();
  }

}
