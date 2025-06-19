import { Routes } from '@angular/router';
import { UserRegister } from './user-register/user-register';
import { UserList } from './user-list/user-list';

export const routes: Routes = [
  {path: '', redirectTo: '/register', pathMatch: 'full' },
  {path:'register',component:UserRegister},
  {path:'users',component:UserList}
];
