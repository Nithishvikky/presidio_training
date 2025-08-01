import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedItems } from '../Models/pagination.model';
import { Order } from '../Models/order.model';


@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = 'http://localhost:5190/api/v1/Order';

  constructor(private http: HttpClient) {}

  getOrders(page: number): Observable<PaginatedItems> {
    return this.http.get<PaginatedItems>(`${this.apiUrl}?page=${page}`);
  }

  getOrderDetails(orderId: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/Details/${orderId}`);
  }
}
