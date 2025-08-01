import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Cart } from '../Models/cart.model';
import { Customer } from '../Models/customer.model';



@Injectable()
export class CartService {
  private apiUrl = 'http://localhost:5190/api/v1/ShoppingCart';

  constructor(private http: HttpClient) {}

  getCart(): Observable<Cart[]> {
    return this.http.get<Cart[]>(`${this.apiUrl}`,{
      withCredentials: true
    })
    .pipe(
      tap((res:any)=>{
        console.log(res);
      })
    );
  }

  addToCart(productId: number): Observable<Cart[]> {
    return this.http.post<Cart[]>(`${this.apiUrl}/AddToCart/${productId}`,"",{
      withCredentials: true
    });
  }

  removeFromCart(productId: number): Observable<Cart[]> {
    return this.http.delete<Cart[]>(`${this.apiUrl}/RemoveFromCart/${productId}`,{
      withCredentials: true
    });
  }

  processOrder(customer : Customer): Observable<any> {
    //console.log(customer);
    return this.http.post(`${this.apiUrl}/Order/COD`, customer,{
      withCredentials: true
    });
  }

  processPaypalOrder(customer : Customer): Observable<any> {
    return this.http.post(`${this.apiUrl}/Order/PayPal`, customer,{
      withCredentials: true
    });
  }
}
