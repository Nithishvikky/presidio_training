import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate,  Router, RouterStateSnapshot } from '@angular/router';

@Injectable()
export class AuthGuard implements CanActivate{
  constructor(private router:Router){}
  canActivate(route:ActivatedRouteSnapshot, state:RouterStateSnapshot):boolean{
    const isAuthenticated = localStorage.getItem("authData");

    if(!isAuthenticated || isTokenExpired(JSON.parse(isAuthenticated).accessToken))
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

function isTokenExpired(token: string): boolean {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const expiry = payload.exp;
    const now = Math.floor(Date.now() / 1000);
    return now >= expiry;
  } catch (e) {
    console.error('Invalid token:', e);
    return true;
  }
}
