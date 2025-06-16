import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { ProductModel } from '../models/product';
import { Product } from "../product/product";
import { CartItem } from '../models/cartItem';
import { debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-products',
  imports: [Product,FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products:ProductModel[]|undefined=undefined;
  cartItems:CartItem[] =[];
  cartCount:number =0;
  searchString:string="";
  searchSubject = new Subject<string>();
  loading:boolean = false;
  constructor(private productService:ProductService){

  }

  handleSearchProducts(){
    // console.log(this.searchString)
    this.searchSubject.next(this.searchString);
  }

  handleAddToCart(event:Number)
  {
    console.log("Handling add to cart - "+event)
    let flag = false;
    for(let i=0;i<this.cartItems.length;i++)
    {
      if(this.cartItems[i].Id==event)
      {
         this.cartItems[i].Count++;
         flag=true;
      }
    }
    if(!flag)
      this.cartItems.push(new CartItem(event,1));
    this.cartCount++;
  }

  ngOnInit(): void {
    // this.productService.getAllProducts().subscribe(
    //   {
    //     next:(data:any)=>{
    //      this.products = data.products as ProductModel[];
    //     },
    //     error:(err)=>{},
    //     complete:()=>{}
    //   }
    // )
      this.searchSubject.pipe(
      debounceTime(5000),
      distinctUntilChanged(),
      tap(()=>this.loading=true),
      switchMap(query=>this.productService.getProductSearchResult(query)),
       tap(()=>this.loading=false)).subscribe({
        next:(data:any)=>{this.products = data.products as ProductModel[];}
      });
  }

}
