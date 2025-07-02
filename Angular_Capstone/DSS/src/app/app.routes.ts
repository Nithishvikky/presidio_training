import { Routes } from '@angular/router';
import { MydocumentComponent } from './pages/mydocument-component/mydocument-component';
import { DocumentComponent } from './pages/document-component/document-component';
import { SignInComponent } from './pages/sign-in-component/sign-in-component';
import { MainLayoutComponent } from './layouts/main-layout-component/main-layout-component';
import { AuthLayoutComponent } from './layouts/auth-layout-component/auth-layout-component';
import { SignUpComponent } from './pages/sign-up-component/sign-up-component';
import { AuthGuard } from './guards/auth-guard';
import { HomeComponent } from './pages/home-component/home-component';
import { BlankComponent } from './blank-component/blank-component';
import { AllDocumentsComponent } from './pages/all-documents-component/all-documents-component';
import { DocumentSharedComponent } from './pages/document-shared-component/document-shared-component';
import { PeopleComponent } from './pages/people-component/people-component';
import { ProfileComponent } from './pages/profile-component/profile-component';


export const routes: Routes = [
  {
    path: '',
    component: BlankComponent
  },
  {
    path:'auth',
    component: AuthLayoutComponent,
    children:[
      {path:'signin',component:SignInComponent},
      {path:'signup',component:SignUpComponent}
    ]
  },
  {
    path:'main',
    component:MainLayoutComponent,
    canActivate:[AuthGuard],
    data:{roles:['Admin','User']},
    children:[
      {path:'home',component:HomeComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'mydocuments',component:MydocumentComponent,canActivate:[AuthGuard],data:{roles:['Admin']}},
      {path:'documentsforme',component:DocumentSharedComponent,canActivate:[AuthGuard],data:{roles:['User']}},
      {path:'document/:filename',component:DocumentComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'alldocuments',component:AllDocumentsComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'community',component:PeopleComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'profile',component:ProfileComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}}
    ],
  },
  { path: '**', redirectTo: '' }
];
