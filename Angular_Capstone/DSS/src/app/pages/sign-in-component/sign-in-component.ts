import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserLoginDto } from '../../models/userLoginDto';
import bootstrap from 'bootstrap';
import { Toast } from 'bootstrap';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { passwordValidator } from '../../validators/PasswordValidator';
import { NotificationResponseDto } from '../../models/notificationResponseDto';
import { NotificationSharedResponseDto } from '../../models/notificationSharedResponse';
import { NotificationService } from '../../services/notification.service';

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
  notificationMessage:NotificationResponseDto[]=[];
  notificationSharedMessage:NotificationSharedResponseDto[]=[];
  role:string = "";

  constructor(private userService:UserService,public notifyService:NotificationService){
    this.loginForm = new FormGroup({
      email:new FormControl(null,[Validators.required,Validators.email]),
      password:new FormControl(null,[Validators.required,Validators.minLength(8),passwordValidator()])
    })
  }

  handleSubmit(){
      if(this.loginForm.invalid) return;
      this.userService.Loginuser(this.loginForm.value).subscribe({
        next:(res:any)=>{
          localStorage.clear();
          const user = {
            accessToken: res.data.accessToken,
            refreshToken : res.data.refreshToken,
            id : res.data.user.id,
            email : res.data.user.email,
            role : res.data.user.role
          };
          console.log(res);
          localStorage.setItem('authData',JSON.stringify(user));
          this.loginForm.reset();
          this.showToast("Signed in succesfully","success");
          setTimeout(()=>{
            this.router.navigateByUrl('/main/home');
            this.EnableNotification();
          },2000)
        },
        error:(err)=>{
          this.showToast(err.error.error.errorMessage,'danger');
        }
      })
  }
    EnableNotification(){
      const authData = localStorage.getItem('authData');
      if(authData){
        this.role = JSON.parse(authData).role;
        this.notifyService.startConnection();
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

      console.log("Notification enabling");
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
