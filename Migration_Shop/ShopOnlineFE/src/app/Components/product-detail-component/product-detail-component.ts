import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../Services/Product.service';
import { Product } from '../../Models/product.model';
import { CommonModule } from '@angular/common';
import { CartService } from '../../Services/Cart.service';

@Component({
  selector: 'app-product-detail-component',
  imports: [CommonModule],
  templateUrl: './product-detail-component.html',
  styleUrl: './product-detail-component.css'
})
export class ProductDetailComponent implements OnInit{
  product?: Product;

  constructor(private route: ActivatedRoute, private productService: ProductService,private cartService:CartService) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.productService.getProductDetails(id).subscribe((res) => {
      console.log(res);
      this.product = res;
    });
  }

  AddToCart(id:number){
    this.cartService.addToCart(id).subscribe((res)=>{
      alert("Product added to cart\nCheck cart page");
      console.log(res);
    });
  }
}
