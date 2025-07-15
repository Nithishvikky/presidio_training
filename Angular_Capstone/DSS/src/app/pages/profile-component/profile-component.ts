import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { UserResponseDto } from '../../models/userResponseDto';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordValidator } from '../../validators/PasswordValidator';
import { Modal, Toast } from 'bootstrap';
import { NotificationService } from '../../services/notification.service';
import { DocumentService } from '../../services/document.service';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { UserModalComponent } from "../user-modal-component/user-modal-component";
import { DeleteModalComponent } from '../delete-modal-component/delete-modal-component';

@Component({
  selector: 'app-profile-component',
  imports: [CommonModule, ReactiveFormsModule,DeleteModalComponent],
  templateUrl: './profile-component.html',
  styleUrl: './profile-component.css'
})
export class ProfileComponent implements OnInit{
  user:UserResponseDto | null = null;
  passwordChangeForm!:FormGroup;
  refreshToken:string="";
  userEmail:string="";
  documentShared:number = 0;
  showConfirmModal = false;

  @ViewChild('PasswordInput') PasswordInput!: ElementRef<HTMLInputElement>;

  constructor(private userService:UserService,private route:Router,private notifyService:NotificationService,
    private documentService:DocumentService,private documentAccessService:DocumentAccessService){
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

      if(JSON.parse(auth_data).role === "User"){
        this.documentAccessService.GetDocumentShared().subscribe();
        this.documentAccessService.sharedFiles$.subscribe(documents=>{
          if(documents) this.documentShared = documents.length;
        })
      }
    }
    this.userService.CurrentUser$.subscribe(user=>{
      this.user = user;
    })
    console.log(this.user);
  }

  openConfirmModal(){
    this.showConfirmModal = true;
  }

  handleConfirm(result: boolean) {
    this.showConfirmModal = false;
    if (result) {
      this.OnSignOut();
    }
  }

  OnSignOut(){
    let auth_data = localStorage.getItem("authData");
    if(auth_data){
      this.refreshToken = JSON.parse(auth_data).refreshToken;
    }
    this.userService.LogoutUser(this.refreshToken).subscribe({
      next:(res:any)=>{
        localStorage.clear();
        this.userService.clearUserCache();
        this.notifyService.clearNotification();
        this.documentService.clearDocumentCaches();
        this.showToast(res.data,"success");
        setTimeout(()=>{
          this.route.navigateByUrl('/auth/signin');
        },2000)
      }
    })
  }

  handleSubmit() {
    if(this.passwordChangeForm.invalid) return;
    this.userService.ChangePassword(this.passwordChangeForm.value,this.userEmail).subscribe({
      next:(res:any)=>{
        this.showToast("Password updated successfully","success");
        this.clearInput();
        setTimeout(()=>{
          localStorage.clear();
          this.route.navigateByUrl('/auth/signin');
        },2000)
      },
      error:(err)=>{
        this.passwordChangeForm.reset({
          OldPassword: null,
          NewPassword: null
        });
        this.clearInput();
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
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
