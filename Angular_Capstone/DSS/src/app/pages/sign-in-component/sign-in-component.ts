import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserLoginDto } from '../../models/userLoginDto';
import bootstrap from 'bootstrap';
import { Toast } from 'bootstrap';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { passwordValidator } from '../../validators/PasswordValidator';

@Component({
  selector: 'app-sign-in-component',
  imports: [FormsModule,ReactiveFormsModule,RouterModule,CommonModule],
  templateUrl: './sign-in-component.html',
  styleUrl: './sign-in-component.css'
})
export class SignInComponent {
  user:UserLoginDto = new UserLoginDto();
  loginForm:FormGroup
  private router = inject(Router);
  showPassword:boolean = false;

  constructor(private userService:UserService){
    this.loginForm = new FormGroup({
      email:new FormControl(null,[Validators.required,Validators.email]),
      password:new FormControl(null,[Validators.required,Validators.minLength(8),passwordValidator()])
    })
  }

  handleSubmit(){
      if(this.loginForm.invalid) return;
      this.userService.Loginuser(this.loginForm.value).subscribe({
        next:(res:any)=>{
          const user = {
            accessToken: res.data.accessToken,
            refreshToken : res.data.refreshToken,
            id : res.data.user.id,
            email : res.data.user.email,
            role : res.data.user.role
          };

          localStorage.setItem('authData',JSON.stringify(user));
          this.loginForm.reset();
          this.router.navigateByUrl('/main/home');
        },
        error:(err)=>{
          this.showToast(err.error.error.errorMessage,'danger');
        }
      })
  }

  togglePasswordVisibility(){
    this.showPassword = !this.showPassword;
  }

  showToast(message: string, type: 'success' | 'danger') {
    const toastEl = document.getElementById('liveToast');
    const toastBody = document.querySelector('.toast-body');

    toastBody!.textContent = message;
    toastEl!.classList.remove('bg-success', 'bg-danger');
    toastEl!.classList.add(type === 'success' ? 'bg-success' : 'bg-danger');

    const toast = new Toast(toastEl!);
    toast.show();
  }
}
