import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../Models/product.model';
import { ProductService } from '../../Services/Product.service';
import { CategorySidebar } from "../category-sidebar/category-sidebar";
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-list-component',
  imports: [CommonModule,RouterLink],
  templateUrl: './product-list-component.html',
  styleUrl: './product-list-component.css'
})
export class ProductListComponent implements OnInit{
  products: Product[] | null = null;
  currentPage = 1;
  selectedCategoryId?: number;

  // @Input() categoryId: number | null = null;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    //this.productService.getProducts(this.currentPage, this.selectedCategoryId).subscribe();

    this.productService.products$.subscribe(products =>{
      this.products = products;
    })
  }

  nextPage(): void {
    this.currentPage++;
    this.loadProducts();
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadProducts();
    }
  }
}
