import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { ProductService } from './services/product.service';
import { RecipeService } from './services/recipe.service';
import { LoginService } from './services/login.service';
import { UserService } from './services/user.service';
import { provideStore,provideState } from '@ngrx/store';
import { userReducer } from './ngrx/user.reducer';
import { BulkInsertService } from './services/BulkInsertService';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideStore(),
    provideState('user',userReducer),
    BulkInsertService,
    ProductService,
    RecipeService,
    LoginService,
    UserService
  ]
};
