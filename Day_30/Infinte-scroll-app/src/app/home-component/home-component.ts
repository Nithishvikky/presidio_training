import { Component, HostListener, OnInit } from '@angular/core';
import { ProductModel } from '../models/product';
import { debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { ProductService } from '../services/product.service';
import { ProductComponent } from '../product-component/product-component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home-component',
  imports: [ProductComponent,FormsModule],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css'
})
export class HomeComponent implements OnInit{
  products:ProductModel[]=[];
  searchString:string="";
  searchSubject = new Subject<string>();
  loading:boolean = false;
  GoToTop:boolean = false;
  skip=0;
  total=0;

  constructor(private productService:ProductService){}

  searchProducts(){
    this.searchSubject.next(this.searchString);
  }

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      tap(() => {
        this.skip = 0;
        this.products = [];
        this.loading = true;
      }),
      switchMap(query => this.productService.getProductSearchResult(query, 12, this.skip)),
      tap(() => this.loading = false)
    ).subscribe((data:any) => {
      this.products = data.products;
      this.total = data.total;
    });

    this.searchSubject.next('');
  }

  @HostListener('window:scroll',[])
  onScroll():void
  {
    const scrollPosition = window.innerHeight + window.scrollY;
    const threshold = document.body.offsetHeight-100;

    this.GoToTop = scrollPosition > 900;

    if(scrollPosition>=threshold && this.products.length < this.total)
    {
      this.loadMore();
    }
  }

  goToTop(){
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  loadMore(){
    this.loading = true;
    this.skip += 12;
    this.productService.getProductSearchResult(this.searchString, 12,this.skip)
        .subscribe({
          next:(data:any)=>{
            console.log(data);
            this.products = [...this.products,...data.products];
            this.loading = false;
          }
        })
  }

}
