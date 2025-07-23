import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { Collapse, Toast } from 'bootstrap';
import { UserService } from '../../services/user.service';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { NotificationBadgeComponent } from "../notification-badge-component/notification-badge-component";
import { DocumentService } from '../../services/document.service';
import { DeleteModalComponent } from '../delete-modal-component/delete-modal-component';
import { CommonModule } from '@angular/common';
import { ConnectionStatusComponent } from '../../components/connection-status-component/connection-status-component';

@Component({
  selector: 'app-menu-component',
  imports: [RouterModule, NotificationBadgeComponent, DeleteModalComponent, CommonModule, ConnectionStatusComponent],
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
  showConfirmModal = false;

  constructor(private userService:UserService,private breakpointObserver: BreakpointObserver,
    private documentService:DocumentService){}

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
  }

  OnModalNotification(){
    // Navigate to notifications page instead of modal
    this.route.navigate(['/main/notifications']);
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
