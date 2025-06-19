import { Component } from '@angular/core';
import { Customer } from './customer/customer';
import { Products } from './products/products';
import { Recipes } from "./recipes/recipes";
import { Login } from './login/login';
import { RouterOutlet } from '@angular/router';
import { AddUser } from "./add-user/add-user";
import { UserList } from "./user-list/user-list";



@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Products, Recipes, Login, RouterOutlet, AddUser, UserList]
})
export class App {
  protected title = 'myApp';
}
