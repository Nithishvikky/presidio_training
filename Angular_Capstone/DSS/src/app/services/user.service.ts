import { HttpClient, HttpParams } from "@angular/common/http";
import { UserLoginDto } from "../models/userLoginDto";
import { Injectable } from "@angular/core";
import { UserRegisterDto } from "../models/userRegisterDto";
import { BehaviorSubject, catchError, Observable, of, tap } from "rxjs";
import { UserResponseDto } from "../models/userResponseDto";
import { PagedResponseDto } from "../models/pagedResponseDto";
import { PasswordChangeDto } from "../models/passwordChangeDto";


@Injectable()
export class UserService{
  constructor(private http:HttpClient){}

  private userSubject = new BehaviorSubject<PagedResponseDto<UserResponseDto>|null>(null);
  users$ = this.userSubject.asObservable();

  private curUser = new BehaviorSubject<UserResponseDto|null>(null);
  CurrentUser$ = this.curUser.asObservable();

  RegisterUser(user:UserRegisterDto){
    return this.http.post(`http://localhost:5015/api/v1/User/RegisterUser`,user);
  }

  Loginuser(user:UserLoginDto){
    return this.http.post(`http://localhost:5015/api/v1/Auth/login`,user);
  }

  RefreshAccessToken(RToken:string):Observable<any>{
    return this.http.post(`http://localhost:5015/api/v1/Auth/refresh`,{RToken});
  }

  LogoutUser(RToken:string):Observable<any>{
    return this.http.post(`http://localhost:5015/api/v1/Auth/logout`,{RToken});
  }

  ChangePassword(dto:PasswordChangeDto,email:string): Observable<any>{
    console.log(dto);
    return this.http.put(`http://localhost:5015/api/v1/User/ChangePassword`,dto)
    .pipe(
      tap(()=> this.GetUser(email).subscribe())
    )
  }

  GetUser(email:string): Observable<any>{
    const params = new HttpParams().set('email',email);
    return this.http.get(`http://localhost:5015/api/v1/User/GetUser`,{params})
    .pipe(
      tap((res:any) =>{
        console.log(res);
        const user = new UserResponseDto(res.Id,res.email,res.username,res.role,res.registeredAt,res.updatedAt,res.uploadedDocuments.$values.length);
        this.curUser.next(user);
      })
    )
  }

  GetUserDetails(email:string):Observable<any>{
    const params = new HttpParams().set('email',email);
    return this.http.get(`http://localhost:5015/api/v1/User/GetUser`,{params});
  }

  GetAllUsers(
    searchByEmail?: string,
    searchByUsername?: string,
    filterBy?:string,
    sortBy?: string,
    ascending: boolean = true,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<any>{
    const params: any = {};

    if (searchByEmail) params.searchByEmail = searchByEmail;
    if (searchByUsername) params.searchByUsername = searchByUsername;
    if(filterBy) params.filterBy = filterBy;
    if (sortBy) params.sortBy = sortBy;

    params.ascending = ascending;
    params.pageNumber = pageNumber;
    params.pageSize = pageSize;

    return this.http.get(`http://localhost:5015/api/v1/User/GetAllUsers`,{
      params: params
    })
    .pipe(
      tap((res:any) =>{
        // console.log(res);
        this.userSubject.next(res.data);
      }),
      catchError((err) => {
        this.userSubject.next(null);
        return of(null);
      })
    )
  }

  clearUserCache(){
    this.curUser.next(null);
    this.userSubject.next(null);
  }

  GetInactiveUsers(days: number = 30): Observable<any> {
    const params = new HttpParams().set('days', days.toString());
    return this.http.get(`http://localhost:5015/api/v1/User/GetInactiveUsers`, { params });
  }
}
