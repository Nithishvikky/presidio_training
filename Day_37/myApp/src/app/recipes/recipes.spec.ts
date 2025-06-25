import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Recipes } from './recipes';
import { RecipeService } from '../services/recipe.service';
import { of } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

class MockRecipeService{}

describe('Recipes', () => {
  let component: Recipes;
  let fixture: ComponentFixture<Recipes>;
  let recipeServiceSpy: jasmine.SpyObj<RecipeService>;

  const dummyRecipeData = {
    recipes:[
      {id: 1, name: 'Briyani'},
      {id: 2, name: 'Parotta'}
    ],
    total: 20,
    skip: 0,
    limit: 10
  }

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('RecipeService',['getAllRecipes']);

    await TestBed.configureTestingModule({
      imports: [Recipes],
      providers:[{provide:RecipeService,useValue:spy}],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Recipes);
    component = fixture.componentInstance;
    recipeServiceSpy = TestBed.inject(RecipeService) as jasmine.SpyObj<RecipeService>;
    recipeServiceSpy.getAllRecipes.and.returnValue(of(dummyRecipeData));
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render recipe component when recipe parameter available',()=>{
    const compiled = fixture.nativeElement as HTMLElement;
    const recipeElements = compiled.querySelectorAll('app-recipe');
    expect(recipeElements.length).toBe(2);
  })

  it('should render sent recipes',()=>{
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Briyani');
    expect(compiled.textContent).toContain('Parotta');
  })

  it('should render no recipes if recipes empty',()=>{
    const dummy = {
      recipes:[],
      total: 0,
      skip: 0,
      limit: 0
    }
    recipeServiceSpy.getAllRecipes.and.returnValue(of(dummy));
    fixture = TestBed.createComponent(Recipes);
    component = fixture.componentInstance;
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('No recipes available');
  })

});
