import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { RecipeService } from "./recipe.service"
import { TestBed } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";



describe("RecipeService",()=>{
  let service:RecipeService;
  let httpMock:HttpTestingController;

  beforeEach(()=>{
    TestBed.configureTestingModule({
      imports:[],
      providers:[RecipeService,provideHttpClient(),provideHttpClientTesting()]
    });
    service = TestBed.inject(RecipeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(()=>{
    httpMock.verify();
  })

  it('Should get All recipes from API',()=>{
    const mockRecipes = {
      recipes:[],
      total: 0,
      skip: 0,
      limit: 0
    };

    service.getAllRecipes().subscribe(res =>{
      expect(res).toBe(mockRecipes);
    })

    const req = httpMock.expectOne('https://dummyjson.com/recipes');
    expect(req.request.method).toBe('GET');
    req.flush(mockRecipes);
  })
})
