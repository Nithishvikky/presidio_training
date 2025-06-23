import { Routes } from '@angular/router';
import { First } from './first/first';
import { Login } from './login/login';
import { Products } from './products/products';
import { Home } from './home/home';
import { UserList } from './user-list/user-list';
import { FileUploadComponent } from './file-upload-component/file-upload-component';

export const routes: Routes = [
   {path:'landing',component:First},
    {path:'login',component:Login},
    {path:'products',component:Products},
    {path:'home/:un',component:Home,children:
        [
            {path:'products',component:Products},
            {path:'first',component:First}
        ]
    },
    {path:'users',component:UserList},
    {path:'msg',component:FileUploadComponent}
];
