import { Component, Input} from '@angular/core';
import { RecipeModel } from '../models/recipe';
import { CommonModule} from '@angular/common';

@Component({
  selector: 'app-recipe',
  imports: [CommonModule],
  templateUrl: './recipe.html',
  styleUrl: './recipe.css'
})
export class Recipe {
  @Input() recipe:RecipeModel|null = new RecipeModel();
  constructor(){}
}
