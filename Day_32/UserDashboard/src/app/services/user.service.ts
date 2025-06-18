import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { UserModel } from "../models/user";


@Injectable()
export class UserService
{
  private http = inject(HttpClient);

  getAllUsers(){
    return this.http.get(`https://dummyjson.com/users?limit=0&skip=0`);
  }

  filterUsers(key:string,value:string){
    return this.http.get(`https://dummyjson.com/users/filter?key=${key}&value=${value}`);
  }
  postUser(user:UserModel)
  {
    console.log(user);
    return this.http.post(`https://dummyjson.com/users/add`,user);
  }
}
