import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-product-card',
  imports: [CommonModule],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css'
})
export class ProductCard {
count:number=0;

  OnClickAddtoCart(){
    this.count = this.count+1;
  }

  products = [
    { title: 'Eraser', price: 2, imageUrl: 'https://m.media-amazon.com/images/I/41TRnNDZmwL._AC_UF894,1000_QL80_.jpg' },
    { title: 'Pencil', price: 5, imageUrl: 'https://4.imimg.com/data4/PL/IY/MY-23695144/natraj-pencil.jpg' },
    { title: 'Notebook', price: 30, imageUrl: 'https://tiimg.tistatic.com/fp/3/008/030/glossy-cover-rectangular-plain-paper-writing-spiral-binding-notebook-for-office-use-183.jpg' }
  ];
}
