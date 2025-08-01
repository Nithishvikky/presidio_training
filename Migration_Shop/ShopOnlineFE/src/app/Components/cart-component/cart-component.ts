import { Component, OnInit } from '@angular/core';

import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Product } from '../../Models/product.model';
import { CartService } from '../../Services/Cart.service';
import { ProductService } from '../../Services/Product.service';
import { CommonModule } from '@angular/common';
import { PaymentService } from '../../Services/payment.service';

@Component({
  selector: 'app-cart-component',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './cart-component.html',
  styleUrl: './cart-component.css'
})
export class CartComponent {
  cartItems: { product: Product; quantity: number }[] = [];
  totalPrice = 0;
  customerForm: FormGroup;
  isLoading = false;

  constructor(
    private cartService: CartService,
    private productService: ProductService,
    private fb: FormBuilder,
    private paymentService : PaymentService
  ) {
    this.customerForm = this.fb.group({
      CustomerName: ['', Validators.required],
      CustomerPhone: ['', Validators.required],
      CustomerEmail: ['', [Validators.required, Validators.email]],
      CustomerAddress: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.isLoading = true;
    this.cartService.getCart().subscribe(async (carts) => {
      console.log(carts);
      const enrichedCart = [];
      for (const cart of carts) {
        const product = await this.productService
          .getProductDetails(cart.productId)
          .toPromise();
        if (product) {
          enrichedCart.push({ product, quantity: cart.quantity });
        }
      }
        this.cartItems = enrichedCart;
        this.calculateTotal();
        this.isLoading = false;
    });
  }

  calculateTotal(): void {
    this.totalPrice = this.cartItems.reduce(
      (sum, item) => sum + item.product.price * item.quantity,
      0
    );
  }

  remove(productId: number): void {
    this.cartService.removeFromCart(productId).subscribe(() => this.loadCart());
  }

  processOrder(isPaypal: boolean): void {
    if (this.customerForm.invalid) return;
    const customer = this.customerForm.value;
    if (isPaypal) {
      this.cartService.processPaypalOrder(customer).subscribe({
        next:(res)=>{
          window.open(res.redirectUrl, '_blank');
          // this.paymentService.createPayment().subscribe({
          //   next:(res)=>{
          //     window.open(res.redirectUrl, '_blank');
          //   },
          //   error: (err) => {
          //     console.log('Create payment failed', err);
          //     alert('Unable to create PayPal payment.');
          //   }
          // })
        },
        error: () => {
          alert('Order failed to process.');
        }
      });
    } else {
      this.cartService.processOrder(customer).subscribe(() => {
        alert('Order placed successfully!');
        this.loadCart();
        this.customerForm.reset();
      });
    }
  }
}
