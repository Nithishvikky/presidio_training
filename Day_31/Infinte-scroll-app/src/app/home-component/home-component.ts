import { Component, HostListener, OnInit } from '@angular/core';
import { ProductModel } from '../models/product';
import { debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { ProductService } from '../services/product.service';
import { ProductComponent } from '../product-component/product-component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MenuComponent } from "../menu-component/menu-component";
import { RouterOutlet } from '@angular/router';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-home-component',
  imports: [MenuComponent],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css'
})
export class HomeComponent{
  username:string|null = null;
  constructor(private userService:UserService){}

  ngOnInit(){
    this.userService.username$.subscribe(name=>{
      this.username = name;
    })
  }
}
