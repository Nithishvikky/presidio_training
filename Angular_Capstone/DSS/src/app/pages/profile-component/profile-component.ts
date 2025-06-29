import { Component, ElementRef, ViewChild } from '@angular/core';
import { UserResponseDto } from '../../models/userResponseDto';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordValidator } from '../../validators/PasswordValidator';
import { Modal, Toast } from 'bootstrap';

@Component({
  selector: 'app-profile-component',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './profile-component.html',
  styleUrl: './profile-component.css'
})
export class ProfileComponent {
  user:UserResponseDto | null = null;
  passwordChangeForm!:FormGroup;
  userEmail:string="";

  @ViewChild('PasswordInput') PasswordInput!: ElementRef<HTMLInputElement>;

  constructor(private userService:UserService,private route:Router){
    this.passwordChangeForm = new FormGroup({
      OldPassword:new FormControl(null,[Validators.required,Validators.minLength(8)]),
      NewPassword:new FormControl(null,[Validators.required,Validators.minLength(8),passwordValidator()])
    })
  }

  ngOnInit():void{
    let auth_data = localStorage.getItem("authData");
    if(auth_data){
      this.userEmail = JSON.parse(auth_data).email;
      this.userService.GetUser(JSON.parse(auth_data).email).subscribe();
    }
    this.userService.CurrentUser$.subscribe(user=>{
      this.user = user;
    })
    console.log(this.user);
  }

  OnSignOut(){
    localStorage.clear();
    this.route.navigateByUrl('/auth/signin');
  }
  handleSubmit() {
    if(this.passwordChangeForm.invalid) return;
    this.userService.ChangePassword(this.passwordChangeForm.value,this.userEmail).subscribe({
      next:(res:any)=>{
        this.showToast("Password updated successfully","success");
        this.modalCloseFunc();
        this.clearInput();
        setTimeout(()=>{
          localStorage.clear();
          this.route.navigateByUrl('/auth/signin');
        },2000)
      },
      error:(err)=>{
        this.modalCloseFunc();
        this.passwordChangeForm.reset({
          OldPassword: null,
          NewPassword: null
        });
        this.clearInput();
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
  }

  modalCloseFunc(){
    const modalEl = document.getElementById('passwordModal')!;
    let modal = Modal.getInstance(modalEl) || new Modal(modalEl);
    modal.hide();

    document.body.classList.remove('modal-open');
    document.querySelectorAll('.modal-backdrop')?.forEach(el => el.remove());
  }

  clearInput() {
    if (this.PasswordInput) {
      this.PasswordInput.nativeElement.value = '';
    }
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
