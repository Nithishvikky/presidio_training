import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Toast } from 'bootstrap';
import { PaymentModel } from '../models/paymentModel';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-form-component',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './form-component.html',
  styleUrl: './form-component.css'
})
export class FormComponent {
  paymentHistory:PaymentModel[]|null = null;
  paymentForm: FormGroup;

  constructor() {
    this.paymentForm = new FormGroup({
      amount: new FormControl(null, [Validators.required, Validators.min(1)]),
      customerName: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      contactNumber: new FormControl('', [Validators.required, Validators.pattern(/^\d{10}$/)])
    });
  }

  handleSubmit() {
    if (this.paymentForm.invalid) return;

    const form = this.paymentForm.value;
    const amountInPaise = form.amount * 100;

    const options: any = {
      key: 'rzp_test_aQKIAGNVcCPIh0',
      amount: amountInPaise,
      currency: 'INR',
      name: 'Demo Store',
      description: 'Test UPI Payment',
      handler: (response: any) => {
        console.log(response);
        this.showToast(`Payment Successful!\nPayment ID: ${response.razorpay_payment_id}`,"success");
        // var dto = new PaymentModel(response.razorpay_payment_id,form.customerName,form.contactNumber,form.email,form.amount);
        // this.paymentHistory?.unshift(dto);
      },
      prefill: {
        name: form.customerName,
        email: form.email,
        contact: form.contactNumber,
      },
      method: {
        upi: true
      },
      theme: {
        color: '#0d6efd'
      }
    };

    const rzp = new (window as any).Razorpay(options);

    rzp.on('payment.failed', (response: any) => {
      console.log('Payment failed', response);
      alert('Payment failed: ' + response.error.description);
    });

    rzp.open();

    this.paymentForm.reset();
  }

  showToast(message: string, type: 'success' | 'danger') {
    const toastEl = document.getElementById('liveToast');
    const toastBody = document.querySelector('.toast-body');

    toastBody!.textContent = message;
    toastEl!.classList.remove('bg-success', 'bg-danger');
    toastEl!.classList.add(type === 'success' ? 'bg-success' : 'bg-danger');

    const toast = new Toast(toastEl!);
    toast.show();
  }
}
