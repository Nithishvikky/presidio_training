import { Component, signal } from '@angular/core';
import { Recipe } from '../recipe/recipe';
import { RecipeModel } from '../models/recipe';
import { RecipeService } from '../services/recipe.service';

@Component({
  selector: 'app-recipes',
  imports: [Recipe],
  templateUrl: './recipes.html',
  styleUrl: './recipes.css'
})
export class Recipes {
  recipes = signal<RecipeModel[]>([]);

  constructor(private recipeService:RecipeService){}

  ngOnInit(): void{
    this.recipeService.getAllRecipes().subscribe(
      {
        next:(data:any)=>{
          console.log(data);
          this.recipes.set(data.recipes);
          console.log(data.recipes);
        },
        error:(err)=>{},
        complete:()=>{}
      }
    )
  }
}
