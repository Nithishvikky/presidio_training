import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private route = inject(Router);

  intercept(req: HttpRequest<any>,next: HttpHandler):Observable<HttpEvent<any>>{
    const authdata = localStorage.getItem('authData');

    const excludedUrls = ['/Auth/login','/User/RegisterUser'];
    const shouldExclude = excludedUrls.some(url => req.url.includes(url));

    if(shouldExclude || !authdata){
      return next.handle(req);
    }
    const token = JSON.parse(authdata).accessToken;

    if(isTokenExpired(token)){
      localStorage.clear();
      console.log("Session expired!!");
      this.route.navigate(['/auth/signin']);
    }

    const authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    })

    console.log('Intercepted Request to:', req.url);
    console.log('Authorization Header:', authReq.headers.get('Authorization'));

    return next.handle(authReq);
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
