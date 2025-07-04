import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, tap } from "rxjs";
import { UserModel } from "../models/user";

@Injectable()
export class UserService
{

  private userSubject = new BehaviorSubject<any|null>(null);
  users$ = this.userSubject.asObservable();

  constructor(private http:HttpClient){
    this.http.get(`https://dummyjson.com/users?limit=0&skip=0`).subscribe({
      next:(data:any)=>{
        console.log(data);
        this.userSubject.next(data.users);
      },
      error:(err:any)=>{
        console.error(err);
      }
    })
  }

  AddUser(user:UserModel){
    console.log(user);
    const NewUser = new UserModel(
      user.username,
      user.age,
      user.gender,
      user.email,
      user.password,
      user.role,
    );
    return this.http.post(`https://dummyjson.com/users/add`,NewUser).pipe(
      tap(()=>{
        console.log(NewUser);
        const currentUsers = this.userSubject.value;
        this.userSubject.next([...currentUsers,NewUser]);
      })
    );
  }
}
