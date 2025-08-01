import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { CategoryService } from '../../Services/Category.service';
import { ProductService } from '../../Services/Product.service';

@Component({
  selector: 'app-category-sidebar',
  imports: [CommonModule],
  templateUrl: './category-sidebar.html',
  styleUrl: './category-sidebar.css'
})
export class CategorySidebar {
  categories: any[] | null = null;
  selectedCategoryId: number = 0;

  constructor(private categoryService:CategoryService,private productService:ProductService){}

  ngOnInit():void{
    this.categoryService.GetCategoryList().subscribe();
    this.categoryService.categories$.subscribe(category =>{this.categories = category});
    this.productService.getProducts(1).subscribe();
  }

  selectCategory(id: number): void {
    this.selectedCategoryId = id;
    if(id !== 0)
      this.productService.getProducts(1,id).subscribe();
    else
      this.productService.getProducts(1).subscribe();
  }
}
