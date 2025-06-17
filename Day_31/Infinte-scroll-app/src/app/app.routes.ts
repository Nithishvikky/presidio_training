import { Routes } from '@angular/router';
import { HomeComponent } from './home-component/home-component';
import { AboutComponent } from './about-component/about-component';
import { ProductsComponent } from './products-component/products-component';
import { ProductDetailComponent } from './product-detail-component/product-detail-component';
import { LoginComponent } from './login-component/login-component';
import { AuthGuard } from './auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },  // default routing
  {path:'login',component:LoginComponent},
  {path:'home',component:HomeComponent},
  {path:'about',component:AboutComponent},
  {path:'products',component:ProductsComponent,canActivate:[AuthGuard]},
  {path:'products/:id',component:ProductDetailComponent,canActivate:[AuthGuard]}

];
