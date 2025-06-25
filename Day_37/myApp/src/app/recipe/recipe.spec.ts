import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Recipe } from './recipe';
import { RecipeModel } from '../models/recipe';


describe('Recipe', () => {
  let fixture: ComponentFixture<Recipe>;
  let Component: Recipe;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Recipe]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Recipe);
    Component = fixture.componentInstance;
    Component.recipe = new RecipeModel(1,"Briyani","cusine",10,"imageUrl",[]);
    fixture.detectChanges();
  });

  it('Check its rendering Recipe object',()=>{
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain("Briyani");
  })
});
