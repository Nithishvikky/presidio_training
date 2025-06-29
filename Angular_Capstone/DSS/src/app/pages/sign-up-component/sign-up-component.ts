import { Component, inject } from '@angular/core';
import { UserRegisterDto } from '../../models/userRegisterDto';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Toast } from 'bootstrap';
import { passwordValidator } from '../../validators/PasswordValidator';
import { passwordMissmatchValidator } from '../../validators/PasswordMismatchValidator';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sign-up-component',
  imports: [FormsModule,ReactiveFormsModule,RouterModule,CommonModule],
  templateUrl: './sign-up-component.html',
  styleUrl: './sign-up-component.css'
})
export class SignUpComponent {
  user:UserRegisterDto = new UserRegisterDto();
  registerForm:FormGroup
  private router = inject(Router);
  showPassword: boolean = false;

  constructor(private userService:UserService){
    this.registerForm = new FormGroup({
      username:new FormControl(null,[Validators.required,Validators.maxLength(7)]),
      email:new FormControl(null,[Validators.required,Validators.email]),
      password:new FormControl(null,[Validators.required,Validators.minLength(8),passwordValidator()]),
      confirmPassword:new FormControl(null,[Validators.required]),
      role:new FormControl(null,[Validators.required])
    },{validators:passwordMissmatchValidator()})
  }

  handleSubmit(){
      if(this.registerForm.invalid) return;

      this.userService.RegisterUser(this.registerForm.value).subscribe({
        next:(res:any)=>{
          console.log(res);
          this.showToast(`Registered as ${res.data.role} !!`,'success');
          this.registerForm.reset();

          setTimeout(() => {
            this.router.navigateByUrl('/signin');
          },2000)
        },
        error:(err)=>{
          console.log(err);
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
