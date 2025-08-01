import { Component } from '@angular/core';
import { Order } from '../../Models/order.model';
import { OrderService } from '../../Services/Order.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-order-detail-component',
  imports: [CommonModule,RouterLink],
  templateUrl: './order-detail-component.html',
  styleUrl: './order-detail-component.css'
})
export class OrderDetailComponent {
  order!: Order;
  isLoading = true;

  constructor(private route: ActivatedRoute, private orderService: OrderService) {}

  ngOnInit(): void {
    const orderId = Number(this.route.snapshot.paramMap.get('id'));
    this.orderService.getOrderDetails(orderId).subscribe((res) => {
      this.order = res;
      this.isLoading = false;
    });
  }
}
