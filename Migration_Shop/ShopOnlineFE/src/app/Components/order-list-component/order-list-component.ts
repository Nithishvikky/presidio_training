import { Component } from '@angular/core';
import { Order } from '../../Models/order.model';
import { OrderService } from '../../Services/Order.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpResponse } from '@angular/common/http';


@Component({
  selector: 'app-order-list-component',
  imports: [CommonModule,RouterLink],
  templateUrl: './order-list-component.html',
  styleUrl: './order-list-component.css'
})
export class OrderListComponent {
  orders: Order[] = [];
  page = 1;
  totalPages = 1;

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.orderService.getOrders(this.page).subscribe((res) => {
      console.log(res);
      this.orders = res.items;
      this.totalPages = res.totalPages;
    });
  }

  downloadPdf(): void {
    const link = document.createElement('a');
    link.href = 'http://localhost:5190/api/v1/Order/export/pdf';
    link.download = '';
    link.style.display = 'none';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }


  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page++;
      this.loadOrders();
    }
  }

  prevPage(): void {
    if (this.page > 1) {
      this.page--;
      this.loadOrders();
    }
  }
}
