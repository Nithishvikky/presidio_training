import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, Observable, tap } from "rxjs";


@Injectable()
export class CategoryService{
  private categoryList = new BehaviorSubject<any[]|null>(null);
  categories$ = this.categoryList.asObservable();

  constructor(private http:HttpClient){}

  GetCategoryList(): Observable<any>{
    return this.http.get(`http://localhost:5190/api/v1/Category/List`)
    .pipe(
      tap((res:any) =>{
        this.categoryList.next(res);
      })
    )
  }
}
