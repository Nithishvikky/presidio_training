import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@microsoft/signalr';
import { BehaviorSubject, catchError, filter, Observable, switchMap, take, throwError } from 'rxjs';
import { UserService } from '../services/user.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private route = inject(Router);
  private userService = inject(UserService);

  private isRefrshing = false;
  private refreshTokenSubject = new BehaviorSubject<string|null>(null);

  intercept(req: HttpRequest<any>,next: HttpHandler):Observable<HttpEvent<any>>{
    const authdata = localStorage.getItem('authData');

    const excludedUrls = ['/Auth/login','/User/RegisterUser','/Auth/refresh','/Auth/logout'];
    const shouldExclude = excludedUrls.some(url => req.url.includes(url));

    if(shouldExclude || !authdata){
      return next.handle(req);
    }
    const accesstoken = JSON.parse(authdata).accessToken;
    const refreshtoken = JSON.parse(authdata).refreshToken;

    if(!accesstoken || !refreshtoken){
      return next.handle(req);
    }

    if(this.isTokenExpired(accesstoken)){
      return this.handleTokenRefresh(req,next,refreshtoken);
    }

    const authReq = this.addToken(req,accesstoken);
    return next.handle(authReq);
  }

  private addToken(req: HttpRequest<any>, token: string): HttpRequest<any> {
    return req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
  }

  private handleTokenRefresh(req:HttpRequest<any>,next: HttpHandler,refreshToken: string): Observable<HttpEvent<any>>{
      if(!this.isRefrshing){
        this.isRefrshing = true;
        this.refreshTokenSubject.next(null);

       return this.userService.RefreshAccessToken(refreshToken).pipe(
          switchMap((res:any)=>{
            if (!res || res.success === false || !res.data?.accessToken) {
              throw new Error(res?.error?.error?.errorMessage || 'Invalid refresh token');
            }
            localStorage.clear();
            const user = {
              accessToken: res.data.accessToken,
              refreshToken : res.data.refreshToken,
              id : res.data.user.id,
              email : res.data.user.email,
              role : res.data.user.role
            };
            localStorage.setItem('authData',JSON.stringify(user));
            console.log("Access Token Renewed in interceptor");

            this.refreshTokenSubject.next(res.data.accessToken);
            this.isRefrshing = false;

            return next.handle(this.addToken(req,res.data.accessToken));
          }),
          catchError((err)=>{
              this.isRefrshing = false;
              localStorage.clear();
              console.log("Session expired!!");
              this.route.navigate(['/auth/signin']);
              return throwError(()=>err);
          })
        )
      }
      else{
        //wait for refresh to complete
        return this.refreshTokenSubject.pipe(
          filter(token => token !== null),
          take(1),
          switchMap((token) => next.handle(this.addToken(req, token!)))
        );
      }
  }

  private isTokenExpired(token: string): boolean {
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
}

