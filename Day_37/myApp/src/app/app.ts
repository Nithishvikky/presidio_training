import { Component } from '@angular/core';
import { Recipes } from "./recipes/recipes";


@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Recipes]
})
export class App {
  protected title = 'myApp';
}
