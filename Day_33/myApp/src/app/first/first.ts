import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-first',
  imports: [FormsModule],
  templateUrl: './first.html',
  styleUrl: './first.css'
})
export class First {
name:string;
className:string = "bi bi-balloon-heart";
like:boolean = false;
  constructor(){
    this.name = "Ramu"
  }
  onButtonClick(uname:string){
      this.name = uname;
  }
  toggleLike(){
      this.like = !this.like;
      if(this.like)
        this.className ="bi bi-balloon-heart-fill";
      else
         this.className ="bi bi-balloon-heart";
  }
}
