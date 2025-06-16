import { Component } from '@angular/core';

@Component({
  selector: 'app-customer',
  imports: [],
  templateUrl: './customer.html',
  styleUrl: './customer.css'
})
export class Customer {
likes:number=0;
dislikes:number=0;
likeClass:string = "bi bi-hand-thumbs-up";
dislikeClass:string = "bi bi-hand-thumbs-down";

  OnClickLike(){
    this.likes = this.likes+1;
    this.likeClass = "bi bi-hand-thumbs-up-fill";
  }
  OnClickDislike(){
    this.dislikes = this.dislikes+1;
    this.dislikeClass = "bi bi-hand-thumbs-down-fill";
  }
  customers = {Name:'Nithish',Email:'nithish@gmail.com'}
}
