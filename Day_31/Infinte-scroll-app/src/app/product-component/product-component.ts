import { Component, inject, Input } from '@angular/core';
import { ProductModel } from '../models/product';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-component',
  imports: [CurrencyPipe, RouterLink],
  templateUrl: './product-component.html',
  styleUrl: './product-component.css'
})
export class ProductComponent {
  @Input() product:ProductModel|null = null;
  constructor(){}
}
