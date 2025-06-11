import { Component } from '@angular/core';
import { Customer } from './customer/customer';
import { ProductCard } from './product-card/product-card';


@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Customer,ProductCard]
})
export class App {
  protected title = 'myApp';
}
