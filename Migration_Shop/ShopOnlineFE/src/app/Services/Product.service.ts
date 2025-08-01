import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Product } from '../Models/product.model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'http://localhost:5190/api/v1/Product';

  private productList = new BehaviorSubject<Product[]|null>(null);
  products$ = this.productList.asObservable();

  constructor(private http: HttpClient) {}

  getProducts(page: number = 1, categoryId?: number): Observable<Product[]> {
    const url = categoryId
      ? `${this.apiUrl}/List?page=${page}&category=${categoryId}`
      : `${this.apiUrl}/List?page=${page}`;
    return this.http.get<Product[]>(url)
    .pipe(
      tap((res:any) =>{
        console.log(res);
        this.productList.next(res.items);
      })
    );
  }

  getProductDetails(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/Detail/${id}`);
  }
}
