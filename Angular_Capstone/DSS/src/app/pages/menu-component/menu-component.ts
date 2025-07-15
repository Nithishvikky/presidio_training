import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { Collapse, Toast } from 'bootstrap';
import { UserService } from '../../services/user.service';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { NotificationModalComponent } from "../notification-modal-component/notification-modal-component";
import { NotificationService } from '../../services/notification.service';
import { NotificationResponseDto } from '../../models/notificationResponseDto';
import { DocumentService } from '../../services/document.service';
import { DeleteModalComponent } from '../delete-modal-component/delete-modal-component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-menu-component',
  imports: [RouterModule, NotificationModalComponent,DeleteModalComponent,CommonModule],
  templateUrl: './menu-component.html',
  styleUrl: './menu-component.css'
})
export class MenuComponent implements OnInit{
  private route = inject(Router);
  role:string = "";
  username:string = "";
  refreshToken:string="";
  isDropdownOpen:boolean = false;
  collapsed:boolean = false;
  UnseenNotification:number = 0;
  notification:NotificationResponseDto[] = [];
  showConfirmModal = false;


  constructor(private userService:UserService,private breakpointObserver: BreakpointObserver,
    private notifyService:NotificationService,private documentService:DocumentService){}

  ngOnInit():void{
    let auth_data = localStorage.getItem("authData");
    if(auth_data){
      this.role = JSON.parse(auth_data).role;
      this.refreshToken = JSON.parse(auth_data).refreshToken;
      this.userService.GetUser(JSON.parse(auth_data).email).subscribe();
    }
    this.userService.CurrentUser$.subscribe(user=>{
      if(user)
        this.username = user?.username;
    })

    this.breakpointObserver.observe([Breakpoints.Small,Breakpoints.XSmall])
      .subscribe(res =>{
        if(res.matches){
          this.collapsed = true;
        }
        else{
          this.collapsed = false
        }
      })
    this.notifyService.notification$.subscribe(n =>{
      if(n) this.notification = n;
      this.UnseenNotification = this.notification.length;
      this.notifyService.SeenNotifi$.subscribe(n =>{
        if(n){
          this.UnseenNotification = this.notification.length - n;
        }
      })
    });
  }

  OnModalNotification(){
    this.notifyService.seenNotification.next(this.notification.length);
  }

  collapseNavbar() {
    const element = document.getElementById('navbarNavAltMarkup');
    if (element?.classList.contains('show')) {
      const bsCollapse = Collapse.getInstance(element) || new Collapse(element, { toggle: false });
      bsCollapse.hide();
    }
  }

  openConfirmModal(){
    this.showConfirmModal = true;
    console.log("clicked");
  }

  handleConfirm(result: boolean) {
    this.showConfirmModal = false;
    if (result) {
      console.log("check");
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
        this.closeDropdown();
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

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  closeDropdown() {
    setTimeout(() => {
      this.isDropdownOpen = false;
    }, 100);
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
