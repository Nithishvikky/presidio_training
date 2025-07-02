import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate,  Router, RouterStateSnapshot } from '@angular/router';
import { UserService } from '../services/user.service';

@Injectable()
export class AuthGuard implements CanActivate{
  constructor(private router:Router,private userService:UserService){}

  canActivate(route:ActivatedRouteSnapshot, state:RouterStateSnapshot):boolean{
    const isAuthenticated = localStorage.getItem("authData");

    if(!isAuthenticated)
    {
      localStorage.clear();
      this.router.navigate(['/auth/signin']);
      return false;
    }

    const expectedRoles = route.data['roles'] as string[];
    const userRole = JSON.parse(isAuthenticated).role;

    if(userRole && expectedRoles.includes(userRole)){
      return true;
    }

    this.router.navigate(['/auth/signin']);
    return false;
  }
}
