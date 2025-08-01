import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private baseUrl = 'http://localhost:5190/api/v1/Payment';

  constructor(private http: HttpClient) {}
  createPayment() {
    return this.http.post<{ redirectUrl: string }>(`${this.baseUrl}/create`, {});
  }

  executePayment(payerId: string, paymentId: string) {
    const params = new HttpParams()
      .set('payerId', payerId)
      .set('paymentId', paymentId);

    return this.http.get(`${this.baseUrl}/execute`, {
      params,
      responseType: 'text'
    });
  }
}
