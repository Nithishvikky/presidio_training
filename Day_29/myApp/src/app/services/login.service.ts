import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { LoginDto } from "../models/login";

@Injectable()
export class LoginService{
    private http = inject(HttpClient);
    private userName:string = "nithish@gmail.com";
    private passWord:string = "nithish@123"

    IsCredentialsValid(Credential:LoginDto){
      if(Credential.Username == this.userName && Credential.Password == this.passWord){
        return true;
      }
      return false;
    }
}
