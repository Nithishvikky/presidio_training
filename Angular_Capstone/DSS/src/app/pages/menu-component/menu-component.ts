import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { Collapse } from 'bootstrap';
import { UserService } from '../../services/user.service';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { NotificationModalComponent } from "../notification-modal-component/notification-modal-component";
import { NotificationService } from '../../services/notification.service';
import { NotificationResponseDto } from '../../models/notificationResponseDto';

@Component({
  selector: 'app-menu-component',
  imports: [RouterModule, NotificationModalComponent],
  templateUrl: './menu-component.html',
  styleUrl: './menu-component.css'
})
export class MenuComponent implements OnInit{
  private route = inject(Router);
  role:string = "";
  username:string = "";
  isDropdownOpen:boolean = false;
  collapsed:boolean = false;
  UnseenNotification:number = 0;
  notification:NotificationResponseDto[] = [];


  constructor(private userService:UserService,private breakpointObserver: BreakpointObserver,private notifyService:NotificationService){}

  ngOnInit():void{
    let auth_data = localStorage.getItem("authData");
    if(auth_data){
      this.role = JSON.parse(auth_data).role;
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

  // Should call logout api here
  OnSignOut(){
    localStorage.clear();
    this.closeDropdown();
    this.route.navigateByUrl('/auth/signin');
  }


  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  closeDropdown() {
    setTimeout(() => {
      this.isDropdownOpen = false;
    }, 100);
  }

}
