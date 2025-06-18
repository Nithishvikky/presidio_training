import { Routes } from '@angular/router';
import { UserForm } from './user-form/user-form';
import { UserListing } from './user-listing/user-listing';
import { Dashboard } from './dashboard/dashboard';

export const routes: Routes = [
  { path: '', redirectTo: '/register', pathMatch: 'full' },  // default routing
  {path:'register',component:UserForm},
  {path:'users',component:UserListing},
  {path:'dashboard',component:Dashboard}
];
