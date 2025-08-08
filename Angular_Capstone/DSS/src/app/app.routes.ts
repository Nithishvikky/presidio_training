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
import { SharedDocumentComponent } from './pages/shared-document-component/shared-document-component';
import { AdminDocumentPreview } from './pages/admin-document-preview/admin-document-preview';
import { NotificationComponent } from './pages/notification-component/notification-component';
import { UserRequestsComponent } from './pages/user-requests-component/user-requests-component';
import { MyRequestsComponent } from './pages/my-requests-component/my-requests-component';
import { AdminDashboard } from './pages/admin-dashboard/admin-dashboard';

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
      {path:'mydocuments',component:MydocumentComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'documentsforme',component:DocumentSharedComponent,canActivate:[AuthGuard],data:{roles:['User']}},
      {path:'document/:filename',component:DocumentComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'alldocuments',component:AllDocumentsComponent,canActivate:[AuthGuard],data:{roles:['Admin']}},
      {path:'community',component:PeopleComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'profile',component:ProfileComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'shareddocument/:filename',component:SharedDocumentComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'documentadmin/:filename',component:AdminDocumentPreview,canActivate:[AuthGuard],data:{roles:['Admin']}},
      {path:'notifications',component:NotificationComponent,canActivate:[AuthGuard],data:{roles:['Admin','User']}},
      {path:'user-requests',component:UserRequestsComponent,canActivate:[AuthGuard],data:{roles:['Admin']}},
      {path:'my-requests',component:MyRequestsComponent,canActivate:[AuthGuard],data:{roles:['User']}},
      {path:'dashboard',component:AdminDashboard,canActivate:[AuthGuard],data:{roles:['Admin']}},
    ],
  },
  { path: '**', redirectTo: '' }
];

// Alternative: If you want to use hash routing instead of history API,
// change your app.config.ts to use provideRouter with useHash: true
// Example:
// import { provideRouter, withHashLocation } from '@angular/router';
// providers: [
//   provideRouter(routes, withHashLocation())
// ]
