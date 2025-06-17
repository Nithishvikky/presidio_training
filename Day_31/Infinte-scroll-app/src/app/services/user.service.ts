import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { UserLoginModel } from "../models/userLogin";
import { Router } from "@angular/router";


@Injectable()
export class UserService
{
    private http = inject(HttpClient);
    private route = inject(Router);
    private usernameSubject = new BehaviorSubject<string|null>(null);
    username$ = this.usernameSubject.asObservable();

    validateUserLogin(user:UserLoginModel)
    {
      this.callLoginAPI(user).subscribe(
      {
        next:(data:any)=>{
          this.usernameSubject.next(user.username);
          localStorage.setItem("token",data.accessToken);
          this.route.navigateByUrl("/home");
        },
        error:(err:any)=>{
          alert("Invalid credentials..Try again...");
          console.log(err);
        }
      })
    }
    callLoginAPI(user:UserLoginModel)
    {
      return this.http.post("https://dummyjson.com/auth/login",user);
    }
}
