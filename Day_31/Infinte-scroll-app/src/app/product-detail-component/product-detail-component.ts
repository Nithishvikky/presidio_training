import { Component, inject } from '@angular/core';
import { MenuComponent } from '../menu-component/menu-component';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../services/product.service';
import { ProductDetailsModel } from '../models/productDetails';


@Component({
  selector: 'app-product-detail-component',
  imports: [MenuComponent],
  templateUrl: './product-detail-component.html',
  styleUrl: './product-detail-component.css'
})
export class ProductDetailComponent{
  id:number = 0;
  product:ProductDetailsModel = new ProductDetailsModel();
  router = inject(ActivatedRoute);

  constructor(private productService:ProductService){}

  ngOnInit():void{
    this.id = this.router.snapshot.params["id"] as number;
    this.productService.getProductById(this.id).subscribe({
          next:(data:any)=>{
            this.product = data;
            console.log(this.product);
          }
        })
  }
}
