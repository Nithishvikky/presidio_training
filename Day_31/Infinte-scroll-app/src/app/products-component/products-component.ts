import { Component, HostListener } from '@angular/core';
import { MenuComponent } from "../menu-component/menu-component";
import { ProductModel } from '../models/product';
import { ProductService } from '../services/product.service';
import { debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { ProductComponent } from '../product-component/product-component';

@Component({
  selector: 'app-products-component',
  imports: [MenuComponent,FormsModule,ProductComponent],
  templateUrl: './products-component.html',
  styleUrl: './products-component.css'
})
export class ProductsComponent {
products:ProductModel[]=[];
  searchString:string="";
  searchSubject = new Subject<string>();
  loading:boolean = false;
  GoToTop:boolean = false;
  IsProductNotAvailable:boolean = false;
  skip=0;
  total=0;

  constructor(private productService:ProductService){}

  searchProducts(){
    this.searchSubject.next(this.searchString);
  }

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      tap(() => {
        this.skip = 0;
        this.products = [];
        this.loading = true;
      }),
      switchMap(query => this.productService.getProductSearchResult(query, 12, this.skip)),
      tap(() => this.loading = false))
      .subscribe((data:any) => {
        this.products = data.products;
        this.total = data.total;

        if(this.total <= 0){
          this.IsProductNotAvailable = true;
        }
        else{
          this.IsProductNotAvailable = false;
        }
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
            this.total = data.total;
            this.loading = false;
          }
        })
  }
}
