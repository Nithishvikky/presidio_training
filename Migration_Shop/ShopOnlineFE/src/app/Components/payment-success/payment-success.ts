import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PaymentService } from '../../Services/payment.service';


@Component({
  selector: 'app-payment-success',
  template: `<p>{{ message }}</p>`
})
export class PaymentSuccessComponent implements OnInit {
  message = 'Processing payment...';

  constructor(
    private route: ActivatedRoute,
    private paymentService: PaymentService
  ) {}

  ngOnInit(): void {
    const success = this.route.snapshot.queryParamMap.get('success');
    const payerId = this.route.snapshot.queryParamMap.get('PayerID');
    const paymentId = this.route.snapshot.queryParamMap.get('paymentId');
    console.log(`${success}\n ${payerId}\n ${paymentId}`);

    if (success && payerId && paymentId) {
      this.paymentService.executePayment(payerId, paymentId).subscribe({
        next: () => this.message = 'Payment completed successfully!',
        error: () => this.message = 'Payment failed!'
      });
    } else {
      this.message = 'Missing payment info in URL.';
    }
  }
}
